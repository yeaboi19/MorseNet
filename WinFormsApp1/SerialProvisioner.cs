using System.IO.Ports;
using System.Text;

namespace WinFormsApp1 {

    /// <summary>
    /// Manages the serial connection to an ESP32 in provisioning mode.
    /// Thread-safe: callbacks are marshalled; all public methods can be
    /// called from the UI thread.
    /// </summary>
    public class SerialProvisioner : IDisposable {

        // ── Protocol constants ────────────────────────────────────────────
        public const int BaudRate = 115200;
        private const string ProvReadyToken = "PROV_READY";
        private const string ProvOkToken = "PROV_OK";
        private const string ProvErrPrefix = "PROV_ERR:";
        private const int ConnectTimeout = 10_000;   // ms to wait for PROV_READY

        // ── State ─────────────────────────────────────────────────────────
        private SerialPort? _port;
        private bool _disposed = false;
        private bool _espReady = false;   // true after PROV_READY received
        private readonly StringBuilder _rxBuf = new();

        // ── UI callbacks ──────────────────────────────────────────────────
        /// <summary>Called when the ESP sends PROV_READY (device is awaiting config).</summary>
        public Action? OnEspReady;

        /// <summary>Called when provisioning completes successfully.</summary>
        public Action? OnProvisionSuccess;

        /// <summary>Called when provisioning fails. Argument is the reason string.</summary>
        public Action<string>? OnProvisionError;

        /// <summary>Called for any other serial output (echo/debug lines).</summary>
        public Action<string>? OnLog;


        // ── Port enumeration (static helper) ──────────────────────────────
        public static string[] GetPortNames() => SerialPort.GetPortNames();


        // ── Connect ───────────────────────────────────────────────────────
        /// <summary>
        /// Opens the given COM port and starts listening.
        /// The caller should wait for <see cref="OnEspReady"/> before calling
        /// <see cref="SendConfig"/>.
        /// Throws <see cref="UnauthorizedAccessException"/> if the port is already
        /// held by another process (e.g. Arduino Serial Monitor).
        /// </summary>
        public void Connect(string portName) {
            Disconnect();

            _espReady = false;
            _rxBuf.Clear();

            _port = new SerialPort(portName, BaudRate, Parity.None, 8, StopBits.One) {
                ReadTimeout = 500,
                WriteTimeout = 2_000,
                NewLine = "\n",
                Encoding = Encoding.UTF8
            };

            _port.DataReceived += OnDataReceived;

            try {
                _port.Open();
            } catch (UnauthorizedAccessException) {
                // Port is held by another process — clean up and re-throw with
                // a friendly message the UI can display directly.
                _port.DataReceived -= OnDataReceived;
                _port.Dispose();
                _port = null;
                throw new UnauthorizedAccessException(
                    $"{portName} is already in use by another program.\n" +
                    "Close the Arduino Serial Monitor (or any other terminal) and try again.");
            } catch (Exception) {
                _port.DataReceived -= OnDataReceived;
                _port.Dispose();
                _port = null;
                throw;
            }

            Log($"Opened {portName} at {BaudRate} baud. Waiting for PROV_READY...");
        }


        // ── Disconnect ────────────────────────────────────────────────────
        public void Disconnect() {
            if (_port == null) return;

            // Capture and null out immediately so no new callbacks fire
            var port = _port;
            _port = null;
            _espReady = false;

            // Unsubscribe before closing — prevents DataReceived firing during Close()
            port.DataReceived -= OnDataReceived;

            // Close on a background thread. SerialPort.Close() can block briefly
            // waiting for pending reads to drain; doing it on the UI thread or from
            // inside a DataReceived callback causes a deadlock.
            Task.Run(() => {
                try {
                    if (port.IsOpen) port.Close();
                    port.Dispose();
                } catch { /* ignore errors during close */ }
            });
        }


        // ── SendConfig ────────────────────────────────────────────────────
        /// <summary>
        /// Sends the provisioning packet to the ESP.
        /// Format: PROV:ssid|pass|serverIP|port\n
        /// </summary>
        public void SendConfig(string ssid, string pass, string serverIp, int port) {
            if (_port == null || !_port.IsOpen)
                throw new InvalidOperationException("Serial port is not open.");

            if (!_espReady)
                throw new InvalidOperationException("ESP is not in provisioning mode yet.");

            string packet = $"PROV:{ssid}|{pass}|{serverIp}|{port}";
            Log($"Sending: {packet}");
            _port.WriteLine(packet);   // WriteLine appends NewLine (\n)
        }


        // ── IsOpen ───────────────────────────────────────────────────────
        public bool IsOpen => _port?.IsOpen == true;


        // ── Serial data handler ───────────────────────────────────────────
        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e) {
            // Use the sender directly — _port may have been nulled by Disconnect()
            // already, but the event can still fire once after unsubscribe.
            if (sender is not SerialPort port || !port.IsOpen) return;

            try {
                // Drain all available bytes into the buffer
                string chunk = port.ReadExisting();
                _rxBuf.Append(chunk);

                // Process complete lines
                string bufStr = _rxBuf.ToString();
                int nl;
                while ((nl = bufStr.IndexOf('\n')) >= 0) {
                    string line = bufStr[..nl].TrimEnd('\r').Trim();
                    bufStr = bufStr[(nl + 1)..];
                    if (line.Length > 0) HandleLine(line);
                }
                _rxBuf.Clear();
                _rxBuf.Append(bufStr);   // keep partial line

            } catch (Exception ex) {
                Log($"Serial read error: {ex.Message}");
            }
        }

        private void HandleLine(string line) {
            Log($"ESP: {line}");

            if (line == ProvReadyToken) {
                // Only fire OnEspReady once per connection session.
                // The ESP re-sends PROV_READY every 3s, but the UI only needs
                // to react to the first one.
                if (!_espReady) {
                    _espReady = true;
                    OnEspReady?.Invoke();
                }
                return;
            }

            if (line == ProvOkToken) {
                OnProvisionSuccess?.Invoke();
                return;
            }

            if (line.StartsWith(ProvErrPrefix)) {
                string reason = line[ProvErrPrefix.Length..];
                OnProvisionError?.Invoke(reason);
                return;
            }
        }

        private void Log(string msg) => OnLog?.Invoke(msg);


        // ── IDisposable ───────────────────────────────────────────────────
        public void Dispose() {
            if (_disposed) return;
            Disconnect();
            _disposed = true;
        }
    }
}
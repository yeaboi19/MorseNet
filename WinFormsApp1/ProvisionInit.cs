using System.IO.Ports;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace WinFormsApp1 {
    public partial class ProvisionInit : Form {

        private readonly SerialProvisioner _provisioner = new();
        private bool _espReady = false;

        public ProvisionInit() {
            InitializeComponent();
        }

        private void ProvisionInit_Load(object sender, EventArgs e) {
            Theme.LoadColors(this);
            RefreshPorts();
            AutoFillLocalIP();
            tbProvSPort.Text = TcpDeviceServer.Port.ToString();

            // ── Wire provisioner callbacks onto the UI thread ─────────────
            _provisioner.OnLog = msg => SafeInvoke(() => AppendLog(msg));

            _provisioner.OnEspReady = () => SafeInvoke(() => {
                _espReady = true;
                btnProvFlash.Enabled = true;
                AppendLog("ESP32 is in provisioning mode — ready to receive config.", LogType.Success);
                lblProvStatus.Text = "ESP ready. Fill in the fields and press Flash.";
                lblProvStatus.ForeColor = Theme.ColAccent;
            });

            _provisioner.OnProvisionSuccess = () => SafeInvoke(() => {
                AppendLog("Provisioning successful! ESP32 is rebooting.", LogType.Success);
                lblProvStatus.Text = "Done! ESP32 rebooting into normal mode.";
                lblProvStatus.ForeColor = Theme.ColAccent;
                btnProvFlash.Enabled = false;
                btnProvConnect.Text = "Connect";
                btnProvConnect.Enabled = true;
                // Defer Disconnect() — closing the SerialPort from inside its own
                // DataReceived callback causes a deadlock on the ThreadPool thread.
                BeginInvoke(() => _provisioner.Disconnect());
            });

            _provisioner.OnProvisionError = reason => SafeInvoke(() => {
                AppendLog($"Provisioning error: {reason}", LogType.Error);
                lblProvStatus.Text = $"Error: {reason}";
                lblProvStatus.ForeColor = Theme.ColDanger;
                btnProvFlash.Enabled = _espReady;
            });
        }

        private void ProvisionInit_FormClosing(object sender, FormClosingEventArgs e) {
            _provisioner.Dispose();
        }


        // ── Connect button ────────────────────────────────────────────────
        private void btnProvConnect_Click(object sender, EventArgs e) {
            if (_provisioner.IsOpen) {
                _provisioner.Disconnect();
                _espReady = false;
                btnProvConnect.Text = "Connect";
                btnProvFlash.Enabled = false;
                lblProvStatus.Text = "Disconnected.";
                lblProvStatus.ForeColor = Theme.ColSubtext;
                AppendLog("Disconnected from serial port.");
                return;
            }

            if (cbProvPort.SelectedItem is not string portName || portName.Length == 0) {
                SetError("Select a COM port first.");
                return;
            }

            try {
                _provisioner.Connect(portName);
                btnProvConnect.Text = "Disconnect";
                btnProvFlash.Enabled = false;   // will enable when PROV_READY arrives
                lblProvStatus.Text = $"Connected to {portName}. Waiting for ESP...";
                lblProvStatus.ForeColor = Theme.ColWarning;
                AppendLog($"Opened {portName}. Waiting for PROV_READY signal...");
            } catch (UnauthorizedAccessException ex) {
                // Port is held by another app (Arduino Serial Monitor, etc.)
                SetError(ex.Message);
                AppendLog("Tip: Close the Arduino IDE Serial Monitor (or any terminal) and try again.", LogType.Warning);
            } catch (Exception ex) {
                SetError($"Could not open port: {ex.Message}");
            }
        }


        // ── Flash button ──────────────────────────────────────────────────
        private void btnProvFlash_Click(object sender, EventArgs e) {
            string ssid = tbProvWSSID.Text.Trim();
            string pass = tbProvWPass.Text;           // password may contain spaces — don't trim
            string sip = tbProvSIP.Text.Trim();
            string sportS = tbProvSPort.Text.Trim();

            // ── Validate inputs ───────────────────────────────────────────
            if (ssid.Length == 0) {
                SetError("WiFi SSID cannot be empty."); tbProvWSSID.Focus(); return;
            }
            if (ssid.Length > 32) {
                SetError("WiFi SSID too long (max 32 chars)."); tbProvWSSID.Focus(); return;
            }
            if (pass.Length > 63) {
                SetError("WiFi password too long (max 63 chars)."); tbProvWPass.Focus(); return;
            }
            if (sip.Length == 0) {
                SetError("Server IP cannot be empty."); tbProvSIP.Focus(); return;
            }
            if (!int.TryParse(sportS, out int port) || port < 1 || port > 65535) {
                SetError("Server port must be 1–65535."); tbProvSPort.Focus(); return;
            }

            // ── Send ──────────────────────────────────────────────────────
            try {
                btnProvFlash.Enabled = false;
                lblProvStatus.Text = "Sending configuration...";
                lblProvStatus.ForeColor = Theme.ColWarning;
                _provisioner.SendConfig(ssid, pass, sip, port);
                AppendLog($"Config sent → SSID: {ssid} | IP: {sip} | Port: {port}", LogType.Info);
            } catch (Exception ex) {
                SetError($"Send failed: {ex.Message}");
                btnProvFlash.Enabled = _espReady;
            }
        }


        // ── Refresh ports button ──────────────────────────────────────────
        private void btnProvRefresh_Click(object sender, EventArgs e) {
            RefreshPorts();
        }


        // ── Helpers ───────────────────────────────────────────────────────

        private void RefreshPorts() {
            string? selected = cbProvPort.SelectedItem as string;
            cbProvPort.Items.Clear();

            string[] ports = SerialProvisioner.GetPortNames();
            if (ports.Length == 0) {
                cbProvPort.Items.Add("(no ports found)");
                cbProvPort.SelectedIndex = 0;
                btnProvConnect.Enabled = false;
            } else {
                cbProvPort.Items.AddRange(ports);
                // Restore previous selection if still present
                int idx = Array.IndexOf(ports, selected);
                cbProvPort.SelectedIndex = idx >= 0 ? idx : 0;
                btnProvConnect.Enabled = true;
            }

            AppendLog($"Found {ports.Length} COM port(s): {string.Join(", ", ports.Length > 0 ? ports : new[] { "none" })}");
        }

        // Tries to auto-detect the local IP of the machine to pre-fill server IP field
        private void AutoFillLocalIP() {
            // Priority order for picking the server IP to pre-fill:
            //   1. Windows Mobile Hotspot     — 192.168.137.x  (ESP connects to this)
            //   2. Private Wi-Fi              — 192.168.x.x / 10.x.x.x / 172.16-31.x.x
            //   3. Anything else non-APIPA    — last resort (avoids VPN adapters where possible)
            try {
                var candidates = new List<(int priority, string ip, string adapterName)>();

                foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces()) {
                    if (ni.OperationalStatus != OperationalStatus.Up) continue;
                    if (ni.NetworkInterfaceType == NetworkInterfaceType.Loopback) continue;
                    if (ni.NetworkInterfaceType == NetworkInterfaceType.Tunnel) continue;

                    foreach (UnicastIPAddressInformation addr in ni.GetIPProperties().UnicastAddresses) {
                        if (addr.Address.AddressFamily != AddressFamily.InterNetwork) continue;
                        string ip = addr.Address.ToString();
                        if (ip.StartsWith("169.254")) continue;   // skip APIPA

                        int priority;
                        if (ip.StartsWith("192.168.137."))
                            priority = 0;   // Windows Mobile Hotspot — highest priority
                        else if (ip.StartsWith("192.168.") || ip.StartsWith("10.") ||
                                 IsPrivate172(ip))
                            priority = 1;   // standard private range
                        else
                            priority = 2;   // public / VPN — lowest priority

                        candidates.Add((priority, ip, ni.Name));
                    }
                }

                if (candidates.Count == 0) return;

                var best = candidates.OrderBy(c => c.priority).First();
                tbProvSIP.Text = best.ip;
                AppendLog($"Auto-detected server IP: {best.ip}  ({best.adapterName})");

                // Warn if we couldn't find a hotspot address
                if (best.priority != 0)
                    AppendLog("Tip: For hotspot provisioning the IP should be 192.168.137.1. " +
                              "Verify the correct adapter is selected.", LogType.Warning);
            } catch {
                // silently ignore — user can fill it in manually
            }
        }

        // Returns true for 172.16.0.0 – 172.31.255.255
        private static bool IsPrivate172(string ip) {
            string[] parts = ip.Split('.');
            if (parts.Length < 2) return false;
            if (!int.TryParse(parts[0], out int a) || !int.TryParse(parts[1], out int b)) return false;
            return a == 172 && b >= 16 && b <= 31;
        }

        private void SetError(string msg) {
            lblProvStatus.Text = msg;
            lblProvStatus.ForeColor = Theme.ColDanger;
            AppendLog(msg, LogType.Error);
        }

        private void AppendLog(string msg, LogType type = LogType.Default) {
            Color color = type switch {
                LogType.Info => Theme.ColLogIn,
                LogType.Error => Theme.ColDanger,
                LogType.Warning => Theme.ColWarning,
                LogType.Success => Theme.ColAccent,
                _ => Theme.ColText
            };

            rtbProvLog.SelectionStart = rtbProvLog.TextLength;
            rtbProvLog.SelectionLength = 0;
            rtbProvLog.SelectionColor = color;
            rtbProvLog.SelectionFont = new Font(rtbProvLog.Font,
                type == LogType.Success ? FontStyle.Bold : FontStyle.Regular);
            rtbProvLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {msg}{Environment.NewLine}");
            rtbProvLog.ScrollToCaret();
        }

        private void SafeInvoke(Action action) {
            if (InvokeRequired) Invoke(action);
            else action();
        }
    }
}
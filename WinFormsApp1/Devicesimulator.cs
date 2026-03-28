using System.Net.Sockets;
using System.Text;

namespace WinFormsApp1 {

    public class DeviceSimulator : IDisposable {

        // ── Config ────────────────────────────────────────────────────────
        private readonly string _host;
        private readonly int _port;

        // how long looping scenarios run before going quiet
        private const int ScenarioDurationMs = 30000;

        private readonly CancellationTokenSource _cts = new();
        private readonly List<Task> _tasks = new();
        private bool _disposed = false;

        public DeviceSimulator(string host, int port) {
            _host = host;
            _port = port;
        }

        // ── Entry point ───────────────────────────────────────────────────

        public void StartAll() {
            var ct = _cts.Token;
            //_tasks.Add(Task.Run(() => RunHealthyDevice(ct), ct));
            //_tasks.Add(Task.Run(() => RunWeakSignalDevice(ct), ct));
            //_tasks.Add(Task.Run(() => RunSilentDevice(ct), ct));
            //_tasks.Add(Task.Run(() => RunSuddenDropDevice(ct), ct));
            //_tasks.Add(Task.Run(() => RunReconnectingDevice(ct), ct));
            //_tasks.Add(Task.Run(() => RunBadHandshakeDevice(ct), ct));
            //_tasks.Add(Task.Run(() => RunDuplicateMacDevice(ct), ct));
            //_tasks.Add(Task.Run(() => RunHighFrequencyDevice(ct), ct));
            //_tasks.Add(Task.Run(() => RunEchoDevice(ct), ct));
        }

        public void StopAll() {
            _cts.Cancel();
            try { Task.WaitAll(_tasks.ToArray(), TimeSpan.FromSeconds(3)); } catch (AggregateException) { }
        }


        // ══════════════════════════════════════════════════════════════════
        // SCENARIO 1 — Healthy device  (goes quiet after ~7 s)
        // ══════════════════════════════════════════════════════════════════
        private async Task RunHealthyDevice(CancellationToken ct) {
            await Task.Delay(500, ct);

            using var client = await ConnectAsync(ct);
            if (client == null) return;

            await SendLine(client, "HELLO:AA:BB:CC:00:01:01", ct);

            using var timed = RunFor(ScenarioDurationMs, ct);
            while (!timed.Token.IsCancellationRequested) {
                await SendLine(client, "PING:-52", timed.Token);
                await Task.Delay(3000, timed.Token);
                await SendLine(client, "MSG:.- -... -.-.  ..", timed.Token);
                await Task.Delay(5000, timed.Token);
            }
            // stays connected but silent — represents an idle healthy device
        }


        // ══════════════════════════════════════════════════════════════════
        // SCENARIO 2 — Weak / degrading signal  (goes quiet after ~7 s)
        // ══════════════════════════════════════════════════════════════════
        private async Task RunWeakSignalDevice(CancellationToken ct) {
            await Task.Delay(1000, ct);

            using var client = await ConnectAsync(ct);
            if (client == null) return;

            await SendLine(client, "HELLO:AA:BB:CC:00:02:02", ct);

            int rssi = -60;
            using var timed = RunFor(ScenarioDurationMs, ct);
            while (!timed.Token.IsCancellationRequested) {
                await SendLine(client, $"PING:{rssi}", timed.Token);
                rssi = Math.Max(rssi - 3, -95);
                await Task.Delay(2000, timed.Token);
            }
        }


        // ══════════════════════════════════════════════════════════════════
        // SCENARIO 3 — Silent device (connects, never pings, holds forever)
        // No change needed — it already produces exactly one log line.
        // ══════════════════════════════════════════════════════════════════
        private async Task RunSilentDevice(CancellationToken ct) {
            await Task.Delay(1500, ct);

            using var client = await ConnectAsync(ct);
            if (client == null) return;

            await SendLine(client, "HELLO:AA:BB:CC:00:03:03", ct);
            await Task.Delay(Timeout.Infinite, ct).ContinueWith(_ => { });
        }


        // ══════════════════════════════════════════════════════════════════
        // SCENARIO 4 — Sudden hard drop
        // Already self-terminating after ~4 s — no change needed.
        // ══════════════════════════════════════════════════════════════════
        private async Task RunSuddenDropDevice(CancellationToken ct) {
            await Task.Delay(2000, ct);

            using var client = await ConnectAsync(ct);
            if (client == null) return;

            await SendLine(client, "HELLO:AA:BB:CC:00:04:04", ct);
            await SendLine(client, "PING:-65", ct);
            await Task.Delay(2000, ct);
            await SendLine(client, "PING:-65", ct);
            await Task.Delay(2000, ct);

            client.Client.Close();
        }


        // ══════════════════════════════════════════════════════════════════
        // SCENARIO 5 — Reconnecting device  (stops cycling after ~7 s)
        // ══════════════════════════════════════════════════════════════════
        private async Task RunReconnectingDevice(CancellationToken ct) {
            await Task.Delay(2500, ct);

            using var timed = RunFor(ScenarioDurationMs, ct);
            while (!timed.Token.IsCancellationRequested) {
                using (var client = await ConnectAsync(timed.Token)) {
                    if (client == null) return;

                    await SendLine(client, "HELLO:AA:BB:CC:00:05:05", timed.Token);

                    for (int i = 0; i < 4 && !timed.Token.IsCancellationRequested; i++) {
                        await SendLine(client, "PING:-70", timed.Token);
                        await Task.Delay(1000, timed.Token);
                    }

                    client.Client.Close();
                }
                await Task.Delay(3000, timed.Token);
            }
        }


        // ══════════════════════════════════════════════════════════════════
        // SCENARIO 6 — Bad handshake
        // Already self-terminating — no change needed.
        // ══════════════════════════════════════════════════════════════════
        private async Task RunBadHandshakeDevice(CancellationToken ct) {
            await Task.Delay(3000, ct);

            using var client = await ConnectAsync(ct);
            if (client == null) return;

            await SendLine(client, "GREETINGS:AA:BB:CC:00:06:06", ct);
            await Task.Delay(1000, ct);
        }


        // ══════════════════════════════════════════════════════════════════
        // SCENARIO 7 — Duplicate MAC
        // Already self-terminating — no change needed.
        // ══════════════════════════════════════════════════════════════════
        private async Task RunDuplicateMacDevice(CancellationToken ct) {
            await Task.Delay(3500, ct);
            using var first = await ConnectAsync(ct);
            if (first == null) return;
            await SendLine(first, "HELLO:AA:BB:CC:00:07:07", ct);
            await SendLine(first, "PING:-55", ct);

            await Task.Delay(1000, ct);
            using var second = await ConnectAsync(ct);
            if (second == null) return;
            await SendLine(second, "HELLO:AA:BB:CC:00:07:07", ct);

            await Task.Delay(5000, ct);
        }


        // ══════════════════════════════════════════════════════════════════
        // SCENARIO 8 — High-frequency sender  (goes quiet after ~7 s)
        // ══════════════════════════════════════════════════════════════════
        private async Task RunHighFrequencyDevice(CancellationToken ct) {
            await Task.Delay(4000, ct);

            using var client = await ConnectAsync(ct);
            if (client == null) return;

            await SendLine(client, "HELLO:AA:BB:CC:00:08:08", ct);

            string[] samples = {
                "MSG:... --- ...",
                "MSG:.... . .-.. .-.. ---",
                "MSG:- . ... -",
            };

            int i = 0;
            using var timed = RunFor(ScenarioDurationMs-6, ct);
            while (!timed.Token.IsCancellationRequested) {
                await SendLine(client, samples[i % samples.Length], timed.Token);
                await SendLine(client, "PING:-58", timed.Token);
                i++;
                await Task.Delay(200, timed.Token);
            }
            // stays connected and silent after the burst
        }


        // ══════════════════════════════════════════════════════════════════
        // SCENARIO 9 — Echo device  (stays online permanently)
        // Connects and waits. When the server pushes a MSG frame to it,
        // it reads the line and immediately sends the same payload back.
        //
        // How to test: in the UI, select "ESP-000909" as the direct target
        // and send any message. A second log entry should appear ~300 ms
        // later showing the echoed morse arriving back from the device.
        // ══════════════════════════════════════════════════════════════════
        private async Task RunEchoDevice(CancellationToken ct) {
            await Task.Delay(1000, ct);

            using var client = await ConnectAsync(ct);
            if (client == null) return;

            await SendLine(client, "HELLO:AA:BB:CC:00:09:09", ct);
            await SendLine(client, "PING:-61", ct);

            var stream = client.GetStream();
            var buffer = new List<byte>(256);
            var single = new byte[1];

            DateTime lastPing = DateTime.Now;

            while (!ct.IsCancellationRequested) {
                try {
                    // send a keepalive ping every 5 s while idle
                    if ((DateTime.Now - lastPing).TotalSeconds >= 5) {
                        await SendLine(client, "PING:-61", ct);
                        lastPing = DateTime.Now;
                    }

                    if (!stream.DataAvailable) {
                        await Task.Delay(100, ct);
                        continue;
                    }

                    // read one full newline-terminated frame
                    buffer.Clear();
                    while (true) {
                        int read = await stream.ReadAsync(single, ct);
                        if (read == 0) return;          // server closed connection
                        if (single[0] == '\n') break;
                        if (single[0] != '\r') buffer.Add(single[0]);
                    }

                    string line = Encoding.UTF8.GetString(buffer.ToArray());

                    // only echo MSG frames
                    if (line.StartsWith("MSG:")) {
                        string payload = line["MSG:".Length..];
                        await Task.Delay(300, ct);                  // simulate processing time
                        await SendLine(client, $"MSG:{payload}", ct);
                        await SendLine(client, "PING:-61", ct);
                        lastPing = DateTime.Now;
                    }
                } catch (OperationCanceledException) {
                    break;
                } catch {
                    break;
                }
            }
        }


        // ── Helpers ───────────────────────────────────────────────────────

        /// <summary>
        /// Returns a linked CancellationTokenSource that expires after
        /// <paramref name="ms"/> ms OR when <paramref name="parent"/> cancels.
        /// Always wrap in a `using` statement.
        /// </summary>
        private static CancellationTokenSource RunFor(int ms, CancellationToken parent) {
            var linked = CancellationTokenSource.CreateLinkedTokenSource(parent);
            linked.CancelAfter(ms);
            return linked;
        }

        private async Task<TcpClient?> ConnectAsync(CancellationToken ct) {
            for (int attempt = 0; attempt < 5; attempt++) {
                try {
                    var client = new TcpClient();
                    await client.ConnectAsync(_host, _port, ct);
                    return client;
                } catch {
                    await Task.Delay(1000, ct);
                }
            }
            return null;
        }

        private static async Task SendLine(TcpClient client, string line, CancellationToken ct) {
            try {
                byte[] data = Encoding.UTF8.GetBytes(line + "\n");
                await client.GetStream().WriteAsync(data, ct);
            } catch { }
        }

        // ── IDisposable ───────────────────────────────────────────────────

        public void Dispose() {
            if (_disposed) return;
            StopAll();
            _cts.Dispose();
            _disposed = true;
        }
    }
}
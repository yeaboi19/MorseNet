using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WinFormsApp1 {
    public class TcpDeviceServer : IDisposable {

        //main Configs --

        //port for TCP server
        public const int Port = 6745;
        //watchdog for every 10 secs - device is dropped after 3 failed pings
        private const int WatchdogIntervalMs = 5_000;
        private const int MaxMissedPings = 3;
        // ----

        //server state
        private TcpListener _listener;
        private readonly List<ESPDevModel> _devices = new();
        private readonly object _lock = new(); // lock for threads and guarding _devices
        private CancellationTokenSource _cts = new(); //cleanly kill tcp services
        private bool _disposed = false; // state flag for killed tcp services

        // UI callbacks 
        public Action<ESPDevModel> OnDeviceConnected;
        public Action<ESPDevModel> OnDeviceDisconnected;
        public Action<ESPDevModel, string> OnMessageRecieved;
        public Action<string, LogType> OnLog;


        // start listening for incoming esp32 connections...
        public void Start() {
            _cts = new CancellationTokenSource();
            _listener = new TcpListener(IPAddress.Any, Port);
            _listener.Start();

            Log($"Server started. Listening on port {Port}...", LogType.Info);

            Task.Run(() => AcceptLoopAsync(_cts.Token));
            Task.Run(()=>WatchdogLoopAsync(_cts.Token));
        }

        // stop the server and disconnect all clients 
        public void Stop() {
            Log("Server Stopping...", LogType.Warning);
            _cts.Cancel();
            _listener?.Stop();

            lock (_lock) {
                foreach (var device in _devices) {
                    CloseDevice(device);

                }
                _devices.Clear();
            }
            Log("Server Stopped", LogType.Warning);
        }

        // return a copy array of all the devices connected
        public List<ESPDevModel> GetDevices() {
            lock (_lock) { return new List<ESPDevModel>(_devices); }
        }

        // sends a message to a specific device ( returns false if device is offline or the write itself fails)
        public bool SendTo(ESPDevModel device, String message) {
            try {
                if (!device.isOnline) return false;
                byte[] data = Encoding.UTF8.GetBytes(message + "\n");
                device.Stream.Write(data, 0, data.Length);
                return true;
            } catch {
                HandleDrop(device);
                return false;
            }
        }

        // sends a message to all of the connected devices (atleast tries to, returns count of successful messages sent)
        public int Broadcast(string message) {
            int sent = 0;
            foreach (var device in _devices) if (SendTo(device, message)) sent++;
            return sent;
        }

        //wait for the Tcp connection calls 
        private async Task AcceptLoopAsync(CancellationToken ct) {
            while (!ct.IsCancellationRequested) {
                try {
                    TcpClient client = await _listener.AcceptTcpClientAsync(ct);

                    // give every connection its own read loop
                    _ = Task.Run(() => HandleClientAsync(client, ct), ct);

                } catch (OperationCanceledException) {
                    break;
                } catch (Exception ex) {
                    Log($"Accept Err: {ex.Message}", LogType.Error);
                }
            }
        }

        // this function fires every "WatchdogIntervalMs
        // if the ping isnt incremented in that timeframe - missed pings is incremented for that device
        // once maxping limit is reached the device is automatically dropped and removed from the list
        private async Task WatchdogLoopAsync(CancellationToken ct) {
            while (!ct.IsCancellationRequested) {
                try {
                    await Task.Delay(WatchdogIntervalMs, ct);
                } catch (OperationCanceledException) {
                    break;
                }
                foreach (var device in GetDevices()) {
                    var scilentFor = DateTime.Now - device.LastSeen;

                    if (scilentFor.TotalMilliseconds < WatchdogIntervalMs) continue;
                    device.MissedPings++;
                    Log($"[WDG] {device.Name} - scilent for {scilentFor.TotalMilliseconds:F0}s" +
                        $"(missed {device.MissedPings}/{MaxMissedPings})",LogType.Warning);

                    if (device.MissedPings >= MaxMissedPings) {
                        Log($"[WDG] {device.Name} exceeded missed-ping limit — dropping.", LogType.Error);
                        HandleDrop(device);
                    }
                }
            }
        }

        // per client handler ( Handshake - expect Hello+MAC packet | Register - give device a name and add to list | Read loop - wait for info)
        private async Task HandleClientAsync(TcpClient client, CancellationToken ct) {
            string? firstline = await ReadLineAsync(client, ct);

            // if the handshake is invalid close the connection
            if (firstline == null || !firstline.StartsWith("HELLO:")) {
                Log("Client sent invalid handshake, rejecting", LogType.Warning);
                client.Close();
                return;
            }

            //extract mac address from the handshake
            string mac = firstline["HELLO:".Length..].Trim();

            // if mac is duplicate close the connection
            lock (_lock) {
                if (_devices.Any(d => d.Mac == mac.ToUpper())) {
                    Log($"Duplicate Mac {mac} - rejected", LogType.Warning);
                    client.Close();
                    return;
                }
            }
            // HANDSHAKE COMPLETE!

            //register device
            var device = new ESPDevModel(mac, client);

            lock (_lock) { _devices.Add(device); }

            Log($"[+] {device.Name} connected ({client.Client.RemoteEndPoint})", LogType.Success);
            OnDeviceConnected?.Invoke(device);
            // REGISTRATION COMPLETE!!

            //setup a readloop
            try {
                while (!ct.IsCancellationRequested && device.isOnline) {
                    string? line = await ReadLineAsync(client, ct);

                    // if line is null then stream closed or the device died
                    if (line == null) break;

                    // resest connection health
                    device.LastSeen = DateTime.Now;
                    device.MissedPings = 0;

                    //send the message to the parser
                    ProcessMessage(device, line);
                }
            } catch (Exception ex) when (ex is IOException or SocketException) {
                //normal when devices power drops or loses wifi
            } finally {
                HandleDrop(device);
            }
        }

        // setup a msg parser which handles 2 types of messages
        // ping and message: "PING:<RSSI>" - is keepalive with signal strength | MSG:<PAYLOAD> - is actual data/morse payload
        private void ProcessMessage(ESPDevModel device, string line) {
            if (line.StartsWith("PING:")) {
                if (int.TryParse(line["PING:".Length..], out int rssi)) {
                    device.Rssi = rssi;
                }
                Log($"[PING] {device.Name} {device.SignalBars} ({device.Rssi} dbm)", LogType.Info);
                return;
            }

            if (line.StartsWith("MSG:")) {
                string payload = line["MSG:".Length..];
                Log($"[MSG] {device.Name} -> {payload}", LogType.Info);
                OnMessageRecieved?.Invoke(device, payload);
                return;
            }

            // if the message has unknown frame - log it
            Log($"[???] {device.Name} sent unknown frame: {line}", LogType.Warning);
        }

        //drop handling - disconnection
        private void HandleDrop(ESPDevModel device) {
            //already handled - skip
            if (!device.isOnline) return;

            device.isOnline = false;
            CloseDevice(device);

            lock (_lock) { _devices.Remove(device); }
            Log($"[-] {device.Name} disconnected.", LogType.Error);
            OnDeviceDisconnected?.Invoke(device);
        }

        private static void CloseDevice(ESPDevModel device) {
            try { device.Client.Close(); } catch {/* ignore */ }
        }

        //utils

        //read as a single line until newline character comes
        //returns null if connection closes or an error occurs
        private static async Task<string?> ReadLineAsync(TcpClient client, CancellationToken ct) {
            try {
                var stream = client.GetStream();
                var buffer = new List<byte>(256);
                var single = new byte[1];

                while (true) {
                    int read = await stream.ReadAsync(single, ct);

                    if (read == 0) return null;
                    if (single[0] == '\n') break;
                    if (single[0] != '\r') buffer.Add(single[0]);
                }
                return Encoding.UTF8.GetString(buffer.ToArray());

            } catch {
                return null;
            }
        }

        private void Log(string msg, LogType type) => OnLog?.Invoke(msg, type);

        public void Dispose() {
            if (_disposed) return;
            Stop();
            _cts.Dispose();
            _disposed = true;
        }
    }
}

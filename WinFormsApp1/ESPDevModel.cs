using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Security;
using System.Text;

namespace WinFormsApp1 {
    public class ESPDevModel {
        public string Mac { get; init; }
        public string Name { get; init; }


        // Tcp connection handling..
        public TcpClient Client { get; init; }
        public NetworkStream Stream => Client.GetStream();

        //connection health tracking
        public DateTime LastSeen { get; set; } = DateTime.Now;
        public int MissedPings { get; set; } = 0;
        public bool isOnline { get; set; } = true;

        // wifi connection stregth in dBm -> -45bdm good connection | -90 weak conneciton
        public int Rssi { get; set; } = 0;


        //constructor get mac and tcp client, generate name from mac
        public ESPDevModel(string mac, TcpClient client) {
            Mac = mac.ToUpper().Trim();
            Name = GenerateName(mac);
            Client = client;
        }

        // generate visual bars for pretty gui stuff :>
        public string SignalBars {
            get {
                int bars = Rssi switch {
                    >= -55 => 4,
                    >= -65 => 3,
                    >= -75 => 2,
                    >= -85 => 1,
                    _ => 0
                };
                
                string[] barChars = { "▂", "▄", "▆", "█" };
                string res = "";
                for (int i = 0; i < 4; i++) {
                    res += (i < bars) ? barChars[i] : "_";

                }
                return res;
            }
        }

        // generate label for the UI
        public string SignalLabel => Rssi switch {
            >= -55 => "Excellent",
            >= -65 => "Good",
            >= -75 => "Fair",
            >= -85 => "Weak",
            _ => "No Signal"
        };


        // override ToString 
        public override string ToString() => $"{Name} {SignalBars} {SignalLabel}";


        // used to generate a name from the mac address, takes last 6 chars of mac and prefix with "ESP-"
        public static string GenerateName(string mac) {
            string cleanMac = mac.Replace(":", "").Replace("-", "");
            if (cleanMac.Length >= 6) {
                return "ESP-" + cleanMac[^6..].ToUpper();
            } else {
                return "ESP-" + cleanMac.ToUpper();
            }
        }
    }
}

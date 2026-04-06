# MorseNet

A Winforms desktop app that acts as a TCP server hub for ESP32 microcontrollers.
Letting the microcontrollers send and recieve morse code messages over a local network.
Plain text is typed on the pc - it gets encoded, transmitted as Morse and played back on the ESP32 hardware(Buzzer,LED and OLED display).

---

## Overview

MorseNet bridges a windows PC and a group of ESP32 devices on the same local network. 
The PC runs a TCP server that handles device registarion, keepalive watchdogging and message routing
Each ESP32 connects as a TCP client, identifies itself with its MAC address, sends periodic ping frames carrying its RSSI signal strength, and receives `MSG:` grames containing Morse encoded payloads to play out on its hardware.

---

## Project Structure

```
WinformsApp1/
├── Program.cs                # Entry point of the Winforms application
├── Form1.cs                  # main UI logic and event handling
├── Form1.Designer.cs         # Winforms designer-generated layout
├── Form1.resx                # Resource file for UI elements
├── TcpDeviceServer.cs        # TCP server: accept, register, watchdog, send/broadcast
├── ESPDevModel.cs            # Device Model:MAC, Name, Signal bars, Connection status
├── MorsecodeLookup.cs        # Morse code mapping and encoding/decoding logic
├── Theme.cs                  # Centralized dark color palette for Dark/Light mode
├── DeviceSimulator.cs        # DEBUG-ONLY :Simulated ESP32 client for testing without hardware
└── readme.md                 # Project documentation
```

---

## The UI

The window is built with nested `SplitContainer`s to divide functionality into sections.

```
┌─────────────────────────────────────────────────────────────┐
│  ESP Morse Server                                           │
├──────────────────────────┬──────────────────────────────────┤
│ Message Logs [Clear]     │  Connected Devices               │
│ ─────────────────────    │ ┌──────────────────────────────┐ │
│ [HH:mm:ss] log line...   │ │  ESP-AABBCC                  │ │
│ [HH:mm:ss] log line...   │ │  ESP-112233                  │ │
│ [HH:mm:ss] log line...   │ └──────────────────────────────┘ │
│                          ├──────────────────────────────────┤
│                          │  Send Message                    │
│                          │  Target:  [ ESP-AABBCC       ▼]  │
│                          │  Mode:  (•) Direct  ( ) Broadcast│
│                          │  Text:   [ hello              ]  │
│                          │  Preview:[.... . .-.. .-.. ---]  │
│                          │                        [ Send ]  │
├──────────────────────────┴──────────────────────────────────┤
│  X device(s) connected                                      │
└─────────────────────────────────────────────────────────────┘
```

### Message Logs (Left Panel)
A `RichTextBox` that shows color-coded timestamped events. Each entry is color-coded by type:

| Color| Type| Description |
|---|---|---|
| Blue | INFO | Ping frames, server startup |
| Red | ERROR | Device disconnected, send failure |
| Orange | WARN | Empty message, no target selected |
| White | DEFAULT | Ingoming/outgoing MSG frames |
| Green | SUCCESS | Device Connected |

The **Clear** button wipes the log. Log entries can be bold (used for connect/disconnect events) or normal weight (for regular messages).

### Device List (Top-Right panel)
A `Listbox` showing all currently connected ESP32S by auto-generated name (`Esp-`+last 6 hex digits of MAC). 
The list is backed by a `BindingList<ESPDevModel>` that updates in real-time as devices connect/disconnect.

### Send Message (bottom-right panel)
A `TableLayoutPanel` with four rows:

- **Target** - `ComboBox` (`cbTarget`) listing all connected devices, shares the same `BindingList` as the device list so it stays in sync automatically.
- **Mode** - `RadioButton`s to select between Direct (single target) and Broadcast (all devices) message sending.
- **Text** - `TextBox` (`tbInput`) for user input of plain text to send.
- **Preview** - read-only `TextBox` (`tbPreviw`) showing the real-time Morse encoding of what you've typed, using dots,  dashes and `_` for spaces.
- **Send Button** - validates input, encodes text to Morse, and sends it to the selected target(s) via the TCP server.

### Status Bar (Bottom)
A `StatusStrip` that shows the current number of connected devices, updated in real-time as devices connect/disconnect.

### Theme 
All colors are defined in `Theme.cs` as a static palette. Controls are tagged in the Designer (`"Header"`,`"Footer"`, `"Focus"`, `"Main"`) 
and the `LoadColors()` method recursively walks the control tree on startup to apply the dark theme.

---

## TCP Protocol
All messages are newline-terminated UTF-8 strings (`\n`). The Exchange follows a strict order:

### Handshake (ESP32 -> HOST/PC)
```
HELLO:<MAC_ADDRESS>
```
The server expects this as the very first line. If it's missing or malformed, the connection is closed immediately. Duplicate MACs are also rejected.

### Keepalive (ESP32 -> HOST/PC)
```
PING:<RSSI_VALUE>
```
Example: `PING:-70`

Sent periodically by the ESP32. The server updates the device's 'LastSeen' timestamp and RSSI Value.
If a device goes scilent for more than `WatchdogIntervalMs` (5sec) it starts accumulating missed pings and is dropped after `MaxMissedPings` (3).

### Outgoing Messages (HOST/PC -> ESP32)
```
MSG:<MORSE_PAYLOAD>
```
Example: `MSG:.... . .-.. .-.. ---`
The PC sends this frame when you click send. The ESP32 receives it and is expected to play it back on its hardware.

### Incoming Message (ESP32 -> HOST/PC)
```
MSG:<MORSE_PAYLOAD>
```
The ESP32 can laso send MSG frames bacl to the PC (for example after decoding a button press on teh device). The server decodes the Morse and logs it.

---
## Morse Code Encoding
`MorsecodeLookup.cs` contains a static dictionary mapping uppercase letters, digits and space to their Morse code representations.

- Spaces between words are encoded as `_` (underscore) in the Morse payload.
- Unknown characters encode as `#` (hash symbol).
- Morse tokens are joined with single spaces: `"HI"` -> `.... ..`
- Decoding splits on spaces and maps each token back to its character.

---
## Watchdog Logic
`TcpDeviceServer.cs` runs a background `WatchdogLoopAsync` task that fires every 5 seconds. 
Any device that hasn't sent anything (PING or MSG) within that windows gets a missed-ping increment logged. 
After 3 missed pings, the device is considered disconnected and `OnDeviceDisconnected` is invoked.

---
## Device Simulator (DEBUG ONLY)
`DeviceSimulator.cs` is compiled in `#if DEBUG` only and spins up fake TCP clients against `127.0.0.1:6745` to let you develop and test the UI without real hardware. Nine scenarios are available (all commented out by default in `StartAll()` — uncomment the ones you want):

| Scenario | Description |
|---|---|
| `RunHealthyDevice` | Connects, pings every 3 s, sends a MSG, then goes idle |
| `RunWeakSignalDevice` | RSSI degrades from -60 to -95 over time |
| `RunSilentDevice` | Connects and never pings (triggers watchdog drop) |
| `RunSuddenDropDevice` | Hard-closes the socket after two pings |
| `RunReconnectingDevice` | Repeatedly connects, pings 4 times, then drops |
| `RunBadHandshakeDevice` | Sends `GREETINGS:` instead of `HELLO:` (rejected) |
| `RunDuplicateMacDevice` | Two connections with the same MAC (second is rejected) |
| `RunHighFrequencyDevice` | Rapid-fires MSG and PING frames |
| `RunEchoDevice` | Echoes any MSG frame it receives back to the server |
###### *Note that the simulator is written by AI*

---

## ESP32 Firmware (IN PROGRESS)
The ESP32 side is still not yet implemented. Based on the protocol above, the firmware will need to:

**On Startup:**
1. Connect to WiFi
2. Open a TCP socket to the PC's local IP (Gateway IP) on port 6745
3. Send the handshake frame: `HELLO:<MAC_ADDRESS>`

**Keepalive Loop:**
- Every ~4-5 seconds, send a ping frame: `PING:<RSSI_VALUE>` (read with `WiFi.RSSI()`)

**Receive Loop:**
- Read incoming newline-terminated frames
- On receiving `MSG:<payload>`:
  - Display the decoded text on the OLED screen (SSD1306 via `Adafruit_SSD1306`)
  - Play the payload on the piezo buzzer (non-blocking, using a timer or `millis()` based state machine)
  - Blink the RGB LED in sync with dots and dashes

**Hardware Targets:**
- Esp32 Dev board 
- SSD1306 OLED display (I2C)
- Passive piezo buzzer
- RGB LED (common cathode, 3 GPIO pins)
- Button(s) (sending messages and controlling targets)
- Power supply (battery)

**Planned libraries:**
- `WiFi.h` / `WiFiClient.h` for network connectivity
- `Adafruit_SSD1306.h` for OLED display
- `Adafruit_GFX.h` for graphics support on the OLED
- Non-blocking Morse playback via `millis()` state machine (no `delay()` calls, so PING keepalives can still fire during playback)

---

## Building and running

**Requirements:**
- .NET 6.0 SDK or later
- Windows (Winforms)
- Visual Studio (`dotnet` should also work from CLI)
- ESP32 hardware for testing (optional, DeviceSimulator can be used instead)

**Steps:**
1. Clone the repository
2. Open `WinformsApp1.sln` in Visual Studio
3. Build the solution (in **DEBUG** mode to enable the device simulator)
4. Run the application, the server starts automatically on port `6745`
5. To test with simulated devices, uncomment the desired scenarios in `DeviceSimulator.StartAll()` and restart the app

Once real hardware is implemented, connect your ESP32 devices to the same local network and they should automatically appear in the device list as they connect. 
You can then send messages to them from the UI and see the interactions in real-time logs.

---

## Port
The server Listens on **TCP port 6745**. Make sure your Windows firewall allows inbound connections on this port from your local network
if the ESP32 is on a different subnet or VLAN. 
###### For local testing with the DeviceSimulator, no firewall changes should be necessary since it connects to `127.0.0.1`.
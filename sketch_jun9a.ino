// ═══════════════════════════════════════════════════════════════════════════
//  MorseNode — ESP32-S3 N16R8 Firmware
//  Hardware: SSD1306 OLED (I2C) | RGB LED common anode | Passive piezo | Button
//
//  Required libraries (install via Arduino Library Manager):
//    - Adafruit SSD1306   (by Adafruit)
//    - Adafruit GFX       (by Adafruit)
//
//  First-boot provisioning flow:
//    1. ESP boots → checks NVS for credentials
//    2. No creds found → Provisioning Mode (magenta LED, OLED prompt)
//    3. PC sends:  PROV:ssid|password|serverIP|port\n
//    4. ESP validates, saves to NVS, replies PROV_OK\n, reboots
//    5. On reboot creds are found → normal operation
// ═══════════════════════════════════════════════════════════════════════════

#include <Wire.h>
#include <Adafruit_GFX.h>
#include <Adafruit_SSD1306.h>
#include <WiFi.h>
#include <Preferences.h>
#include <esp_task_wdt.h>
#include <esp_mac.h>          // esp_read_mac() — reads MAC from eFuse, no WiFi needed




// ── NVS namespace / keys ─────────────────────────────────────────────────────
#define NVS_NAMESPACE   "morsenode"
#define NVS_KEY_SSID    "ssid"
#define NVS_KEY_PASS    "pass"
#define NVS_KEY_IP      "sip"
#define NVS_KEY_PORT    "sport"
#define NVS_KEY_READY   "ready"   // "1" when fully provisioned

Preferences prefs;

// ── Runtime credentials (loaded from NVS or filled during provisioning) ───────
char cfg_ssid[64]   = "";
char cfg_pass[64]   = "";
char cfg_sip[40]    = "";
uint16_t cfg_port   = 6745;

#define OLED_SDA           21   // Labeled "21" on the right side
#define OLED_SCL           22   // Labeled "22" on the right side

//   RGB LED — Common ANODE (+), so LOW = ON, HIGH = OFF
#define LED_R              4    // Labeled "4" on the right side
#define LED_G              5    // Labeled "5" on the right side
#define LED_B              18   // Labeled "18" on the right side

//   Passive piezo (PWM pin)
#define BUZZER_PIN         19   // Labeled "19" on the right side

//   Tactile button (INPUT_PULLUP → LOW when pressed)
#define BTN_PIN            14   // Labeled "14" on the left side

// ── Timing Constants ─────────────────────────────────────────────────────────
#define PING_INTERVAL_MS       5000
#define RECONNECT_INTERVAL_MS  8000
#define WDT_TIMEOUT_S          12

#define BTN_DEBOUNCE_MS        50
#define BTN_HOLD_1_MS          800
#define BTN_HOLD_2_MS          3000

#define OLED_DIM_AFTER_MS      20000

// Morse playback timing
#define MORSE_WPM              18
#define MORSE_UNIT_MS          (1200 / MORSE_WPM)
#define MORSE_DASH_MS          (MORSE_UNIT_MS * 3)
#define MORSE_SYM_GAP_MS       MORSE_UNIT_MS
#define MORSE_CHAR_GAP_MS      (MORSE_UNIT_MS * 3)
#define MORSE_WORD_GAP_MS      (MORSE_UNIT_MS * 7)
#define MORSE_FREQ_HZ          750

// Provisioning serial timeout
#define PROV_TIMEOUT_MS        120000   // 2 min before giving up and showing error

// Message queue
#define QUEUE_SIZE             5
#define MSG_MAX_LEN            160


// ── OLED ─────────────────────────────────────────────────────────────────────
#define SCREEN_W   128
#define SCREEN_H   64
#define OLED_ADDR  0x3C




Adafruit_SSD1306 display(SCREEN_W, SCREEN_H, &Wire, -1);


// ── Device State ─────────────────────────────────────────────────────────────
enum class State {
    Provisioning,       // awaiting config over serial
    WifiConnecting,
    TcpConnecting,
    Connected,
    Disconnected
};

State deviceState = State::WifiConnecting;


// ── TCP ──────────────────────────────────────────────────────────────────────
WiFiClient    tcpClient;
String        rxBuffer      = "";
unsigned long lastPingMs    = 0;
unsigned long lastReconnMs  = 0;
String        macAddress    = "";

// ── Message Queue ─────────────────────────────────────────────────────────────
struct Message {
    char morse[MSG_MAX_LEN];
    char text[MSG_MAX_LEN];
};
Message  msgQueue[QUEUE_SIZE];
int      qHead = 0, qTail = 0, qCount = 0;

String   lastMorse   = "";
String   lastDecoded = "";

// ── Morse Playback State Machine ──────────────────────────────────────────────
enum class MorsePhase { Idle, Symbol, SymGap, CharGap, WordGap };
MorsePhase    morsePhase   = MorsePhase::Idle;
String        morseString  = "";
int           tokenStart   = 0;
int           symIdx       = 0;
unsigned long morseTimer   = 0;
String        _activeToken = "";

// ── TX Morse State ─────────────────────────────────────────────────────────
String        pendingTxMorse = "";
unsigned long lastBtnReleaseMs = 0;
const unsigned long MORSE_DASH_MS_THRESH  = 200; // Press >= 200ms becomes a dash
const unsigned long MORSE_SPACE_MS_THRESH = 600; // Gap >= 600ms adds a space (new character)
const unsigned long AUTO_SEND_MS          = 2000; // 2 seconds of inactivity sends the message


// ── Button ────────────────────────────────────────────────────────────────────
bool          btnPrev    = HIGH;
unsigned long btnDownAt  = 0;
bool          hold1Fired = false;
bool          hold2Fired = false;

// ── OLED Dim ──────────────────────────────────────────────────────────────────
unsigned long lastActivityMs = 0;
bool          isDimmed       = false;

// ── Provisioning serial buffer ────────────────────────────────────────────────
String        provRxBuf        = "";
unsigned long provStartMs      = 0;
unsigned long provLastAnnounce = 0;
// last time PROV_READY was sent
#define PROV_ANNOUNCE_INTERVAL_MS  3000  // re-announce every 3s so PC reconnects work


// ═══════════════════════════════════════════════════════════════════════════
//  RGB LED   (common anode — invert all values)
// ═══════════════════════════════════════════════════════════════════════════

void setRgb(uint8_t r, uint8_t g, uint8_t b) {
    analogWrite(LED_R, 255 - r);
    analogWrite(LED_G, 255 - g);
    analogWrite(LED_B, 255 - b);
}

void ledOff()     { setRgb(0,   0,   0);   }
void ledRed()     { setRgb(255, 0,   0);   }
void ledGreen()   { setRgb(0,   200, 0);   }
void ledBlue()    { setRgb(0,   0,   255); }
void ledYellow()  { setRgb(255, 180, 0);   }
void ledCyan()    { setRgb(0,   200, 200); }
void ledWhite()   { setRgb(200, 200, 200); }
void ledMagenta() { setRgb(200, 0,   200); }  // provisioning mode indicator

void applyStateLed() {
    switch (deviceState) {
        case State::Provisioning:   ledMagenta(); break;
        case State::WifiConnecting: ledYellow();  break;
        case State::TcpConnecting:  ledCyan();    break;
        case State::Connected:      ledGreen();   break;
        case State::Disconnected:   ledRed();     break;
    }
}


// ═══════════════════════════════════════════════════════════════════════════
//  Buzzer
// ═══════════════════════════════════════════════════════════════════════════

void buzzerOn()  { tone(BUZZER_PIN, MORSE_FREQ_HZ); }
void buzzerOff() { noTone(BUZZER_PIN); }

void beepClick()        { tone(BUZZER_PIN, 1200, 25); }
void beepConnected()    { tone(BUZZER_PIN, 600, 80);  delay(100); tone(BUZZER_PIN, 1000, 120); }
void beepDisconnected() { tone(BUZZER_PIN, 900, 80);  delay(100); tone(BUZZER_PIN, 500, 180);  }
void beepProvDone()     { tone(BUZZER_PIN, 800, 80);  delay(100); tone(BUZZER_PIN, 1200, 120); delay(100); tone(BUZZER_PIN, 1600, 200); }


// ═══════════════════════════════════════════════════════════════════════════
//  NVS helpers
// ═══════════════════════════════════════════════════════════════════════════

// Returns true if the device has been provisioned before
bool nvsHasCredentials() {
    prefs.begin(NVS_NAMESPACE, true); // read-only
    bool ready = prefs.getBool(NVS_KEY_READY, false);
    prefs.end();
    return ready;
}

// Load credentials from NVS into runtime variables
void nvsLoadCredentials() {
    prefs.begin(NVS_NAMESPACE, true);
    prefs.getString(NVS_KEY_SSID, cfg_ssid, sizeof(cfg_ssid));
    prefs.getString(NVS_KEY_PASS, cfg_pass, sizeof(cfg_pass));
    prefs.getString(NVS_KEY_IP,   cfg_sip,  sizeof(cfg_sip));
    cfg_port = (uint16_t)prefs.getUInt(NVS_KEY_PORT, 6745);
    prefs.end();
}

// Save credentials to NVS (called after successful provisioning)
void nvsSaveCredentials(const char* ssid, const char* pass,
                        const char* sip,  uint16_t port) {
    prefs.begin(NVS_NAMESPACE, false); // read-write
    prefs.putString(NVS_KEY_SSID, ssid);
    prefs.putString(NVS_KEY_PASS, pass);
    prefs.putString(NVS_KEY_IP,   sip);
    prefs.putUInt(NVS_KEY_PORT,   port);
    prefs.putBool(NVS_KEY_READY,  true);
    prefs.end();
}

// Erase all stored credentials (factory reset via long-hold button)
void nvsEraseCredentials() {
    prefs.begin(NVS_NAMESPACE, false);
    prefs.clear();
    prefs.end();
}


// ═══════════════════════════════════════════════════════════════════════════
//  Morse decode  (morse token → character)
// ═══════════════════════════════════════════════════════════════════════════

struct MorseEntry { const char* code; char ch; };
static const MorseEntry MORSE_TABLE[] = {
    {".-",'A'}, {"-...",'B'}, {"-.-.",'C'}, {"-..",'D'},  {".",'E'},
    {"..-.",'F'},{"--.", 'G'},{"....",'H'}, {"..",'I'},   {".---",'J'},
    {"-.-",'K'}, {".-..",'L'},{"--",'M'},   {"-.",'N'},   {"---",'O'},
    {".--.",'P'},{"--.-",'Q'},{".-.", 'R'}, {"...",'S'},  {"-",'T'},
    {"..-",'U'}, {"...-",'V'},{".--",'W'},  {"-..-",'X'}, {"-.--",'Y'},
    {"--..",'Z'},{"-----",'0'},{".----",'1'},{"..---",'2'},
    {"...--",'3'},{"....-",'4'},{".....",'5'},{"-....",'6'},
    {"--...",'7'},{"---..",'8'},{"----.",'9'}, {"_",' '}
};

char morseTokenToChar(const String& token) {
    for (auto& e : MORSE_TABLE) {
        if (token == e.code) return e.ch;
    }
    return '?';
}

String decodeMorse(const String& morse) {
    String result = "";
    int start = 0, len = morse.length();
    while (start <= len) {
        int sp = morse.indexOf(' ', start);
        if (sp == -1) sp = len;
        if (sp > start) result += morseTokenToChar(morse.substring(start, sp));
        start = sp + 1;
    }
    return result;
}


// ═══════════════════════════════════════════════════════════════════════════
//  Message Queue
// ═══════════════════════════════════════════════════════════════════════════

bool queuePush(const char* morse, const char* text) {
    if (qCount >= QUEUE_SIZE) return false;
    strncpy(msgQueue[qTail].morse, morse, MSG_MAX_LEN - 1);
    strncpy(msgQueue[qTail].text,  text,  MSG_MAX_LEN - 1);
    qTail = (qTail + 1) % QUEUE_SIZE;
    qCount++;
    return true;
}

bool queuePop(Message& out) {
    if (qCount == 0) return false;
    out   = msgQueue[qHead];
    qHead = (qHead + 1) % QUEUE_SIZE;
    qCount--;
    return true;
}


// ═══════════════════════════════════════════════════════════════════════════
//  Morse Playback State Machine   (fully non-blocking)
// ═══════════════════════════════════════════════════════════════════════════

void morsePlay(const String& morse) {
    morseString = morse;
    tokenStart  = 0;
    symIdx      = 0;
    morsePhase  = MorsePhase::CharGap;
    morseTimer  = millis();
    ledWhite();
}

String nextToken() {
    int len = morseString.length();
    if (tokenStart >= len) return "";
    int sp = morseString.indexOf(' ', tokenStart);
    if (sp == -1) sp = len;
    String t = morseString.substring(tokenStart, sp);
    tokenStart = sp + 1;
    return t;
}

String currentToken()  { return _activeToken; }
char   currentSymbol() { return (symIdx < (int)_activeToken.length()) ? _activeToken[symIdx] : '.'; }

void playCurrentSymbol(char /*sym*/) {
    buzzerOn();
    morseTimer = millis();
    morsePhase = MorsePhase::Symbol;
}

void morseUpdate() {
    if (morsePhase == MorsePhase::Idle) {
        Message msg;
        if (queuePop(msg)) {
            morsePlay(String(msg.morse));
            lastMorse   = String(msg.morse);
            lastDecoded = String(msg.text);
            drawDisplay();
        }
        return;
    }

    unsigned long now = millis();

    switch (morsePhase) {

        case MorsePhase::Symbol: {
            char sym = currentSymbol();
            unsigned long dur = (sym == '-') ? MORSE_DASH_MS : MORSE_UNIT_MS;
            if (now - morseTimer >= dur) {
                buzzerOff();
                morseTimer = now;
                morsePhase = MorsePhase::SymGap;
            }
            break;
        }

        case MorsePhase::SymGap: {
            if (now - morseTimer >= MORSE_SYM_GAP_MS) {
                symIdx++;
                String token = currentToken();
                if (symIdx < (int)token.length()) {
                    playCurrentSymbol(token[symIdx]);
                } else {
                    morseTimer = now;
                    morsePhase = MorsePhase::CharGap;
                }
            }
            break;
        }

        case MorsePhase::CharGap: {
            if (now - morseTimer >= MORSE_CHAR_GAP_MS) {
                _activeToken = nextToken();
                symIdx = 0;
                if (_activeToken.length() == 0) {
                    morsePhase = MorsePhase::Idle;
                    applyStateLed();
                    drawDisplay();
                } else if (_activeToken == "_") {
                    morseTimer = now;
                    morsePhase = MorsePhase::WordGap;
                } else {
                    playCurrentSymbol(_activeToken[0]);
                }
            }
            break;
        }

        case MorsePhase::WordGap: {
            if (now - morseTimer >= MORSE_WORD_GAP_MS) {
                morseTimer = now;
                morsePhase = MorsePhase::CharGap;
            }
            break;
        }

        default: break;
    }
}


// ═══════════════════════════════════════════════════════════════════════════
//  OLED Display
// ═══════════════════════════════════════════════════════════════════════════

void drawSignalBars(int x, int y, int rssi) {
    int bars = 0;
    if      (rssi >= -55) bars = 4;
    else if (rssi >= -65) bars = 3;
    else if (rssi >= -75) bars = 2;
    else if (rssi >= -85) bars = 1;

    int heights[] = {3, 5, 7, 9};
    for (int i = 0; i < 4; i++) {
        int bx = x + i * 4;
        int by = y + (9 - heights[i]);
        if (i < bars)
            display.fillRect(bx, by, 3, heights[i], SSD1306_WHITE);
        else
            display.drawRect(bx, by, 3, heights[i], SSD1306_WHITE);
    }
}

String fitString(const String& s, int maxChars) {
    if ((int)s.length() <= maxChars) return s;
    return s.substring(0, maxChars - 2) + "..";
}

void drawDisplay() {
    if (isDimmed) return;
    display.clearDisplay();

    switch (deviceState) {

        // ── Provisioning ────────────────────────────────────────────────
        case State::Provisioning:
            display.setTextSize(1);
            display.setTextColor(SSD1306_WHITE);
            display.setCursor(0, 0);  display.println("-- PROVISIONING --");
            display.drawFastHLine(0, 11, SCREEN_W, SSD1306_WHITE);
            display.setCursor(0, 16); display.println("Waiting for config");
            display.setCursor(0, 28); display.println("via USB Serial...");
            display.setCursor(0, 44); display.println("Connect PC app and");
            display.setCursor(0, 54); display.println("press Flash!");
            break;

        case State::WifiConnecting:
            display.setTextSize(1);
            display.setTextColor(SSD1306_WHITE);
            display.setCursor(0, 0);  display.println("Connecting to WiFi");
            display.setCursor(0, 14); display.println(cfg_ssid);
            display.setCursor(0, 30); display.print(".");
            break;

        case State::TcpConnecting:
            display.setTextSize(1);
            display.setTextColor(SSD1306_WHITE);
            display.setCursor(0, 0);  display.println("Connecting to server");
            display.setCursor(0, 14); display.println(cfg_sip);
            display.setCursor(0, 28); display.print("Port: "); display.println(cfg_port);
            break;

        case State::Connected: {
            display.setTextSize(1);
            display.setTextColor(SSD1306_WHITE);

            String name = "ESP-" + macAddress.substring(9);
            name.replace(":", "");
            display.setCursor(0, 0);
            display.print(fitString(name, 14));
            drawSignalBars(110, 0, WiFi.RSSI());
            display.drawFastHLine(0, 11, SCREEN_W, SSD1306_WHITE);

            if (lastDecoded.length() == 0) {
                display.setCursor(0, 16);
                display.print("IP: "); display.println(WiFi.localIP());
                display.setCursor(0, 30); display.println("Ready");
                display.setCursor(0, 42); display.print("Q:"); display.print(qCount);
                display.print(" RSSI:"); display.println(WiFi.RSSI());
            } else {
                display.setCursor(0, 14);
                display.setTextSize(1); display.println("Received:");
                display.setTextSize(2);
                display.setCursor(0, 24); display.println(fitString(lastDecoded, 10));
                display.setTextSize(1);
                display.setCursor(0, 54); display.println(fitString(lastMorse, 21));
            }
            break;
        }

        case State::Disconnected:
            display.setTextSize(1);
            display.setTextColor(SSD1306_WHITE);
            display.setCursor(0, 0);   display.println("Disconnected");
            display.drawFastHLine(0, 11, SCREEN_W, SSD1306_WHITE);
            display.setCursor(0, 16);  display.println("Retrying...");
            display.setCursor(0, 30);  display.print("IP: "); display.println(WiFi.localIP());
            break;
    }

    display.display();
}

void wakeDisplay() {
    lastActivityMs = millis();
    if (isDimmed) {
        isDimmed = false;
        display.ssd1306_command(SSD1306_DISPLAYON);
        drawDisplay();
    }
}

void checkDisplayDim() {
    if (!isDimmed && millis() - lastActivityMs > OLED_DIM_AFTER_MS) {
        isDimmed = true;
        display.ssd1306_command(SSD1306_DISPLAYOFF);
    }
}


// ═══════════════════════════════════════════════════════════════════════════
//  Provisioning Mode   (Serial-based, non-blocking)
//
//  Expected packet:  PROV:ssid|password|serverIP|port\n
//  Response:         PROV_OK\n   OR   PROV_ERR:<reason>\n
// ═══════════════════════════════════════════════════════════════════════════

// Parse and validate the provisioning packet
// Returns true and fills out parameters on success
bool parseProvPacket(const String& line,
                     char* ssid, char* pass, char* sip, uint16_t& port) {
    // Must start with "PROV:"
    if (!line.startsWith("PROV:")) return false;
    String payload = line.substring(5);   // everything after "PROV:"

    // Split by '|'
    int d1 = payload.indexOf('|');
    if (d1 < 0) return false;
    int d2 = payload.indexOf('|', d1 + 1);
    if (d2 < 0) return false;
    int d3 = payload.indexOf('|', d2 + 1);
    if (d3 < 0) return false;

    String s_ssid = payload.substring(0, d1);
    String s_pass = payload.substring(d1 + 1, d2);
    String s_ip   = payload.substring(d2 + 1, d3);
    String s_port = payload.substring(d3 + 1);

    // Basic validation
    if (s_ssid.length() == 0 || s_ssid.length() > 32) return false;
    if (s_pass.length() > 63)                          return false;
    if (s_ip.length()   == 0 || s_ip.length() > 39)   return false;
    int p = s_port.toInt();
    if (p <= 0 || p > 65535)                           return false;

    s_ssid.toCharArray(ssid, 64);
    s_pass.toCharArray(pass, 64);
    s_ip.toCharArray(sip, 40);
    port = (uint16_t)p;
    return true;
}

// Called every loop() iteration while in provisioning state.
// Reads Serial non-blockingly until a full line arrives.
void provisioningLoop() {
    // Periodically re-send PROV_READY so the PC app sees it even if it
    // connects after the ESP booted, or after a DTR-triggered reset.
    if (millis() - provLastAnnounce >= PROV_ANNOUNCE_INTERVAL_MS) {
        provLastAnnounce = millis();
        Serial.println("PROV_READY");
    }

    // Read incoming serial bytes
    while (Serial.available()) {
        char c = (char)Serial.read();
        if (c == '\n') {
            provRxBuf.trim();
            if (provRxBuf.length() > 0) {
                char ssid[64], pass[64], sip[40];
                uint16_t port = 6745;

                if (parseProvPacket(provRxBuf, ssid, pass, sip, port)) {
                    // Save to NVS
                    nvsSaveCredentials(ssid, pass, sip, port);

                    // Acknowledge
                    Serial.println("PROV_OK");
                    Serial.flush();

                    // Visual/audio feedback
                    display.clearDisplay();
                    display.setTextSize(1);
                    display.setTextColor(SSD1306_WHITE);
                    display.setCursor(0, 0);  display.println("Provisioning OK!");
                    display.setCursor(0, 14); display.print("SSID: "); display.println(ssid);
                    display.setCursor(0, 28); display.print("IP:   "); display.println(sip);
                    display.setCursor(0, 42);
                    display.print("Port: "); display.println(port);
                    display.setCursor(0, 56); display.println("Rebooting...");
                    display.display();
                    beepProvDone();
                    delay(2000);

                    ESP.restart();
                } else {
                    Serial.println("PROV_ERR:bad_format");
                }
            }
            provRxBuf = "";
        } else if (c != '\r') {
            provRxBuf += c;
            if (provRxBuf.length() > 200) provRxBuf = "";   // guard against garbage
        }
    }
}


// ═══════════════════════════════════════════════════════════════════════════
//  TCP / Protocol
// ═══════════════════════════════════════════════════════════════════════════

void sendLine(const String& line) {
    if (tcpClient.connected()) tcpClient.println(line);
}

void sendPing() {
    sendLine("PING:" + String(WiFi.RSSI()));
    lastPingMs = millis();
}

void onConnectedToServer() {
    deviceState = State::Connected;
    applyStateLed();
    beepConnected();
    drawDisplay();
    wakeDisplay();
    sendLine("HELLO:" + macAddress);
    sendPing();
}

void onDisconnectedFromServer() {
    deviceState = State::Disconnected;
    applyStateLed();
    beepDisconnected();
    drawDisplay();
}

void processLine(const String& line) {
    wakeDisplay();
    if (line.startsWith("MSG:")) {
        String morse   = line.substring(4);
        String decoded = decodeMorse(morse);
        queuePush(morse.c_str(), decoded.c_str());
        if (morsePhase == MorsePhase::Idle) {
            Message msg;
            if (queuePop(msg)) {
                morsePlay(String(msg.morse));
                lastMorse   = String(msg.morse);
                lastDecoded = String(msg.text);
                drawDisplay();
            }
        }
    }
}

void tcpLoop() {
    while (tcpClient.available()) {
        char c = tcpClient.read();
        if (c == '\n') {
            rxBuffer.trim();
            if (rxBuffer.length() > 0) processLine(rxBuffer);
            rxBuffer = "";
        } else if (c != '\r') {
            rxBuffer += c;
        }
    }

    if (deviceState == State::Connected && !tcpClient.connected())
        onDisconnectedFromServer();

    if (deviceState == State::Connected &&
        millis() - lastPingMs >= PING_INTERVAL_MS)
        sendPing();

    if (deviceState != State::Connected &&
        WiFi.status() == WL_CONNECTED &&
        millis() - lastReconnMs >= RECONNECT_INTERVAL_MS) {

        lastReconnMs = millis();
        deviceState  = State::TcpConnecting;
        applyStateLed();
        drawDisplay();

        if (tcpClient.connect(cfg_sip, cfg_port)) {
            onConnectedToServer();
        } else {
            deviceState = State::Disconnected;
            applyStateLed();
            drawDisplay();
        }
    }
}


// ═══════════════════════════════════════════════════════════════════════════
//  Button
// ═══════════════════════════════════════════════════════════════════════════

// Button gesture summary:
//   Tap  (< BTN_HOLD_1_MS)  + Connected  → send last received morse back to server
//   Hold (>= BTN_HOLD_1_MS) + any state  → force TCP reconnect
//   Long (>= BTN_HOLD_2_MS) + any state  → factory reset
// ═══════════════════════════════════════════════════════════════════════════
//  Button
// ═══════════════════════════════════════════════════════════════════════════

void buttonLoop() {
    bool btn = digitalRead(BTN_PIN);
    unsigned long now = millis();

    // ── Press ─────────────────────────────────────────────────────────────
    if (btn == LOW && btnPrev == HIGH) {
        btnDownAt  = now;
        hold1Fired = false;
        hold2Fired = false;

        // If we are already typing a message, check if we need to add a space 
        // to separate characters based on how long it's been since the last release.
        if (pendingTxMorse.length() > 0) {
            unsigned long gap = now - lastBtnReleaseMs;
            if (gap >= MORSE_SPACE_MS_THRESH) {
                pendingTxMorse += " ";
            }
        }

        // Provide immediate audio feedback as they type
        if (deviceState == State::Connected) {
            buzzerOn();
        } else {
            beepClick(); 
        }
        wakeDisplay();
    }

    // ── Held ──────────────────────────────────────────────────────────────
    if (btn == LOW) {
        unsigned long held = now - btnDownAt;

        // Short hold — force TCP reconnect
        if (!hold1Fired && held >= BTN_HOLD_1_MS) {
            hold1Fired = true;
            buzzerOff();               // Kill the typing beep
            pendingTxMorse = "";       // Cancel any accidental typing
            tone(BUZZER_PIN, 800, 60); // Reconnect beep
            
            if (deviceState != State::Provisioning) {
                tcpClient.stop();
                lastReconnMs = 0;
            }
        }

        // Long hold — factory reset (erase NVS, reboot into provisioning)
        if (!hold2Fired && held >= BTN_HOLD_2_MS) {
            hold2Fired = true;
            tone(BUZZER_PIN, 400, 300);
            delay(350);

            display.clearDisplay();
            display.setTextSize(1);
            display.setTextColor(SSD1306_WHITE);
            display.setCursor(0, 0);  display.println("!! FACTORY RESET !!");
            display.setCursor(0, 16); display.println("Erasing credentials");
            display.setCursor(0, 30); display.println("and rebooting...");
            display.display();
            delay(1500);

            nvsEraseCredentials();
            ESP.restart();
        }
    }

    // ── Release ───────────────────────────────────────────────────────────
    if (btn == HIGH && btnPrev == LOW) {
        buzzerOff(); 
        unsigned long held = now - btnDownAt;
        lastBtnReleaseMs = now;

        // Translate the press duration into a Dot or Dash
        if (!hold1Fired && deviceState == State::Connected) {
            if (held < MORSE_DASH_MS_THRESH) {
                pendingTxMorse += ".";
            } else {
                pendingTxMorse += "-";
            }
        }
    }

    // ── Auto-Send on Inactivity ───────────────────────────────────────────
    if (btn == HIGH && pendingTxMorse.length() > 0) {
        if (now - lastBtnReleaseMs >= AUTO_SEND_MS) {
            sendLine("MSG:" + pendingTxMorse);
            
            // High-pitched success beep to confirm it sent
            tone(BUZZER_PIN, 1200, 60); 
            
            pendingTxMorse = ""; // Clear the buffer for the next message
        }
    }

    btnPrev = btn;
}

// ═══════════════════════════════════════════════════════════════════════════
//  WiFi
// ═══════════════════════════════════════════════════════════════════════════

void wifiLoop() {
    static unsigned long lastWifiCheck = 0;
    if (millis() - lastWifiCheck < 2000) return;
    lastWifiCheck = millis();

    if (WiFi.status() != WL_CONNECTED) {
        if (deviceState != State::WifiConnecting) {
            deviceState = State::WifiConnecting;
            applyStateLed();
            drawDisplay();
            WiFi.reconnect();
        }
    } else if (deviceState == State::WifiConnecting) {
        deviceState  = State::Disconnected;
        lastReconnMs = 0;
        applyStateLed();
        drawDisplay();
    }
}


// ═══════════════════════════════════════════════════════════════════════════
//  Setup
// ═══════════════════════════════════════════════════════════════════════════

void setup() {
    Serial.begin(115200);

    // ── Hardware watchdog ────────────────────────────────────────────────
    esp_task_wdt_config_t wdtCfg = { .timeout_ms = WDT_TIMEOUT_S * 1000,
                                     .idle_core_mask = (1 << 0),
                                     .trigger_panic  = true };
    esp_task_wdt_reconfigure(&wdtCfg);
    esp_task_wdt_add(NULL);

    // ── Pins ─────────────────────────────────────────────────────────────
    pinMode(LED_R, OUTPUT);
    pinMode(LED_G, OUTPUT);
    pinMode(LED_B, OUTPUT);
    pinMode(BUZZER_PIN, OUTPUT);
    pinMode(BTN_PIN, INPUT_PULLUP);
    ledYellow();

    // ── OLED ─────────────────────────────────────────────────────────────
    Wire.begin(OLED_SDA, OLED_SCL);
    if (!display.begin(SSD1306_SWITCHCAPVCC, OLED_ADDR)) {
        Serial.println("OLED init failed");
        while (true);
    }
    display.clearDisplay();
    display.setTextColor(SSD1306_WHITE);
    display.setTextSize(1);
    display.setCursor(0, 0);  display.println("MorseNode v1.1");
    display.setCursor(0, 12); display.println("Booting...");
    display.display();

    // ── MAC address — read from eFuse directly, no WiFi init required ─────────
    // WiFi.macAddress() returns 00:00:00:00:00:00 until the radio fully warms up.
    // esp_read_mac() reads the hardware-burned eFuse value instantly and reliably.
    uint8_t rawMac[6];
    esp_read_mac(rawMac, ESP_MAC_WIFI_STA);
    char macBuf[18];
    snprintf(macBuf, sizeof(macBuf), "%02X:%02X:%02X:%02X:%02X:%02X",
             rawMac[0], rawMac[1], rawMac[2], rawMac[3], rawMac[4], rawMac[5]);
    macAddress = String(macBuf);
    Serial.print("MAC: "); Serial.println(macAddress);

    WiFi.mode(WIFI_STA);

    // ── Check NVS for stored credentials ─────────────────────────────────
    if (!nvsHasCredentials()) {
        // ── PROVISIONING MODE ────────────────────────────────────────────
        deviceState  = State::Provisioning;
        provStartMs  = millis();
        applyStateLed();
        drawDisplay();

        // provisioningLoop() sends PROV_READY every 3s — no need to send here
        return;
    }

    // ── Load credentials and start normally ──────────────────────────────
    nvsLoadCredentials();

    display.setCursor(0, 24); display.print("MAC: ");
    display.println(macAddress);
    display.display();

    WiFi.begin(cfg_ssid, cfg_pass);   // mode already set to WIFI_STA above

    unsigned long wifiStart = millis();
    while (WiFi.status() != WL_CONNECTED && millis() - wifiStart < 15000) {
        delay(300);
        esp_task_wdt_reset();
    }

    if (WiFi.status() == WL_CONNECTED) {
        deviceState = State::TcpConnecting;
        Serial.print("WiFi connected: "); Serial.println(WiFi.localIP());
    } else {
        deviceState = State::WifiConnecting;
        Serial.println("WiFi timed out — will retry.");
    }

    applyStateLed();
    lastActivityMs = millis();
    drawDisplay();

    if (deviceState == State::TcpConnecting) {
        if (tcpClient.connect(cfg_sip, cfg_port)) {
            onConnectedToServer();
        } else {
            deviceState  = State::Disconnected;
            lastReconnMs = millis();
            applyStateLed();
            drawDisplay();
        }
    }
}


// ═══════════════════════════════════════════════════════════════════════════
//  Loop
// ═══════════════════════════════════════════════════════════════════════════

void loop() {
    esp_task_wdt_reset();

    // Provisioning mode — only run the serial provisioning handler
    if (deviceState == State::Provisioning) {
        provisioningLoop();
        buttonLoop();        // still allow factory reset via button
        return;
    }

    // Normal operation
    wifiLoop();
    tcpLoop();
    morseUpdate();
    buttonLoop();
    checkDisplayDim();
}
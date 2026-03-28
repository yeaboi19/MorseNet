using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WinFormsApp1 {

    public enum LogType { Info, Error, Warning, Success, Default };

    public partial class Form1 : Form {



        private readonly Dictionary<string, ESPDevModel> _deviceMap = new();
        // all the devices in one list
        private readonly BindingList<string> connectedDevices = new();


        // tcpServer instance
        private readonly TcpDeviceServer _server = new();

        public Form1() {
            InitializeComponent();

            // link the device list and target combo box with the connected devices list
            lstDevices.DataSource = connectedDevices;
            cbTarget.DataSource = connectedDevices;

            UpdateStatus();

            WireServerCallbacks();
            _server.Start();
            StartSimulator(); // DEBUG FOR SIMULATING DEVICES 
        }


        //DEBUG START 
#if DEBUG
        private DeviceSimulator? _sim;

        private void StartSimulator() {
            AppendLog("TEST",LogType.Info);
            _sim = new DeviceSimulator("127.0.0.1", TcpDeviceServer.Port);
            _sim.StartAll();
        }

        protected override void OnFormClosing(FormClosingEventArgs e) {
            _sim?.Dispose();
            _server.Dispose();
            base.OnFormClosing(e);
        }
#endif
        //DEBUG END


        private void Form1_Load(object sender, EventArgs e) {
            LoadColors(this);
        }

        //attach tcp server callbacks to UI
        private void WireServerCallbacks() {
            _server.OnLog = (msg, type) => SafeInvoke(() => AppendLog(msg, type) );


            _server.OnDeviceConnected = device => SafeInvoke(() => {
                _deviceMap[device.Mac] = device;
                connectedDevices.Add(device.Name);
                UpdateStatus();
                AppendLog($"{device.Name} joined the network.", LogType.Success, true);
            });

            _server.OnDeviceDisconnected = device => SafeInvoke(() => {
                _deviceMap.Remove(device.Mac);
                connectedDevices.Remove(device.Name);
                UpdateStatus();
                AppendLog($"{device.Name} left the network.", LogType.Error, true);
            });

            _server.OnMessageRecieved = (device, payload) => SafeInvoke(() => {
                string decoded = MorsecodeLookup.Decode(payload);
                AppendLog($"{device.Name} -> \"{decoded}\" [{payload}]", LogType.Default);
            });
        }

        private void UpdateStatus() =>
            lblStatus.Text = $"{connectedDevices.Count} device(s) connected";

        // marshal any action to UI thread safely
        private void SafeInvoke(Action action) {
            if (InvokeRequired) Invoke(action);
            else action();
        }


        // send the message itself with 'send' button (with edgecases)
        private void btnSend_Click(object sender, EventArgs e) {
            // some edgecases
            if (tbInput.Text == "") {
                AppendLog("Message is empty. Please write your text", LogType.Warning, true);
                return;
            }
            if (!rbBroadcast.Checked && cbTarget.SelectedItem == null) {
                AppendLog("Target device not selected.", LogType.Warning, true);
                return;
            }
            string morse = MorsecodeLookup.Encode(tbInput.Text);

            // check if we are sending broadcast or direct mode
            if (rbBroadcast.Checked) {
                if (connectedDevices.Count == 0) {
                    AppendLog("No devices connected - broadcast lost.", LogType.Warning, true);
                    return;
                }
                int sent = _server.Broadcast($"MSG:{morse}");
                AppendLog($"HOST -> BROADCAST ({sent} devices) [{morse}]", LogType.Default);
            } else {
                //find an espdevice that matches
                var device = _deviceMap.Values.FirstOrDefault(d => d.Name == cbTarget.SelectedItem?.ToString());
                if (device == null) {
                    AppendLog($"Device \"{cbTarget.Text}\" not found.", LogType.Error);
                    return;
                }
                bool ok = _server.SendTo(device, $"MSG:{morse}");
                AppendLog(ok
                    ? $"HOST -> {device.Name} [{morse}]"
                    : $"Send to {device.Name} failed - device may have dropped.",
                    ok ? LogType.Default : LogType.Error
                    );
            }

        }


        // Load colors according to the element tags
        private void LoadColors(Control parent) {
            List<Control> headerList = GetAllControlsWithTag(parent, "Header");
            List<Control> footerList = GetAllControlsWithTag(parent, "Footer");
            List<Control> focusList = GetAllControlsWithTag(parent, "Focus");
            List<Control> mainList = GetAllControlsWithTag(parent, "Main");

            ApplyColors(headerList, Theme.BackgroundFocused, Theme.Foreground);
            ApplyColors(footerList, Theme.BackgroundFocused, Theme.Foreground);
            ApplyColors(focusList, Theme.BackgroundFocused, Theme.Foreground);
            ApplyColors(mainList, Theme.Background, Theme.Foreground);
        }


        //helper function to apply the colors to elements
        private void ApplyColors(List<Control> control, Color back, Color fore) {
            foreach (Control c in control) {
                c.BackColor = back;
                c.ForeColor = fore;
            }
        }

        // recursively searches for given tag in the parents scope and returns the list of elements
        private List<Control> GetAllControlsWithTag(Control parent, string tag) {
            List<Control> list = new List<Control>();

            foreach (Control c in parent.Controls) {
                c.Tag?.ToString().Split(",").ToList().ForEach(c1 => {
                    if (c1 == tag) {
                        list.Add(c);
                    }
                });
                if (c.HasChildren) {
                    list.AddRange(GetAllControlsWithTag(c, tag));
                }
            }
            return list;
        }

        // helper function to write text in the rich text box - with colors :> 
        public void AppendLog(string message, LogType type = LogType.Default, bool bold = false) {
            Color color = type switch {
                LogType.Info => Theme.ColLogIn,
                LogType.Error => Theme.ColDanger,
                LogType.Warning => Theme.ColWarning,
                LogType.Success => Theme.ColAccent,
                _ => Theme.ColText
            };
            AppendLog(message, color, bold);
        }
        // rich textbox text helper with more manual control over colors
        public void AppendLog(string message, Color color, bool bold = false) {
            rtbLog.SelectionStart = rtbLog.TextLength;
            rtbLog.SelectionLength = 0;

            rtbLog.SelectionColor = color;
            if (bold) {
                rtbLog.SelectionFont = new Font(rtbLog.Font, FontStyle.Bold);
            } else {
                rtbLog.SelectionFont = new Font(rtbLog.Font, FontStyle.Regular);
            }

            rtbLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
        }

        // simple function to clear the richtextbox
        private void btnLogClear_Click(object sender, EventArgs e) {
            rtbLog.Clear();
        }

        // when writing in the 'Text' textbox - automatically encode to morsecode to preview 
        private void tbInput_TextChanged(object sender, EventArgs e) {
            tbPreview.Text = MorsecodeLookup.Encode(tbInput.Text);
        }


        // manipulate the target combobox according to broadcast/direct modes
        // TODO: needs working buggy and ugly
        private void rbBroadcast_CheckedChanged(object sender, EventArgs e) {
            cbTarget.Enabled = !rbBroadcast.Checked;
        }

    }
}


/*  pnlTitle -> lblTitle
 * 
 *  spltMain -> Panel1 ; Panel2
 *          Panel1 -> pnlLogHeader; rtbLog
 *          Panel2 -> spltControl
 *  pnlLogHeader -> lblMsgLog; btnLogClear
 *  spltControl -> Panel1 ; Panel2
 *          Panel1 -> lstDevices
 *          Panel2 -> pnlSend
 *  
 *  pnlSend -> lblMsg ; tblSend
 *  
 *  tblSend -> 4 row 3 column
 *         
+--------+-------------+------------------------------+-----------+
|        | Column1     | Column2                      | Column3   |
+--------+-------------+------------------------------+-----------+
| Row1   | Label       | ComboBox                     |           |
+--------+-------------+------------------------------+-----------+
| Row2   | Label       | RadioButton1 RadioButton2    |           |
+--------+-------------+------------------------------+-----------+
| Row3   | Label       | TextBox                      |           |
+--------+-------------+------------------------------+-----------+
| Row4   |             |                              | Button    |
+--------+-------------+------------------------------+-----------+
 *          
 *
 *  statStrip -> lblStatus
 */

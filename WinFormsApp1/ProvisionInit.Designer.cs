namespace WinFormsApp1 {
    partial class ProvisionInit {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent() {
            splitContainer1 = new SplitContainer();
            rtbProvLog = new RichTextBox();
            pnlRight = new Panel();
            tableLayoutPanel1 = new TableLayoutPanel();
            lblWSSID = new Label();
            lblWPass = new Label();
            lblSIP = new Label();
            lblSPort = new Label();
            tbProvWSSID = new TextBox();
            tbProvWPass = new TextBox();
            tbProvSIP = new TextBox();
            tbProvSPort = new TextBox();
            pnlPortBar = new Panel();
            btnProvConnect = new Button();
            btnProvRefresh = new Button();
            cbProvPort = new ComboBox();
            lblPort = new Label();
            pnlRightHeader = new Panel();
            lblProvTitle = new Label();
            pnlFooter = new Panel();
            btnProvFlash = new Button();
            lblProvStatus = new Label();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            pnlRight.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            pnlPortBar.SuspendLayout();
            pnlRightHeader.SuspendLayout();
            pnlFooter.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(rtbProvLog);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(pnlRight);
            splitContainer1.Size = new Size(860, 480);
            splitContainer1.SplitterDistance = 452;
            splitContainer1.TabIndex = 0;
            // 
            // rtbProvLog
            // 
            rtbProvLog.BorderStyle = BorderStyle.None;
            rtbProvLog.Dock = DockStyle.Fill;
            rtbProvLog.Location = new Point(0, 0);
            rtbProvLog.Name = "rtbProvLog";
            rtbProvLog.ReadOnly = true;
            rtbProvLog.Size = new Size(452, 480);
            rtbProvLog.TabIndex = 0;
            rtbProvLog.Tag = "Main";
            rtbProvLog.Text = "";
            // 
            // pnlRight
            // 
            pnlRight.Controls.Add(tableLayoutPanel1);
            pnlRight.Controls.Add(pnlPortBar);
            pnlRight.Controls.Add(pnlRightHeader);
            pnlRight.Controls.Add(pnlFooter);
            pnlRight.Dock = DockStyle.Fill;
            pnlRight.Location = new Point(0, 0);
            pnlRight.Name = "pnlRight";
            pnlRight.Size = new Size(404, 480);
            pnlRight.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            tableLayoutPanel1.Controls.Add(lblWSSID, 0, 0);
            tableLayoutPanel1.Controls.Add(lblWPass, 0, 1);
            tableLayoutPanel1.Controls.Add(lblSIP, 0, 2);
            tableLayoutPanel1.Controls.Add(lblSPort, 0, 3);
            tableLayoutPanel1.Controls.Add(tbProvWSSID, 1, 0);
            tableLayoutPanel1.Controls.Add(tbProvWPass, 1, 1);
            tableLayoutPanel1.Controls.Add(tbProvSIP, 1, 2);
            tableLayoutPanel1.Controls.Add(tbProvSPort, 1, 3);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 80);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.Padding = new Padding(8, 0, 8, 0);
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.Size = new Size(404, 340);
            tableLayoutPanel1.TabIndex = 2;
            tableLayoutPanel1.Tag = "Main";
            // 
            // lblWSSID
            // 
            lblWSSID.Dock = DockStyle.Fill;
            lblWSSID.Location = new Point(11, 0);
            lblWSSID.Name = "lblWSSID";
            lblWSSID.Size = new Size(129, 85);
            lblWSSID.TabIndex = 0;
            lblWSSID.Text = "WiFi SSID";
            lblWSSID.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblWPass
            // 
            lblWPass.Dock = DockStyle.Fill;
            lblWPass.Location = new Point(11, 85);
            lblWPass.Name = "lblWPass";
            lblWPass.Size = new Size(129, 85);
            lblWPass.TabIndex = 1;
            lblWPass.Text = "WiFi Password";
            lblWPass.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblSIP
            // 
            lblSIP.Dock = DockStyle.Fill;
            lblSIP.Location = new Point(11, 170);
            lblSIP.Name = "lblSIP";
            lblSIP.Size = new Size(129, 85);
            lblSIP.TabIndex = 2;
            lblSIP.Text = "Server IP";
            lblSIP.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblSPort
            // 
            lblSPort.Dock = DockStyle.Fill;
            lblSPort.Location = new Point(11, 255);
            lblSPort.Name = "lblSPort";
            lblSPort.Size = new Size(129, 85);
            lblSPort.TabIndex = 3;
            lblSPort.Text = "Server Port";
            lblSPort.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tbProvWSSID
            // 
            tbProvWSSID.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            tbProvWSSID.BorderStyle = BorderStyle.FixedSingle;
            tbProvWSSID.Location = new Point(146, 31);
            tbProvWSSID.Name = "tbProvWSSID";
            tbProvWSSID.Size = new Size(247, 23);
            tbProvWSSID.TabIndex = 4;
            tbProvWSSID.Tag = "Focus";
            // 
            // tbProvWPass
            // 
            tbProvWPass.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            tbProvWPass.BorderStyle = BorderStyle.FixedSingle;
            tbProvWPass.Location = new Point(146, 116);
            tbProvWPass.Name = "tbProvWPass";
            tbProvWPass.PasswordChar = '●';
            tbProvWPass.Size = new Size(247, 23);
            tbProvWPass.TabIndex = 5;
            tbProvWPass.Tag = "Focus";
            tbProvWPass.UseSystemPasswordChar = true;
            // 
            // tbProvSIP
            // 
            tbProvSIP.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            tbProvSIP.BorderStyle = BorderStyle.FixedSingle;
            tbProvSIP.Location = new Point(146, 201);
            tbProvSIP.Name = "tbProvSIP";
            tbProvSIP.Size = new Size(247, 23);
            tbProvSIP.TabIndex = 6;
            tbProvSIP.Tag = "Focus";
            // 
            // tbProvSPort
            // 
            tbProvSPort.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            tbProvSPort.BorderStyle = BorderStyle.FixedSingle;
            tbProvSPort.Location = new Point(146, 286);
            tbProvSPort.Name = "tbProvSPort";
            tbProvSPort.Size = new Size(247, 23);
            tbProvSPort.TabIndex = 7;
            tbProvSPort.Tag = "Focus";
            // 
            // pnlPortBar
            // 
            pnlPortBar.Controls.Add(btnProvConnect);
            pnlPortBar.Controls.Add(btnProvRefresh);
            pnlPortBar.Controls.Add(cbProvPort);
            pnlPortBar.Controls.Add(lblPort);
            pnlPortBar.Dock = DockStyle.Top;
            pnlPortBar.Location = new Point(0, 36);
            pnlPortBar.Name = "pnlPortBar";
            pnlPortBar.Padding = new Padding(8, 6, 8, 6);
            pnlPortBar.Size = new Size(404, 44);
            pnlPortBar.TabIndex = 1;
            pnlPortBar.Tag = "Footer";
            // 
            // btnProvConnect
            // 
            btnProvConnect.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            btnProvConnect.FlatStyle = FlatStyle.Flat;
            btnProvConnect.Location = new Point(293, 8);
            btnProvConnect.Name = "btnProvConnect";
            btnProvConnect.Size = new Size(90, 27);
            btnProvConnect.TabIndex = 3;
            btnProvConnect.Tag = "Focus";
            btnProvConnect.Text = "Connect";
            btnProvConnect.Click += btnProvConnect_Click;
            // 
            // btnProvRefresh
            // 
            btnProvRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            btnProvRefresh.FlatStyle = FlatStyle.Flat;
            btnProvRefresh.Location = new Point(210, 8);
            btnProvRefresh.Name = "btnProvRefresh";
            btnProvRefresh.Size = new Size(75, 27);
            btnProvRefresh.TabIndex = 2;
            btnProvRefresh.Tag = "Focus";
            btnProvRefresh.Text = "Refresh";
            btnProvRefresh.Click += btnProvRefresh_Click;
            // 
            // cbProvPort
            // 
            cbProvPort.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            cbProvPort.DropDownStyle = ComboBoxStyle.DropDownList;
            cbProvPort.FlatStyle = FlatStyle.Flat;
            cbProvPort.Location = new Point(72, 10);
            cbProvPort.Name = "cbProvPort";
            cbProvPort.Size = new Size(130, 23);
            cbProvPort.TabIndex = 1;
            cbProvPort.Tag = "Focus";
            // 
            // lblPort
            // 
            lblPort.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            lblPort.Location = new Point(8, 6);
            lblPort.Name = "lblPort";
            lblPort.Size = new Size(60, 32);
            lblPort.TabIndex = 0;
            lblPort.Text = "COM Port:";
            lblPort.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pnlRightHeader
            // 
            pnlRightHeader.Controls.Add(lblProvTitle);
            pnlRightHeader.Dock = DockStyle.Top;
            pnlRightHeader.Location = new Point(0, 0);
            pnlRightHeader.Name = "pnlRightHeader";
            pnlRightHeader.Size = new Size(404, 36);
            pnlRightHeader.TabIndex = 0;
            pnlRightHeader.Tag = "Header";
            // 
            // lblProvTitle
            // 
            lblProvTitle.Dock = DockStyle.Fill;
            lblProvTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblProvTitle.Location = new Point(0, 0);
            lblProvTitle.Name = "lblProvTitle";
            lblProvTitle.Size = new Size(404, 36);
            lblProvTitle.TabIndex = 0;
            lblProvTitle.Tag = "Header";
            lblProvTitle.Text = "ESP32 Provisioning";
            lblProvTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pnlFooter
            // 
            pnlFooter.Controls.Add(btnProvFlash);
            pnlFooter.Controls.Add(lblProvStatus);
            pnlFooter.Dock = DockStyle.Bottom;
            pnlFooter.Location = new Point(0, 420);
            pnlFooter.Name = "pnlFooter";
            pnlFooter.Padding = new Padding(8);
            pnlFooter.Size = new Size(404, 60);
            pnlFooter.TabIndex = 3;
            pnlFooter.Tag = "Footer";
            // 
            // btnProvFlash
            // 
            btnProvFlash.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            btnProvFlash.Enabled = false;
            btnProvFlash.FlatStyle = FlatStyle.Flat;
            btnProvFlash.Location = new Point(314, 10);
            btnProvFlash.Name = "btnProvFlash";
            btnProvFlash.Size = new Size(80, 40);
            btnProvFlash.TabIndex = 1;
            btnProvFlash.Tag = "Focus";
            btnProvFlash.Text = "Flash!";
            btnProvFlash.Click += btnProvFlash_Click;
            // 
            // lblProvStatus
            // 
            lblProvStatus.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            lblProvStatus.Location = new Point(8, 8);
            lblProvStatus.Name = "lblProvStatus";
            lblProvStatus.Size = new Size(370, 44);
            lblProvStatus.TabIndex = 0;
            lblProvStatus.Text = "Select a COM port and click Connect.";
            lblProvStatus.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // ProvisionInit
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(860, 480);
            Controls.Add(splitContainer1);
            MinimumSize = new Size(700, 420);
            Name = "ProvisionInit";
            Text = "MorseNet — ESP32 Provisioning";
            FormClosing += ProvisionInit_FormClosing;
            Load += ProvisionInit_Load;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            pnlRight.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            pnlPortBar.ResumeLayout(false);
            pnlRightHeader.ResumeLayout(false);
            pnlFooter.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer1;
        private RichTextBox rtbProvLog;
        private Panel pnlRight;
        private Panel pnlRightHeader;
        private Label lblProvTitle;
        private Panel pnlPortBar;
        private Label lblPort;
        private ComboBox cbProvPort;
        private Button btnProvRefresh;
        private Button btnProvConnect;
        private TableLayoutPanel tableLayoutPanel1;
        private Label lblWSSID;
        private Label lblWPass;
        private Label lblSIP;
        private Label lblSPort;
        private TextBox tbProvWSSID;
        private TextBox tbProvWPass;
        private TextBox tbProvSIP;
        private TextBox tbProvSPort;
        private Panel pnlFooter;
        private Label lblProvStatus;
        private Button btnProvFlash;
    }
}
namespace WinFormsApp1
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            pnlTitle = new Panel();
            btnServerControl = new Button();
            lblTitle = new Label();
            statStrip = new StatusStrip();
            lblStatus = new ToolStripStatusLabel();
            lstDevices = new ListBox();
            spltMain = new SplitContainer();
            rtbLog = new RichTextBox();
            pnlLogHeader = new Panel();
            btnLogClear = new Button();
            lblMsgLog = new Label();
            spltControl = new SplitContainer();
            panel2 = new Panel();
            lblDeviceList = new Label();
            panel1 = new Panel();
            tblControl = new TableLayoutPanel();
            lblTarget = new Label();
            lblMode = new Label();
            lblText = new Label();
            cbTarget = new ComboBox();
            flrbcontainer = new FlowLayoutPanel();
            rbDirect = new RadioButton();
            rbBroadcast = new RadioButton();
            tbInput = new TextBox();
            tbPreview = new TextBox();
            lblMorsePrev = new Label();
            btnSend = new Button();
            lblSendMsg = new Label();
            pnlTitle.SuspendLayout();
            statStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)spltMain).BeginInit();
            spltMain.Panel1.SuspendLayout();
            spltMain.Panel2.SuspendLayout();
            spltMain.SuspendLayout();
            pnlLogHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)spltControl).BeginInit();
            spltControl.Panel1.SuspendLayout();
            spltControl.Panel2.SuspendLayout();
            spltControl.SuspendLayout();
            panel2.SuspendLayout();
            panel1.SuspendLayout();
            tblControl.SuspendLayout();
            flrbcontainer.SuspendLayout();
            SuspendLayout();
            // 
            // pnlTitle
            // 
            pnlTitle.Controls.Add(btnServerControl);
            pnlTitle.Controls.Add(lblTitle);
            pnlTitle.Dock = DockStyle.Top;
            pnlTitle.Location = new Point(0, 0);
            pnlTitle.MaximumSize = new Size(0, 30);
            pnlTitle.MinimumSize = new Size(0, 30);
            pnlTitle.Name = "pnlTitle";
            pnlTitle.Size = new Size(724, 30);
            pnlTitle.TabIndex = 0;
            pnlTitle.Tag = "Header";
            // 
            // btnServerControl
            // 
            btnServerControl.Dock = DockStyle.Right;
            btnServerControl.FlatStyle = FlatStyle.Flat;
            btnServerControl.Location = new Point(605, 0);
            btnServerControl.Name = "btnServerControl";
            btnServerControl.Size = new Size(119, 30);
            btnServerControl.TabIndex = 1;
            btnServerControl.Tag = "Focus";
            btnServerControl.Text = "Start Server";
            btnServerControl.UseVisualStyleBackColor = true;
            btnServerControl.Click += btnServerControl_Click;
            // 
            // lblTitle
            // 
            lblTitle.Anchor = AnchorStyles.Left;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(10, 7);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(97, 15);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "ESP Morse Server";
            // 
            // statStrip
            // 
            statStrip.Items.AddRange(new ToolStripItem[] { lblStatus });
            statStrip.Location = new Point(0, 439);
            statStrip.MaximumSize = new Size(0, 22);
            statStrip.MinimumSize = new Size(0, 22);
            statStrip.Name = "statStrip";
            statStrip.Size = new Size(724, 22);
            statStrip.TabIndex = 2;
            statStrip.Tag = "Footer";
            statStrip.Text = "statStrp";
            // 
            // lblStatus
            // 
            lblStatus.DisplayStyle = ToolStripItemDisplayStyle.Text;
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(78, 17);
            lblStatus.Text = "nika was here";
            // 
            // lstDevices
            // 
            lstDevices.BorderStyle = BorderStyle.None;
            lstDevices.Dock = DockStyle.Fill;
            lstDevices.FormattingEnabled = true;
            lstDevices.HorizontalScrollbar = true;
            lstDevices.Location = new Point(0, 30);
            lstDevices.Margin = new Padding(0);
            lstDevices.Name = "lstDevices";
            lstDevices.Size = new Size(327, 148);
            lstDevices.TabIndex = 0;
            lstDevices.Tag = "Main";
            // 
            // spltMain
            // 
            spltMain.BackColor = SystemColors.Control;
            spltMain.BorderStyle = BorderStyle.FixedSingle;
            spltMain.Dock = DockStyle.Fill;
            spltMain.Location = new Point(0, 30);
            spltMain.Name = "spltMain";
            // 
            // spltMain.Panel1
            // 
            spltMain.Panel1.Controls.Add(rtbLog);
            spltMain.Panel1.Controls.Add(pnlLogHeader);
            // 
            // spltMain.Panel2
            // 
            spltMain.Panel2.Controls.Add(spltControl);
            spltMain.Size = new Size(724, 409);
            spltMain.SplitterDistance = 391;
            spltMain.TabIndex = 3;
            spltMain.Tag = "Main";
            // 
            // rtbLog
            // 
            rtbLog.BorderStyle = BorderStyle.None;
            rtbLog.Dock = DockStyle.Fill;
            rtbLog.Location = new Point(0, 30);
            rtbLog.Name = "rtbLog";
            rtbLog.Size = new Size(389, 377);
            rtbLog.TabIndex = 0;
            rtbLog.Tag = "Main";
            rtbLog.Text = "";
            // 
            // pnlLogHeader
            // 
            pnlLogHeader.Controls.Add(btnLogClear);
            pnlLogHeader.Controls.Add(lblMsgLog);
            pnlLogHeader.Dock = DockStyle.Top;
            pnlLogHeader.Location = new Point(0, 0);
            pnlLogHeader.Name = "pnlLogHeader";
            pnlLogHeader.Size = new Size(389, 30);
            pnlLogHeader.TabIndex = 1;
            pnlLogHeader.Tag = "Header";
            // 
            // btnLogClear
            // 
            btnLogClear.Dock = DockStyle.Right;
            btnLogClear.FlatStyle = FlatStyle.Flat;
            btnLogClear.Location = new Point(314, 0);
            btnLogClear.Name = "btnLogClear";
            btnLogClear.Size = new Size(75, 30);
            btnLogClear.TabIndex = 1;
            btnLogClear.Tag = "Focus";
            btnLogClear.Text = "Clear";
            btnLogClear.Click += btnLogClear_Click;
            // 
            // lblMsgLog
            // 
            lblMsgLog.Anchor = AnchorStyles.Left;
            lblMsgLog.AutoSize = true;
            lblMsgLog.Location = new Point(10, 7);
            lblMsgLog.Margin = new Padding(0);
            lblMsgLog.Name = "lblMsgLog";
            lblMsgLog.Size = new Size(81, 15);
            lblMsgLog.TabIndex = 0;
            lblMsgLog.Text = "Message Logs";
            lblMsgLog.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // spltControl
            // 
            spltControl.Dock = DockStyle.Fill;
            spltControl.Location = new Point(0, 0);
            spltControl.Name = "spltControl";
            spltControl.Orientation = Orientation.Horizontal;
            // 
            // spltControl.Panel1
            // 
            spltControl.Panel1.Controls.Add(lstDevices);
            spltControl.Panel1.Controls.Add(panel2);
            // 
            // spltControl.Panel2
            // 
            spltControl.Panel2.Controls.Add(panel1);
            spltControl.Size = new Size(327, 407);
            spltControl.SplitterDistance = 178;
            spltControl.TabIndex = 0;
            // 
            // panel2
            // 
            panel2.Controls.Add(lblDeviceList);
            panel2.Dock = DockStyle.Top;
            panel2.Location = new Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(327, 30);
            panel2.TabIndex = 0;
            panel2.Tag = "Header";
            // 
            // lblDeviceList
            // 
            lblDeviceList.Anchor = AnchorStyles.Left;
            lblDeviceList.AutoSize = true;
            lblDeviceList.Location = new Point(6, 8);
            lblDeviceList.Name = "lblDeviceList";
            lblDeviceList.Size = new Size(63, 15);
            lblDeviceList.TabIndex = 0;
            lblDeviceList.Text = "Device List";
            lblDeviceList.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panel1
            // 
            panel1.Controls.Add(tblControl);
            panel1.Controls.Add(lblSendMsg);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(327, 225);
            panel1.TabIndex = 0;
            // 
            // tblControl
            // 
            tblControl.ColumnCount = 3;
            tblControl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22.23662F));
            tblControl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 57.8806F));
            tblControl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 19.8827858F));
            tblControl.Controls.Add(lblTarget, 0, 0);
            tblControl.Controls.Add(lblMode, 0, 1);
            tblControl.Controls.Add(lblText, 0, 2);
            tblControl.Controls.Add(cbTarget, 1, 0);
            tblControl.Controls.Add(flrbcontainer, 1, 1);
            tblControl.Controls.Add(tbInput, 1, 2);
            tblControl.Controls.Add(tbPreview, 1, 3);
            tblControl.Controls.Add(lblMorsePrev, 0, 3);
            tblControl.Controls.Add(btnSend, 2, 3);
            tblControl.Dock = DockStyle.Fill;
            tblControl.Location = new Point(0, 30);
            tblControl.Margin = new Padding(0);
            tblControl.Name = "tblControl";
            tblControl.RowCount = 4;
            tblControl.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tblControl.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tblControl.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tblControl.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tblControl.Size = new Size(327, 195);
            tblControl.TabIndex = 1;
            tblControl.Tag = "Main";
            // 
            // lblTarget
            // 
            lblTarget.Anchor = AnchorStyles.None;
            lblTarget.Location = new Point(6, 7);
            lblTarget.Margin = new Padding(3);
            lblTarget.Name = "lblTarget";
            lblTarget.Size = new Size(60, 34);
            lblTarget.TabIndex = 0;
            lblTarget.Text = "Target:";
            lblTarget.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblMode
            // 
            lblMode.Anchor = AnchorStyles.None;
            lblMode.Location = new Point(6, 55);
            lblMode.Margin = new Padding(3);
            lblMode.Name = "lblMode";
            lblMode.Size = new Size(60, 34);
            lblMode.TabIndex = 1;
            lblMode.Text = "Mode:";
            lblMode.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblText
            // 
            lblText.Anchor = AnchorStyles.None;
            lblText.Location = new Point(6, 103);
            lblText.Margin = new Padding(3);
            lblText.Name = "lblText";
            lblText.Size = new Size(60, 34);
            lblText.TabIndex = 2;
            lblText.Text = "Text:";
            lblText.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cbTarget
            // 
            cbTarget.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            cbTarget.FlatStyle = FlatStyle.Popup;
            cbTarget.FormattingEnabled = true;
            cbTarget.Items.AddRange(new object[] { "Device_01", "Device_02", "Device_09", "Device_06" });
            cbTarget.Location = new Point(75, 12);
            cbTarget.Name = "cbTarget";
            cbTarget.Size = new Size(183, 23);
            cbTarget.TabIndex = 3;
            cbTarget.Tag = "Focus,Main";
            // 
            // flrbcontainer
            // 
            flrbcontainer.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            flrbcontainer.Controls.Add(rbDirect);
            flrbcontainer.Controls.Add(rbBroadcast);
            flrbcontainer.Location = new Point(72, 52);
            flrbcontainer.Margin = new Padding(0);
            flrbcontainer.Name = "flrbcontainer";
            flrbcontainer.Size = new Size(189, 40);
            flrbcontainer.TabIndex = 4;
            flrbcontainer.WrapContents = false;
            // 
            // rbDirect
            // 
            rbDirect.Checked = true;
            rbDirect.Location = new Point(3, 3);
            rbDirect.Name = "rbDirect";
            rbDirect.Size = new Size(56, 19);
            rbDirect.TabIndex = 0;
            rbDirect.TabStop = true;
            rbDirect.Text = "Direct";
            rbDirect.UseVisualStyleBackColor = true;
            // 
            // rbBroadcast
            // 
            rbBroadcast.Location = new Point(65, 3);
            rbBroadcast.Name = "rbBroadcast";
            rbBroadcast.Size = new Size(77, 19);
            rbBroadcast.TabIndex = 1;
            rbBroadcast.Text = "Broadcast";
            rbBroadcast.UseVisualStyleBackColor = true;
            rbBroadcast.CheckedChanged += rbBroadcast_CheckedChanged;
            // 
            // tbInput
            // 
            tbInput.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            tbInput.BorderStyle = BorderStyle.FixedSingle;
            tbInput.Location = new Point(75, 108);
            tbInput.Name = "tbInput";
            tbInput.PlaceholderText = "Sample text";
            tbInput.Size = new Size(183, 23);
            tbInput.TabIndex = 5;
            tbInput.Tag = "Focus";
            tbInput.TextChanged += tbInput_TextChanged;
            // 
            // tbPreview
            // 
            tbPreview.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            tbPreview.BorderStyle = BorderStyle.FixedSingle;
            tbPreview.Location = new Point(75, 158);
            tbPreview.Name = "tbPreview";
            tbPreview.PlaceholderText = "... .- -- .--. .-.. . _ - . -..- -";
            tbPreview.ReadOnly = true;
            tbPreview.Size = new Size(183, 23);
            tbPreview.TabIndex = 6;
            tbPreview.Tag = "Focus";
            // 
            // lblMorsePrev
            // 
            lblMorsePrev.Anchor = AnchorStyles.None;
            lblMorsePrev.Location = new Point(6, 152);
            lblMorsePrev.Margin = new Padding(3);
            lblMorsePrev.Name = "lblMorsePrev";
            lblMorsePrev.Size = new Size(60, 34);
            lblMorsePrev.TabIndex = 7;
            lblMorsePrev.Text = "Preview:";
            lblMorsePrev.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnSend
            // 
            btnSend.Anchor = AnchorStyles.None;
            btnSend.FlatStyle = FlatStyle.Flat;
            btnSend.Location = new Point(266, 147);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(56, 45);
            btnSend.TabIndex = 8;
            btnSend.Tag = "Focus";
            btnSend.Text = "Send";
            btnSend.UseVisualStyleBackColor = true;
            btnSend.Click += btnSend_Click;
            // 
            // lblSendMsg
            // 
            lblSendMsg.Dock = DockStyle.Top;
            lblSendMsg.Location = new Point(0, 0);
            lblSendMsg.Margin = new Padding(0);
            lblSendMsg.Name = "lblSendMsg";
            lblSendMsg.Size = new Size(327, 30);
            lblSendMsg.TabIndex = 0;
            lblSendMsg.Tag = "Header";
            lblSendMsg.Text = "Send Message";
            lblSendMsg.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(724, 461);
            Controls.Add(spltMain);
            Controls.Add(statStrip);
            Controls.Add(pnlTitle);
            MinimumSize = new Size(740, 500);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            pnlTitle.ResumeLayout(false);
            pnlTitle.PerformLayout();
            statStrip.ResumeLayout(false);
            statStrip.PerformLayout();
            spltMain.Panel1.ResumeLayout(false);
            spltMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)spltMain).EndInit();
            spltMain.ResumeLayout(false);
            pnlLogHeader.ResumeLayout(false);
            pnlLogHeader.PerformLayout();
            spltControl.Panel1.ResumeLayout(false);
            spltControl.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)spltControl).EndInit();
            spltControl.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel1.ResumeLayout(false);
            tblControl.ResumeLayout(false);
            tblControl.PerformLayout();
            flrbcontainer.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel pnlTitle;
        private StatusStrip statStrip;
        private ToolStripStatusLabel lblStatus;
        private Label lblTitle;
        private ListBox lstDevices;
        private SplitContainer spltMain;
        private SplitContainer spltControl;
        private RichTextBox rtbLog;
        private Label lblSendMsg;
        private TableLayoutPanel tblControl;
        private Label lblTarget;
        private Label lblMode;
        private Label lblText;
        private ComboBox cbTarget;
        private FlowLayoutPanel flrbcontainer;
        private RadioButton rbDirect;
        private RadioButton rbBroadcast;
        private TextBox tbInput;
        private TextBox tbPreview;
        private Label lblMorsePrev;
        private Button btnSend;
        private Panel pnlLogHeader;
        private Label lblMsgLog;
        private Button btnLogClear;
        private Panel panel1;
        private Panel panel2;
        private Label lblDeviceList;
        private Button btnServerControl;
    }
}

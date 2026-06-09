namespace WinFormsApp1 {
    partial class LoginForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            label1 = new Label();
            tableLayoutPanel1 = new TableLayoutPanel();
            label2 = new Label();
            label3 = new Label();
            tbUsr = new TextBox();
            tbPwd = new TextBox();
            panel2 = new Panel();
            lblError = new Label();
            btnRegister = new Button();
            btnLogin = new Button();
            tableLayoutPanel1.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.Dock = DockStyle.Top;
            label1.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label1.Location = new Point(0, 0);
            label1.Name = "label1";
            label1.Size = new Size(384, 38);
            label1.TabIndex = 0;
            label1.Tag = "Header";
            label1.Text = "MorseNet Login";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            tableLayoutPanel1.Controls.Add(label2, 0, 0);
            tableLayoutPanel1.Controls.Add(label3, 0, 1);
            tableLayoutPanel1.Controls.Add(tbUsr, 1, 0);
            tableLayoutPanel1.Controls.Add(tbPwd, 1, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 38);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.Padding = new Padding(0, 0, 5, 0);
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new Size(384, 254);
            tableLayoutPanel1.TabIndex = 0;
            tableLayoutPanel1.Tag = "Main";
            // 
            // label2
            // 
            label2.Dock = DockStyle.Fill;
            label2.Location = new Point(3, 0);
            label2.Name = "label2";
            label2.Size = new Size(126, 127);
            label2.TabIndex = 3;
            label2.Text = "Username: ";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            label3.Dock = DockStyle.Fill;
            label3.Location = new Point(3, 127);
            label3.Name = "label3";
            label3.Size = new Size(126, 127);
            label3.TabIndex = 4;
            label3.Text = "Password: ";
            label3.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tbUsr
            // 
            tbUsr.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            tbUsr.BorderStyle = BorderStyle.FixedSingle;
            tbUsr.Location = new Point(135, 52);
            tbUsr.Name = "tbUsr";
            tbUsr.Size = new Size(241, 23);
            tbUsr.TabIndex = 5;
            tbUsr.Tag = "Focus";
            // 
            // tbPwd
            // 
            tbPwd.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            tbPwd.BorderStyle = BorderStyle.FixedSingle;
            tbPwd.Location = new Point(135, 179);
            tbPwd.Name = "tbPwd";
            tbPwd.Size = new Size(241, 23);
            tbPwd.TabIndex = 6;
            tbPwd.Tag = "Focus";
            tbPwd.UseSystemPasswordChar = true;
            tbPwd.KeyDown += tbPwd_KeyDown;
            // 
            // panel2
            // 
            panel2.Controls.Add(lblError);
            panel2.Controls.Add(btnRegister);
            panel2.Controls.Add(btnLogin);
            panel2.Dock = DockStyle.Bottom;
            panel2.Location = new Point(0, 292);
            panel2.Name = "panel2";
            panel2.Padding = new Padding(0, 17, 0, 17);
            panel2.Size = new Size(384, 69);
            panel2.TabIndex = 2;
            panel2.Tag = "Footer";
            // 
            // lblError
            // 
            lblError.Dock = DockStyle.Left;
            lblError.Location = new Point(0, 17);
            lblError.Name = "lblError";
            lblError.Size = new Size(226, 35);
            lblError.TabIndex = 2;
            // 
            // btnRegister
            // 
            btnRegister.Dock = DockStyle.Right;
            btnRegister.FlatStyle = FlatStyle.Flat;
            btnRegister.Location = new Point(234, 17);
            btnRegister.Margin = new Padding(5);
            btnRegister.Name = "btnRegister";
            btnRegister.Size = new Size(75, 35);
            btnRegister.TabIndex = 1;
            btnRegister.Tag = "Focus";
            btnRegister.Text = "Register";
            btnRegister.UseVisualStyleBackColor = true;
            btnRegister.Click += btnRegister_Click;
            // 
            // btnLogin
            // 
            btnLogin.Dock = DockStyle.Right;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.Location = new Point(309, 17);
            btnLogin.Margin = new Padding(5);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(75, 35);
            btnLogin.TabIndex = 0;
            btnLogin.Tag = "Focus";
            btnLogin.Text = "Login";
            btnLogin.UseVisualStyleBackColor = true;
            btnLogin.Click += btnLogin_Click;
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(384, 361);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(label1);
            Controls.Add(panel2);
            MinimumSize = new Size(400, 400);
            Name = "LoginForm";
            Text = "LoginForm";
            Load += LoginForm_Load;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            panel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Label label1;
        private TableLayoutPanel tableLayoutPanel1;
        private Label label2;
        private Label label3;
        private TextBox tbUsr;
        private TextBox tbPwd;
        private Panel panel2;
        private Button btnRegister;
        private Button btnLogin;
        private Label lblError;
    }
}
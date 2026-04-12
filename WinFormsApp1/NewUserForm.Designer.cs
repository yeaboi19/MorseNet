namespace WinFormsApp1 {
    partial class NewUserForm {
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
            panel1 = new Panel();
            panel2 = new Panel();
            tableLayoutPanel1 = new TableLayoutPanel();
            btnCancel = new Button();
            btnCreateUsr = new Button();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            tbNewUser = new TextBox();
            tbNewPass = new TextBox();
            tbCheckPass = new TextBox();
            lblError = new Label();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.Dock = DockStyle.Top;
            label1.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            label1.Location = new Point(0, 0);
            label1.Name = "label1";
            label1.Size = new Size(384, 35);
            label1.TabIndex = 0;
            label1.Tag = "Header";
            label1.Text = "Create New Account";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            panel1.Controls.Add(tableLayoutPanel1);
            panel1.Controls.Add(panel2);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 35);
            panel1.Name = "panel1";
            panel1.Size = new Size(384, 326);
            panel1.TabIndex = 1;
            // 
            // panel2
            // 
            panel2.Controls.Add(lblError);
            panel2.Controls.Add(btnCreateUsr);
            panel2.Controls.Add(btnCancel);
            panel2.Dock = DockStyle.Bottom;
            panel2.Location = new Point(0, 257);
            panel2.MinimumSize = new Size(0, 69);
            panel2.Name = "panel2";
            panel2.Padding = new Padding(0, 17, 0, 17);
            panel2.Size = new Size(384, 69);
            panel2.TabIndex = 0;
            panel2.Tag = "Footer";
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(label2, 0, 0);
            tableLayoutPanel1.Controls.Add(label3, 0, 1);
            tableLayoutPanel1.Controls.Add(label4, 0, 2);
            tableLayoutPanel1.Controls.Add(tbNewUser, 1, 0);
            tableLayoutPanel1.Controls.Add(tbNewPass, 1, 1);
            tableLayoutPanel1.Controls.Add(tbCheckPass, 1, 2);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.Size = new Size(384, 257);
            tableLayoutPanel1.TabIndex = 1;
            tableLayoutPanel1.Tag = "Main";
            // 
            // btnCancel
            // 
            btnCancel.Dock = DockStyle.Right;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Location = new Point(309, 17);
            btnCancel.Margin = new Padding(7, 3, 3, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 35);
            btnCancel.TabIndex = 0;
            btnCancel.Tag = "Focus";
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnCreateUsr
            // 
            btnCreateUsr.Dock = DockStyle.Right;
            btnCreateUsr.FlatStyle = FlatStyle.Flat;
            btnCreateUsr.Location = new Point(234, 17);
            btnCreateUsr.Margin = new Padding(3, 3, 7, 3);
            btnCreateUsr.Name = "btnCreateUsr";
            btnCreateUsr.Size = new Size(75, 35);
            btnCreateUsr.TabIndex = 1;
            btnCreateUsr.Tag = "Focus";
            btnCreateUsr.Text = "Create";
            btnCreateUsr.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.None;
            label2.Location = new Point(46, 31);
            label2.Name = "label2";
            label2.Size = new Size(100, 23);
            label2.TabIndex = 0;
            label2.Text = "Username:";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.None;
            label3.Location = new Point(46, 116);
            label3.Name = "label3";
            label3.Size = new Size(100, 23);
            label3.TabIndex = 1;
            label3.Text = "Password:";
            label3.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.None;
            label4.Location = new Point(36, 202);
            label4.Name = "label4";
            label4.Size = new Size(120, 23);
            label4.TabIndex = 2;
            label4.Text = "Confirm Password:";
            label4.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tbNewUser
            // 
            tbNewUser.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            tbNewUser.Location = new Point(195, 31);
            tbNewUser.Name = "tbNewUser";
            tbNewUser.Size = new Size(186, 23);
            tbNewUser.TabIndex = 3;
            tbNewUser.Tag = "Focus";
            // 
            // tbNewPass
            // 
            tbNewPass.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            tbNewPass.Location = new Point(195, 116);
            tbNewPass.Name = "tbNewPass";
            tbNewPass.PasswordChar = '●';
            tbNewPass.Size = new Size(186, 23);
            tbNewPass.TabIndex = 4;
            tbNewPass.Tag = "Focus";
            tbNewPass.UseSystemPasswordChar = true;
            // 
            // tbCheckPass
            // 
            tbCheckPass.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            tbCheckPass.Location = new Point(195, 202);
            tbCheckPass.Name = "tbCheckPass";
            tbCheckPass.PasswordChar = '●';
            tbCheckPass.Size = new Size(186, 23);
            tbCheckPass.TabIndex = 5;
            tbCheckPass.Tag = "Focus";
            tbCheckPass.UseSystemPasswordChar = true;
            // 
            // lblError
            // 
            lblError.Dock = DockStyle.Left;
            lblError.Location = new Point(0, 17);
            lblError.Name = "lblError";
            lblError.Size = new Size(198, 35);
            lblError.TabIndex = 2;
            lblError.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // NewUserForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(384, 361);
            Controls.Add(panel1);
            Controls.Add(label1);
            MinimumSize = new Size(400, 400);
            Name = "NewUserForm";
            Text = "NewUserForm";
            Load += NewUserForm_Load;
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label label1;
        private Panel panel1;
        private Panel panel2;
        private TableLayoutPanel tableLayoutPanel1;
        private Button btnCreateUsr;
        private Button btnCancel;
        private Label label2;
        private Label label3;
        private Label label4;
        private TextBox tbNewUser;
        private TextBox tbNewPass;
        private TextBox tbCheckPass;
        private Label lblError;
    }
}
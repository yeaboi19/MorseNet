using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WinFormsApp1 {
    public partial class LoginForm : Form {
        public LoginForm() {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e) {
            Theme.LoadColors(this);
        }


        private void btnLogin_Click(object sender, EventArgs e) {
            string username = tbUsr.Text.Trim();
            string password = tbPwd.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) {
                ShowError("Please enter your username and password"); return;
            }

            if (UserStore.Authenticate(username, password)) {
                DialogResult = DialogResult.OK;
                Close();
            } else {
                ShowError("Incorrect Username or Password");
                tbUsr.Clear();
                tbPwd.Clear();
            }
        }

        private void btnRegister_Click(object sender, EventArgs e) {
            using var dlg = new NewUserForm();
            if (dlg.ShowDialog(this) == DialogResult.OK) {
                lblError.Text = $"User \'{dlg.CreatedUsername}\' was created, you may log in.";
                lblError.ForeColor = Theme.ColAccent;
                tbUsr.Text = dlg.CreatedUsername;
                tbPwd.Clear();
                tbPwd.Focus();
            }
        }

        private void tbPwd_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) btnLogin_Click(sender, e);
        }


        private void ShowError(string message) {
            lblError.Text = message;
            lblError.ForeColor = Theme.ColDanger;
        }

    }
}

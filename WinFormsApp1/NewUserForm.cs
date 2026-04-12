using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WinFormsApp1 {
    public partial class NewUserForm : Form {

        public string CreatedUsername { get; private set; } = string.Empty;

        public NewUserForm() {
            InitializeComponent();
        }

        private void NewUserForm_Load(object sender, EventArgs e) {
            Theme.LoadColors(this);
        }

        

        private void btnCreateUsr_Click(object sender, EventArgs e) {
            string username = tbNewUser.Text.Trim();
            string password = tbNewPass.Text;
            string check = tbCheckPass.Text;

            if (string.IsNullOrEmpty(username)) {
                ShowError("Username must not be empty");
                return;
            } else if (username.Length < 3) {
                ShowError("Username must be at least 3 characters");
                return;
            } else if (string.IsNullOrEmpty(password)) {
                ShowError("Password must not be empty");
                return;
            } else if (password.Length < 4) {
                ShowError("Password must be at least 4 characters");
                return;
            } else if (password != check) {
                ShowError("Passwords do not match");
                tbNewPass.Clear();
                tbCheckPass.Clear();
                tbNewPass.Focus();
                return;
            } else if (UserStore.UserExists(username)) {
                ShowError($"Username \"{username}\" is taken");
            }

            UserStore.CreateUser(username, password);
            CreatedUsername = username;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void tbCheckPass_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) btnCreateUsr_Click(sender, e);
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }


        private void ShowError(String msg) {
            lblError.Text = msg;
            lblError.ForeColor = Theme.ColDanger;
        }
    }
}

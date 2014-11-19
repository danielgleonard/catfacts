using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Google.Voice;

namespace GoogleVoice.NET
{
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            SetUp();
        }

        private void btnSignIn_Click(object sender, EventArgs e)
        {
            groupBox1.Enabled = false;

            var loginResult = Program.GoogleVoice.Login(txtEmail.Text, txtPassword.Text);

            if (!loginResult.RequiresRelogin)
            {
                Hide();

                if (!Settings.Default.EmailCache.Contains(txtEmail.Text))
                {
                    Settings.Default.EmailCache.Add(txtEmail.Text);
                    Settings.Default.Save();
                }

                txtPassword.Text = "";

                Program.ControlsForm = new FormControls();
                Program.ControlsForm.Show();
                return;
            }

            MessageBox.Show("The username or password you entered is incorrect.");
            groupBox1.Enabled = true;
        }

        private void ReloadAutoCompleteValues()
        {
            txtEmail.AutoCompleteCustomSource = new AutoCompleteStringCollection();
            if (Settings.Default.EmailCache == null)
            {
                Settings.Default.EmailCache = new System.Collections.Specialized.StringCollection();
                Settings.Default.Save();
            }
            else
            {
                foreach (string s in Settings.Default.EmailCache)
                {
                    if (!txtEmail.AutoCompleteCustomSource.Contains(s))
                    {
                        txtEmail.AutoCompleteCustomSource.Add(s);
                    }
                }
            }
            txtEmail.Refresh();
        }

        public new void Show()
        {
            SetUp();
            base.Show();
            
            groupBox1.Enabled = true;
        }

        private void SetUp()
        {
            ReloadAutoCompleteValues();
            if (!string.IsNullOrEmpty(Program.GoogleVoice.Session))
            {
                txtEmail.Enabled = false;
                txtPassword.Focus();
            }
            else if (!string.IsNullOrEmpty(txtEmail.Text))
            {
                txtEmail.Enabled = true;
                txtPassword.Focus();
            }
            else
            {
                txtEmail.Enabled = true;
                txtEmail.Focus();
            }
        }

        private void FormLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (!string.IsNullOrEmpty(Program.GoogleVoice.Session)) Program.GoogleVoice.Logout();
        }

        private void DetectEnter(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                e.Handled = true;
                btnSignIn_Click(sender, (e as EventArgs));
            }
        }
    }
}

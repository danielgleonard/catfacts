using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Google.Voice;
using Google.Voice.Extensions;
using Google.Voice.Entities;
using System.Text.RegularExpressions;

namespace GoogleVoice.NET
{
    public partial class FormControls : Form
    {
        private bool doResetEvents = true;
        private AutoCompleteStringCollection contactsAutoComplete = new AutoCompleteStringCollection();
        private bool forceClose = false;
        private GoogleVoiceEventArgs CurrentEvent;
        bool creating = false;
        private string CurrendEventID 
        {
            get
            {
                return CurrentEvent.ID;
            }
            set
            {
                CurrentEvent.ID = value;
            }
        }

        public FormControls()
        {
            InitializeComponent();
        }

        private void FormControls_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!forceClose)
            {
                e.Cancel = true;
                WindowState = FormWindowState.Minimized;
            }
            else
            {
                Program.LoginForm.Show();
            }
        }

        private void lblSignOut_Click(object sender, EventArgs e)
        {
            Program.HaltEvents = true;
            Program.GoogleVoice.Logout();
            forceClose = true;
            Close();
        }

        private void lblHide_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void lblLock_Click(object sender, EventArgs e)
        {
            Hide();
            Program.LoginForm.Show();
        }

        private void btnCall_Click(object sender, EventArgs e)
        {
            try
            {
                if (chkRemember.Checked)
                {
                    Settings.Default.DefaultPhoneName = (listPhones.SelectedItem as Variable).Name;
                    Settings.Default.Save();
                }

                string to = "0";

                if (btnContactsCall.Checked)
                {
                    //to = (listContactsCall.SelectedItem as Contact).phoneNumber;
                    Match match = Regex.Match(listContactsCall.SelectedItem.ToString(), @"\d{11}");
                    if (match.Success)
                    {
                        to = match.Value;
                    }
                }
                else
                {
                    to = LoadAutoCompleteString(txtCall.Text).phoneNumber;
                }
                string from = "0";
                Match match2 = Regex.Match((listPhones.SelectedItem as ForwardingPhone).phoneNumber, @"\d{11}");
                if (match2.Success)
                {
                    from = match2.Value;
                }
                Program.GoogleVoice.Call(from, to);
                btnCancelCall.Enabled = true;
                timerCancelCall.Enabled = true;
                btnCall.Enabled = false;
                try
                {
                    var lstItem = new ListViewItem();
                    string call = "Call to " + to + " at " + DateTime.Now;
                    lstItem.Text = call;
                    StreamWriter writer = File.AppendText("History.txt");
                    writer.WriteLine(call);
                    writer.Close();
                    lstHistory.Items.Add(lstItem);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            catch
            {
                MessageBox.Show(
                    "You supplied an invalid number or contact to call. " + 
                    Environment.NewLine + 
                    "Only enter numbers; Example: 8008675309");
            }
        }

        private Contact LoadAutoCompleteString(string p)
        {
            if (IsNumeric(p))
            {
                return new Contact()
                {
                    name = "Unknown",
                    phoneNumber = Convert.ToInt64(p).ToString()
                };
            }

            foreach (var pair in Program.GoogleVoice.Account.contacts)
            {
                foreach (PhoneNumber c in pair.Value.numbers)
                {
                    if (p.ToUpper() == c.ToString().ToUpper())
                    {
                        return pair.Value;
                    }
                }
            }

            string number = "";

            foreach (char c in p.ToCharArray())
            {
                if (IsNumeric(c.ToString())) number += c.ToString();
            }

            if (number.Length >= 10)
            {
                return new Contact()
                {
                    name = "Unknown",
                    phoneNumber = Convert.ToInt64(number).ToString()
                };
            }

            return new Contact()
            {
                name = "Unknown",
                phoneNumber = Convert.ToInt64(0).ToString()
            };
        }

        private bool IsNumeric(string s)
        {
            try
            {
                decimal n = Convert.ToDecimal(s);
            }
            catch
            {
                return false;
            }
            return true;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                object to = 0;

                if (btnContactsText.Checked)
                {
                    //to = (listContactsText.SelectedItem as Contact).phoneNumber;
                    Match match = Regex.Match(listContactsText.SelectedItem.ToString(), @"\d{11}");
                    if (match.Success)
                    {
                        to = match.Value;
                    }
                }
                else
                {
                    to = LoadAutoCompleteString(txtTextNumber.Text).phoneNumber;
                }

                Program.GoogleVoice.SMS(to.ToString(), txtText.Text);
                try
                {
                    var lstItem = new ListViewItem();
                    string text = "SMS: " + to.ToString() + " - \"" + txtText.Text + "\" at " + DateTime.Now;
                    lstItem.Text = text;
                    lstHistory.Items.Add(lstItem);
                    StreamWriter writer = File.AppendText("History.txt");
                    writer.WriteLine(text);
                    writer.Close();
                }
                catch (Exception)
                {
                    throw;
                }
                txtText.Text = "";
                MessageBox.Show("The message has been sent!");
            }
            catch
            {
                MessageBox.Show(
                    "You supplied an invalid number or contact to send a SMS. " + 
                    Environment.NewLine + 
                    "Only enter numbers; Example: 8008675309");
            }
        }

        private void FormControls_Load(object sender, EventArgs e)
        {
            int i = 0;
            foreach (var pair in Program.GoogleVoice.Account.phones)
            {
                ForwardingPhone variable = pair.Value;
                listPhones.Items.Add(variable);
                listPhonesSchedule.Items.Add(variable);
                if (variable.name == Settings.Default.DefaultPhoneName)
                {
                    listPhones.SelectedIndex = i;
                    listPhonesSchedule.SelectedIndex = i;
                }
                i++;
            }

            foreach (var pair in Program.GoogleVoice.Account.contacts)
            {
                var contact = pair.Value;
                try
                {
                    //foreach (PhoneNumber contactPhone in contact.numbers)
                    //{
                    string number = contact.name + " " + contact.phoneNumber;
                        contactsAutoComplete.Add(number);

                        listContactsScheduleDest.Items.Add(number);
                        listContactsCall.Items.Add(number);
                        listContactsText.Items.Add(number);
                    //}
                }
                catch (Exception)
                {
                }
            }

            txtCall.AutoCompleteCustomSource = contactsAutoComplete;
            txtTextNumber.AutoCompleteCustomSource = contactsAutoComplete;
            txtScheduleDest.AutoCompleteCustomSource = contactsAutoComplete;
            StreamReader reader = new StreamReader("History.txt");
            string text = "";
            while (!reader.EndOfStream)
            {
                text = reader.ReadLine();
                var lstItem = new ListViewItem();
                lstItem.Text = text;
                lstHistory.Items.Add(lstItem);
            };
            reader.Close();

            ResetEvents();

            GoogleVoiceEventWatcher.Run(Program.GoogleVoice);
        }

        private void timerCancelCall_Tick(object sender, EventArgs e)
        {
            btnCancelCall.Enabled = false;
            timerCancelCall.Enabled = false;
            btnCall.Enabled = true;
        }

        private void btnCancelCall_Click(object sender, EventArgs e)
        {
            //long phone = (listPhones.SelectedItem as Variable).AsLong;
            //Program.GoogleVoice.CancelCall(Convert.ToInt64(txtCall.Text), phone);
            btnCancelCall.Enabled = false;
            timerCancelCall.Enabled = false;
            btnCall.Enabled = true;
        }


        private void btnContactsText_CheckedChanged(object sender, EventArgs e) { }
        private void btnContactsText_Click(object sender, EventArgs e)
        {
            btnContactsText.Checked = !btnContactsText.Checked;
            listContactsText.Visible = btnContactsText.Checked;
            if (btnContactsText.Checked) listContactsText.Focus();
        }

        private void btnContactsCall_CheckedChanged(object sender, EventArgs e) { }
        private void btnContactsCall_Click(object sender, EventArgs e)
        {
            btnContactsCall.Checked = !btnContactsCall.Checked;
            listContactsCall.Visible = btnContactsCall.Checked;
            txtCall.Visible = !btnContactsCall.Checked;
            if (btnContactsCall.Checked) listContactsCall.Focus();
        }

        private void btnScheduleEventType_Click(object sender, EventArgs e)
        {
            btnScheduleEventType.Text = (btnScheduleEventType.Text == "SMS" ? "Call" : "SMS");
        }

        private void chkContactsScheduleDest_CheckedChanged(object sender, EventArgs e) { }
        private void chkContactsScheduleDest_Click(object sender, EventArgs e)
        {
            chkContactsScheduleDest.Checked = !chkContactsScheduleDest.Checked;
            listContactsScheduleDest.Visible = chkContactsScheduleDest.Checked;
            if (chkContactsScheduleDest.Checked) listContactsScheduleDest.Focus();
        }

        private void listEvents_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentEvent = (listEvents.SelectedItem as GoogleVoiceEventArgs);

            creating = false;

            pnlEventAdd.Visible = false;
            pnlEventManager.Visible = true;
            pnlEventPicker.Visible = false;

            btnScheduleEventType.Text = (CurrentEvent.Type == VoiceEventType.Call ? "Call" : "SMS");

            chkContactsScheduleDest.Checked = false;
            listContactsScheduleDest.Visible = false;
            txtScheduleDest.Text = CurrentEvent.ContactPhoneNumber;

            int i = 0;
            foreach (var variable in Program.GoogleVoice.Account.phones)
            {
                if (variable.Value.ToString() == CurrentEvent.OutgoingPhoneNumber)
                {
                    listPhonesSchedule.SelectedIndex = i;
                }
                i++;
            }

            txtScheduleText.Text = CurrentEvent.TextMessage;
            try
            {
                datePickerScheduleDateTime.Value = CurrentEvent.Time;
            }
            catch
            {

            }
            btnScheduleDelete.Enabled = true;
        }

        private void lblScheduleCall_Click(object sender, EventArgs e)
        {
            creating = true;
            Schedule("Call");
        }

        private void lblScheduleSMS_Click(object sender, EventArgs e)
        {
            creating = true;
            Schedule("SMS");
        }

        private void Schedule(string type)
        {
            creating = true;

            pnlEventAdd.Visible = false;
            pnlEventManager.Visible = true;
            pnlEventPicker.Visible = false;

            btnScheduleEventType.Text = type;

            lblModType.Text = "Add an Event...";

            btnScheduleSave.Enabled = true;
            btnScheduleCancel.Enabled = true;
        }

        private void btnScheduleCancel_Click(object sender, EventArgs e)
        {
            creating = false;

            ResetEvents();

            pnlEventManager.Visible = false;
            pnlEventAdd.Visible = true;
            pnlEventPicker.Visible = true;
        }

        private void btnScheduleDelete_Click(object sender, EventArgs e)
        {
            creating = false;
            Program.VoiceEvents.DeleteEvent(CurrendEventID);
            Program.SaveVoiceEvents();

            ResetEvents();

            pnlEventAdd.Visible = true;
            pnlEventManager.Visible = false;
            pnlEventPicker.Visible = true;
        }

        public long ToLong(string number)
        {
            try
            {
                return Convert.ToInt64(number);
            }
            catch
            {
                return 0;
            }
        }

        private void btnScheduleSave_Click(object sender, EventArgs e)
        {
            if (datePickerScheduleDateTime.Value > DateTime.Now)
            {
                GoogleVoiceEventArgs ev = (creating ? new GoogleVoiceEventArgs() : CurrentEvent);
                ev.Type = (btnScheduleEventType.Text == "Call" ? VoiceEventType.Call : VoiceEventType.SMS);

                Contact destination;

                if (chkContactsScheduleDest.Checked)
                {
                    destination = (listContactsScheduleDest.SelectedItem as Contact);
                }
                else
                {
                    destination = LoadAutoCompleteString(txtScheduleDest.Text);
                }

                ev.ContactName = destination.name;
                ev.ContactPhoneNumber = destination.phoneNumber;

                ev.Handled = false;
                ev.OutgoingPhoneNumber = (listPhonesSchedule.SelectedItem as Variable).ToLong().ToString();
                ev.TextMessage = txtScheduleText.Text;
                ev.Time = datePickerScheduleDateTime.Value;
                
                Program.VoiceEvents.UpdateEvent(ev);
                Program.SaveVoiceEvents();

                MessageBox.Show("The event has been scheduled, but will only occur if this " +
                    "application is running and you are connected to the internet.");

                ResetEvents();

                creating = false;
                pnlEventManager.Visible = false;
                pnlEventAdd.Visible = true;
                pnlEventPicker.Visible = true;
            }
            else
            {
                MessageBox.Show("You may only schedule future events.");
            }

            Settings.Default.Save();
            btnScheduleDelete.Enabled = false;
        }

        private void ResetEvents()
        {
            if (doResetEvents == true)
            {
                doResetEvents = false;
                datePickerScheduleDateTime.MinDate = DateTime.Now;

                string s = (listEventDates.SelectedItem == null ? "" : listEventDates.SelectedItem.ToString());
                listEventDates.Items.Clear();
                listEventDates.Refresh();
                listEvents.Items.Clear();
                listEvents.Refresh();

                int i = 0;
                foreach (GoogleVoiceEventArgs e in Program.VoiceEvents.Sorted)
                {
                    if (!listEventDates.Items.Contains(e.Time.ToShortDateString()))
                    {
                        listEventDates.Items.Add(e.Time.ToShortDateString());
                        if (i == 0)
                        {
                            listEventDates.SelectedIndex = 0;
                        }

                        if (listEventDates.Items[i].ToString() == s)
                        {
                            listEventDates.SelectedIndex = i;
                        }
                        i++;
                    }
                }
                foreach (GoogleVoiceEventArgs ev in Program.VoiceEvents.Sorted)
                {
                    if (ev.Time.ToShortDateString() == DateTime.Parse(listEventDates.SelectedItem.ToString()).ToShortDateString())
                    {
                        listEvents.Items.Add(ev);
                    }
                }
                doResetEvents = true;
            }
        }

        private void listEventDates_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetEvents();
        }
    }
}

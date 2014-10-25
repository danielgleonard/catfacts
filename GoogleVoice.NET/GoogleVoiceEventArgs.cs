using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.Voice;

namespace GoogleVoice.NET
{
    public class GoogleVoiceEventArgs : EventArgs
    {
        private const string BAR_REPLACEMENT = "18424a53*0e5a8e7e!1e2f8af1^1dc278b8@379b0bca";

        public GoogleVoiceEventArgs()
        {

        }

        public static GoogleVoiceEventArgs Wake(string serializationString)
        {
            GoogleVoiceEventArgs e = new GoogleVoiceEventArgs();

            string[] values = serializationString.Split('|');

            e.id = values[0];
            e.contactName = values[1];
            e.contactPhoneNumber = values[2];
            e.outgoingPhoneNumber = values[3];
            try
            {
                e.time = DateTime.FromBinary(Convert.ToInt64(values[4]));
            }
            catch
            {
                e.time = DateTime.Parse("1/1/1970");
            }
            e.textMessage = values[5];
            try
            {
                e.type = (VoiceEventType)Enum.Parse(typeof(VoiceEventType), values[6]);
            }
            catch
            {
                e.type = VoiceEventType.Call;
            }

            return e;
        }

        public string Sleep()
        {
            return Sleep(this);
        }

        public static string Sleep(GoogleVoiceEventArgs e)
        {
            string s = e.id + "|" + e.contactName + "|" + e.contactPhoneNumber + "|" + e.outgoingPhoneNumber;
            s += "|" + e.time.ToBinary().ToString() + "|" + e.textMessage + "|" + e.type.ToString();

            return s;
        }

        static Guid newguid = Guid.NewGuid();
        private string id = newguid.ToString();
        private string contactName, contactPhoneNumber, outgoingPhoneNumber, textMessage;
        private DateTime time;
        private VoiceEventType type;
        private bool handled = false;

        public string ID
        {
            get { return id; }
            set { id = value; }
        }
        public string ContactName
        {
            get { return Deserialize(contactName); }
            set { contactName = Serialize(value); }
        }

        public string ContactPhoneNumber
        {
            get { return Deserialize(contactPhoneNumber); }
            set { contactPhoneNumber = Serialize(value); }
        }

        public string OutgoingPhoneNumber
        {
            get { return Deserialize(outgoingPhoneNumber); }
            set { outgoingPhoneNumber = Serialize(value); }
        }

        public string TextMessage
        {
            get { return Deserialize(textMessage); }
            set { textMessage = Serialize(value); }
        }

        public DateTime Time
        {
            get { return time; }
            set { time = value; }
        }

        private static string Serialize(string s)
        {
            if (s == null) s = "";
            return s.Replace("|", BAR_REPLACEMENT);
        }

        private static string Deserialize(string s)
        {
            return s.Replace(BAR_REPLACEMENT, "|");
        }

        public VoiceEventType Type
        {
            get { return type; }
            set { type = value; }
        }

        public bool Handled
        {
            get { return handled; }
            set { handled = value; }
        }

        public override string ToString()
        {
            string e = "";
            if (type == VoiceEventType.Call)
            {
                e += "Call: ";
            }
            else
            {
                e += "SMS: ";
            }
            return e + ContactName + " " + ContactPhoneNumber + " at " + Time.ToString();
        }

        internal void Handle(Google.Voice.GoogleVoice c)
        {
            Handled = true;

            if (type == VoiceEventType.Call)
            {
                c.Call(ContactPhoneNumber, OutgoingPhoneNumber);
            }
            else if (type == VoiceEventType.SMS)
            {
                //c.SMS(c.GoogleNumber.ToString(), "Sending SMS to " + contactName + ": " + TextMessage);
                c.SMS(ContactPhoneNumber, TextMessage);
            }
        }
    }
}

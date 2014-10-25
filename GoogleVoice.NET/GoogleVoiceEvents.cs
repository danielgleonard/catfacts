using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Google.Voice;

namespace GoogleVoice.NET
{
    public class GoogleVoiceEvents
    {
        public GoogleVoiceEvents(StringCollection s)
        {
            foreach (string e in s)
            {
                Items.Add(GoogleVoiceEventArgs.Wake(e));
            }
        }

        public List<GoogleVoiceEventArgs> Sorted
        {
            get
            {
                Items.Sort(delegate(GoogleVoiceEventArgs e1, GoogleVoiceEventArgs e2) { return e1.Time.CompareTo(e2.Time); });
                return Items;
            }
        }

        public StringCollection Serialize()
        {
            StringCollection s = new StringCollection();
            foreach (GoogleVoiceEventArgs e in Items)
            {
                s.Add(e.Sleep());
            }
            return s;
        }

        public void DeleteEvent(string id)
        {
            int i = 0;
            bool found = false;
            foreach (GoogleVoiceEventArgs e in Items)
            {
                if (e.ID == id)
                {
                    found = true;
                    break;
                }
                i++;
            }

            if (found)
            {
                Items.RemoveAt(i);
            }

            Settings.Default.GoogleVoiceEvents = Serialize();
            Settings.Default.Save();
        }

        public void UpdateEvent(GoogleVoiceEventArgs ev)
        {
            int i = 0;
            bool found = false;
            foreach (GoogleVoiceEventArgs e in Items)
            {
                if (e.ID == ev.ID)
                {
                    found = true;
                    break;
                }
                i++;
            }

            if (found)
            {
                Items[i] = ev;
            }
            else
            {
                Items.Add(ev);
            }

            Settings.Default.GoogleVoiceEvents = Serialize();
            Settings.Default.Save();
        }

        List<GoogleVoiceEventArgs> items;

        public List<GoogleVoiceEventArgs> Items
        {
            get
            {
                if (items == null) items = new List<GoogleVoiceEventArgs>();
                return items;
            }
            set
            {
                items = value;
            }
        }

        public void FireCurrentEvents(Google.Voice.GoogleVoice c, TimeSpan withinPastTimeFrame)
        {
            DateTime since = DateTime.Now.Subtract(withinPastTimeFrame);

            foreach (GoogleVoiceEventArgs e in Items)
            {
                if (e.Time <= since && e.Handled == false)
                {
                    e.Handle(c);
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.Voice;
using System.Windows.Forms;

namespace GoogleVoice.NET
{
    public static class GoogleVoiceEventWatcher
    {
        private static Timer timer = new Timer();
        private static Google.Voice.GoogleVoice googleVoice;

        public static void Run(Google.Voice.GoogleVoice c)
        {
            timer.Interval = 45000;
            timer.Tick += new EventHandler(HandleEvents);
            googleVoice = c;
            timer.Enabled = true;
            timer.Start();
        }

        public static void HandleEvents(object sender, EventArgs e)
        {
            if (Program.HaltEvents)
            {
                timer.Stop();
                timer.Enabled = false;
            }
            else
            {
                foreach (GoogleVoiceEventArgs ev in Program.VoiceEvents.Items)
                {
                    DateTime since = DateTime.Now.AddMinutes(-10);
                    if (ev.Handled == false && ev.Time >= since && ev.Time <= DateTime.Now)
                    {
                        ev.Handle(googleVoice);
                    }
                }
            }
        }
    }
}

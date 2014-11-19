using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Forms;
using Google.Voice;
using System.Reflection;
using Google.Voice;

namespace GoogleVoice.NET
{
    static class Program
    {

        public static Google.Voice.GoogleVoice GoogleVoice { get; set; }
        private static StringCollection serializedEvents = Settings.Default.GoogleVoiceEvents;
        private static GoogleVoiceEvents voiceEvents;
        public static GoogleVoiceEvents VoiceEvents 
        {
            get
            {
                if (voiceEvents == null)
                {
                    if (serializedEvents == null)
                    {
                        serializedEvents = new StringCollection();
                    }

                    voiceEvents = new GoogleVoiceEvents(serializedEvents);
                }
                
                return voiceEvents;
            }
            set
            {
                voiceEvents = value;
                Settings.Default.GoogleVoiceEvents = value.Serialize();
                Settings.Default.Save();
            }
        }
        public static FormLogin LoginForm { get; set; }
        public static FormControls ControlsForm { get; set; }
        private static volatile bool haltEvents;
        public static bool HaltEvents 
        {
            get { return Program.haltEvents; }
            set { Program.haltEvents = value; }
        }
        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            GoogleVoice = new Google.Voice.GoogleVoice();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            LoginForm = new FormLogin();

            Application.Run(LoginForm);
        }

        public static void SaveVoiceEvents()
        {
            Settings.Default.Save();
        }
    }
}

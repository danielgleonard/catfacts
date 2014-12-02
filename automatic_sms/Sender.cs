using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Voice;
using System.IO;

namespace automatic_sms
{
    class Sender
    {        
        public  static  GoogleVoice Voice = new GoogleVoice();

        public Sender() { }

        public void Authenticate(string username, string password)
        {
            // Got this from the example code
            Google.Voice.Web.LoginResult loginResult = Voice.Login(username, password);

            // If there's an error, throw an exception
            if (loginResult.RequiresRelogin)
            {
                throw new Exception("The username or password was incorrect");
            }
        }

        public void SendSMS(string number, string text)
        {
            try
            {
                Voice.SMS(number, text);                
                Console.WriteLine("Message sent");
            }
            catch
            {
                throw new Exception("Bad number or null text or other idiotic error");
            }
        }

        public string GetResourceTextFile(string filename)
        {
            string result = string.Empty;

            using (System.IO.Stream stream = this.GetType().Assembly.
                       GetManifestResourceStream("automatic_sms.assets." + filename))
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(stream))
                {
                    result = sr.ReadToEnd();
                }
            }
            return result;
        }
    }
}

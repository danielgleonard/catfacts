using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Voice;
using System.IO;

namespace catfacts
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
                
                try
                {
                    string record = "SMS: " + number + " - \"" + text + "\" at " + DateTime.Now;
                    StreamWriter writer = File.AppendText("History.txt");
                    writer.WriteLine(record);
                    writer.Close();
                }
                catch (Exception)
                {
                    throw;
                }
                
                Console.WriteLine("Message sent");
            }
            catch
            {
                throw new Exception("Bad number or null text or other idiotic error");
            }
        }
    }
}

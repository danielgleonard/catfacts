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
        public  string  Username        { get; set; }
        public  string  Password        { get; set; }

#if DEBUG
        private const string  debugUsername   =   "crazyworkoutkid";
#endif

        public  static  GoogleVoice Voice = new GoogleVoice();

        public Sender() { }

        public void GetCredentialsFromConsole()
        {
            // Get the username
            Console.Write("Username: ");
#if DEBUG
            // If in debug mode, use the debugUsername as the username
            Username = debugUsername;
            Console.WriteLine(Username);
#else
            // If in release mode, read password from console input
            Username = Console.ReadLine();
#endif

            // Get the password
            Console.Write("Password: ");
            Password = Console.ReadLine();
        }

        public void Authenticate()
        {
            // Got this from the example code
            Google.Voice.Web.LoginResult loginResult = Voice.Login(Username, Password);

            // If there's an error, throw an exception
            if (loginResult.RequiresRelogin)
            {
                throw new Exception("The username or password was incorrect");
            }
            
            // Remove the password string from memory
            Password = "";
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Voice;

namespace catfacts
{
    class Sender
    {
        public  string  Username        { get; set; }
        public  string  Password        { get; set; }

        private string  debugUsername   =   "crazyworkoutkid";
        public  static  Google.Voice.GoogleVoice Voice { get; set; }

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
    }
}

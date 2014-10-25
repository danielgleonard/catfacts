using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace catfacts
{
    class Sender
    {
        public  string  Username    { get; set; }
        public  string  Password    { get; set; }

        private string  debugUsername   =   "crazyworkoutkid";

        public void ConsoleLogin()
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
    }
}

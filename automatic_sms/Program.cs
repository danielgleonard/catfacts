using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace automatic_sms
{
    class Automatic_sms
    {
        private static  Sender      messageSender;
        private static  string[]    credentials =   new string[2];

#if DEBUG
        private static  string  debugNumber     =   "";
        private static  string  debugUsername   =   "";
        private static  string  debugPassword   =   "";
#endif

        static void Main(string[] args)
        {
            // Set color of console
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Beep(10000, 2000);

            // Display title
            Console.Title = "automatic_sms";
#if DEBUG
            Console.Title += " | DEBUG MODE";
#endif
            Console.WriteLine("     CAT FACTS     ");
            Console.WriteLine("===================");
            Console.WriteLine("by CombustibleLemon");
            Console.Write("\n\n\n");

            // Set up the message sender
            messageSender = new Sender();

#if DEBUG
            // Set up for debugging
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(messageSender.GetResourceTextFile("debugValues.xml"));
            XmlNode usernameNode    =   doc.SelectSingleNode("//values/email/text()");
            XmlNode passwordNode    =   doc.SelectSingleNode("//values/password/text()");
            XmlNode numberNode      =   doc.SelectSingleNode("//values/number/text()");

            // Kill the program if dev isn't ready for debug mode
            if (usernameNode == null || passwordNode == null || numberNode == null)
            {
                Console.WriteLine("You are in debug mode but don't have your debug settings set.");
                Console.WriteLine("Open \"debugValues.xml\" and set them.");
                Console.WriteLine("Make sure that the .gitignore will ignore your debug values!");
                Console.ReadLine();
                return;
            }

            // Set up variables if all is well
            debugUsername = usernameNode.Value;
            debugPassword = passwordNode.Value;
            debugNumber = numberNode.Value;
#endif

            // Log in
            credentials = GetCredentialsFromConsole();
            messageSender.Authenticate(credentials[0], credentials[1]);
            Console.WriteLine();
            SendMessages();
        }

        private static string[] GetCredentialsFromConsole()
        {
            string[] credentials = new string[2];

            // Get the username
            Console.Write("Username: ");
#if DEBUG
            // If in debug mode, use the debugUsername as the username
            credentials[0] = debugUsername;
            Console.WriteLine(credentials[0]);
#else
            // If in release mode, read password from console input
            credentials[0] = Console.ReadLine();
#endif

            // Get the password
            Console.Write("Password: ");
#if DEBUG
            credentials[1] = debugPassword;
            for (int i = 0; i < debugPassword.Length; i++)
            {
                Console.Write("*");
            }
            Console.WriteLine();
#else
            credentials[1] = Console.ReadLine();
#endif
            return credentials;
        }

        private static void SendMessages()
        {
            string  message     =   "";
            string  recipient   =   "";
            while (message != "exit")
            {
                Console.Write("Your recipient:");
#if DEBUG
                recipient = debugNumber;
                Console.WriteLine(recipient);
#else
                recipient = Console.ReadLine();
#endif
                Console.Write("Your message:");
                message = Console.ReadLine();
                messageSender.SendSMS(recipient, message);
                Console.WriteLine();
            }
        }
    }
}

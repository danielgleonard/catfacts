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
            Console.Title = "Automatic SMS Sender";
#if DEBUG
            Console.Title += " | DEBUG MODE";
#endif
            Console.WriteLine("   AUTOMATIC SMS   ");
            Console.WriteLine("===================");
            Console.WriteLine("by CombustibleLemon");
            Console.Write("\n\n\n");

            // Set up the message sender
            messageSender = new Sender();

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
            // If in release mode, read password from console input
            credentials[0] = Console.ReadLine();

            // Get the password
            Console.Write("Password: ");
            credentials[1] = Console.ReadLine();
            return credentials;
        }

        private static void SendMessages()
        {
            string  message     =   "";
            string  recipient   =   "";
            while (message != "exit")
            {
                Console.Write("Your recipient:");
                recipient = Console.ReadLine();
                Console.Write("Your message:");
                message = Console.ReadLine();
                messageSender.SendSMS(recipient, message);
                Console.WriteLine();
            }
        }
    }
}

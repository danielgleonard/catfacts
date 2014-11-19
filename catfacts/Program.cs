using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace catfacts
{
    class CatFacts
    {
        private static Sender messageSender;
        private static string[] credentials = new string[2];

#if DEBUG
        private const   string  debugNumber     =   "8477077458";
        private const   string  debugUsername   =   "crazyworkoutkid@gmail.com";
#endif

        static void Main(string[] args)
        {
            // Set color of console
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Beep(10000, 2000);

            // Display title
            Console.Title = "CatFacts";
            Console.WriteLine("     CAT FACTS     ");
            Console.WriteLine("===================");
            Console.WriteLine("by CombustibleLemon");
            Console.Write("\n\n\n");

            // Run the stuff
            messageSender = new Sender();
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
            Username = Console.ReadLine();
#endif

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
#if DEBUG
                recipient = debugNumber;
                Console.WriteLine(recipient);
#else
                recipient = Console.Read();
#endif
                Console.Write("Your message:");
                message = Console.ReadLine();
                messageSender.SendSMS(debugNumber, message);
                Console.WriteLine();
            }
        }
    }
}

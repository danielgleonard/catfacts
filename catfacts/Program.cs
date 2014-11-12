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
            messageSender.GetCredentialsFromConsole();
        }
    }
}

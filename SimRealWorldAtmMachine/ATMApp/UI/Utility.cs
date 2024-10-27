using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATMApp.UI
{
    public static class Utility
    {
        public static void PrintMessage(string msg, bool success = true)
        {
            if (success) 
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else
            {
                Console.ForegroundColor= ConsoleColor.Red;
            }
            Console.WriteLine(msg);
            Console.ForegroundColor= ConsoleColor.White;
            PressEnterToContinue();

        }
        public static string GetuserInput(string prompt) 
        {
            Console.WriteLine($"Enter {prompt}");
            return Console.ReadLine();
        
        }

        public static void PressEnterToContinue()
        {
            Console.WriteLine("\n press enter to continue...");
            Console.ReadLine();
        }
    }
}

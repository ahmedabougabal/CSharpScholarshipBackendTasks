using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATMApp.UI
{
    public static class AppScreen
    {
        internal static void Welcome()
        {

            Console.Clear();
            Console.Title = "My ATM App";
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\n\n-------------Welcome to my ATM app-------------\n\n");
            //ask the user for card number
            Console.WriteLine("Please insert your ATM card");
            Utility.PressEnterToContinue();
        }

   
    }
}

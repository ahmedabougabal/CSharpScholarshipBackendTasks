using ATMApp.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATMApp.App
{
    class Entry
    {
        static void Main(string[] args)
        {
            AppScreen.Welcome();
            long CardNumber = Validator.Convert<long>("your card number");
            Console.WriteLine($"your card number is {CardNumber}");
            Utility.PressEnterToContinue();
        }
    }
}

using ATMApp.Domain.Entities;
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

        internal static UserAccount UserLoginForm()
        {
            UserAccount tempUserAccount = new UserAccount();

            tempUserAccount.CardNumber = Validator.Convert<long>("your card number.");
            tempUserAccount.CardPin = Convert.ToInt32(Utility.GetSecretInput("enter your Card PIN"));
            return tempUserAccount;
        }

        internal static void LoginProgress()
        {
            Console.WriteLine("\nChecking Card Number and Card Pin...");
            Utility.PrintDotAnimation();
        }
        internal static void PrintLockScreen() 
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Utility.PrintMessage("Your account is Locked, please go to the nearest branch to unlock your account" +
                " Thank you.", false);
            Console.ForegroundColor = ConsoleColor.White;
            Utility.PressEnterToContinue();
            Environment.Exit(1);
        }


    }
}

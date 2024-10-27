using System;
using ATMApp.Domain.Entities;
using ATMApp.Domain.Entities.Interfaces;
using ATMApp.Domain.Enums;
using ATMApp.UI;
namespace ATMApp.App
{
    public class ATMApp : IUserLogin
    {
        private List<UserAccount> userAccountlist;
        private UserAccount SelectedAcccount;

        public void Run()
        {
            AppScreen.Welcome();
            CheckUserCardNumAndPassword();
            AppScreen.WelcomeCustomer(SelectedAcccount.FullName);
            AppScreen.DisplayAppMenu();
            ProcessMenuOption();


        }


        public void InitializeData()
        {
            userAccountlist = new List<UserAccount>
            {
                new UserAccount{Id = 1, FullName="Ahmed Abou Gabal", AccountNumber=123456, CardNumber = 321321 , CardPin=241023,
                AccountBalance= 10000, isLocked=false},
                new UserAccount{Id = 2, FullName="Mr X", AccountNumber=12345678, CardNumber = 32132132 , CardPin=241023,
                AccountBalance= 5000, isLocked=false},
                new UserAccount{Id = 3, FullName="Mrs Y", AccountNumber=123456789, CardNumber = 321321321 , CardPin=241023,
                AccountBalance= 700, isLocked=true}
            };

        }
        public void CheckUserCardNumAndPassword()
        {
            // check for credentials against the predefined list database
            bool isCorrectLogin = false;
            while (isCorrectLogin == false)
            {
                UserAccount inputAccount = AppScreen.UserLoginForm();
                AppScreen.LoginProgress();
                foreach (UserAccount account in userAccountlist)
                {
                    SelectedAcccount = account;
                    if (inputAccount.CardNumber.Equals(SelectedAcccount.CardNumber))
                    {
                        SelectedAcccount.TotalLogin++;
                        if (inputAccount.CardPin.Equals(SelectedAcccount.CardPin))
                        {
                            SelectedAcccount = account;
                            if (SelectedAcccount.isLocked || SelectedAcccount.TotalLogin > 3)
                            {
                                //print a lock message on the screen to the screen
                                AppScreen.PrintLockScreen();
                            }
                            else
                            {
                                SelectedAcccount.TotalLogin = 0;
                                isCorrectLogin = true;
                                break;
                            }
                        }
                    }
                    if (isCorrectLogin == false)
                    {
                        Utility.PrintMessage("\n invalid CardNumber or CardPIN", false);
                        SelectedAcccount.isLocked = SelectedAcccount.TotalLogin == 3;
                        if (SelectedAcccount.isLocked)
                        {
                            AppScreen.PrintLockScreen();
                        }
                    }
                    Console.Clear();

                }
            }
           
        }
        private void ProcessMenuOption()
        {
            switch (Validator.Convert<int>("an option"))
            {
                case (int)AppMenu.CheckBalance:
                    Console.WriteLine("Checking account balance...");
                    break;
                case (int)AppMenu.PlaceDeposit:
                    Console.WriteLine("Placing Deposit...");
                    break;
                case (int)AppMenu.MakeWithdrawal:
                    Console.WriteLine("Making WithDrawal...");
                    break;
                case (int)AppMenu.InternalTransfer:
                    Console.WriteLine("Making Transfer...");
                    break;
                case (int)AppMenu.ViewTransactions:
                    Console.WriteLine("Viewing Transactions...");
                    break;
                case (int)AppMenu.Logout:
                    AppScreen.LogOutProgress();
                    Utility.PrintMessage("you have succesfully logged out, please collect your ATM card");
                    Run();
                    break;
                default:
                    Utility.PrintMessage("Invalid option.", false);
                    break;
            

            }
        }
       


    }
}

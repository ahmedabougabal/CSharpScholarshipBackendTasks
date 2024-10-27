using System;
using ATMApp.Domain.Entities;
using ATMApp.Domain.Entities.Interfaces;
using ATMApp.Domain.Enums;
using ATMApp.UI;
namespace ATMApp.App
{
    public class ATMApp : IUserLogin, IUserAccountActions
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
                AccountBalance= 10000m, isLocked=false},
                new UserAccount{Id = 2, FullName="Mr X", AccountNumber=12345678, CardNumber = 32132132 , CardPin=241023,
                AccountBalance= 5000m, isLocked=false},
                new UserAccount{Id = 3, FullName="Mrs Y", AccountNumber=123456789, CardNumber = 321321321 , CardPin=241023,
                AccountBalance= 700m, isLocked=true}
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
                    CheckBalance();
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

        public void CheckBalance()
        {
            Utility.PrintMessage($"your account balance is: {Utility.FormatAmount(SelectedAcccount.AccountBalance)}");
        }

        public void PlaceDeposit()
        {
            Console.WriteLine("\n Only multiples of 500 and 1000 are allowed\n");
            var transaction_amount = Validator.Convert<int>($"amount {AppScreen.Cur}");

            // simulate counting 
            Console.WriteLine("\n Checking and Counting bank notes.");
            Utility.PrintDotAnimation();
            Console.WriteLine("");

            // some guard clauses
            if (transaction_amount > 0) {
                Utility.PrintMessage("Amount needs to be greater than 0, try again", false);
                return;
            }
            if (transaction_amount % 500 != 0) {
                Utility.PrintMessage("Enter Deposit Amount in multiples of 500 or 1000, try again", false);
                return;
            }
            if (PreviewBankNotesCount(transaction_amount)==false)
            {
                Utility.PrintMessage($"You have cancelled your action");
            }
        }

        public void MakeWithdrawal()
        {
            throw new NotImplementedException();
        }

        private bool PreviewBankNotesCount(int amount)
        {
            int thousandNotesCount = amount / 1000;
            int fiveHundredNotesCount = (amount % 1000) / 500;
            Console.WriteLine("\n Summary");
            Console.WriteLine("------------");
            Console.WriteLine($"{AppScreen.Cur}1000 X {thousandNotesCount} = {1000 * thousandNotesCount}");
            Console.WriteLine($"{AppScreen.Cur}500 X {fiveHundredNotesCount} = {500 * fiveHundredNotesCount}");
            Console.WriteLine($"total amount : {Utility.FormatAmount(amount)}\n \n");

            int opt = Validator.Convert<int>("1 to Confirm");
            return opt.Equals(1);
        }
    }
}

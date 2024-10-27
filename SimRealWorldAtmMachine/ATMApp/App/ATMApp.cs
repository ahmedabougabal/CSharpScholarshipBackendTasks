using System;
using ATMApp.Domain.Entities;
using ATMApp.UI;
namespace ATMApp.App
{
    public class ATMApp
    {
        private List<UserAccount> userAccountlist;
        private UserAccount SelectedAcccount;

        public void InitializeData()
        {
            userAccountlist = new List<UserAccount>
            {
                new UserAccount{Id = 1, FullName="Ahmed Abou Gabal", AccountNumber=123456, CardNumber = 321321 , CardPin=2410,
                AccountBalance= 10000, isLocked=false},
                new UserAccount{Id = 2, FullName="Mr X", AccountNumber=12345678, CardNumber = 32132132 , CardPin=2410,
                AccountBalance= 5000, isLocked=false},
                new UserAccount{Id = 3, FullName="Mrs Y", AccountNumber=123456789, CardNumber = 321321321 , CardPin=2410,
                AccountBalance= 700, isLocked=true}
            };

        }

        
    }
}

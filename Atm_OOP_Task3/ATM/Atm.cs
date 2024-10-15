using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;

namespace ATM;

public class User
{
    public string Username { get; set; }
    public string Password { get; set; }
    public decimal Balance { get; set; }

    public User(string username, string password, decimal balance)
    {
        Username = username;
        Password = password;
        Balance = 0;
    }
}


public class Atm
{
    private List<User> users = new List<User>();
    private User currentUser;

    public void Run()
    {
        while (true)
        {
            Console.WriteLine("1. Sign up");
            Console.WriteLine("2. Login");
            Console.WriteLine("2. Exit");
            Console.WriteLine("Choose an option:");
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    SignUp();
                    break;
                case "2":
                    if (Login())
                    {
                        performOperations();
                    };
                    break;
                case "3":
                    return;
                default:
                    Console.WriteLine("Invalid choice, please try again.");
                    break;
            }
        }
    }

    private void SignUp()
    {
        Console.WriteLine("Enter username:");
        string username = Console.ReadLine();
        Console.WriteLine("Enter password:");
        string password = Console.ReadLine();
        
        users.Add(new User(username, password, 0));
        Console.WriteLine("User registration successful.");
    }

    private bool Login()
    {
        Console.WriteLine("Enter username:");
        string username = Console.ReadLine();
        Console.WriteLine("Enter password:");
        string password = Console.ReadLine();
        currentUser = users.FirstOrDefault(u => u.Username == username && u.Password == password);

        if (currentUser != null)
        {
            Console.WriteLine("You have logged in.");
            return true;
        }
        else
        {
            Console.WriteLine("Username or password is incorrect, try again.");
            return false;
        }
    }

    private void performOperations()
    {
        while (true)
        {
            
        Console.WriteLine("\n 1. check balance");
        Console.WriteLine("2. Make a deposit");
        Console.WriteLine("3. Withdraw Balance");
        Console.WriteLine("4. Exit");
        Console.Write("choose an option: ");
        string choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                checkBalance();
                break;
            case "2":
                makeDeposit();
                break;
            case "3":
                withdrawBalance();
                break;
            case "4":
                return;
            default:
                Console.WriteLine("Invalid choice, please try again.");
                break;
        }
        Console.WriteLine("\n Do you want to perform any other operations? (y/n)");
        string answer = Console.ReadLine();
        if (answer.ToLower() != "y")
        {
            break; 
        } 
        }

        
    }

    private void checkBalance()
    {
        Console.WriteLine($"your current balance is {currentUser.Balance} ");
    }

    private void makeDeposit()
    {
        Console.WriteLine("Enter the deposit amount:");
        if (decimal.TryParse(Console.ReadLine(), out decimal amount) && amount > 0)
        {
            currentUser.Balance += amount; 
            Console.WriteLine($"Your new balance is {currentUser.Balance} ");   
        }
        else
        {
            Console.WriteLine("Invalid deposit amount, please enter a positive number.");
        }
    }

    private void withdrawBalance()
    {
        Console.WriteLine("enter the desired withdraw amount:");
        if (decimal.TryParse(Console.ReadLine(), out decimal amount) && amount > 0)
        {
            if (amount <= currentUser.Balance)
            {
                currentUser.Balance -= amount;
                Console.WriteLine($"withdraw successful, Your balance is now {currentUser.Balance} ");
            }
            else
            {
                Console.WriteLine("insufficient balance, your new balance is {0} ", currentUser.Balance);
            }
        }
        else
        {
            Console.WriteLine("Invalid withdrawing amount, please enter a positive number.");
        }
    }
}
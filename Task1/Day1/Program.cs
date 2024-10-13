// See https://aka.ms/new-console-template for more information

// Console.WriteLine("WELCOME TO .NET");
using System.Text.RegularExpressions;
string street = "23 Main Street";
string city ;
string district= "NY 10001 USA";

Console.WriteLine("enter your name");
string name=Console.ReadLine();;
string namePattern = @"^[aA][Ll][eE][xX]$";
while (!Regex.IsMatch(name, namePattern))
{
    Console.WriteLine("not alex, \n try again");
    name = Console.ReadLine();
}
Console.WriteLine("enter your city");
city=Console.ReadLine();
string cityPattern = @"^[nN][eE][Ww]\s[yY][oO][Rr][Kk]$"; 

while (!Regex.IsMatch(city, cityPattern))
{
            Console.WriteLine("not the right city, try again>> ");
            city=Console.ReadLine(); 
} 
string tolower_name = name.ToLower();
string tolower_city = city.ToLower();
Console.WriteLine(tolower_name+" "+street+" "+tolower_city+" "+district); 






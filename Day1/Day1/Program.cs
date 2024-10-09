// See https://aka.ms/new-console-template for more information

// Console.WriteLine("WELCOME TO .NET");

string street = "23 Main Street";
string city ;
string district= "NY 10001 USA";

Console.WriteLine("enter your name");
string name=Console.ReadLine();;

while (name != "Alex")
{ 
    Console.WriteLine("not alex, \n try again");
    name =Console.ReadLine();
    if (name == "alex")
    {
        Console.WriteLine("enter your city");
        city=Console.ReadLine();
        while(city != "new york")
        { 
            Console.WriteLine("not the right city, try again>> ");
            city=Console.ReadLine();
        } 
        Console.WriteLine(name+" "+street+" "+city+" "+district); 
        break;
    }
}




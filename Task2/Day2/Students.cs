using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;

namespace Day2;

public class Students<T>
{
  private List<T> _students = new List<T>();

  public void AddStudent(T student)
  {
    _students.Add(student);
  }

  public string ShowStudentsCount()
  {
    if (typeof(T) == typeof(int))
    {
      int sum = 0;
      foreach (T student in _students)
      {
        sum += (int)(object)student; // cast to int and add to sum :)
      }

      return $"{sum}";
    }
    else if (typeof(T) == typeof(string))
    {
      string names = string.Join(", \n", _students.Cast<string>());
      return $"unsupported student type to count but their names are : {names}";
    }
    return $"unsupported student type: {typeof(T).Name}";
  }

//Implementing operator+ overloading 
  public static Students<T> operator +(Students<T> s1, Students<T> s2)
  {
    Students<T> result = new Students<T>();
    result._students.AddRange(s1._students);
    result._students.AddRange(s2._students);
    return result;
  }
}

public class ClassRooms
{
  public static void TestStudents()
  {
    Students<int> class1 = new Students<int>();
    class1.AddStudent(25);
    class1.AddStudent(24);
    Console.WriteLine($"the sum of students in classroom1 is {class1.ShowStudentsCount()}");

    Students<string> class2 = new Students<string>();
      class2.AddStudent("ahmed");
      class2.AddStudent("GHOST");
      Console.WriteLine($"{class2.ShowStudentsCount()}");
  }
}
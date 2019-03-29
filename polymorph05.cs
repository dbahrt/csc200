// source: taken from https://www.youtube.com/watch?v=4a_iTOtGhM8 
//     Part 23 - C# Tutorial - Polymorhpism in c#.avi
// author: kudvenkat
// published on Jun 9, 2012
// Text version of the video at http://csharp-video-tutorials.blogspot.com/...
//     /2012/06/part-23-c-tutorial-polymorphism.html
// student: Dan Bahrt
// synopsis:


using System;

public class Employee {
    public string FirstName = "FN";
    public string LastName = "LN";

    // sets up base class method to be overridden by derived class methods
    public virtual void PrintFullName() {
        Console.WriteLine(FirstName + " " + LastName + " base default");
    }
}

public class FullTimeEmployee : Employee {
    // derived class method overrides virtual base class method 
    public override void PrintFullName() {
        Console.WriteLine(FirstName + " " + LastName + " Full Time");
    }
}

public class PartTimeEmployee : Employee {
    // derived class method overrides virtual base class method
    public override void PrintFullName() {
        Console.WriteLine(FirstName + " " + LastName + " Part Time");
    }
}

public class TemporaryEmployee : Employee {
    // derived class method overrides virtual base class method
    public override void PrintFullName() {
        Console.WriteLine(FirstName + " " + LastName + " Temporary");
    }
}

public class InternEmployee : Employee {
    // derived class inherits vitural base class method
}

public class Program {
    public static void Main() {
        Employee [] employees = new Employee[5];
        employees[0] = new FullTimeEmployee();
        employees[1] = new PartTimeEmployee();
        employees[2] = new TemporaryEmployee();
        employees[3] = new InternEmployee();
        employees[4] = new Employee();

        foreach(Employee e in employees) {
            e.PrintFullName();
        }
    }
}

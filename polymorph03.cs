// source: taken from https://www.youtube.com/watch?v=4a_iTOtGhM8 
//     Part 23 - C# Tutorial - Polymorhpism in c#.avi
// author: kudvenkat
// published on Jun 9, 2012
// Text version of the video at http://csharp-video-tutorials.blogspot.com/...
//     /2012/06/part-23-c-tutorial-polymorphism.html
// student: Dan Bahrt
// synopsis: full blown polymorphism (virtual methods in base class; overriding
//     virtual methods in derived class; intentionally hiding virtual method
//     in base class; unintentionally hiding virtual method in base class;
//     inheriting base class method)

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
    // dynamic run-time polymorphic mapping
    public override void PrintFullName() {
        Console.WriteLine(FirstName + " " + LastName + " Full Time");
    }
}

public class PartTimeEmployee : Employee {
    // virtual base class method intentionally hides derived class method
    public new void PrintFullName() {
        Console.WriteLine(FirstName + " " + LastName + " Part Time");
    }
}

public class TemporaryEmployee : Employee {
    // will cause compile time warning because
    // virtual base class method unintentionally hides derived class method
    public void PrintFullName() {
        Console.WriteLine(FirstName + " " + LastName + " Temporary");
    }
}

public class InternEmployee : Employee {
    // derived class inherits vitural base class method
}

public class Program {
    public static void Main() {
        // notice this is polmorphism because
        // we are creating array of base class objects
        // and filling it with derived class objects
        Employee [] employees = new Employee[5];
        // dynamic runtime polymorphic mapping
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

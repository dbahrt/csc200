// SOURCE: taken from https://www.youtube.com/watch?v=4a_iTOtGhM8 
//     Part 23 - C# Tutorial - Polymorhpism in c#.avi
// Text version of the video at http://csharp-video-tutorials.blogspot.com/...
//     /2012/06/part-23-c-tutorial-polymorphism.html
// AUTHOR: kudvenkat
// PUBDATE: Jun 9, 2012
// STUDENT: Dan Bahrt
// DOWNLOAD: Mar 29, 2019
// SYNOPSIS: full blown polymorphism (virtual methods in base class; overriding
//     virtual methods in derived class; intentionally hiding virtual method
//     in base class; unintentionally hiding virtual method in base class;
//     inheriting base class method)

using System;

//==========
public class Program {

    //----------
    public static void Main() {
        // notice this is simple polymorphism because
        // we are creating an array of base class objects
        // and filling it with derived class objects
        Employee [] employees = new Employee[5];
        // dynamic runtime polymorphic mapping
        employees[0] = new FullTimeEmployee();
        employees[1] = new PartTimeEmployee();
        employees[2] = new TemporaryEmployee();
        employees[3] = new InternEmployee();
        employees[4] = new Employee();

        // loop through array of base class objects and
        // call member methods that give evidence of execution
        foreach(Employee e in employees) {
            e.PrintFullName();
        }
    }
}

//==========
// base class definition
//==========
public class Employee {
    public string FirstName = "FN";
    public string LastName = "LN";

    //----------
    // sets up base class method to be overridden by derived class methods
    //----------
    public virtual void PrintFullName() {
        Console.WriteLine(FirstName + " " + LastName + " base default");
    }
}

//==========
// FullTimeEmployee derived class definition
//==========
public class FullTimeEmployee : Employee {

    //----------
    // derived class method overrides virtual base class method 
    // demonstrates dynamic run-time polymorphic mapping
    //----------
    public override void PrintFullName() {
        Console.WriteLine(FirstName + " " + LastName + " Full Time");
    }
}

//==========
// PartTimeEmployee derived class definition
//==========
public class PartTimeEmployee : Employee {

    //----------
    // derived class method overrides virtual base class method
    // demonstrates member hiding
    //----------
    public new void PrintFullName() {
        Console.WriteLine(FirstName + " " + LastName + " Part Time");
    }
}

//==========
// TemporaryEmployee derived class definition
//==========
public class TemporaryEmployee : Employee {

    //----------
    // will cause compile time warning because
    // virtual base class method unintentionally hides derived class method
    //----------
    public void PrintFullName() {
        Console.WriteLine(FirstName + " " + LastName + " Temporary");
    }
}

//==========
// InternEmployee derived class definition
//==========
public class InternEmployee : Employee {

    //----------
    // derived class inherits vitural base class method
    //----------

}

// MODIFICATIONS:  MINIMAL.  added comments.  minor code restructuring.

// ANALYSIS: instantiate base class object and several derived class objects
// most of which have distinguishing members that make them
// different from each other.
// shove all of these objects into an array of base class objects.
// loop through array of base class objects and
// call member methods that give evidence of execution.
// note that because only the FullTimeEmployee object successfully 
// overrides the virtual method in the base class, it is the only object that
// will exhibit interesting run-time polymorphic mapping

/* SAMPLE OUTPUT:
polymorph03.cs(92,17): warning CS0114: `TemporaryEmployee.PrintFullName()' hides inherited member `Employee.PrintFullName()'. To make the current member override that implementation, add the override keyword. Otherwise add the new keyword
polymorph03.cs(50,25): (Location of the symbol related to previous warning)
Compilation succeeded - 1 warning(s)


FN LN Full Time
FN LN base default
FN LN base default
FN LN base default
FN LN base default
*/

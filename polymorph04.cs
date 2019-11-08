// SOURCE: taken from https://www.youtube.com/watch?v=4a_iTOtGhM8 
//     Part 23 - C# Tutorial - Polymorhpism in c#.avi
// Text version of the video at http://csharp-video-tutorials.blogspot.com/...
//     /2012/06/part-23-c-tutorial-polymorphism.html
// AUTHOR: kudvenkat
// PUBDATE: Jun 9, 2012
// STUDENT: Dan Bahrt
// DOWNOLOAD: Mar 29, 2019
// SYNOPSIS: abstract class example (cannot create base class object)
//     everything else is full-blown polymorphic

using System;

//==========
public class Program {

    //----------
    public static void Main() {
        // notice this is simple polymorphism because
        // we are creating an array of base class objects
        // and filling it with derived class objects
        // i.e. we have derived class objects masquerading as
        // base class objects
        // have I explained how there is no such thing as a generic dog?
        Employee [] employees = new Employee[5];
        employees[0] = new FullTimeEmployee();
        employees[1] = new PartTimeEmployee();
        employees[2] = new TemporaryEmployee();
        employees[3] = new InternEmployee();
        // employee class is now abstract...
        // so we cannot instantiate an object from it
        // it gives us a compile-time error
        // employees[4] = new Employee();

        // loop throught array of base class objects and
        // call member methods that give evidence of execution
        foreach(Employee e in employees) {
            if(e==null) {
                continue;
            }
            e.PrintFullName();
        }
    }
}

//==========
// abstract base class definition
//==========
public abstract class Employee {
    public string FirstName = "FN";
    public string LastName = "LN";

    //----------
    // sets up base class method to be overridden by derived class methods.
    // if it is abstract, that means it is automatically virtual.
    // it must be overridden.
    //----------
    public abstract /*virtual*/ void PrintFullName(); /* {
        Console.WriteLine(FirstName + " " + LastName + " base default");
    } */
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
    //----------
    public override void PrintFullName() {
        Console.WriteLine(FirstName + " " + LastName + " Part Time");
    }
}

//==========
// TemporaryEmployee derived class definition
//==========
public class TemporaryEmployee : Employee {

    //----------
    // derived class method overrides virtual base class method
    //----------
    public override void PrintFullName() {
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
    public override void PrintFullName() {
        Console.WriteLine(FirstName + " " + LastName + " Intern");
    }
}


// MODIFICATIONS:  MINIMAL.  added comments.  minor code restructuring.

// ANALYSIS: instantiate several derived class objects
// all of which have distinguishing members that make them
// different from each other.
// (cannot instantiate abstract base class.)
// shove all of these objects into an array of base class objects.
// loop through array of base class objects and
// call member methods that give evidence of execution.
// note that because all of the derived class objects successfully
// override the abstract/virtual method in the base class,
// they should all exhibit interesting run-time polymorphic mapping.

/* SAMPLE OUTPUT:
FN LN Full Time
FN LN Part Time
FN LN Temporary
FN LN Intern
*/

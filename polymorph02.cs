// SOURCE: taken from https://www.youtube.com/watch?v=4a_iTOtGhM8 
//     Part 23 - C# Tutorial - Polymorhpism in c#.avi
// Text version of the video at http://csharp-video-tutorials.blogspot.com/...
//     /2012/06/part-23-c-tutorial-polymorphism.html
// AUTHOR: kudvenkat
// PUBDATE: Jun 9, 2012
// STUDENT: Dan Bahrt
// DOWNLOAD: Mar 29, 2019
// SYNOPSIS: simple inheritance; creating array that demonstrates
//     simple polymorphism (treating derived class object as base class object)

using System;

//==========
public class Program {

    //----------
    public static void Main() {
        // instantiate objects and shove reference pointers into 
        // array of base class objects.
        Employee [] employees = new Employee[5];
        employees[0] = new FullTimeEmployee();
        employees[1] = new PartTimeEmployee();
        employees[2] = new TemporaryEmployee();
        employees[3] = new InternEmployee();
        employees[4] = new Employee();

        // loop through array of base class objects and 
        // call member methods that give evidence of execution.
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
    public void PrintFullName() {
        Console.WriteLine(FirstName + " " + LastName + " base default");
    }
}

//==========
// FullTimeEmployee derived class definition
//==========
public class FullTimeEmployee : Employee {
}

//==========
// PartTimeEmployee derived class definition
//==========
public class PartTimeEmployee : Employee {
}

//==========
// TemporaryEmployee derived class definition
//==========
public class TemporaryEmployee : Employee {
}

//==========
// InternEmployee derived class definition
//==========
public class InternEmployee : Employee {
}

// MODIFICATIONS:  MINIMAL.  added comments.  minor code restructuring.

// ANALYSIS: instantiate base class object and several derived class objects
// that at this point are indistinguishable from each other.
// all of the derived objects inherit all of the members of the base class,
// but none of them differentiate themselves in any way from the base class.
// shove all of these objects into an array of base class objects.
// loop through array of base class objects and 
// call member methods that give evidence of execution.

/* SAMPLE OUTPUT:
FN LN base default
FN LN base default
FN LN base default
FN LN base default
FN LN base default
*/

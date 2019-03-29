// source: taken from https://www.youtube.com/watch?v=4a_iTOtGhM8 
//     Part 23 - C# Tutorial - Polymorhpism in c#.avi
// author: kudvenkat
// published on Jun 9, 2012
// Text version of the video at http://csharp-video-tutorials.blogspot.com/...
//     /2012/06/part-23-c-tutorial-polymorphism.html
// student: Dan Bahrt
// synopsis: simple object instantiation; no inheritance; no polymorphism

using System;

//==========
public class Program {
    public static void Main() {
        // using Employee class as a pattern to make an Employee object
        // 1. new operator allocates memory to hold object
        // 2. new operator calls constructor to initialize object
        // 3. new operator returns ref pointer to the object
        // 4. assignment operator stores ref pointer in variable
        Employee E = new Employee();
        E.PrintFullName(/* hidden parameter: Employee this, */ 10,"name");
        // 2 main differences between function and method:
        // 1. function location is statically determined by compiler.
        // method is dynamically addressed at runtime, when object is created.
        // 2. method has a hiddern parameter (this) pointing to the object to
        // which it belongs
    }
}

//==========
public class Employee {
    public string FirstName = "FN";
    public string LastName = "LN";

    //----------
    public void PrintFullName(int dummy1,string dummy2) {
        Console.WriteLine(FirstName + " " + LastName);
    }
}


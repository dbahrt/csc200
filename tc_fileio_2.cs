// URL: https://www.youtube.com/watch?v=cST5TT3OFyg
// AUTHOR: Tim Corey
// TITLE: C# Data Access: Text Files
// FILE: tc_fileio_2.cs
// CAPTURE DATE: 24 Apr 2019
// STUDENT: Dan Bahrt
// SYNOPSIS: read in CSV text file; dump objects out to console

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace ConsoleUI {

//=========
class Program {

    //----------
    static void Main(string[] args) {
        string filePath = "./tc_fileio.txt";

        List<Person> people = new List<Person>();

        List<string> lines = File.ReadAllLines(filePath).ToList();

        foreach (string line in lines) {
            string[] entries = line.Split(',');

            if(entries.Length!=3) {
                Console.WriteLine("skipping invalid line in file: "+line);
                continue;
            }

            Person newPerson = new Person(entries[0],entries[1],entries[2]);

            people.Add(newPerson);
        }

        Console.WriteLine();

        foreach (var person in people) {
            Console.WriteLine($"{ person.FirstName } { person.LastName }: { person.URL }");
        }

        Console.ReadLine();
    } 
}

//==========
class Person {
public string FirstName;
public string LastName;
public string URL;

    //----------
    public Person(string fn,string ln,string url) {
        FirstName=fn;
        LastName=ln;
        URL=url;
    }
}
}

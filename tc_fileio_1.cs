// URL: https://www.youtube.com/watch?v=cST5TT3OFyg
// AUTHOR: Tim Corey
// TITLE: C# Data Access: Text Files
// FILE: tc_fileio_1.cs
// CAPTURE DATE: 24 Apr 2019
// STUDENT: Dan Bahrt
// SYNOPSIS: read in CSV text file; turn around and write it out

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

        List<string> lines = File.ReadAllLines(filePath).ToList();

        foreach (string line in lines) {
            Console.WriteLine(line);
        }

        lines.Add("Sue,Storm,www.stormy.com");

        File.WriteAllLines(filePath, lines);

        Console.ReadLine();
    } 
}
}

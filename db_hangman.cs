// SOURCE: https://www.sanfoundry.com/csharp-programs-gaming-hangman/
// AUTHOR: Manish Bhojasia, a technology veteran with 20+ years @ Cisco &
//     Wipro, [along with consultancies at IBM, Brocade, Quantum, etc.]
//     is Founder and CTO at Sanfoundry. He is Linux Kernel Developer and
//     SAN Architect and is passionate about competency development....
//     He lives in Bangalore ....
// FILENAME: hangman.cs
// PURPOSE: C# Program to Create a Hangman Game
// STUDENT: Dan Bahrt
// DATE: 08 Feb 2019

// STYLE MODIFICATIONS:
//   * I use 1tbs indentation style (4 spaces per level),
//     with mandatory braces for all control structures.
//     I do not indent classes definitions within namespaces.
//   * I add minimal "flower box" comments to set apart
//     class and function definitions.
//   * I put "// end" comments on closing braces for
//     namespaces, classes, and functions.
//   * To better document their purposes, I freely change the names of
//     namespaces, classes, functions, variables and constants.
//   * I indiscriminately add or change comments according to my whims.
//   * I eliminate unnecessary using statements.

// FUNCTIONAL MODIFICATIONS:


using System;
 
namespace mypgms {

//==========
class Hangman {

    //----------
    public static void Main(string[] args) {
 
        Console.WriteLine("Welcome to Hangman!!!!!!!!!!");

        string[] listwords = new string[10];
        listwords[0] = "sheep";
        listwords[1] = "goat";
        listwords[2] = "computer";
        listwords[3] = "america";
        listwords[4] = "watermelon";
        listwords[5] = "icecream";
        listwords[6] = "jasmine";
        listwords[7] = "pineapple";
        listwords[8] = "orange";
        listwords[9] = "mango";

        Random randGen = new Random();
        var idx = randGen.Next(0, 9);

        string mysteryWord = listwords[idx];

        char[] guess = new char[mysteryWord.Length];

        Console.Write("Please enter your guess: ");
 
        for (int p = 0; p < mysteryWord.Length; p++) {
            guess[p] = '*';
        }
 
        while (true) {
            char playerGuess = char.Parse(Console.ReadLine());
            for (int j = 0; j < mysteryWord.Length; j++) {
                if (playerGuess == mysteryWord[j]) {
                    guess[j] = playerGuess;
                }
            }
            Console.WriteLine(guess);
        }
    } // end function Main()
} // end class Hangman

} // end namespace mypgms:

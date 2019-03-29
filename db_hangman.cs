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

// FUNCTIONAL ISSUES (soon to be MODIFICATIONS):
//   * There is an infinite loop in the program (i.e. a while(true)
//     statement that does not contain a break or a return statement.
//     As a consequence of this, there is no graceful way for the user
//     to terminate the program. The program does not detect when the
//     user has solved the puzzle.
//
//   * The program does not properly handle user input.  If the user
//     enters more than one character at a time, the program crashes.
//     "System.FormatException: String must be exactly one character long"
//     is not a friendly message. The user gets the same message when
//     he/she does not enter any input.
//
//   * The program does not properly prompt the user for input.
//     The word mask should be displayed before every request for input,
//     and the user should prompted for every input.
//
//   * ALL of the program functionality is stuffed in the Main() function.
//     Also, that program functionality is not broken up into bite-sized
//     logical functions. 
//
//   * There is no loss condition in the program.  The program does not 
//     detect when the user has failed to solve the puzzle.
//
//   * Besides the fact that the program does not detect when the user
//     has solved or not solved the puzzle, it does not provide a way to
//     replay the game... other than re-running the program from the 
//     command line.
//
//   * Could the word list be stored in a data file, rather than
//     hard-coded into the program?
//
//   * In the word mask, let's use dashes rather than asterisks.
//
//   * Playing Hangman, the user really expects to see a gallows and a
//     graphical representation of their progress towards winning or
//     losing the game.
//
//   * For my CSC200 students, let's split up the Hangman class into 
//     a non-object-oriented Startup class and an object-oriented
//     HangmanGame class. Create objects with methods, and
//     where desirable, eliminate static functions.


using System;
 
namespace mypgms {

//==========
class Hangman {

    //----------
    static void Main(string[] args) {
 
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

} // end namespace mypgms

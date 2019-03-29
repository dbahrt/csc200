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
//
// STYLE MODIFICATIONS:
//   * I use 1tbs indentation style (4 spaces per level),
//     with mandatory braces for all control structures.
//     I do not indent class definitions within namespaces.
//   * I add minimal "flower box" comments to set apart
//     class and function definitions.
//   * I put "// end" comments on closing braces for
//     namespaces, classes, and functions.
//   * To better document their purposes, I freely change the names of
//     namespaces, classes, functions, variables and constants.
//   * I indiscriminately add, change or remove comments according to
//     my whims. As annotating and documenting the code with comments
//     is a big part of the exercise, I expect students to supply 
//     their own comments. Mine don't count.
//   * I eliminate unnecessary using statements.
//
// FUNCTIONAL MODIFICATIONS:
//   * There is an infinite loop in the program (i.e. a while(true)
//     statement that does not contain a break or a return statement.
//     As a consequence of this, there is no graceful way for the user
//     to terminate the program. The program does not detect when the
//     user has solved the puzzle.
//     [resolved, 09 Feb 2019]
//   * The program does not properly handle user input.  If the user
//     enters more than one character at a time, the program crashes.
//     "System.FormatException: String must be exactly one character long"
//     is not a friendly message. The user gets the same message when
//     he/she does not enter any input.
//     [resolved, 09 Feb 2019]
//   * The program does not properly prompt the user for input.
//     The word mask should be displayed before every request for input,
//     and the user should prompted for every input.
//     [resolved, 09 Feb 2019]
//   * ALL of the program functionality is stuffed in the Main() function.
//     Also, that program functionality is not broken up into bite-sized
//     logical functions. 
//     [resolved, 11 Feb 2019]
//   * There is no loss condition in the program.  The program does not 
//     detect when the user has failed to solve the puzzle.
//     [resolved, 09 Feb 2019]
//   * Besides the fact that the program does not detect when the user
//     has solved or not solved the puzzle, it does not provide a way to
//     replay the game... other than re-running the program from the 
//     command line.
//     [resolved, 11 Feb 2019]
//   * Could the word list be stored in a data file, rather than
//     hard-coded into the program?
//     [resolved, 12 Feb 2019]
//   * In the word mask, let's use dashes rather than asterisks.
//     [resolved, 11 Feb 2019]
//   * Playing Hangman, the user really expects to see a gallows and a
//     graphical representation of their progress towards winning or
//     losing the game.
//     [resolved, 12 Feb 2019]
//     I am still looking for a more compact way of representing the
//     gallows pictures.  (That's art?!?) Perhaps, you can suggest a
//     better way?
//   * For my CSC200 students, let's split up the Hangman class into 
//     a non-object-oriented Startup class and an object-oriented
//     HangmanGame class. Create objects with methods, and
//     where desirable, eliminate static functions.
//     [resolved, 12 Feb 2019]


using System;
using System.IO;
using System.Collections.Generic;
 
namespace mypgms {

//==========
class Startup {

    private readonly static string [] ynOpts = { "y", "n" };

    //----------
    public static void Main(string[] args) {
        HangmanGame hangman=new HangmanGame();
        if((args.Length!=1)||(!File.Exists(args[0]))||
                (!hangman.initWordList(args[0]))) {
            Console.WriteLine("usage: hangman.exe <wordlist file name>.txt");
            return;
        }
        do {
            hangman.gameDriver();
        } while(Useful.selectOpt("\nPlay again (Y|N)? ",ynOpts)==0);
        return;
    } // end function Main()
} // end class Startup


//==========
class HangmanGame {
    private Random randGen;

    private string [] listwords;

    private string mysteryWord;
    private char [] guess;

    //----------
    public HangmanGame() {
        randGen=new Random();
    } // end constructor HangmanGame()

    //----------
    public bool initWordList(string filepath) {
        try {
            List<string> templist=new List<string>();
            foreach (string line in File.ReadLines(filepath)) {
                templist.Add(line);
            }
            listwords=templist.ToArray();
            return true;
        } catch(Exception) {
            return false;
        }
    } // end function initWordList()

    //----------
    public void gameDriver() {
        Console.WriteLine("Welcome to Hangman!!!!!!!!!!");

        mysteryWord=listwords[randGen.Next(0,9)];
        initMask(mysteryWord);
 
        gameLoop();
        return;
    } // end function gameDriver()

    //----------
    private bool gameLoop() {
        for(int badguesses=0;badguesses<10;) {
            displayGallows(badguesses);
            Console.WriteLine(guess);
            Console.Write("\nPlease enter your guess: ");
            string userinput=Console.ReadLine();

            for(int ii=0;ii<userinput.Length;ii++) {
                char playerGuess=userinput[ii];
                bool badguess=applyGuessToMask(userinput[ii]);

                if(isSolved(guess)) {
                    Console.Write("\nCongratulations! ");
                    Console.WriteLine("You guessed the word.");
                    return true;
                }
                if(!badguess) {
                    badguesses++;
                    if(badguesses>=10) {
                        break;
                    }
                }
            }
        }
        displayGallows(10);
        Console.WriteLine(mysteryWord);
        Console.Write("\nToo many bad guesses! ");
        Console.WriteLine("You lose.");
        return false;
    } // end function gameLoop()

    //----------
    private void displayGallows(int badGuesses) {
        if((badGuesses<0)||(badGuesses>10)) {
            badGuesses=0;
        }
        Console.WriteLine();
        for(int ii=0;ii<14;ii++) {
            Console.WriteLine(gallows[badGuesses,ii]);
        }
        Console.WriteLine();
    } // end function displayGallows()

    //----------
    private bool applyGuessToMask(char playerGuess) {
        bool result=false;
        for(int ii=0;ii<mysteryWord.Length;ii++) {
            if(playerGuess==mysteryWord[ii]) {
                guess[ii]=playerGuess;
                result=true;
            } else {
                // this will never happen!
            }
        }
        return result;
    } // end function applyGuessToMask()

    //----------
    private void initMask(string mysteryWord) {
        guess=new char[mysteryWord.Length];
        for (int p=0; p < mysteryWord.Length; p++) {
            guess[p]='-';
        }
    } // end function initMask()

    //----------
    private bool isSolved(char [] mask) {
        for(int ii=0;ii<mask.Length;ii++) {
            if(mask[ii]=='-') {
                return false;
            }
        }
        return true;
    } // end function isSolved()

    private static string [,] gallows = {
    { // 00 - empty gallows
        "  +-------+        ",
        "  |       |        ",
        "  |                ",
        "  |                ",
        "  |                ",
        "  |                ",
        "  |                ",
        "  |                ",
        "  |                ",
        "  |                ",
        "  |                ",
        "  |                ",
        "  |                ",
        "--+----------------",
    }, { // 01 - head
        "  +-------+        ",
        "  |       |        ",
        "  |     (0 0)      ",
        "  |      +++       ",
        "  |                ",
        "  |                ",
        "  |                ",
        "  |                ",
        "  |                ",
        "  |                ",
        "  |                ",
        "  |                ",
        "  |                ",
        "--+----------------",
    }, { // 02 - torso
        "  +-------+        ",
        "  |       |        ",
        "  |     (0 0)      ",
        "  |      +++       ",
        "  |     -   -      ",
        "  |                ",
        "  |     |   |      ",
        "  |     =====      ",
        "  |     | | |      ",
        "  |                ",
        "  |                ",
        "  |                ",
        "  |                ",
        "--+----------------",
    }, { // 03 - right arm
        "  +-------+        ",
        "  |       |        ",
        "  |     (0 0)      ",
        "  |      +++       ",
        "  |    --   -      ",
        "  |   /            ",
        "  |  / /|   |      ",
        "  |     =====      ",
        "  |     | | |      ",
        "  |                ",
        "  |                ",
        "  |                ",
        "  |                ",
        "--+----------------",
    }, { // 04 - right hand
        "  +-------+        ",
        "  |       |        ",
        "  |     (0 0)      ",
        "  |      +++       ",
        "  |    --   -      ",
        "  |   /            ",
        "  |  / /|   |      ",
        "  |  o  =====      ",
        "  |     | | |      ",
        "  |                ",
        "  |                ",
        "  |                ",
        "  |                ",
        "--+----------------",
    }, { // 05 - left arm
        "  +-------+        ",
        "  |       |        ",
        "  |     (0 0)      ",
        "  |      +++       ",
        "  |    --   --     ",
        "  |   /       \\    ",
        "  |  / /|   |\\ \\   ",
        "  |  o  =====      ",
        "  |     | | |      ",
        "  |                ",
        "  |                ",
        "  |                ",
        "  |                ",
        "--+----------------",
    }, { // 06 - left hand
        "  +-------+        ",
        "  |       |        ",
        "  |     (0 0)      ",
        "  |      +++       ",
        "  |    --   --     ",
        "  |   /       \\    ",
        "  |  / /|   |\\ \\   ",
        "  |  o  =====  o   ",
        "  |     | | |      ",
        "  |                ",
        "  |                ",
        "  |                ",
        "  |                ",
        "--+----------------",
    }, { // 07 - right leg
        "  +-------+        ",
        "  |       |        ",
        "  |     (0 0)      ",
        "  |      +++       ",
        "  |    --   --     ",
        "  |   /       \\    ",
        "  |  / /|   |\\ \\   ",
        "  |  o  =====  o   ",
        "  |     | | |      ",
        "  |     | |        ",
        "  |     | |        ",
        "  |                ",
        "  |                ",
        "--+----------------",
    }, { // 08 - right foot
        "  +-------+        ",
        "  |       |        ",
        "  |     (0 0)      ",
        "  |      +++       ",
        "  |    --   --     ",
        "  |   /       \\    ",
        "  |  / /|   |\\ \\   ",
        "  |  o  =====  o   ",
        "  |     | | |      ",
        "  |     | |        ",
        "  |     | |        ",
        "  |      o         ",
        "  |                ",
        "--+----------------",
    }, { // 09 - left leg
        "  +-------+        ",
        "  |       |        ",
        "  |     (0 0)      ",
        "  |      +++       ",
        "  |    --   --     ",
        "  |   /       \\    ",
        "  |  / /|   |\\ \\   ",
        "  |  o  =====  o   ",
        "  |     | | |      ",
        "  |     | | |      ",
        "  |     | | |      ",
        "  |      o         ",
        "  |                ",
        "--+----------------",
    }, { // 10 - left foot
        "  +-------+        ",
        "  |       |        ",
        "  |     (+ +)      ",
        "  |      +++       ",
        "  |    --   --     ",
        "  |   / LOSER \\    ",
        "  |  / /|   |\\ \\   ",
        "  |  o  =====  o   ",
        "  |     | | |      ",
        "  |     | | |      ",
        "  |     | | |      ",
        "  |      o o       ",
        "  |                ",
        "--+----------------",
    } };
        
} // end class Hangman

//==========
static class Useful {

    //----------
    public static int selectOpt(string prompt,string [] options) {
        for(;;) {
            Console.Write(prompt);
            string userinp=Console.ReadLine().ToLower();

            for(int ii=0;ii<options.Length;ii++) {
                if(userinp==options[ii]) {
                    return ii;
                }
            }
                        
            Console.WriteLine("Invalid input. Try again...");
        }
    } // end function selectOpt()

} // end class Useful

} // end namespace mypgms

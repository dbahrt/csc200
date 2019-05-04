// filename: ta_testadmin.cs
// author: Dan Bahrt
// date: 4 May 2019

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace ta {

//==========
public class startup {

    //----------
    public static void Main(string [] args) {
        if(Console.WindowWidth<80) {
            Console.Write("This program requires that the Console Window ");
            Console.Write("Size must be ");
            Console.WriteLine();
            Console.Write("at least (80 columns by 30 rows). ");
            Console.Write("Presently, it is ("+Console.WindowWidth+
                " by "+Console.WindowHeight+") which is too small. ");
            Console.Write("Please resize this window, before attempting to ");
            Console.WriteLine("re-run this program.\n");
            Useful.enterContinue();
            return;
        }
        ConsoleColor origFGColor=Console.ForegroundColor;
        ConsoleColor origBGColor=Console.BackgroundColor;

        StudentInfo si=new StudentInfo();

        DateTime starttime=DateTime.Now;
        if(!File.Exists(si.getCourse()+"_"+si.getExamination()+"_questions.txt")) {
            Console.WriteLine("\nfile not found: "+si.getCourse()+"_"+si.getExamination()+"_questions.txt");

            Console.WriteLine("\nThis program requires that a text data file containing ");
            Console.WriteLine("the test questions be placed in the local directory from ");
            Console.WriteLine("whence this program is executed.  You can download this ");
            Console.WriteLine("data file from the Exam Assignment in our Moodle course ");
            Console.WriteLine("shell.");
            Console.WriteLine("\nThe information that you just provided ");
            Console.WriteLine("(YourName, the course name, and the exam type) ");
            Console.WriteLine("are all used to locate the files used by the program.  ");
            Console.WriteLine("If you did not copy the data file into the directory,  ");
            Console.WriteLine("or if you entered an invalid course name, or an invalid ");
            Console.WriteLine("exam type (which are parts of the text data file name), ");
            Console.WriteLine("you will see this error message and be given an opportunity ");
            Console.WriteLine("to restart the program.");
            Console.WriteLine("\nAlso be aware that this program produces a text file ");
            Console.WriteLine("(which includes YourName) containing your responses to the ");
            Console.WriteLine("test questions. Theoretically, your responses should be ");
            Console.WriteLine("preserved across multiple runs of the program. ");
            Console.WriteLine("Still you need to check your work, and maybe even re-enter ");
            Console.WriteLine("some of your responses, because.... No guarantees! ");
            Console.WriteLine();
            Useful.enterContinue();
            return;
        }

        TestAdministrator.takeTest(si.getCourse()+"_"+si.getExamination(),si.getStudentName(),starttime);

        Useful.clearLine(5);
Console.WriteLine("You will need to upload your responses file ");
Console.WriteLine("("+si.getCourse()+"_"+si.getExamination()+"_"+si.getStudentName()+".txt)");
Console.WriteLine("into the final exam dropbox in the Moodle shell for our course."); 
Console.WriteLine();
Console.WriteLine("It would also be a very good idea for you to keep "); 
Console.WriteLine("a backup copy of that file, just in case I encounter "); 
Console.WriteLine("problems in processing it. "); 
Console.WriteLine();
        Useful.enterContinue();

/*
        Console.Clear();
        // Console.ResetColor();
        Useful.setColors(ConsoleColor.Green,ConsoleColor.Black);
        for(int ii=0;ii<iterativeDevelopmentMessage.Length;ii++) {
            Console.WriteLine(iterativeDevelopmentMessage[ii]);
        }
*/

        Console.WriteLine();

        return;
    } // end function Main()

    //----------
    private static string [] iterativeDevelopmentMessage = {
        "And now I'd like to preach a bit about a couple of Computer Science topics: ",
        "I want you all to take note and remember... ",
        "",
        "This examination program started with the following sample program:",
        "",
        "    Hello World!",
        "    Goodbye cruel World...",
        "",
        "and from thence it grew iteratively, step-by-step, over a period of a couple ",
        "of weeks. At no point did I try to work with any large block of questionably ",
        "functional code, and I always kept a known-working fallback version of the ",
        "program so that I could revert to in case some provisional block of code ",
        "failed miserably. I know it seems slow and tedious, but this approach will ",
        "save you more time than you can imagine. Debugging a large block of buggy ",
        "code can take a very long time. ",
        "",
        "To those students who felt that we did not do enough actual programming in ",
        "this class, I would encourage you to adjust your notion of what constitutes ",
        "effective programming. ",
        "",
        "A mature programming process is more about becoming famialiar with ",
        "pre-existing code, reading it and figuring out what it does, documenting it, ",
        "fixing it (if necessary), organizing it, refactoring it, and synthesizing new ",
        "solutions than it is about simply writing your own code. ",
        "",
        "Just because you wrote it does not automatically make it better, ",
        "more familiar perhaps, but that advantage can easily slip away simply by ",
        "walking away from your code for a short while.  Then you slip into the mode ",
        "of becoming familiar with your own pre-existing code, reading your own code ",
        "and figuring out what it does.... ",
        "",
        "While I was developing this examination program, many times I incorporated ",
        "entire chunks of working code from my personal ",
        "portfolio of sample programs. You remember that FileStream example program ",
        "that I distributed and discussed during the second week of class? I copied ",
        "that almost verbatim into this program.  Other times I wrote small snippets ",
        "of original code (I guess you could call them sample programs), working with ",
        "and debugging them until they performed as needed... and then I incorporated ",
        "into this examination program.",
        "",
        "Eventually, the program grew into a substantial 800+ line application ",
        "program. It's not finished yet. It may never be completely finished, but ",
        "that's a quality of a good program. It can grow as the developer's ",
        "understanding of the problem grows. But already it has been useful, and it ",
        "shows substantial promise.  The semi-automated grading program comes next. "};

} // end class startup


//==========
public class TestAdministrator {

    // setup by TestAdministrator();
    private static string [] quids;
    private static int [] weights;
    private static string [] questions;
    private static string [] responses;
    private static int maxq, unanswered;

    //----------
    private static void initta() {
        quids=new string[200];
        weights=new int[200];
        questions=new string[200];
        responses=new string[200];
        maxq=0;
        unanswered=0;
    } // end constructor TestAdministrator

    //----------
    public static void takeTest(string examname,string studentname,DateTime starttime) {
        initta();

        readQuestions(examname+"_questions.txt");
        ReadResponses(examname+"_"+studentname+".txt");

        Console.Clear();

        Question qst=new Question(Console.WindowWidth-4,12,2,6);
        Response rsp=new Response(Console.WindowWidth-4,12,2,18);

        for(int curq=0;;) {
            displayQuestion(curq,starttime,qst,rsp);

            int optnum;
            string option=navbar(out optnum);

            if((option=="")&&((optnum>=1)&&(optnum<=maxq))) {
                curq=optnum-1;
                displayQuestion(curq,starttime,qst,rsp);
                continue;
            }
            if((option=="")||(option.ToLower()=="a")) {
                if(answerQuestion(curq,starttime,rsp)) {
                    // response was changed
                    // write changes out to disk

                    WriteResponses(examname+"_"+studentname+".txt");
                }
                continue;
            }
            if(option.ToLower()=="n") {
                curq++;
                if(curq>=maxq) {
                    curq=0;
                }
                displayQuestion(curq,starttime,qst,rsp);
                continue;
            }
            if(option.ToLower()=="p") {
                curq--;
                if(curq<0) {
                    curq=maxq-1;
                }
                displayQuestion(curq,starttime,qst,rsp);
                continue;
            }
            if(option.ToLower()=="s") {
                if(unanswered==0) {
                    Console.WriteLine("all questions have been answered.");
                } else {
                    int end_search=curq;
                    for(;;) {
                        curq++;
                        if(curq>=maxq) {
                            curq=0;
                        }
                        if(curq==end_search) {
                            Console.WriteLine("all questions appear to have been answered.");
                            break;
                        }
                        if(responses[curq]=="") {
                            break;
                        }
                    }
                    displayQuestion(curq,starttime,qst,rsp);
                }   
                continue;
            }
            if(option.ToLower()=="d") {
                Console.Clear();
                Useful.setColors(ConsoleColor.Green,ConsoleColor.Black);
                Useful.clearLine(0);
                Console.Write("All done?  ");
                Useful.clearLine(1);
                unanswered=0;
                for(int ii=0;ii<maxq;ii++) {
                    if(responses[ii].Trim()=="") {
                        unanswered++;
                    }
                }
                if(unanswered==0) {
                    Console.Write("All questions appear to have some kind of an answer.");
                } else {
                    Console.Write("    UNANSWERED="+unanswered);
                }
                Useful.clearLine(2);
                Console.Write("Are you sure your want to leave ");
                Console.Write("this program at this time (YES|else)? ");
                if(Console.ReadLine()=="YES") {
                    break;
                }
                continue;
            }

        } // end for()
    } // end method takeTest()

    //----------
    private static string navbar(out int qnum) {
        for(;;) {
            Useful.setColors(ConsoleColor.Green,ConsoleColor.Black);
            Useful.clearLine(0);
Console.Write("Navigate? ___      Specify QUESTION # or one of these one-letter commands: ");
            Useful.clearLine(1);
Console.Write("                   A)nswer_current  N)ext  P)rev  S)can_next_unanswered  D)one");

            Console.SetCursorPosition(10,0);
            string option=Console.ReadLine().Trim();
            int optnum=0;
            if(Int32.TryParse(option,out optnum)) {
                if((optnum>=1)&&(optnum<=maxq)) {
                    Useful.clearLine(3);
                    qnum=optnum;
                    return "";
                }

                Console.SetCursorPosition(0,3);
                Console.Write("Invalid input....  QUESTION # must be ");
                Console.WriteLine("between 1 and "+maxq+". Try again.");
                continue;
            }

            if((option=="")||
               (option.ToLower()=="a")||
               (option.ToLower()=="n")||
               (option.ToLower()=="p")||
               (option.ToLower()=="s")||
               (option.ToLower()=="d")) {
                Useful.clearLine(3);
                qnum=0;
                return option.ToLower();
            }
            
            Useful.clearLine(3);
            Console.Beep();
            Console.WriteLine("invalid command entered: "+option);
        } // end for()
    }

    //----------
    private static void displayQuestion(int curq,DateTime starttime,
        Question qst,Response rsp) {

        qst.drawQuestionHdr(curq,maxq,unanswered,starttime,weights[curq]);
        qst.fillQuestionBox(questions[curq]);
        qst.drawQuestionBox();
        rsp.fillResponseBox(responses[curq]);
        rsp.drawResponseBox();
    }

    //----------
    private static bool answerQuestion(int curq,DateTime starttime,
        Response rsp) {

        string preresponse=responses[curq];
        rsp.fillResponseBox(preresponse);
        rsp.drawResponseBox();
        responses[curq]=rsp.editResponseBox();
        if((preresponse=="")&&(responses[curq]!="")) {
            unanswered--;
        }
        if(preresponse==responses[curq]) {
            return false;
        }
        // response was changed
        return true;
    } 

    //----------
    private static void ReadResponses(string filepath) {
        if(!File.Exists(filepath)) {
            for(int ii=0;ii<maxq;ii++) {
                responses[ii]="";
            } 
            unanswered=maxq;
            WriteResponses(filepath);
            return;
        }

        string[] inlines=File.ReadAllLines(filepath);

        unanswered=maxq;
        int tmpoff=0;
        for(int ii=0;ii<inlines.Length;ii++) {
            string inplin=inlines[ii];
            if(inplin=="") { continue; }
            int off=inplin.IndexOf(", ");
            if(off<=0) {
                continue;
            }
            quids[tmpoff]=inplin.Substring(0,off);
            inplin=inplin.Substring(off+2).Trim();

            responses[tmpoff++]=inplin;
            if(inplin!="") {
                unanswered--;
            }
            if(tmpoff>=200) { break; }
        }
    } // end method ReadResponses()

    //----------
    private static void WriteResponses(string filepath) {
        List<string> outlines=new List<string>();
        for(int ii=0;ii<maxq;ii++) {
            outlines.Add(quids[ii]+", "+responses[ii]);
        }

        File.WriteAllLines(filepath,outlines);
    } // end method WriteResponses()

    //----------
    private static void readQuestions(string filepath) {
        string[] inlines=File.ReadAllLines(filepath);

        string col1,col2,col3;
        maxq=0;
        for(int ii=0;ii<inlines.Length;ii++) {
            string inplin=inlines[ii];
            if(inplin=="") { continue; }
            int off=inplin.IndexOf(", ");
            if(off<=0) {
                continue;
            }
            col1=inplin.Substring(0,off);
            inplin=inplin.Substring(off+2).Trim(); 
            off=inplin.IndexOf(", ");
            if(off<=0) {
                continue;
            }
            col2=inplin.Substring(0,off);
            inplin=inplin.Substring(off+2).Trim();
            col3=inplin;
            if(!Int32.TryParse(col2,out weights[maxq])) {
                continue;
            }
                
            quids[maxq]=col1;
            questions[maxq++]=col3;
            if(maxq>=200) { break; }
        }
    } // end method readQuestions()

} // end class TestAdministrator


//==========
public class StudentInfo {
    private string studentname;
    private string course;
    private string examination;

    //----------
    public StudentInfo() {
        Useful.setColors(ConsoleColor.Green,ConsoleColor.Black);
        Console.Clear();
        Useful.clearLine(0);
        Console.WriteLine("Greetings!  Before we get started...");
        Console.WriteLine("    I need to get a couple of pieces of information from you,");
        Console.WriteLine("    in order to process your examination.");

        studentname=getPromptedInput(4,
            "What is your name (FirstLast, no spaces)?  ");

        course=getPromptedInput(6,
            "What course are you taking (csc999, no spaces)?  ");

        examination=getPromptedInput(8,
            "What examination are you taking (midterm or final)?  ");

        Console.WriteLine();
        Useful.enterContinue();
    } // end constructor StudentInfo()

    //----------
    public string getStudentName() { 
        return studentname;
    } 

    //----------
    public string getCourse() {
        return course;
    }

    //----------
    public string getExamination() {
        return examination;
    }

    //----------
    private static string getPromptedInput(int row,string prompt) {
        string result="";
        for(;;) {
            Useful.clearLines(row,5);
            Console.Write(prompt);
            string temp=Console.ReadLine();
            StringBuilder tempsb=new StringBuilder();
            for(int ii=0;ii<temp.Length;ii++) {
                if(temp[ii]==' ') {
                    continue;
                }
                tempsb.Append(temp[ii]);
            }
            result=tempsb.ToString();
            Console.WriteLine();
            Console.WriteLine("    Here's what I got:  "+result);
            Console.WriteLine();
            Console.Write("        Is this correct (y|else)? ");
            temp=Console.ReadLine().ToLower();
            if(temp=="y") {
                Useful.clearLines(row,5);
                Console.Write(prompt+result);
                break;
            }
        }  
        return result;
    } // end method getPromptedInput()

} // end class StudentInfo


//==========
public class Question {
    char[] questbuf;
    int questoff,maxoff,maxrow,maxcol;
    int xoff,yoff;

    //----------
    public Question(int cols, int rows, int xoff, int yoff) {
        this.maxrow=rows;
        this.maxcol=cols;
        this.xoff=xoff;
        this.yoff=yoff;
        maxoff=maxrow*maxcol;
        questbuf=new char[maxoff];
        clearQuestbuf();
        questoff=0;
    }

    //----------
    public void clearQuestbuf() {
        for(int ii=0;ii<maxoff;ii++) {
            questbuf[ii]=' ';
        }
    }

    //----------
    public void drawQuestionBox() {
        Useful.setColors(ConsoleColor.Yellow,ConsoleColor.Black);
        for(int ii=0;ii<12;ii++) {
            Console.SetCursorPosition(xoff,yoff+ii);
            for(int jj=0;jj<maxcol;jj++) {
                Console.Write(questbuf[ii*maxcol+jj]);
            }
        }
        Console.SetCursorPosition(xoff,yoff);
    }

    //----------
    public void drawQuestionHdr(int curq,int maxq,int unanswered,
        DateTime starttime,int weight) {

        Console.SetCursorPosition(0,5);
        Useful.setColors(ConsoleColor.White,ConsoleColor.Black);
        Console.Write("QUESTION #: "+(curq+1).ToString("D3")+"    ");
        Console.Write("ANSWERED: "+(maxq-unanswered).ToString("D3")+" of "+
            maxq.ToString("D3")+"    ");
        DateTime currtime=DateTime.Now;
        TimeSpan elapsed = DateTime.Now.Subtract(starttime);
        long totseconds= (long) elapsed.TotalSeconds;
        Console.Write("ELAPSED TIME: ");
        Console.Write(elapsed.Hours.ToString("D2"));
        Console.Write(":"+elapsed.Minutes.ToString("D2"));
        Console.Write(":"+elapsed.Seconds.ToString("D2"));
        Console.Write("    WEIGHT: "+weight.ToString("D2"));
    }

    //----------
    public void fillQuestionBox(string text) {
        clearQuestbuf();
        questoff=0;
        
        int off;
        string temp="";
        while((questoff<maxoff)&&(text.Length>0)) {
            if(text.Length<maxcol) {
                temp=text;
                text="";
            } else {
                temp=text.Substring(0,maxcol);
                off=temp.LastIndexOf(" ");
                if(off>0) {
                    temp=text.Substring(0,off);
                    text=text.Substring(off+1);
                }
            }
            off=questoff;
            for(int ii=0;ii<temp.Length;ii++) {
                if((temp[ii]>=' ')&&(temp[ii]<='~')) {
                    questbuf[off++]=temp[ii];
                    if(off>=maxoff) { break; }
                }
            }
            questoff+=maxcol;
        }
    }

} // end class Question

//==========
public class Response {
    char[] respbuf;
    int respoff,maxoff,maxrow,maxcol;
    int xoff,yoff;

    //----------
    public Response(int cols, int rows, int xoff, int yoff) {
        maxrow=rows;
        maxcol=cols;
        this.xoff=xoff;
        this.yoff=yoff;
        maxoff=maxrow*maxcol;
        respbuf=new char[maxoff];
        clearRespbuf();
        respoff=0;
    }

    //----------
    public void clearRespbuf() {
        for(int ii=0;ii<maxoff;ii++) {
            respbuf[ii]=' ';
        }
    }

    //----------
    public void drawResponseBox() {
// Useful.dbg("drawResponseBox():  xoff="+xoff+"  yoff="+yoff);
        Useful.setColors(ConsoleColor.Black,ConsoleColor.White);
        for(int ii=0;ii<12;ii++) {
            Console.SetCursorPosition(xoff,yoff+ii);
            for(int jj=0;jj<maxcol;jj++) {
                Console.Write(respbuf[ii*maxcol+jj]);
            }
        }
        Console.SetCursorPosition(xoff,yoff);
    }

    //----------
    public void fillResponseBox(string text) {
        clearRespbuf();
        respoff=0;
        
        for(int ii=0;ii<text.Length;ii++) {
            if((text[ii]>=' ')&&(text[ii]<='~')) {
                respbuf[respoff]=text[ii];
                respoff++;
                if(respoff>maxoff) { respoff=respoff%maxoff; }
                continue;
            }
            if(text[ii]=='\n') {
                int scrrow=respoff/maxcol;
                int scrcol=respoff-scrrow*maxcol;
                respoff-=scrcol;
                respoff+=maxcol;
                if(respoff>=maxoff) {
                    Console.Beep();
                    respoff=respoff%maxoff;
                }
            }
        }
    }

    //----------
    public string editResponseBox() {
        // clearRespbuf();
        respoff=0;
        posresp();

        char inpkey;
        for(;;) {
            Char inpchr=Useful.getKeyPress(out inpkey);
            if((inpchr>=' ')&&(inpchr<='~')) {
                Console.Write(inpchr);
                respbuf[respoff++]=inpchr;
                if(respoff>=maxoff) {
                    Console.Beep();
                    respoff=respoff%maxoff;
                }
                posresp();
                continue;
            }
            if((inpchr==8)||(inpkey==(int)ConsoleKey.Backspace)) {
                respoff--;
                if(respoff<0) {
                    Console.Beep();
                    respoff=(respoff+maxoff)%maxoff;
                }
                posresp();

                Console.Write(' ');
                respbuf[respoff]=' ';
                posresp();
                continue;
            }
            if((inpchr==9)&&(inpkey==(int)ConsoleKey.Tab)) {
                break;
            }
            if(inpkey==(int)ConsoleKey.Enter) {
                int scrrow=respoff/maxcol;
                int scrcol=respoff-scrrow*maxcol;
                int respoff2=respoff;
                respoff2-=scrcol;
                respoff2+=maxcol;
                for(int ii=respoff;ii<respoff2;ii++) {
                    Console.Write(' ');
                    respbuf[ii]=' ';
                }
                respoff=respoff2;   
                if(respoff>=maxoff) {
                    Console.Beep();
                    respoff=respoff%maxoff;
                }
                posresp();
                continue;
            }

            if((inpchr==0)&&(inpkey==(int)ConsoleKey.RightArrow)) {
                respoff++;
                if(respoff>=maxoff) {
                    Console.Beep();
                    respoff=respoff%maxoff;
                }
                posresp();
                continue;
            }
            if((inpchr==0)&&(inpkey==(int)ConsoleKey.LeftArrow)) {
                respoff--;
                if(respoff<0) {
                    Console.Beep();
                    respoff=(respoff+maxoff)%maxoff;
                }
                posresp();
                continue;
            }
            if((inpchr==0)&&(inpkey==(int)ConsoleKey.UpArrow)) {
                respoff-=maxcol;
                if(respoff<0) {
                    Console.Beep();
                    respoff+=maxoff;
                }
                posresp();
                continue;
            }
            if((inpchr==0)&&(inpkey==(int)ConsoleKey.DownArrow)) {
                respoff+=maxcol;
                if(respoff>=maxoff) {
                    Console.Beep();
                    respoff-=maxoff;
                }
                posresp();
                continue;
            }
            if((inpchr==0)&&(inpkey==(int)ConsoleKey.Tab)) {
        Console.SetCursorPosition(10,24);
        Console.Write("tab<<<");
                Console.Beep();
        posresp();
                continue;
            }
            if((inpchr==0)&&(inpkey==(int)ConsoleKey.Insert)) {
        Console.SetCursorPosition(10,24);
        Console.Write("insert");
                Console.Beep();
        posresp();
                continue;
            }
            if((inpchr==0)&&(inpkey==(int)ConsoleKey.Delete)) {
        Console.SetCursorPosition(10,24);
        Console.Write("delete");
                Console.Beep();
        posresp();
                continue;
            }
        } // end for()

        int offb=0, offx=0;
        for(int ii=0;ii<respbuf.Length;ii++) {
            offb=ii;
            if(respbuf[ii]!=' ') {
                break;
            }
        }

        for(int ii=respbuf.Length-1;ii>0;ii--) {
            offx=ii;
            if(respbuf[ii]!=' ') {
                break;
            }
        }

        StringBuilder result=new StringBuilder();
        char last=' ';
        for(int ii=offb;ii<=offx;ii++) {
            if((last==' ')&&(respbuf[ii]==' ')) {
                continue;
            }
            result.Append(respbuf[ii]);
            last=respbuf[ii];
        }
        return result.ToString().ToLower();
    }

    //----------
    private void posresp() {
        Console.SetCursorPosition(respoff%maxcol+xoff,respoff/maxcol+yoff);
    }


} // end class Response


//==========
class Useful {

    //----------
    public static void setColors(ConsoleColor fg,ConsoleColor bg) {
        Console.ForegroundColor=fg;
        Console.BackgroundColor=bg;
    }

    //----------
    public static void clearLine(int row) {
        clearLines(row,1);
    }

    //----------
    public static void clearLines(int row,int count) {
        Console.SetCursorPosition(0,row);
        int maxcol=Console.WindowWidth*count;
        for(int ii=0;ii<maxcol;ii++) {
            Console.Write(" ");
        }
        Console.SetCursorPosition(0,row);
    }

    //----------
    public static Char getKeyPress(out char key) {
        ConsoleKeyInfo cki;
        for(;;) {
            cki = Console.ReadKey(true);
            if((cki.KeyChar>=' ')&&(cki.KeyChar<='~')) {
                key=(char)cki.Key;
                return cki.KeyChar;
            }
            if((cki.KeyChar==8)||(cki.KeyChar==9)||(cki.KeyChar==10)) {
                key=(char)cki.Key;
                return cki.KeyChar;
            }

            if((cki.KeyChar==0)&&
               ((cki.Key==ConsoleKey.Backspace)||
                (cki.Key==ConsoleKey.LeftArrow)||
                (cki.Key==ConsoleKey.RightArrow)||
                (cki.Key==ConsoleKey.UpArrow)||
                (cki.Key==ConsoleKey.DownArrow)||
                (cki.Key==ConsoleKey.Insert)||
                (cki.Key==ConsoleKey.Delete)||
                (cki.Key==ConsoleKey.Tab))) {
                key=(char)cki.Key;
                return cki.KeyChar;
            }
        }
    }

    //----------
    public static void enterContinue() {
        Console.WriteLine();
        Console.Write("Press Enter to continue...");
        Console.ReadLine();
    }

    //----------
    public static void dbg(string msg) {
        FileStream fs;

        try {
            fs=new FileStream("dbg.txt",FileMode.Append,FileAccess.Write);
        } catch(Exception) {
            Console.WriteLine("unable to open file: "+"dbg.txt"); 
            throw;
        }
        
        byte[] info = new UTF8Encoding(true).GetBytes(msg+"\r\n");
        try {
            fs.Write(info, 0, info.Length);
        } catch(Exception) {
            Console.WriteLine("unable to write to file: dbg.txt"); 
            throw;
        }

        fs.Close();
    } // end method dbg()


} // end class Useful

} // end namespace ta

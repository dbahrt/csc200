// filename: ta_testadmin.cs
// author: Dan Bahrt
// date: 30 April 2019

using System;
using System.IO;
using System.Text;

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
            return;
        }
        ConsoleColor origFGColor=Console.ForegroundColor;
        ConsoleColor origBGColor=Console.BackgroundColor;

        StudentInfo si=new StudentInfo();
args[0]=si.getCourse()+"_"+si.getExamination();

Console.WriteLine("args[0]="+args[0]);

        DateTime starttime=DateTime.Now;

        TestAdministrator.takeTest(si.getCourse()+"_"+si.getExamination(),si.getStudentName(),starttime);

        Useful.clearLine(5);
Console.WriteLine("You will need to upload your responses file (");
Console.WriteLine(si.getCourse()+"_"+si.getExamination()+"_"+si.getStudentName()+".txt)");
Console.WriteLine("into the final exam dropbox ");
Console.WriteLine("in the Moodle shell for our course."); 
Console.WriteLine();
Console.WriteLine("It would also be a very good idea for you to keep"); 
Console.WriteLine("a backup copy of that file, just in case I encounter"); 
Console.WriteLine("problems in processing it. "); 
Console.WriteLine();
Console.WriteLine("I have your email address as ");
Console.WriteLine(si.getEmailAddress());
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
    private static void setColors(ConsoleColor fg,ConsoleColor bg) {
        Console.ForegroundColor=fg;
        Console.BackgroundColor=bg;
    }

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
        readQuestions(examname);

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

                    WriteResponses(examname+"_"+studentname);
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
    private static void WriteResponses(string exam) {
        string responses_path = exam+".txt";

        if (File.Exists(responses_path)) {
            File.Delete(responses_path);
        }

        FileStream fs;

        try {
            fs=new FileStream(responses_path,FileMode.Create,FileAccess.Write);
        } catch(Exception) {
            Console.WriteLine("unable to create file: "+responses_path); 
            throw;
        }
        
        for(int ii=0;ii<maxq;ii++) {
            WriteLine(fs,quids[ii]+"  "+responses[ii]);
        }

        fs.Close();
    } // end method WriteResponses()

    //----------
    private static void WriteLine(FileStream fs, string value) {
        byte[] info = new UTF8Encoding(true).GetBytes(value+"\r\n");
        try {
            fs.Write(info, 0, info.Length);
        } catch(Exception) {
            Console.WriteLine("unable to write to file: "); 
            throw;
        }
    } // end method WriteLine()

    private static bool EOFDetected=false;
    //----------
    private static void readQuestions(string exam) {

        string questions_path = exam+"_questions.txt";

        FileStream fs;

        try {
            fs=new FileStream(questions_path,FileMode.Open,FileAccess.Read);
        } catch(Exception) {
            Console.WriteLine("unable to open file: "+questions_path); 
            throw;
        }

        EOFDetected=false;
        maxq=0;
        for(;;) {
            string inplin=ReadLine(fs);
            if(inplin==null) {
                break;
            }
            if(inplin=="") { continue; }
            int off=inplin.IndexOf("  ");
            if(off>0) {
                quids[maxq]=inplin.Substring(0,off);
                inplin=inplin.Substring(off).Trim();
            } else {
                quids[maxq]="";
            }

            off=inplin.IndexOf("  ");
            if(off>0) {
                string temp=inplin.Substring(0,off);
                inplin=inplin.Substring(off).Trim();
                if(!Int32.TryParse(temp,out weights[maxq])) {
                    weights[maxq]=1;
                }
            } else {
                weights[maxq]=1;
            }
                
            questions[maxq++]=inplin;
            if(maxq>=200) { break; }
        }

        fs.Close();

        for(int ii=0;ii<maxq;ii++) {
            responses[ii]="";
        } 
        unanswered=maxq;

    } // end method readQuestions()

    //----------
    // we are reading lines one-by-one.
    //----------
    private static string ReadLine(FileStream fs) {
        if(EOFDetected) {
            return null;
        }

        // TODO: rewrite this function to allow for arbitrarily long lines
        byte[] buf = new byte[1024576];
        int bufptr = 0;

        int inchr;
        for(;;) {
            inchr=fs.ReadByte();
            if(inchr==-1) {
                EOFDetected=true;
                break;
            }
            if(inchr==10) {
                break;
            }
            if(inchr==13) {
                continue;
            }
            buf[bufptr++]=(byte)inchr;
        }
        UTF8Encoding temp = new UTF8Encoding(true);
        return(temp.GetString(buf,0,bufptr));
    } // end method ReadLine()

} // end class TestAdministrator


//==========
public class StudentInfo {
    private string studentname;
    private string emailaddress;
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

        emailaddress=getPromptedInput(10,
            "What is your email address?  ");

        Console.WriteLine();
        Useful.enterContinue();
    } // end constructor StudentInfo()

    //----------
    public string getStudentName() { 
        return studentname;
    } 

    //----------
    public string getEmailAddress() {
        return emailaddress;
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
        setColors(ConsoleColor.Yellow,ConsoleColor.Black);
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
        setColors(ConsoleColor.White,ConsoleColor.Black);
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

    //----------
    private Char getKeyPress(out char key) {
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
    private void setColors(ConsoleColor fg,ConsoleColor bg) {
        Console.ForegroundColor=fg;
        Console.BackgroundColor=bg;
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
        setColors(ConsoleColor.Black,ConsoleColor.White);
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
            Char inpchr=getKeyPress(out inpkey);
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

    //----------
    private Char getKeyPress(out char key) {
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
    private void setColors(ConsoleColor fg,ConsoleColor bg) {
        Console.ForegroundColor=fg;
        Console.BackgroundColor=bg;
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
    } // end method WriteResponses()


} // end class Useful

} // end namespace ta

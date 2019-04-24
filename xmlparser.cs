// Dan Bahrt
// Startup.cs

using System;
using System.IO;

namespace mypgms {


//==========
class Startup {

    private const int SUCCESS=0;
    private const int FAIL=1;

    private static string gDefType="knowt";

    //----------
    public static int Main(string [] args) {

        if(args.Length!=1) {
            usageMessage("Startup");
            Console.Error.WriteLine("\treads and dumps content of "+
               "specified XML file.");
            return FAIL;
        }

        //----------
        // just in case the specified XML file does not exist,
        // try to premake an empty file.
        //----------
        string infilename = args[0];
        if(!File.Exists(infilename)) {
            string fileContents="<"+gDefType+"s>\n</"+gDefType+"s>\n";
            try {
                File.WriteAllText(infilename,fileContents);
            } catch (Exception) {
                usageMessage("Startup");
                Console.Error.WriteLine("\tcannot create specified file "+
                   infilename+"\n.");
                return FAIL;
            }
        }

        //----------
        // open file for input AND output
        // this is a bit different from what you have seen before...
        // usually, we open a file for Read or Write.
        // with this file, we are going to do both.
        //----------
        FileStream infile;
        try {
            infile=new FileStream(infilename,FileMode.Open,
                FileAccess.ReadWrite);
        } catch (Exception) {
            usageMessage("Startup");
            Console.Error.WriteLine("\tcannot open specified file "+
               infilename+"\n.");
            return FAIL;
        }


        int result=fileMaintenanceDriver(infile);


        //----------
        // close the input file as we are all done with it
        //----------
        infile.Close();

        if(result==XMLObject.ERROR) {
            Console.WriteLine("\nErrors detected...");
            return FAIL;
        } else if(result==XMLObject.FAIL) {
            return FAIL;
        }
        // no news is good news...
        return SUCCESS;
    } // end Main() function

    //----------
    private static void usageMessage(string programName) {
        Console.Error.WriteLine("usage: "+programName+
            " <XML_filename>");
    }

    //----------
    private static int fileMaintenanceDriver(FileStream infile) {
        XMLObject xmlobj;
        try {
            xmlobj=new XMLObject(50,gDefType);
        } catch(Exception) {
            Console.Error.WriteLine("fileMaintenanceDriver(): "+
                "cannot instantiate XMLObject()");
            return FAIL;
        }

        int result;
        for(;;) {
            result=xmlobj.parseFile(infile);
            if(result!=XMLObject.SUCCESS) {
                break;
            }
            xmlobj.dump();

            // Console.WriteLine(xmlobj.generateXML());
            // Console.WriteLine();
        }

        return result;
    }  // end fileMaintenanceDriver() function

} // end Startup class



//==========
class XMLObject {

    public const int SUCCESS=0;
    public const int KEEPLOOPING=2;
    public const int FAIL=1;
    public const int ERROR=-1;

    //----------

    private string gXMLContainer;

    private string [] gAttributes;
    private string [] gValues;

    private int gAttributeCount;
    private int gAllocatedCount;

    private long gDataFileLine=0;
    private long gDataFileOffset=0;
    private long gTagOffset=0;

    // We need to keep track of where
    // the last closing container tag
    // is located in the data file.
    // This is where we will add new containers.
    private long gLastContainerOffset=0;

    private bool gInsideContainer=false;
    private string gParseToken="";
    private string gOpeningTag="";
    private string gAttribute="";

    //----------
    public XMLObject(int allocate,string container) {
        if(allocate<1) {
            Console.Error.WriteLine("XMLObject.init(): allocate = {0}",
                allocate);
            throw new Exception("XMLObject instantiation failed.");
        }

        gXMLContainer=container;

        try {
            gAttributes=new string[allocate];
        } catch (OutOfMemoryException e) {
            Console.Error.WriteLine("e={0}\n\n",e);

            Console.Error.WriteLine("XMLObject.init(): "+
                "unable to allocate gAttributes[]");
            throw new Exception("XMLObject instantiation failed.");
        }

        try {
            gValues=new string[allocate];
        } catch (OutOfMemoryException e) {
            Console.Error.WriteLine("e={0}\n\n",e);

            Console.Error.WriteLine("XMLObject.init(): "+
                "unable to allocate gValues[]");
            throw new Exception("XMLObject instantiation failed.");
        }

        gAllocatedCount=allocate;
        clear();


        gDataFileLine=1;
        gDataFileOffset=0;
        gTagOffset=0;

        gLastContainerOffset=0;

        gInsideContainer=false;
        gParseToken="";
        gOpeningTag="";
        gAttribute="";
    } // end constructor XMLObject()

    //----------
    // empty out our XML object.  later on, we'll find that 
    // we probably should do more housekeeping than this, but...
    //----------
    public void clear() {
        gAttributeCount=0;
    
        setAttribute("BEGPTR","");
        setAttribute("ENDPTR","");
    }

    //----------
    // dump out the parallel arrays of the XML object out to the console.
    //----------
    public void dump() {
        Console.WriteLine("\n"+gXMLContainer);
        for(int ii=0;ii<gAttributeCount;ii++) {
            Console.WriteLine( "    "+gAttributes[ii]+":  "+gValues[ii]);
        }
        Console.WriteLine();
    }

    //----------
    private int findAttribute(string attrib) {
        for(int ii=0;ii<gAttributeCount;ii++) {
            if(string.Compare(gAttributes[ii],attrib)==0) {
                return ii;
            }
        }
        return -1;
    }

    //----------
    public string generateXML() {
        string xmlString="";
    
        xmlString+="<"+gXMLContainer+">\r\n";
        for(int ii=0;ii<gAttributeCount;ii++) {
            string attr=gAttributes[ii];
            if((String.Compare(attr,"BEGPTR")==0)||
               (String.Compare(attr,"ENDPTR")==0)) {
                continue;
            }
            xmlString+="  <"+attr+">"+ gValues[ii]+ "</"+attr+">\r\n";
        }
        xmlString+="</"+gXMLContainer+">\r\n";
        return xmlString;
    }

    //----------
    // scan the parallel arrays of our XML object to find 
    // a specified data attribute
    //----------
    public string getAttribute(string attrib) {
        for(int ii=0;ii<gAttributeCount;ii++) {
            if(String.Compare(gAttributes[ii],attrib)==0) {
                return(gValues[ii]);
            }
        }
        return(null);
    }

    //----------
    public bool setAttribute(string attrib,string value) {
        int index=findAttribute(attrib);
        if(index>=0) {
            gValues[index]=value;
            return true;
        }

        // TODO: we could expand the tables
        if(gAttributeCount>=gAllocatedCount) {
            Console.Error.WriteLine("addAttribute(): table is full");
            return false;
        }
    
        gAttributes[gAttributeCount]=attrib;
        gValues[gAttributeCount]=value;
        gAttributeCount++;

        return true;
    }


    //----------
    public void setFileOffset(long value) {
        gDataFileOffset=value;
    }

    //----------
    public void captureLastClosingContainerTagOffset() {
        gLastContainerOffset=gTagOffset;
    }

    //----------
    public void setLastClosingContainerTagOffset(long value) {
        gLastContainerOffset=value;
    }

    //----------
    public long getLastClosingContainerTagOffset() {
        return gLastContainerOffset;
    }

    //----------
    // process a '<' character
    //----------
    private void parse1(char inchr) {
        // we're processing a '<' character...
        // XML tags start with '<', but at the same time,
        // the '<' also terminates embedded text
        if(gInsideContainer) {
            // we only care about attributes gInsideContainer...
            gParseToken.Trim();
            gAttribute=gParseToken;
        } else {
            // keep track of where this tag started...
            // just in case it is the start of a container
            gTagOffset=gDataFileOffset;
        }

        // get ready for the next token... a tag of some sort
        gParseToken="";
    }

    //----------
    // process a '>' character
    //----------
    private int parse2(char inchr) {
        // we're processing a '>' character...
        // XML tags end with '>', but at the same time,
        // the '>' character also starts embedded text
        gParseToken=gParseToken.Trim();

        if(gParseToken[0]!='/') {
            // we are processing an opening tag...
            if(!gInsideContainer) {
                // outside container: ignore all opening tags
                // except the one that starts a container...
                if(String.Compare(gParseToken,gXMLContainer)==0) {
                    gInsideContainer=true;

                    // save the offset of the beginning of the
                    // container into the container object
                    setAttribute("BEGPTR",gTagOffset.ToString());
                }
            } else {
                // inside container: we care about all
                // opening XML tags
                gOpeningTag=gParseToken;
            }
        } else {
            if(!gInsideContainer) {
                // outside container: ignore all closing tags

                // We need to keep track of where the last closing
                // container tag is located in the data file.
                // That is where we will add new containers to the file.
                // at this point in the code, we learn that
                // when we drop out of the XMLObject.parseFile() routine,
                // gTagOffset happens to point to last closing tag.
                // it's important to note.

            } else {
                // inside container: it was a closing XML tag...
                if(String.Compare(gParseToken,"/"+gXMLContainer)==0) {
                    gInsideContainer=false;

                    // save the offset of the ending of the
                    // container into the container object
                    setAttribute("ENDPTR",
                        gDataFileOffset.ToString());
                    return SUCCESS;
                } else if(String.Compare(gParseToken,
                    "/"+gOpeningTag)==0) {

                    // if we can, save away any embedded data attributes
                    setAttribute(gOpeningTag,gAttribute.Trim());
                } else if(gOpeningTag!="") {
                    // but it did not match the previous (opening) tag,
                    // and it was not our container tag
                    Console.Error.WriteLine("\nXML nesting error");
                    Console.Error.WriteLine("gOpeningTag=\""+
                        gOpeningTag+"\"");
                    Console.Error.WriteLine("gParseToken="+
                        gParseToken);
                    return ERROR;
                }
                gOpeningTag="";
            }
        }

        // get ready for the next token... whatever that may be
        gParseToken="";
        return KEEPLOOPING;
    }

    //----------
    // process characters other than '<' and '>'
    //----------
    private void parse3(char inchr) {
        // we're processing something other than a '<' or a '>'...
        if((inchr=='\n')||(inchr=='\r')||(inchr=='\t')||(inchr=='\f')) {
            inchr=' ';
        }
        if((gParseToken.Length==0)&&(inchr==' ')) {
            // automagically eliminate leading spaces
        } else {
            gParseToken+=(char)inchr;
        }
    }

    //----------
    public int parseFile(FileStream infile) {
        clear();

        gInsideContainer=false;
        gParseToken="";
        gOpeningTag="";
        // used to accumulate XML tokens
        gAttribute="";

        //----------
        int result=SUCCESS;
        int inchr;
        while(true) {
            // ReadByte() returns a character cast as an int
            inchr=infile.ReadByte();
            if(inchr==(-1)) {
                // eof means we are all done
                // Console.WriteLine("end of file detected\n");
                result=FAIL;
                break;
            }

            //----------

            if(inchr=='<') {
                // XML tags start with '<', but at the same time,
                // the '<' character also terminates embedded text
                parse1((char)inchr);
            } else if(inchr=='>') {
                // XML tags end with '>', but at the same time,
                // the '>' character also starts embedded text
                result=parse2((char)inchr);
                if((result==SUCCESS)||(result==ERROR)) {
                    gDataFileOffset++;
                    break;
                }
            } else {
                // we're processing something other than '<' or a '>'...
                parse3((char)inchr);
            }
    
            //----------


            // update our counters
            gDataFileOffset++;

            if(inchr=='\n') {
                gDataFileLine++;
            }

            // loop back for more characters
        }

        // drop out of the loop when...
        // 1) we reach end-of-file: result=FAIL
        // 2) we find the end of a container: result=SUCCESS
        // 3) we detect an XML nesting error: result=ERROR

        return result;
    }

} // end XMLObject class

} // end namespace mypgms

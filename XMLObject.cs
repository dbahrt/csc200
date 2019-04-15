// Dan Bahrt
// xmlobject.cs

using System;
using System.IO;
// using System.Text;

namespace mypgms {


//==========
// none of this class is static.
// in order to have an XMLObject, we must
// instantiate an XMLObject object.
// we can have multiple XMLObject objects at a time.
// this is considered object-oriented.
//==========
class XMLObject {

    //----------
    // global constant data
    //----------

    public const int SUCCESS=0;
    public const int KEEPLOOPING=2;
    public const int FAIL=1;
    public const int ERROR=-1;

    //----------
    // instance variables
    //----------

    private string gXMLContainer;

    private string [] gAttributes;
    private string [] gValues;

    private int gAttributeCount;
    private int gAllocatedCount;

    private AppDefinition appdef;

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
    // this is where we initialize the XMLObject...
    // a constructor is a special kind of function
    // (its name is the same as the class name,
    // and it can does not have a return type --
    // not even void) whose only responsibility is
    // to initialize the instance variables that are
    // put in the object.
    // when we are done with the constructor,
    // the object should be viable and consistant.
    //----------
    public XMLObject(int allocate,string container,AppDefinition ad) {
        if(allocate<1) {
            Console.Error.WriteLine("XMLObject.init(): allocate = {0}",
                allocate);
            throw new Exception("XMLObject instantiation failed.");
        }

        gXMLContainer=container;
        appdef=ad;

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
    // display FLDNAM label and content of the specified field
    //----------
    public bool displayAttribute(int fldnum) {
        string attrib=appdef.flds[fldnum,AppDefinition.FLDNAM];

        int index=findAttribute(attrib);
        if(index<0) {
            setAttribute(attrib,"");
            index=gAttributeCount-1;
        }

        // all other data types...  integer, real, and boolean
        // are simple strings that can be displayed
        // just by dumping out their content
        Console.WriteLine(Useful.padString(attrib+": ",
            findMaxLabelLength()+2)+gValues[index]);
        return true;
    }

    //----------
    private void displayString(string attrib,int index,int fldlen,
        string fldnul) {

        int labelLength=findMaxLabelLength();
        if((gValues[index].Length!=0)||
           (fldnul=="true")) {
            Console.WriteLine(Useful.padString(attrib+": ",labelLength+2)+
                gValues[index]);
        } else {
            // invalid string
            string mask="";
            for(int ii=0;ii<fldlen;ii++) {
                mask+="_";
            }
            Console.Write(Useful.padString(attrib+": ",labelLength+2)+mask);
            Console.WriteLine();
        }
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
    // step through appdef to find length of longest FLDNAM.
    // we'll use it to "pretty print" the file maintenance screen.
    //----------
    private int findMaxLabelLength() {
        int labelLength=0;
        for(int ii=0;ii<gAttributeCount;ii++) {
            if(gAttributes[ii].Length>labelLength) {
                labelLength=gAttributes[ii].Length;
            }
        }
        return labelLength;
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
    // accept data for the specified field
    //----------
    public bool inputAttribute(int fldnum) {
        string attrib=gAttributes[fldnum];

        int labelLength=findMaxLabelLength();
        int index=findAttribute(attrib);

        if(index<0) {
            setAttribute(attrib,"");
            index=gAttributeCount-1;
        }


        // construct the input mask
        int fldlen=Int32.Parse(gValues[fldnum]);
        string mask="";
        for(int ii=0;ii<fldlen;ii++) {
            mask+="-";
        }

        // make sure that the user provides a valid entry...
        string saved=gValues[index];
        for(;;) {
            Console.Write(Useful.padString(attrib+": ",labelLength+2)+
                mask+"\r"+
                Useful.padString(attrib+": ",labelLength+2));
            gValues[index]=Console.ReadLine().Trim();
            // assume that the value entered is valid
            break;
            // Console.WriteLine("Invalid input.  Try again...\n\n\n");
        }

        return true;
    }

    //----------
    // scan the parallel arrays of our XML object to find
    // a specified data attribute.  if we are successful,
    // set that data attribute with the specified value;
    // if not, add the data attribute to the arrays, using
    // the specified value.
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
    private bool validateBoolean(string type,string length,
        string nullok,ref string input,string saved) {

        if(input.Length==0) {
            if(nullok=="true") {
                return true;
            }
            return false;
        }
        input=input.ToLower();
        if((input!="true")&&
           (input!="false")) {
            return false;
        }
        return true;
    }

    //----------
    private bool validateInput(string type,string length,
        string nullok,ref string input,string saved) {

        // when inputting an attribute, we need to
        // make sure that any user entry is valid...
        // inputAttribute(), validateInput(), and displayAttribute()
        // is mainly where we are implementing data types.

        if((type=="integer")||(type=="real")) {
            return validateNumber(type,length,nullok,ref input,saved);
        }
        if(type=="boolean") {
            return validateBoolean(type,length,nullok,ref input,saved);
        }
        if(type=="string") {
            return validateString(type,length,nullok,ref input,saved);
        }

        Console.Error.WriteLine("FLDTYP ("+type+") is not valid!");
        return true;
    }

    //----------
    private bool validateNumber(string type,string length,
        string nullok,ref string input,string saved) {

        if(input.Length==0) {
            if(nullok=="true") {
                return true;
            }
            return false;
        }
        int ii=0;
        if(input[0]=='-') {
            ii=1;
        }
        for( ;ii<input.Length;ii++) {
            if((type=="real")&&(input[ii]=='.')) {
                ii++;
                break;
            }
            if((input[ii]<'0')||(input[ii]>'9')) {
                return false;
            }
        }
        if(type=="real") {
            for( ;ii<input.Length;ii++) {
                if((input[ii]<'0')||(input[ii]>'9')) {
                    return false;
                }
            }
        }
        return true;
    }

    //----------
    private bool validateString(string type,string length,
        string nullok,ref string input,string saved) {

        if(input.Length==0) {
            if(nullok=="true") {
                return true;
            }
            return false;
        }
        int len=Useful.toInt(length);
        if(input.Length>len) {
            input=input.Substring(0,len);
            Console.Write("\nField length exceeded. ");
            Console.WriteLine("Input truncated.");
            Console.Write("\nPress ENTER to continue...");
            Console.ReadLine();
            return true;
        }
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
            // treat all whitespace characters as spaces
            inchr=' ';
        }
        if((gParseToken.Length==0)&&(inchr==' ')) {
            // gParseToken contains no characters AND
            // input character is a space...
            // just ignore it to automagically eliminate leading spaces
        } else {
            // append the input character to gParseToken IF...
            // gParseToken already has characters in it, OR
            // input character is not a space.
            gParseToken+=(char)inchr;
        }
    }

    //----------
    // XML parser scans for an opening gXMLContainer tag
    // and then it collects attributes until it runs into
    // a closing gXMLContainer TAG
    //----------
    public int parseFile(FileStream infile) {
        clear();

        gInsideContainer=false;
        gParseToken="";
        gOpeningTag="";
        // used to accumulate XML tokens
        gAttribute="";

        //----------
        // read input file to separate XML tags from attributes.
        // we can read and process file content character-by-character
        // using ReadByte(), which reads one char at a time.
        // other than that, all we are doing in this loop is
        // collecting parseTokens and determining where XML tags
        // start and end.  oh, and by the way, we are still counting
        // characters (in gDataFileOffset) and lines (in gDataFileLine).
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
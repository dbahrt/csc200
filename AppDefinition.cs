// Dan Bahrt
// AppDefinition.cs

using System;
using System.IO;

namespace mypgms {


//==========
class AppDefinition {

    //----------
    // global constant class data (only 1 per class)
    //----------
    private static string [,] hardCodedFields={
// NAME      USE       TYPE      LEN   DEC  DEFAULT   NULLOK   EMPTYOK
{ "BEGPTR", "system", "int",    "11", "0", "0",      "false", "false", },
{ "ENDPTR", "system", "int",    "11", "0", "0",      "false", "false", },

{ "fldnam", "key",    "string", "30", "0", "",       "false", "false", },

{ "flduse", "data",   "string", "6",  "0", "data",   "false", "false", },
{ "fldtyp", "data",   "string", "7",  "0", "string", "false", "false", },
{ "fldlen", "data",   "int",    "2",  "0", "30",     "false", "false", },
{ "flddec", "data",   "int",    "2",  "0", "0",      "false", "false", },
{ "flddfl", "data",   "string", "30", "0", "",       "false", "false", },
{ "fldnul", "data",   "boolean","5",  "0", "false",  "false", "false", },
{ "fldmpt", "data",   "boolean","5",  "0", "false",  "false", "false", },
    };

    public const int FLDNAM=0;  // NAME
    public const int FLDUSE=1;  // USE
    public const int FLDTYP=2;  // TYPE
    public const int FLDLEN=3;  // LENGTH
    public const int FLDDEC=4;  // DECIMALPLACES
    public const int FLDDFL=5;  // DEFAULT
    public const int FLDNUL=6;  // NULLOK
    public const int FLDMPT=7;  // EMPTYOK

    //----------
    // global instance variables data (a different copy for each object)
    //----------
    public string [,] flds;

    //----------
    public AppDefinition() {
        int allocated=0;
        int row=0;
        for(int ii=0;ii<hardCodedFields.GetLength(0);ii++) {
            //----------
            // copy hardCodedFields[,] table into the flds[,] table
            //----------
            expand(allocated++);

            int col=0;
            for(int jj=0;jj<hardCodedFields.GetLength(0);jj++) {
                if(hardCodedFields[jj,FLDUSE]=="system") {
                    break;
                }
                flds[row,col]=hardCodedFields[row,col];
Console.WriteLine("["+row+","+col+"]="+flds[row,col]);
                col++;
            }
            row++;
        }
    } // end constructor AppDefinition()

    //----------
    public AppDefinition(string filename) {
        
        // let calling function handle exceptions...
        // all it will say is that it
        // cannot make sense of specified <definition_file>.

        //----------
        // open file for input
        //----------
        FileStream infile;
        infile=new FileStream(filename,FileMode.Open,
            FileAccess.Read);

        //----------
        // read and parse definition file
        //----------
        AppDefinition ad=new AppDefinition();
        ad.dump("hardCodedFields:");
        XMLObject xmlobj=new XMLObject(50,"fld",ad);
        int allocated=0;
        int row=0;
        for(;;) {
            int result=xmlobj.parseFile(infile);
            if(result==XMLObject.ERROR) {
                throw new Exception("cannot parse definition file");
            }
            if(result==XMLObject.FAIL) {
                break;
            }

            //----------
            // into the flds[,] table
            //----------
            expand(allocated++);

            int col=0;
            for(int jj=0;jj<hardCodedFields.GetLength(0);jj++) {
                if(hardCodedFields[jj,FLDUSE]=="system") {
                    continue;
                }
                flds[row,col]=
                    xmlobj.getAttribute(hardCodedFields[jj,FLDNAM]);
                col++;
            }
            row++;
        }

        //----------
        // close the input file
        //----------
        infile.Close();
        this.dump("knowts.xdf:");
    } // end constructor AppDefinition()

    //----------
    // flds[,] is a 2 dimensional array
    // 
    // normally, once allocated, the size and shape of
    // an array cannot be changed. however, there are a
    // couple of tricks that enable us to implement
    // dynamically sizable arrays:
    // 1) probably the best way is to store the data in
    // a linked list (which is really efficient for
    // growing and shrinking). then when the data is all
    // present and placed, convert the list to an array...
    // allocate a fixed-sized array (knowing how big
    // it has to be), and copy the list elements into
    // that array.
    // 2) the ArrayList data structure (behind the scenes)
    // allocates an initial array that has some extra unused
    // elements. we have keep track how many elements are
    // used, and how many are available but unused.
    // then as we place data, if we run out of available
    // space, we allocate a new array with twice as much
    // available space. finally. when all the data has been
    // placed, we convert that (probably oversized) array
    // into a properly-sized array...  allocate a fixed-sized
    // array (knowing how big it has to be), and copy
    // the elements into that array.
    // 3) in this program, I am doing the same thing as #2,
    // but when we run out of array elements (for
    // illustration purposes), I only allocate 1 additional
    // element at a time. thus we don't have to fix
    // the array size at the end of the process.
    // this is fairly inefficient, but the arrays we are
    // dealing with are very small. so it should not be
    // a big deal. 
    //----------
    private void expand(int allocated) {
        if(allocated==0) {
            flds=new String[1,hardCodedFields.GetLength(1)];
            return;
        }
        string [,] newflds=new string[flds.GetLength(0)+1,flds.GetLength(1)];
        for(int ii=0;ii<flds.GetLength(0);ii++) {
            for(int jj=0;jj<flds.GetLength(1);jj++) {
                newflds[ii,jj]=flds[ii,jj];
            }
        }
        flds=newflds;
    }

    //----------
    public void dump(string label) {
Console.WriteLine(label);
Console.WriteLine("flds.GetLength(0)="+flds.GetLength(0));
        for(int ii=0;ii<flds.GetLength(0);ii++) {
Console.WriteLine("flds.GetLength(1)="+flds.GetLength(1));
            for(int jj=0;jj<flds.GetLength(1);jj++) {
Console.WriteLine("["+ii+","+jj+"]="+flds[ii,jj]);
            }
        }
    } // end method dump()

} // end AppDefinition class

} // end namespace mypgms

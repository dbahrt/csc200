// source: https://www.tutorialspoint.com/csharp/csharp_classes.htm
// filename: tp_class3.cs
// capture date: 27 Mar 2019
// student: Dan Bahrt
// summary: ...
// modifications: ...


using System;

namespace LineApplication {
   class Line {
      private double length;   // Length of a line
      
      public Line() {
         Console.WriteLine("Object is being created");
      }
      public void setLength( double len ) {
         length = len;
      }
      public double getLength() {
         return length;
      }

      static void Main(string[] args) {
         Line line = new Line();    
         
         // set line length
         line.setLength(6.0);
         Console.WriteLine("Length of line : {0}", line.getLength());
         Console.ReadKey();
      }
   }
}

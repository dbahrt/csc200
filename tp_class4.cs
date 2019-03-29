// source: https://www.tutorialspoint.com/csharp/csharp_classes.htm
// filename: tp_class4.cs
// capture date: 27 Mar 2019
// student: Dan Bahrt
// summary: ...
// modifications: ...


using System;

namespace LineApplication {
   class Line {
      private double length;   // Length of a line
      
      public Line(double len) {  //Parameterized constructor
         Console.WriteLine("Object is being created, length = {0}", len);
         length = len;
      }
      public void setLength( double len ) {
         length = len;
      }
      public double getLength() {
         return length;
      }
      static void Main(string[] args) {
         Line line = new Line(10.0);
         Console.WriteLine("Length of line : {0}", line.getLength()); 
         
         // set line length
         line.setLength(6.0);
         Console.WriteLine("Length of line : {0}", line.getLength()); 
         Console.ReadKey();
      }
   }
}

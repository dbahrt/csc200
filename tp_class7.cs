// source: https://www.tutorialspoint.com/csharp/csharp_classes.htm
// filename: tp_class7.cs
// capture date: 27 Mar 2019
// student: Dan Bahrt
// summary: ...
// modifications: ...


using System;

namespace StaticVarApplication {
   class StaticVar {
      public static int num;
      
      public void count() {
         num++;
      }
      public static int getNum() {
         return num;
      }
   }
   class StaticTester {
      static void Main(string[] args) {
         StaticVar s = new StaticVar();
         
         s.count();
         s.count();
         s.count();
         
         Console.WriteLine("Variable num: {0}", StaticVar.getNum());
         Console.ReadKey();
      }
   }
}

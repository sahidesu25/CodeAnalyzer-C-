using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestForSma

{

    class Func
    {

        List<String> myList = new List<string>();
        int larger(int a, int b, int c)
        {
            if (a > b && a > c)
            {
                Console.WriteLine("{0}", a);
            }
            else if (b > c)
            {
                Console.WriteLine("{0}", b);
            }
            else
            {
                Console.WriteLine("{0}", c);
            }
        }
    }
 class Test2
    {

        List<String> myList = new List<string>();
        int larger(int a, int b, int c)
        {
            if (a > b && a > c)
            {
                Console.WriteLine("{0}", a);
            }
            else if (b > c)
            {
                Console.WriteLine("{0}", b);
            }
            else
            {
                Console.WriteLine("{0}", c);
            }
        }
    }

    public interface test
    {
        int a();
        int b();
    }
    class Program:test
    {
        enum Importance
        {
            None,
            Trivial,
            Regular,
            Important,
            Critical
        };

        int a(Test2 a, Func f)
        {

        }
    


        static void Main(string[] args)
        {
            int a = 15;
            int b = 24;
           
            Func f = new Func();
            Console.WriteLine("{0}", value);

            Importance value = Importance.Critical;
            if(a>b)
            {
                Console.WriteLine("b smaller");
	     }

            Test2 f1 = new Test2();

if(a==0)
a=5;
else
b=5;

        }
    }
}

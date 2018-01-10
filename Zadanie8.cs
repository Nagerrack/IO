using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IO_final
{
    class Zadanie8
    {
        delegate ulong Delegacik(ulong args);
        static Delegacik del;
        static Delegacik del2;
        public static void Start()
        {

            del = factorial_Recursion;
            del2 = factorial_Iteration;
            IAsyncResult iar = del.BeginInvoke(10000, null, null);
            IAsyncResult iar2 = del2.BeginInvoke(10500, null, null);

            int whichOne = WaitHandle.WaitAny(new WaitHandle[] { iar.AsyncWaitHandle, iar2.AsyncWaitHandle });

            Console.WriteLine(whichOne);
            Console.ReadKey();

        }


        public static ulong factorial_Recursion(ulong number)
        {
            //Thread.Sleep(200);
            if (number == 1)
                return 1;
            else
                return number + factorial_Recursion(number - 1);
        }

        public static ulong factorial_Iteration(ulong number)
        {
            
            ulong result = 1;
            while (number != 1)
            {
                result = result + number;
                number = number - 1;
            }
            return result;
        }

        public static ulong fibonacci_Recursion(ulong number)
        {
            if (number < 3) return 1;
            else return fibonacci_Recursion(number - 2) + fibonacci_Recursion(number - 1);
        }

        public static ulong fibonacci_Iteration(ulong number)
        {
            ulong a0 = 0;
            ulong a1 = 1;
            for (ulong i = 0; i < number; i++)
            {
                ulong temp = a0;
                a0 = a1;
                a1 = temp + a1;
            }
            return a0;
        }
    }
}

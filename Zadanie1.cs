using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IO_final
{
    class Zadanie1
    {
        void ThreadProc(object stateInfo)
        {
            var number = (int) ((object[]) stateInfo)[0];
            var sleepTime = (int) ((object[]) stateInfo)[1];
            Console.WriteLine("Thread start");
            Thread.Sleep(sleepTime);
            Console.WriteLine("Thread " + number + "slept " + sleepTime + " ms");
            Console.WriteLine("Thread end");
        }

        public void Start()
        {
            Console.WriteLine("Main start");
            ThreadPool.QueueUserWorkItem(ThreadProc, new object[] {1, 800});
            ThreadPool.QueueUserWorkItem(ThreadProc, new object[] {2, 500});
            Thread.Sleep(1000);
            Console.WriteLine("Main end");
        }
    }
}
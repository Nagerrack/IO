using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace IO_final
{
    class State
    {
        public int current;
        public AutoResetEvent are;

        public State(int current, AutoResetEvent are)
        {
            this.current = current;
            this.are = are;
        }
    }

    class Zadanie5
    {
        private static int[] tab;
        private static AutoResetEvent[] are;
        private static int batchsize;
        private static int fragments;
        private static int sum;
        private static object locke = new object();


        public static void Init(int size, int b)
        {  
            sum = 0;
            tab = new int[size];

            batchsize = b;
            Random r = new Random();
            for (int i = 0; i < tab.Length; i++)
            {
                tab[i] = r.Next(1, 100);
            }

            if (tab.Length % batchsize == 0)
            {
                fragments = tab.Length / batchsize;
            }
            else
            {
                fragments = tab.Length / batchsize + 1;
            }

            are = new AutoResetEvent[fragments];
            for (int i = 0; i < are.Length; i++)
            {
                are[i] = new AutoResetEvent(false);
            }
        }

        public static void sumChunk(object StateInfo)
        {
            lock (locke)
            {
                State info = (State) StateInfo;
                int current = info.current;

                int temp = 0;

                for (int i = current; i < tab.Length && i < current + batchsize; i++)
                {
                    temp += tab[i];
                    sum += tab[i];
                }

                Console.WriteLine("#" + current / batchsize + " " + temp);
                info.are.Set();
            }
        }


        public static void Start(int size, int batchSize)
        {
            Init(size,batchSize);
            for (int i = 0; i < fragments; i++)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(sumChunk), new State(i * batchsize, are[i]));
            }

            WaitHandle.WaitAll(are);
            Console.WriteLine("Suma: " + sum);
        }
    }
}

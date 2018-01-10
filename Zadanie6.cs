using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IO_final
{
    class Zadanie6
    {
        static AutoResetEvent are = new AutoResetEvent(false);

        static public void Start()
        {
            FileStream f = new FileStream("text.txt", FileMode.Open);
            byte[] buffer = new byte[1024];
            f.BeginRead(buffer, 0, buffer.Length, Call, new object[] {f, buffer});

            are.WaitOne();
            Console.ReadKey();
        }

        static void Call(IAsyncResult iar)
        {
            FileStream f = (FileStream) ((object[]) iar.AsyncState)[0];
            var buffer = (byte[]) ((object[]) iar.AsyncState)[1];
            //int read = f.Read(buffer, 0, buffer.Length);


            Console.WriteLine(Encoding.ASCII.GetString(buffer).Replace("\0", String.Empty));
            f.Close();
            are.Set();
        }
    }
}
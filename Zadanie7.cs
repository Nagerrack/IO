using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IO_final
{
    class Zadanie7
    {
        public static void Start()
        {
            FileStream f = new FileStream("text.txt", FileMode.Open);
            byte[] buffer = new byte[1024];

            IAsyncResult iar = f.BeginRead(buffer, 0, buffer.Length, null, null);

            int read = f.EndRead(iar);
            Console.WriteLine(Encoding.ASCII.GetString(buffer, 0, read));

            f.Close();
            Console.ReadKey();

        }

    }
}

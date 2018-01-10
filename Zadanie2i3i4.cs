using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IO_final;

namespace IO_final
{
    class Client
    {
        public static object lockObject = new object();

        public static void writeConsoleMessage(string message, ConsoleColor color)
        {
            lock (lockObject)
            {
                Console.ForegroundColor = color;
                Console.WriteLine(message);
                Console.ResetColor();
            }
        }

        public void Start()
        {
            TcpClient client = new TcpClient();
            client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2048));
            byte[] message = new ASCIIEncoding().GetBytes("wiadomosc");
            client.GetStream().Write(message, 0, message.Length);

            int len = client.GetStream().Read(message, 0, message.Length);
            string s = $"[C] Server message : {Encoding.ASCII.GetString(message, 0, len)}";
            writeConsoleMessage(s, ConsoleColor.Red);
        }
    }


    class Server
    {
        public void Start()
        {
            TcpListener server = new TcpListener(IPAddress.Any, 2048);
            server.Start();
            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                byte[] buffer = new byte[1024];
                int len = client.GetStream().Read(buffer, 0, 1024);
                string s = $"[S] Client message : {Encoding.ASCII.GetString(buffer, 0, len)}";
                Client.writeConsoleMessage(s, ConsoleColor.Green);
                client.GetStream().Write(buffer, 0, buffer.Length);
            }
        }
    }

}

class Zadanie2i3i4
{
    public void Start()
    {
        Server s = new Server();
        Client c1 = new Client();
        Client c2 = new Client();

        ThreadPool.QueueUserWorkItem(o => s.Start());
        ThreadPool.QueueUserWorkItem(o => c1.Start());
        ThreadPool.QueueUserWorkItem(o => c2.Start());
        Thread.Sleep(2000);
    }
}
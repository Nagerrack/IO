using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IO_final
{
    class Zadanie11
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

        static void DoWork(object sender, DoWorkEventArgs e)
        {
            TcpListener server = new TcpListener(IPAddress.Any, 2048);
            server.Start();
            int progress = 0;
            var Worker = sender as BackgroundWorker;
            while (true)
            {
                if (Worker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
                TcpClient client = server.AcceptTcpClient();
                byte[] buffer = new byte[1024];
                int len = client.GetStream().Read(buffer, 0, 1024);
                string s = $"[S] Client message : {Encoding.ASCII.GetString(buffer, 0, len)}";
                Client.writeConsoleMessage(s, ConsoleColor.Green);
                client.GetStream().Write(buffer, 0, buffer.Length);

                progress += 20;
                Worker.ReportProgress(progress);
                

            }
        }

        static void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Client.writeConsoleMessage("Progress: " + e.ProgressPercentage + "%", ConsoleColor.White);
        }

        static void Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                Console.WriteLine("Cancelled");
            }
            else if (e.Error != null)
            {
                Console.WriteLine("Exception: " + e.Error);
            }
            else
            {
                Console.WriteLine("Completed: " + e.Result);
            }
        }

        public static void Start()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += DoWork;
            bw.ProgressChanged += ProgressChanged;
            bw.RunWorkerCompleted += Completed;
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;

            Console.WriteLine("5 Clients");

            bw.RunWorkerAsync();

            Client c1 = new Client();
            c1.Start();
            Client c2 = new Client();
            c2.Start();
            Client c3 = new Client();
            c3.Start();
            Client c4 = new Client();
            c4.Start();
            Client c5 = new Client();
            c5.Start();
        }
    }
}
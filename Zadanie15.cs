using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;


namespace IO_final
{
    class Zadanie15
    {
        private static object lockObject = new object();

        class Log
        {
            public static void CreateLog(string text)
            {
                lock (lockObject)
                {
                    string log = "[LOG] " + System.DateTime.Now + " " + text + Environment.NewLine;
                    File.AppendAllText("logs.txt", log);
                }
            }
        }

        class Server
        {
            #region Variables

            TcpListener server;
            int port;
            IPAddress address;
            bool running = false;
            CancellationTokenSource cts = new CancellationTokenSource();
            Task serverTask;

            public Task ServerTask
            {
                get { return serverTask; }
            }

            #endregion

            #region Properties

            public IPAddress Address
            {
                get { return address; }
                set
                {
                    if (!running) address = value;
                    else ;
                }
            }

            public int Port
            {
                get { return port; }
                set
                {
                    if (!running)
                        port = value;
                    else ;
                }
            }

            #endregion

            #region Constructors

            public Server()
            {
                Address = IPAddress.Any;
                port = 2048;
            }

            public Server(int port)
            {
                this.port = port;
            }

            public Server(IPAddress address)
            {
                this.address = address;
            }

            #endregion

            #region Methods

            public async Task RunAsync(CancellationToken ct)
            {
                server = new TcpListener(address, port);

                try
                {
                    server.Start();
                    running = true;
                }
                catch (SocketException ex)
                {
                    throw (ex);
                }
                while (!ct.IsCancellationRequested)
                {
                    TcpClient client = await server.AcceptTcpClientAsync();
                    byte[] buffer = new byte[1024];
                    using (ct.Register(() => client.GetStream().Close()))
                    {
                        await client.GetStream().ReadAsync(buffer, 0, buffer.Length, ct).ContinueWith(
                            async (t) =>
                            {
                                int i = t.Result;
                                
                                while (!ct.IsCancellationRequested)
                                {
                                    await client.GetStream().WriteAsync(buffer, 0, i, ct);
                                    try
                                    {
                                        i = await client.GetStream().ReadAsync(buffer, 0, buffer.Length, ct);
                                        string text = Encoding.ASCII.GetString(buffer, 0, i);
                                        Console.WriteLine("[S] Message: " + text);
                                        Log.CreateLog(text);
                                        Thread.Sleep(200);
                                    }
                                    catch
                                    {
                                        break;
                                    }
                                }
                            }, ct);
                    }
                }
            }


            public void RequestCancellation()
            {
                cts.Cancel();
                server.Stop();
            }

            public void Run()
            {
                serverTask = RunAsync(cts.Token);
            }

            public void StopRunning()
            {
                RequestCancellation();
            }

            #endregion
        }

        class Client
        {
            #region variables

            TcpClient client;

            #endregion

            #region properties

            #endregion

            #region Constructors

            #endregion

            #region Methods

            public void Connect()
            {
                client = new TcpClient();
                client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2048));
            }

            public async Task<string> Ping(string message)
            {
                byte[] buffer = new ASCIIEncoding().GetBytes(message);
                await client.GetStream().WriteAsync(buffer, 0, buffer.Length);
                buffer = new byte[1024];
                var t = await client.GetStream().ReadAsync(buffer, 0, buffer.Length);
                return Encoding.UTF8.GetString(buffer, 0, t);
            }

            public async Task<IEnumerable<string>> keepPinging(string message, CancellationToken token)
            {
                List<string> messages = new List<string>();
                bool done = false;
                while (!done)
                {
                    messages.Add(await Ping(message));
                    if (token.IsCancellationRequested)
                    {
                        done = true;
                        Console.WriteLine("Cancelled");
                    }

                }
                return messages;
            }

            #endregion
        }

        public static async Task serverTask()
        {
            TcpListener server = new TcpListener(IPAddress.Any, 2048);
            server.Start();
            while (true)
            {
                TcpClient client = await server.AcceptTcpClientAsync();
                byte[] buffer = new byte[1024];
               
                await client.GetStream().ReadAsync(buffer, 0, buffer.Length).ContinueWith(
                    async (t) =>
                    {
                        int i = t.Result;

                        while (true)
                        {
                            
                            await client.GetStream().WriteAsync(buffer, 0, i);
                            i = await client.GetStream().ReadAsync(buffer, 0, buffer.Length);
                            Console.WriteLine("[S] Message: " + Encoding.ASCII.GetString(buffer, 0, i));
                        }
                    }
                );
            }
        }

        public static async Task clientTask(int clientID)
        {
            string message = "[Client " + clientID + "] message";

            TcpClient client = new TcpClient();
            client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2048));

           
            byte[] buffer = new ASCIIEncoding().GetBytes(message);

            await client.GetStream().WriteAsync(buffer, 0, buffer.Length);
            buffer = new byte[1024];

            int i = await client.GetStream().ReadAsync(buffer, 0, buffer.Length);
            Console.WriteLine("[C] " + Encoding.ASCII.GetString(buffer,0,i));
            
        }


        public static void Start()
        {
            File.Delete("logs.txt");
            Server s = new Server();
            s.Run();
            Client c1 = new Client();
            Client c2 = new Client();
            Client c3 = new Client();
            c1.Connect();
            c2.Connect();
            c3.Connect();
            CancellationTokenSource ct1 = new CancellationTokenSource();
            CancellationTokenSource ct2 = new CancellationTokenSource();
            CancellationTokenSource ct3 = new CancellationTokenSource();
            var client1T = c1.keepPinging("message1", ct1.Token);
            var client2T = c2.keepPinging("message2", ct2.Token);
            var client3T = c3.keepPinging("message3", ct3.Token);

            ct1.CancelAfter(500);
            ct2.CancelAfter(1000);
            ct3.CancelAfter(1500);

            Task.WaitAll(client1T, client2T, client3T);
            Console.WriteLine("WaitAll");
            s.StopRunning();
        }
    }
}
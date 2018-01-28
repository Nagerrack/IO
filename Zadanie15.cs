﻿using System;
using System.Collections.Generic;
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
                while (true && !ct.IsCancellationRequested)
                {
                    TcpClient client = await server.AcceptTcpClientAsync();
                    byte[] buffer = new byte[1024];
                    using (ct.Register(() => client.GetStream().Close()))
                    {
                        client.GetStream().ReadAsync(buffer, 0, buffer.Length, ct).ContinueWith(
                            async (t) =>
                            {
                                int i = t.Result;
                                while (true)
                                {
                                    client.GetStream().WriteAsync(buffer, 0, i, ct);
                                    try
                                    {
                                        i = await client.GetStream().ReadAsync(buffer, 0, buffer.Length, ct);
                                    }
                                    catch
                                    {
                                        break;
                                    }
                                }
                            });
                    }
                }
            }

            //public static async Task serverTask()
            //{
            //    TcpListener server = new TcpListener(IPAddress.Any, 2048);
            //    server.Start();
            //    while (true)
            //    {
            //        TcpClient client = await server.AcceptTcpClientAsync();
            //        byte[]buffer =new byte[1024];
            //        client.GetStream().ReadAsync(buffer, 0, buffer.Length).ContinueWith(
            //            async (t) =>
            //            {
            //                int i = t.Result;
            //                while (true)
            //                {
            //                    client.GetStream().WriteAsync(buffer, 0, i);
            //                    i = await client.GetStream().ReadAsync(buffer, 0, buffer.Length);
            //                }
            //            }
            //        );
            //    }

            //}
            public void RequestCancellation()
            {
                cts.Cancel();
                //serverTask.Wait();
                //serverTask.Dispose();
                server.Stop();
            }

            public void Run()
            {
                serverTask = RunAsync(cts.Token);
            }

            public void StopRunning()
            {
                RequestCancellation();
                //serverTask.Dispose();
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
                client.GetStream().WriteAsync(buffer, 0, buffer.Length);
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
                    if (token.IsCancellationRequested)
                        done = true;
                    messages.Add(await Ping(message));
                }
                return messages;
            }

            #endregion
        }

        public void Zadanie48()
        {
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
            var client1T = c1.keepPinging("message", ct1.Token);
            var client2T = c2.keepPinging("message", ct2.Token);
            var client3T = c3.keepPinging("message", ct3.Token);
            ct1.CancelAfter(2000);
            ct2.CancelAfter(3000);
            ct3.CancelAfter(4000);
            Task.WaitAll(new Task[] {client1T, client2T, client3T});
            s.StopRunning();
        }
    }
}
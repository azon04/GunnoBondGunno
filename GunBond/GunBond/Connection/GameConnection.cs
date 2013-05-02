using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using UNOS_Sister;

namespace GunBond.Connection
{
    class GameConnection
    {
        public List<string> IPTable;
        public string IP;
        public Socket Socket;
        List<ClientHandler> ClientHandlers;
        List<String> peerID; //peer yang ada di room itu
        Room room;
        Configurator configurator;
        Thread ListenThread;

        bool shutdown = false;

        delegate void del(); // declare a delegate

        public GameConnection()
        {
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            Console.WriteLine((IP = ipAddress.ToString()));
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            // Create a TCP/IP socket
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket
            Socket.Bind(localEndPoint);
            Socket.Listen(10);

            ClientHandlers = new List<ClientHandler>();
            ListenThread = new Thread(Listening);
            IPTable = new List<string>();

            ListenThread.Start();

        }

        public void StartConfig(List<string> IPTable)
        {
            configurator = new Configurator(IP,IPTable);
            configurator.Status = Configurator.State.starting;
            foreach (string ip in configurator.IPToConnect())
            {
                ClientHandler ch = Connect(ip);
                ch.SendMsg(configurator.ConstructMessageConfig());
            }
        }

        public void StartConfig()
        {
            configurator.Status = Configurator.State.starting;
            foreach (string ip in configurator.IPToConnect())
            {
                ClientHandler ch = Connect(ip);
                ch.SendMsg(configurator.ConstructMessageConfig());
            }
        }

        ~GameConnection()
        {
            Console.WriteLine("Deconstructor");
        }

        public void Listening()
        {
            while (!shutdown)
            {
                lock (ClientHandlers)
                {
                    if (ClientHandlers.Count >= room.getMaxPlayer()-1) continue;
                }
                try
                {
                    Console.WriteLine("Waiting for game client..");
                    Socket handler = Socket.Accept();

                    // Create Client Handler
                    lock (ClientHandlers)
                    {
                        ClientHandlers.Add(new ClientHandler(this, handler));
                    }

                    byte[] bytes = new byte[1024];
                    int bytesRec = handler.Receive(bytes);

                    if (configurator != null)
                    {
                        if (configurator.Status == Configurator.State.start)
                        {
                            configurator.Parse(bytes);
                            StartConfig();
                        }
                        else
                        {
                            configurator.Status = Configurator.State.done;
                        }
                    }
                    else
                    {
                        configurator = new Configurator(IP);
                        configurator.Parse(bytes);
                        StartConfig();
                    }
                    Console.WriteLine(ClientHandlers.Count);
                }
                catch (Exception E)
                {

                }
            }
        }

        public ClientHandler Connect(string IP)
        {
            try
            {
                IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
                IPAddress ipAddress = System.Net.IPAddress.Parse(IP);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

                //Create TCP/IP socket
                Socket handler = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                ClientHandler clientHandler = new ClientHandler(this, handler);
                lock (ClientHandlers)
                {
                    ClientHandlers.Add(clientHandler);
                }
                return clientHandler;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }

        }

        public void Close()
        {
            foreach (ClientHandler client in ClientHandlers)
            {
                client.Close();
            }
            shutdown = true;
            //Socket.Shutdown(SocketShutdown.Both);
            Socket.Close();
        }

        public string IPAddress
        {
            get { return IP; }
        }

        public void SendMessage(String msg)
        {
            if (ClientHandlers.Count > 0)
            {
                ClientHandlers[0].SendMsg(msg);
            }
        }

        public void BroadCastMessage (String msg)
        {
            foreach (ClientHandler handler in ClientHandlers)
            {
                handler.SendMsg(msg);
            }
        }

        #region ClientHandler
        class ClientHandler
        {
            Socket handler;
            Thread MsgThread;
            GameConnection GameConnection;
            string PeerID;

            int time = 0;
            long LastTime;
            int maxTime = 15000;

            private bool running = true;
            public ClientHandler(GameConnection tc, Socket handler)
            {
                this.handler = handler;
                GameConnection = tc;

                PeerID = (handler.RemoteEndPoint as IPEndPoint).Address.ToString();
                string[] split_res = PeerID.Split('.');
                PeerID = "P" + split_res[split_res.Length - 1];

                MsgThread = new Thread(RecvrMsgCallback);
                MsgThread.Start();

                Thread CounterThread = new Thread(Counter);
                CounterThread.Start();
            }

            public void SendMsg(string msg)
            {
                byte[] msgBytes = Encoding.ASCII.GetBytes(msg);
                handler.Send(msgBytes);
                Console.WriteLine("Message Sent");
            }


            public void SendMsg(byte[] msg)
            {
                handler.Send(msg);
                Console.WriteLine("Message Sent");
            }

            private void Counter()
            {
                LastTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                while (running)
                {
                    long NowTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

                    time += (int)(NowTime - LastTime);

                    LastTime = NowTime;

                    if (time >= maxTime)
                    {
                        Close();
                        running = false;
                        lock (GameConnection.ClientHandlers)
                        {
                            GameConnection.ClientHandlers.Remove(this);
                        }
                        break;
                    }
                }
            }

            private void RecvrMsgCallback()
            {
                while (running)
                {
                    try
                    {
                        byte[] bytes = new byte[1024];
                        int bytesRec = handler.Receive(bytes);
                        time = 0;

                        // Message
                        Console.WriteLine(bytes[bytes.Length - 1]);
                        Console.WriteLine("Message : {0}", Encoding.ASCII.GetString(bytes, 0, bytesRec));

                        string response = "OK";

                        // Message Handling Here
                       
                        //Response
                        SendMsg(response);
                    }
                    catch (SocketException se)
                    {
                        running = false;
                        lock (GameConnection.ClientHandlers)
                        {
                            GameConnection.ClientHandlers.Remove(this);
                        }
                    }
                }
            }

            public void Close()
            {
                running = false;
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
        }
        #endregion
    }
}

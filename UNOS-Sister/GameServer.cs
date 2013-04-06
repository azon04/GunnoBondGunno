using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Windows.Forms;

namespace UNOS_Sister
{
    class GameServer
    {
        public string IP;
        public Socket Socket;
        List<ClientHandler> ClientHandlers;
        List<String> peerID; //peer yang ada di room itu
        Room room;  
        Thread ListenThread;

        bool shutdown = false;

        delegate void del(); // declare a delegate

        public GameServer()
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

            ListenThread.Start();
        }

        ~GameServer()
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

                    Console.WriteLine(ClientHandlers.Count);
                }
                catch (Exception E)
                {

                }
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

        #region ClientHandler
        class ClientHandler
        {
            Socket handler;
            Thread MsgThread;
            GameServer GameServer;
            string PeerID;

            int time = 0;
            long LastTime;
            int maxTime = 15000;

            private bool running = true;
            public ClientHandler(GameServer tc, Socket handler)
            {
                this.handler = handler;
                GameServer = tc;

                PeerID = (handler.RemoteEndPoint as IPEndPoint).Address.ToString();
                string[] split_res = PeerID.Split('.');
                PeerID = "P" + split_res[split_res.Length - 1];

                MsgThread = new Thread(RecvrMsgCallback);
                MsgThread.Start();

                Thread CounterThread = new Thread(Counter);
                CounterThread.Start();
            }

            private void SendMsg(string msg)
            {
                byte[] msgBytes = Encoding.ASCII.GetBytes(msg);
                handler.Send(msgBytes);
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
                }
            }

            private void RecvrMsgCallback()
            {
                while (running)
                {
                    try
                    {
                        Console.WriteLine(time);
                        if (time >= maxTime)
                        {
                            Close();
                            running = false;
                            lock (GameServer.ClientHandlers)
                            {
                                GameServer.ClientHandlers.Remove(this);
                            }
                            break;
                        }

                        byte[] bytes = new byte[1024];
                        int bytesRec = handler.Receive(bytes);
                        time = 0;
                        // Message
                        Console.WriteLine(bytes[bytes.Length - 1]);
                        Console.WriteLine("Message : {0}", Encoding.ASCII.GetString(bytes, 0, bytesRec));

                        string response = "OK";
                        string log = "";

                        // Message Handling
                        if (bytes[19] == 135)
                        {
                            response = PeerID;
                            log += "Handshake from " + (handler.RemoteEndPoint as IPEndPoint).Address.ToString() +
                                ", Peer ID : " + PeerID + "\n";
                        }
                        else if (bytes[19] == 235)
                        {

                        }
                        else if (bytes[19] == 182)
                        {
                            response = "OK, I will not kill you this time";
                            log += PeerID + ":" + "Still Alive\n";
                            time = 0;
                        }
                        else if (bytes[19] == 254)
                        {
                            response = "Oke list of room will be sent sooon";
                            log += PeerID + ":" + "Request List of Room\n";
                        }
                        else if (bytes[19] == 255)
                        {
                            // Create Room
                            response = "Oke Room will be created";
                            log += PeerID + ":" + "Created Room ID : X , MaxPlayer : 10\n";
                        }
                        //Response
                        SendMsg(response);
                    }
                    catch (SocketException se)
                    {
                        running = false;
                        lock (GameServer.ClientHandlers)
                        {
                            GameServer.ClientHandlers.Remove(this);
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

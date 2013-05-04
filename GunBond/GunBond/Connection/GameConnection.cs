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
    public class GameConnection
    {
        public List<string> IPTable;
        public string IP;
        public Socket Socket;
        List<ClientHandler> ClientHandlers;
        public string peerID; //peer yang ada di room itu
        Room room;
        Configurator configurator;
        Thread ListenThread;

        bool shutdown = false;

        public List<Message> MessageBox;

        delegate void del(); // declare a delegate

        public GameConnection(string peerID)
        {
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            Console.WriteLine((IP = ipAddress.ToString()));
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 12000);

            // Create a TCP/IP socket
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket
            Socket.Bind(localEndPoint);
            Socket.Listen(10);

            ClientHandlers = new List<ClientHandler>();
            ListenThread = new Thread(Listening);
            IPTable = new List<string>();
            this.peerID = peerID;

            //ListenThread.Start();
            configurator = new Configurator(IP);
            MessageBox = new List<Message>();
        }

        public void StartConfig(List<string> IPTable)
        {
            System.Diagnostics.Debug.WriteLine("Start Config - Start");
            configurator = new Configurator(IP, IPTable);
            configurator.Status = Configurator.State.starting;
            System.Diagnostics.Debug.WriteLine("Start Config " + IPTable.Count);
            List<string> toConnect = configurator.IPToConnect();
            Thread.Sleep(2000);
            foreach (string ip in toConnect)
            {
                ClientHandler ch = Connect(ip);
                System.Diagnostics.Debug.WriteLine(ip + "," + ip.Count());
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

        public void WaitConfigComplete()
        {
            while (configurator.Status != Configurator.State.done)
            {
                try
                {
                    Console.WriteLine("Waiting for game client..");
                    Socket handler = Socket.Accept();

                    byte[] bytes = new byte[1024];
                    int bytesRec = handler.Receive(bytes);

                        BroadCastMessage(configurator.ConstructMessageCompleteConfig());
                        configurator.Status = Configurator.State.done;

                    // Create Client Handler
                    lock (ClientHandlers)
                    {
                        ClientHandlers.Add(new ClientHandler(this, handler));
                    }

                    Console.WriteLine(ClientHandlers.Count);
                }
                catch (Exception e)
                {

                }
            }
        }

        public void WaitConfig()
        {
            Thread WaitConfigThread = new Thread(WaitConfigRun);
            WaitConfigThread.Start();
            while (configurator.Status != Configurator.State.done)
            {
            }
        }

        private void WaitConfigRun()
        {
            while (configurator.Status != Configurator.State.done)
            {
                try
                {
                    Console.WriteLine("Waiting for game client..");
                    Socket handler = Socket.Accept();

                    byte[] bytes = new byte[1024];
                    int bytesRec = handler.Receive(bytes);

                    configurator.Parse(bytes);
                    if (configurator.IPTable.Count > 2)
                    {
                        StartConfig();
                    }
                    ClientHandler handlerClient = new ClientHandler(this, handler);
                    if (configurator.IPTable.Count > 2)
                    {
                        handlerClient.WaitConfigComplete();
                    }
                    else
                    {
                        configurator.Status = Configurator.State.done;
                    }
                    
                    // Create Client Handler
                    lock (ClientHandlers)
                    {
                        ClientHandlers.Add(handlerClient);
                    }

                    Console.WriteLine(ClientHandlers.Count);
                }
                catch (Exception E)
                {

                }
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
                try
                {
                    Console.WriteLine("Waiting for game client..");
                    Socket handler = Socket.Accept();

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

                    // Create Client Handler
                    lock (ClientHandlers)
                    {
                        Console.WriteLine("GameConnection Listened :" + IP);
                        ClientHandlers.Add(new ClientHandler(this, handler));
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
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 12000);

                //Create TCP/IP socket
                Socket handler = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                handler.Connect(remoteEP);

                ClientHandler clientHandler = new ClientHandler(this, handler);
                lock (ClientHandlers)
                {
                    ClientHandlers.Add(clientHandler);
                }
                return clientHandler;
            }
            catch (SocketException se)
            {
                if (se.ErrorCode == (int)SocketError.ConnectionRefused)
                {
                    return Connect(IP);
                }
                return null;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(IP + "," + e.ToString());
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

        public void BroadCastMessage(String msg)
        {
            foreach (ClientHandler handler in ClientHandlers)
            {
                handler.SendMsg(msg);
            }
        }

        public void BroadCastMessage(byte[] msg)
        {
            foreach (ClientHandler handler in ClientHandlers)
            {
                handler.SendMsg(msg);
            }
        }

        public void Start()
        {
            foreach (ClientHandler handler in ClientHandlers)
            {
                handler.Start();
            }
        }

        public bool ifMessageRepeated(Message msg) {
            return msg.PeerID.Equals(this.peerID) || MessageBox.Contains(msg);
        }

        #region ClientHandler
        public class ClientHandler
        {
            Socket handler;
            Thread MsgThread;
            GameConnection Connection;
            string PeerID;

            int time = 0;
            long LastTime;
            int maxTime = 15000;

            private bool running = true;
            public ClientHandler(GameConnection tc, Socket handler)
            {
                this.handler = handler;
                Connection = tc;

                PeerID = (handler.RemoteEndPoint as IPEndPoint).Address.ToString();
                string[] split_res = PeerID.Split('.');
                PeerID = "P" + split_res[split_res.Length - 1];
                
            }

            public void Start()
            {
                MsgThread = new Thread(RecvrMsgCallback);
                MsgThread.Start();

                Thread CounterThread = new Thread(Counter);
                CounterThread.Start();
            }

            public void WaitConfigComplete()
            {
                Thread configThread = new Thread(ConfigComplete);
                configThread.Start();
            }

            private void ConfigComplete()
            {
                while (Connection.configurator.Status != Configurator.State.done)
                {
                    try
                    {
                        byte[] bytes = new byte[1024];
                        int bytesRec = handler.Receive(bytes);

                            if(Connection.configurator.IsMessageComplete(bytes)) {
                                Connection.configurator.Status = Configurator.State.done;
                            }
                            Connection.BroadCastMessage(Connection.configurator.ConstructMessageCompleteConfig());
                       
                    }
                    catch (Exception e)
                    {
                    }
                }
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
                        lock (Connection.ClientHandlers)
                        {
                            Connection.ClientHandlers.Remove(this);
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
                        Message m = new Message();
                        m.Parse(bytes);
                        if (m.msgCode == Message.FIRE)
                        {
                            //nembak
                            Game1.GameObject.Bullets.Add(new Bullet(Game1.GameObject,m.playerPos, m.bulletV0, m.playerRot, new Microsoft.Xna.Framework.Vector2(0,10))); 
                        }
                        else if (m.msgCode == Message.KEEP_ALIVE)
                        {
                                //keep alive
                            Player player = Game1.GameObject.Players[m.PeerID];
                            if (player != null)
                            {
                                player.setHealthPoint(m.HP);
                            }
                        }
                        else if (m.msgCode == Message.NEXT_PLAYER)
                        {
                            Game1.GameObject.WhoseTurn = m.nextPlayer;

                            //next player
                            if (Connection.peerID.Equals(m.nextPlayer))
                            {
                                // Set Fire true
                                Game1.GameObject.myPlayer.setFire(false);
                            }
                        } 
                        else if (m.msgCode == Message.POS)
                        {
                            //kirim position
                            Player player = Game1.GameObject.Players[m.PeerID];
                            if (player != null)
                            {
                                player.setPosition(m.playerPos);
                                player.setAngle(m.playerRot);
                                player.setOrientation(m.playerOrt);
                            }
                        }
                        else if (m.msgCode == Message.INIT)
                        {
                            //init
                            Player player = new Player(m.PeerID, m.playerPos0);
                            switch (m.playerTexture)
                            {
                                case 0 :
                                    player.setPlayerTexture(AssetsManager.AssetsList["orang1"]);
                                    break;
                                case 1:
                                    player.setPlayerTexture(AssetsManager.AssetsList["orang2"]);
                                    break;
                                case 2:
                                    player.setPlayerTexture(AssetsManager.AssetsList["orang3"]);
                                    break;
                                case 3:
                                    player.setPlayerTexture(AssetsManager.AssetsList["orang4"]);
                                    break;
                                default:
                                    player.setPlayerTexture(AssetsManager.AssetsList["orang1"]);
                                    break;
                            }

                            Game1.GameObject.Players.Add(m.PeerID, player);
                        }

                        //Response
                        //SendMsg(response);
                        if (!Connection.ifMessageRepeated(m))
                        {
                            if(Connection.MessageBox.Count >= 5)
                                Connection.MessageBox.Remove(Connection.MessageBox[0]);
                            Connection.MessageBox.Add(m);
                            Connection.BroadCastMessage(m.Construct());
                        }
                    }
                    catch (SocketException se)
                    {
                        running = false;
                        lock (Connection.ClientHandlers)
                        {
                            Connection.ClientHandlers.Remove(this);
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

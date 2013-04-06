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
    class Tracker
    {
        public string IP;
        public Socket Socket;
        List<ClientHandler> ClientHandlers;
        List<Room> Rooms;
        Thread ListenThread;

        public int max_peer = 10;
        public int max_room = 10;
        public bool log = true;
        bool shutdown = false;

        FileStream fileLog = null;

        public RichTextBox LogText;

        delegate void del(); // declare a delegate

        public Tracker()
        {
            Rooms = new List<Room>();

            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            Console.WriteLine((IP=ipAddress.ToString()));
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            // Create a TCP/IP socket
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket
            Socket.Bind(localEndPoint);
            Socket.Listen(10);

            ClientHandlers = new List<ClientHandler>();
            ListenThread = new Thread(Listening);

            ListenThread.Start();

            if (log)
            {
                fileLog = new FileStream("log.txt", FileMode.OpenOrCreate, FileAccess.Write);
            }

            LogText = null;
        }

        ~Tracker()
        {
            Console.WriteLine("Deconstructor");
            
        }

        public void Listening()
        {
            while (!shutdown)
            {
                lock (ClientHandlers)
                {
                    if (ClientHandlers.Count >= max_peer) continue;
                }
                try
                {
                    Console.WriteLine("Waiting for galz ..");
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
            fileLog.Close();
            shutdown = true;
            Socket.Close();
        }

        public string IPAddress
        {
            get {return IP; }
        }

        #region ClientHandler
        class ClientHandler
        {
            Socket handler;
            Thread MsgThread;
            Tracker Tracker;
            string PeerID;

            int time = 0;
            long LastTime;
            int maxTime = 15000;

            private bool running = true;
            public ClientHandler(Tracker tc, Socket handler)
            {
                this.handler = handler;
                Tracker = tc;
                
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
                            lock (Tracker.ClientHandlers)
                            {
                                Tracker.ClientHandlers.Remove(this);
                            }
                            break;
                        }

                        byte[] bytes = new byte[1024];
                        int bytesRec = handler.Receive(bytes);
                        time = 0;
                        // Message
                        UNOS_Sister.Message msg = new UNOS_Sister.Message();
                        msg.parseMe(bytes);

                        Console.WriteLine(bytes[bytes.Length - 1]);
                        Console.WriteLine("Message : {0}", Encoding.ASCII.GetString(bytes, 0, bytesRec));

                        List<byte> response = new List<byte>(); 
                        response.AddRange(Encoding.ASCII.GetBytes("GunbondGame00000000"));

                        UNOS_Sister.Message msgResponse = new UNOS_Sister.Message();
                        
                        string log = "";

                        msgResponse.msgPeerID = PeerID;
                        // Message Handling
                        // Handsake
                        if (msg.msgCode == UNOS_Sister.Message.HANDSHAKE)
                        {
                            response.Add(135);
                            response.AddRange(Encoding.ASCII.GetBytes(PeerID)); ;
                            log += "Handshake from " + (handler.RemoteEndPoint as IPEndPoint).Address.ToString() +
                                ", Peer ID : " + PeerID + "\n";
                            msgResponse.msgCode = UNOS_Sister.Message.HANDSHAKE;
                        }
                        else if (msg.msgCode == UNOS_Sister.Message.QUIT) // Quit Room
                        {
                            msgResponse.msgCode = UNOS_Sister.Message.SUCCESS;
                            Console.WriteLine("Quit Room");
                        }
                        else if (msg.msgCode == UNOS_Sister.Message.START) // Start Room
                        {
                            msgResponse.msgCode = UNOS_Sister.Message.SUCCESS;
                            Console.WriteLine("Start Room");
                        }
                        else if (msg.msgCode == UNOS_Sister.Message.KEEP_ALIVE) // Keep Alive 
                        { 
                            //response = "OK, I will not kill you this time";
                            msgResponse.msgCode = UNOS_Sister.Message.SUCCESS;
                            log += PeerID + ":" + "Still Alive\n";
                            time = 0;
                        }
                        else if (msg.msgCode == UNOS_Sister.Message.LIST) // Listing Room
                        {
                            //response = "Oke list of room will be sent sooon";
                            log += PeerID + ":" + "Request List of Room\n";
                            msgResponse.msgCode = UNOS_Sister.Message.ROOM;
                            msgResponse.Rooms.Clear();
                            msgResponse.Rooms.AddRange(Tracker.Rooms);
                        }
                        else if (msg.msgCode == UNOS_Sister.Message.CREATE_ROOM) //Create Room
                        {
                            // Create Room
                            //response = "Oke Room will be created";
                            if (Tracker.Rooms.Count() + 1 <= Tracker.max_room)
                            {
                                msgResponse.msgCode = UNOS_Sister.Message.SUCCESS;
                                response.Add(127);
                                msg.printMsg();
                                lock (Tracker.Rooms)
                                {
                                    Tracker.Rooms.AddRange(msg.Rooms);
                                }
                            }
                            else
                            {
                                response.Add(128);
                                msgResponse.msgCode = UNOS_Sister.Message.FAILED;                                
                            }

                            Console.WriteLine("Max Player :" + msg.Rooms[0].getMaxPlayer());
                            log += PeerID + ":" + "Created Room ID :"+ msg.Rooms[0].getRoomID().Substring(0,msg.Rooms[0].getRoomID().IndexOf('\0')) + ", MaxPlayer :" +
                                 msg.Rooms[0].getMaxPlayer().ToString()  + "\n";
                            Console.WriteLine(log);
                        }
                        else if (msg.msgCode == 253) // Join Room
                        {
                            Console.WriteLine("Join Room");
                            msgResponse.msgCode = UNOS_Sister.Message.SUCCESS;
                        }

                        response.Clear(); 
                        
                        response.AddRange(msgResponse.Construct());

                        if (Tracker.log)
                        {
                            Console.WriteLine("Write Log");
                            lock (Tracker.fileLog)
                            {
                                Tracker.fileLog.Write(Encoding.ASCII.GetBytes(log), 0, log.Length);
                                Tracker.fileLog.Write(Encoding.ASCII.GetBytes("->"), 0, 2);
                                Tracker.fileLog.Write(response.ToArray(), 0, response.Count);
                                Tracker.fileLog.Write(Encoding.ASCII.GetBytes("\n"), 0, 1);
                            }

                            if (Tracker.LogText != null)
                            {
                                lock (Tracker.LogText)
                                {
                                    del update_log_text = new del(()=> {
                                        Tracker.LogText.Text += log;
                                        Tracker.LogText.Text += "->" + Encoding.ASCII.GetString(response.ToArray()) + "\n";
                                    });
                                    Tracker.LogText.Invoke(update_log_text);
                                }
                            }
                        }

                        //Response
                        handler.Send(response.ToArray());
                    }
                    catch (SocketException se)
                    {
                        running = false;
                        lock (Tracker.ClientHandlers)
                        {
                            Tracker.ClientHandlers.Remove(this);
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

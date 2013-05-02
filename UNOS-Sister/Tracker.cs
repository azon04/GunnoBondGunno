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
        #region data member
        public string IP;
        public Socket Socket;
        Dictionary<string, ClientHandler> ClientHandlers;
        Dictionary<string, Room> Rooms;
        Dictionary<string, string> IPPeers;

        Thread ListenThread;

        public int max_peer = 10;
        public int max_room = 10;
        public bool log = true;
        bool shutdown = false;

        FileStream fileLog = null;

        public RichTextBox LogText;

        delegate void del(); // declare a delegate
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public Tracker()
        {
            // Initialize Data Member
            Rooms = new Dictionary<string, Room>();
            IPPeers = new Dictionary<string, string>();

            // Create ENdPoint
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            Console.WriteLine((IP = ipAddress.ToString()));
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            // Create a TCP/IP socket
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket
            Socket.Bind(localEndPoint);
            Socket.Listen(10);

            // Listener Thread
            ClientHandlers = new Dictionary<String, ClientHandler>();
            ListenThread = new Thread(Listening);
            ListenThread.Start();

            // Log Activity if "log" set true
            if (log)
            {
                fileLog = new FileStream("log.txt", FileMode.OpenOrCreate, FileAccess.Write);
            }

            LogText = null;
        }

        /// <summary>
        /// Deconstructor
        /// </summary>
        ~Tracker()
        {
            Console.WriteLine("Deconstructor");
        }

        /// <summary>
        /// Listening Function
        /// </summary>
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
                        ClientHandlers.Add((handler.RemoteEndPoint as IPEndPoint).Address.ToString(), new ClientHandler(this, handler));
                    }

                    Console.WriteLine(ClientHandlers.Count);
                }
                catch (Exception E)
                {

                }
            }
        }

        /// <summary>
        /// Close the tracker | Close Connection to Peers
        /// </summary>
        public void Close()
        {
            foreach (ClientHandler client in ClientHandlers.Values)
            {
                client.Close();
            }
            fileLog.Close();
            shutdown = true;
            Socket.Close();
        }

        /// <summary>
        /// Get IP Address
        /// </summary>
        public string IPAddress
        {
            get { return IP; }
        }

        public Room GetRoom(string PeerID)
        {
            Room rRes = null;
            foreach (Room room in Rooms.Values)
            {
                if (room.getPeerID().Equals(PeerID))
                {
                    rRes = room;
                    break;
                }
            }
            return rRes;
        }

        #region ClientHandler
        /// <summary>
        /// Handler for Peer from-to Tracker
        /// </summary>
        class ClientHandler
        {
            Socket handler;
            Thread MsgThread;
            Tracker Tracker;
            string PeerID;
            string IP;

            int time = 0;
            long LastTime;
            int maxTime = 15000;

            private bool running = true;
            private bool waitConfirmation = false;

            /// <summary>
            /// Constructor of Client Handler
            /// </summary>
            /// <param name="tc">Tracker</param>
            /// <param name="handler">Socket handler</param>
            public ClientHandler(Tracker tc, Socket handler)
            {
                this.handler = handler;
                Tracker = tc;

                // Get IP and Peer ID | Peer ID must be 4 length byte
                IP = (handler.RemoteEndPoint as IPEndPoint).Address.ToString();
                PeerID = (handler.RemoteEndPoint as IPEndPoint).Address.ToString();
                string[] split_res = PeerID.Split('.');
                if (split_res[split_res.Length - 1].Length < 3)
                {
                    PeerID = "P";
                    for (int i = 0; i < 3 - split_res[split_res.Length - 1].Length; i++) PeerID += "0";
                    PeerID += split_res[split_res.Length - 1];
                }
                else
                {
                    PeerID = "P" + split_res[split_res.Length - 1];
                }

                // Thread for Receving Message from Peer
                MsgThread = new Thread(RecvrMsgCallback);
                MsgThread.Start();

                // Thread for timeout
                Thread CounterThread = new Thread(Counter);
                CounterThread.Start();

                // Add PeerID to Dictionary of IP
                Tracker.IPPeers.Add(PeerID, IP);
            }

            /// <summary>
            /// Send string message to Peer
            /// </summary>
            /// <param name="msg">Message to be sent</param>
            private void SendMsg(string msg)
            {
                byte[] msgBytes = Encoding.ASCII.GetBytes(msg);
                handler.Send(msgBytes);
                Console.WriteLine("Message Sent");
            }

            /// <summary>
            /// Counter Function
            /// </summary>
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

            /// <summary>
            /// Callbacks for Receiving Message from Client and Handle it.
            /// </summary>
            private void RecvrMsgCallback()
            {

                while (running)
                {
                    while (waitConfirmation) { };
                    try
                    {

                        Console.WriteLine(time);
                        if (time >= maxTime)
                        {
                            Close();
                            running = false;
                            lock (Tracker.ClientHandlers)
                            {
                                Tracker.ClientHandlers.Remove(IP);
                            }
                            break;
                        }

                        byte[] bytes = new byte[1024];
                        int bytesRec = handler.Receive(bytes);
                        time = 0; // time

                        // Message
                        UNOS_Sister.Message msg = new UNOS_Sister.Message();
                        msg.parseMe(bytes);

                        Console.WriteLine(bytes[bytes.Length - 1]);
                        Console.WriteLine("Message : {0}", Encoding.ASCII.GetString(bytes, 0, bytesRec));

                        List<byte> response = new List<byte>();
                        response.AddRange(Encoding.ASCII.GetBytes("GunbondGame00000000"));

                        UNOS_Sister.Message msgResponse = new UNOS_Sister.Message();

                        // TO DO INITIAL MESSAGE RESPONSE

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
                            bool found = false;
                            int i = 0;
                            Room room = null;
                            while (!found)
                            {
                                room = Tracker.Rooms.ElementAt(i).Value;
                                foreach (string id in room.PeerIDs)
                                {
                                    if (id.Equals(msg.msgPeerID))
                                    {
                                        Console.WriteLine("Peer " + msg.msgPeerID + "is found in " + room.getRoomID());
                                        found = true;
                                    }
                                }
                            }
                            if (found)
                            {
                                Console.WriteLine("QUIT");
                                string sRoom = room.getPeerID();
                                Console.WriteLine("Peer ID : " + sRoom);

                                string s = Tracker.IPPeers[sRoom];

                                Console.WriteLine("Key : " + s);

                                ClientHandler handlerCreatorPeer = Tracker.ClientHandlers[s];
                                UNOS_Sister.Message msgConfirmation = msg;
                                msgResponse.msgCode = handlerCreatorPeer.SentForConfirmation(msgConfirmation.Construct()).msgCode;
                                if (msgResponse.msgCode == Message.SUCCESS)
                                {
                                    Console.WriteLine("Sukses");
                                    room.PeerIDs.Remove(msg.msgPeerID);
                                    if (room.PeerIDs.Count == 0) Tracker.Rooms.Remove(room.getRoomID());
                                }
                                else if (msgResponse.msgCode == Message.FAILED)
                                {
                                    Console.WriteLine("Failed");
                                }
                                else
                                {
                                    Console.WriteLine("APA ? " + msgResponse.msgCode);
                                    msgResponse.msgCode = Message.FAILED;
                                }

                            }
                            else
                                msgResponse.msgCode = UNOS_Sister.Message.FAILED;
                            Console.WriteLine("Quit Room");
                        }
                        else if (msg.msgCode == UNOS_Sister.Message.START) // Start Room
                        {
                            Room sRoom = Tracker.GetRoom(msg.msgPeerID);
                            if (sRoom != null)
                            {
                                msgResponse.msgCode = UNOS_Sister.Message.SUCCESS;
                                foreach (string id in sRoom.PeerIDs)
                                {
                                    if (!id.Equals(msg.msgPeerID))
                                    {
                                        Tracker.ClientHandlers[id].handler.Send(msg.Construct());
                                    }
                                }
                            } else {
                                msgResponse.msgCode = UNOS_Sister.Message.FAILED;
                            
                            }
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
                            msgResponse.Rooms.AddRange(Tracker.Rooms.Values);
                        }
                        else if (msg.msgCode == UNOS_Sister.Message.CREATE_ROOM) //Create Room
                        {
                            // Create Room
                            // response = "Oke Room will be created";
                            if (Tracker.Rooms.Count() + 1 <= Tracker.max_room)
                            {
                                msgResponse.msgCode = UNOS_Sister.Message.SUCCESS;
                                response.Add(127);
                                msg.printMsg();
                                lock (Tracker.Rooms)
                                {
                                    Console.WriteLine("Peer ID :" + msg.msgPeerID);
                                    msg.Rooms.ElementAt(0).PeerIDs.Add(msg.msgPeerID);
                                    Tracker.Rooms.Add(msg.Rooms[0].getRoomID(), msg.Rooms.ElementAt(0));
                                }
                            }
                            else
                            {
                                response.Add(128);
                                msgResponse.msgCode = UNOS_Sister.Message.FAILED;
                            }

                            Console.WriteLine("Max Player :" + msg.Rooms[0].getMaxPlayer());
                            log += PeerID + ":" + "Created Room ID :" + msg.Rooms[0].getRoomID().Substring(0, msg.Rooms[0].getRoomID().IndexOf('\0')) + ", MaxPlayer :" +
                                 msg.Rooms[0].getMaxPlayer().ToString() + "\n";
                            Console.WriteLine(log);
                        }
                        else if (msg.msgCode == Message.JOIN) // Join Room
                        {
                            Console.WriteLine("Join Room");
                            Console.WriteLine("Room ID : " + msg.Rooms[0].getRoomID());

                            string sRoom = Tracker.Rooms[msg.Rooms[0].getRoomID()].getPeerID();
                            if (sRoom != null)
                            {
                                Console.WriteLine("Peer ID : " + sRoom);

                                string s = Tracker.IPPeers[sRoom];

                                Console.WriteLine("Key : " + s);

                                ClientHandler handlerCreatorPeer = Tracker.ClientHandlers[s];
                                UNOS_Sister.Message msgConfirmation = new UNOS_Sister.Message();
                                msgConfirmation.msgCode = Message.CHECK;
                                msgConfirmation.msgPeerID = msg.msgPeerID;
                                msgResponse.msgCode = handlerCreatorPeer.SentForConfirmation(msgConfirmation.Construct()).msgCode;
                                if (msgResponse.msgCode == Message.SUCCESS)
                                {
                                    Console.WriteLine("Sukses");
                                    Room room = Tracker.Rooms[msg.Rooms[0].getRoomID()];
                                    room.PeerIDs.Add(msg.msgPeerID);
                                }
                                else if (msgResponse.msgCode == Message.FAILED)
                                {
                                    Console.WriteLine("Failed");
                                }
                                else
                                {
                                    Console.WriteLine("APA ? " + msgResponse.msgCode);
                                    msgResponse.msgCode = Message.FAILED;
                                }
                            }
                            else
                            {
                                Room room = Tracker.Rooms[msg.Rooms[0].getRoomID()];
                                room.PeerIDs.Add(msg.msgPeerID);
                                msgResponse.msgCode = Message.SUCCESS;
                            }
                        }

                        response.Clear();
                        Console.WriteLine("Message to Write : " + msgResponse.msgCode);

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
                                    del update_log_text = new del(() =>
                                    {
                                        Tracker.LogText.Text += log;
                                        Tracker.LogText.Text += "->" + Encoding.ASCII.GetString(response.ToArray()) + "\n";
                                    });
                                    Tracker.LogText.Invoke(update_log_text);
                                }
                            }
                        }

                        // Send Response
                        handler.Send(response.ToArray());
                    }
                    catch (SocketException se)
                    {
                        running = false;
                        lock (Tracker.ClientHandlers)
                        {
                            Tracker.ClientHandlers.Remove(IP);
                        }
                    }
                }
            }

            /// <summary>
            /// Send msg and wait for confirmation
            /// </summary>
            /// <param name="msg">Bytes to Sent</param>
            /// <returns>Message Confirmation</returns>
            public Message SentForConfirmation(byte[] msg)
            {
                waitConfirmation = true;
                handler.Send(msg);

                byte[] bytes = new byte[1024];
                int bytesRec = handler.Receive(bytes);
                time = 0;
                // Message
                UNOS_Sister.Message msgConfirm = new UNOS_Sister.Message();
                msgConfirm.parseMe(bytes);
                waitConfirmation = false;

                return msgConfirm;
            }

            /// <summary>
            /// Close Peer Connection
            /// </summary>
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

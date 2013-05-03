using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.Net.Sockets;

namespace UNOS_Sister
{
    class Peer
    {
        PeerUI peerUI;

        int playStatus; // 0 = closed, 1 = player, 2 = room creator

        public String PeerID = "";
        Socket sender;
        public bool connected = false;

        Queue<byte[]> sendMsg;
        Queue<byte[]> receivedMsg;

        List<Room> roomList;
        Room myRoom;
        List<String> peerList;

        Thread keepAliveThread;
        Thread senderThread;
        Thread processThread;

        delegate void del();
        delegate void sendDel();
        delegate void recvDel();

        byte[] roomIDbytes_ = new byte[50];

        Boolean inRoom = false;
        Boolean canQuit = true;
        String joininRoom = "";

        public Peer(PeerUI peerUI)
        {
            this.peerUI = peerUI;
            playStatus = 0;
            sendMsg = new Queue<byte[]>();
            receivedMsg = new Queue<byte[]>();
            roomList = new List<Room>();
            peerList = new List<String>();
            myRoom = new Room();

            keepAliveThread = new Thread(KeepAlive);
            senderThread = new Thread(sendMessage);
            //processThread = new Thread(processMessage);
        }

        public void ConnectToServer(string serverIP)
        {
            try
            {
                IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
                IPAddress ipAddress = IPAddress.Parse(serverIP);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

                //Create TCP/IP socket
                sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to remote endpoint
                try
                {
                    sender.Connect(remoteEP);

                    byte[] bytes = new byte[1024];
                    byte[] msg = Encoding.ASCII.GetBytes("GunbondGame00000000");
                    List<byte> byteList = new List<byte>();
                    byteList.AddRange(msg);
                    byteList.Add(135);
                    msg = byteList.ToArray();

                    // Send the data through the socket
                    int byteSent = sender.Send(msg);

                    // Receive the response from remote device
                    int byteRec = sender.Receive(bytes);
                    PeerID = Encoding.ASCII.GetString(bytes, 0, byteRec);

                    if (PeerID != "")
                    {
                        //parse peerID 
                        PeerID = PeerID.Substring(PeerID.Length-4, 4);
                        Console.WriteLine("Peer id : " + PeerID);

                        connected = true;
                        keepAliveThread.Start();
                        senderThread.Start();
                        //processThread.Start();
                    }
                    else
                        connected = false;
                }
                catch (ArgumentNullException ane)
                {
                    connected = false;
                }
                catch (SocketException se)
                {
                    connected = false;
                }
                catch (Exception e)
                {
                    connected = false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                connected = false;
            }
        }

        public void DisconnectFromServer()
        {
            connected = false;
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
            peerUI.Close();
            Console.WriteLine("Disconnect from server");            
        }

        public void GetRoomList()
        {
            if (connected == true)
            {
                byte[] bytes = new byte[1024];
                byte[] msg = Encoding.ASCII.GetBytes("GunbondGame00000000");
                List<byte> byteList = new List<byte>();
                byteList.AddRange(msg);
                byteList.Add(254);
                byteList.AddRange(Encoding.ASCII.GetBytes(PeerID));
                msg = byteList.ToArray();

                lock (sendMsg) //add to Queue
                {
                    sendMsg.Enqueue(msg);
                }
            }
            else
            {
                Console.WriteLine("You are not connected");
            }
        }

        public void createRoom(string roomID, string max_player)
        {
            if (connected == true)
            {
                if (inRoom == false)
                {
                    //<pstr><reserved><create_ code><peer_id><max_ player_num><room_id>
                    byte[] bytes = new byte[1024];
                    byte[] msg = Encoding.ASCII.GetBytes("GunbondGame00000000");
                    List<byte> byteList = new List<byte>();
                    byteList.AddRange(msg);
                    byteList.Add(255); //<create_code>
                    byteList.AddRange(Encoding.ASCII.GetBytes(PeerID)); //<peer_id>

                    //<max_player_num>
                    if (max_player == "2 Player")
                    {
                        byteList.Add(2);
                    }
                    else if (max_player == "4 Player")
                    {
                        byteList.Add(4);
                    }
                    else if (max_player == "6 Player")
                    {
                        byteList.Add(6);
                    }
                    else if (max_player == "8 Player")
                    {
                        byteList.Add(8);
                    }

                    //<room_id>
                    byte[] roomIDbytes = new byte[50];
                    roomIDbytes = Encoding.ASCII.GetBytes(roomID);
                    if (50 - roomID.Length > 0)
                    {
                        roomIDbytes = roomIDbytes.Concat(new byte[50 - roomID.Length]).ToArray();
                    }
                    byteList.AddRange(roomIDbytes);
                    roomIDbytes_ = roomIDbytes;

                    msg = byteList.ToArray();
                    lock (sendMsg) //add to Queue
                    {
                        sendMsg.Enqueue(msg);
                    }
                }
                else
                {
                    Console.WriteLine("Can't create room. You are currently joining another ROOM");
                }
            }
            else
            {
                Console.WriteLine("You are not connected");
            }
        }

        public void joinRoom(string room_id)
        {
            joininRoom = room_id;
            if (connected == true)
            {

                //<pstr><reserved><join_code><peer_id><room_id>
                byte[] bytes = new byte[1024];
                byte[] msg = Encoding.ASCII.GetBytes("GunbondGame00000000");
                List<byte> byteList = new List<byte>();
                byteList.AddRange(msg);
                byteList.Add(253); //<join_code>
                byteList.AddRange(Encoding.ASCII.GetBytes(PeerID)); //<peer_id>

                byte[] roomIDbyte = new byte[50];
                roomIDbyte = Encoding.ASCII.GetBytes(room_id);
                if (50 - room_id.Length > 0)
                {
                    roomIDbyte = roomIDbyte.Concat(new byte[50 - room_id.Length]).ToArray();
                }
                
                byteList.AddRange(roomIDbyte); //<room_id>
                msg = byteList.ToArray();
                lock (sendMsg) //add to Queue
                {
                    sendMsg.Enqueue(msg);
                }
            }
            else
            {
                Console.WriteLine("You are not connected");
            }
        }

        public void quitRoom()
        {
            if (connected == true)
            {
                if (canQuit == true)
                {
                    //<pstr><reserved><quit_code><peer_id>
                    byte[] bytes = new byte[1024];
                    byte[] msg = Encoding.ASCII.GetBytes("GunbondGame00000000");
                    List<byte> byteList = new List<byte>();
                    byteList.AddRange(msg);
                    byteList.Add(235); //<quit_code>
                    byteList.AddRange(Encoding.ASCII.GetBytes(PeerID)); //<peer_id>
                    msg = byteList.ToArray();
                    lock (sendMsg) //add to Queue
                    {
                        sendMsg.Enqueue(msg);
                    }
                }
                else
                {
                    Console.WriteLine("Creator Peer can not quit room. HAHA sukurin!");
                }
            }
            else
            {
                Console.WriteLine("You are not connected");
            }
        }

        public void startGame(string room_id)
        {
            if (connected == true)
            {
                //<pstr><reserved><start_code><peer_id><room_id>
                byte[] bytes = new byte[1024];
                byte[] msg = Encoding.ASCII.GetBytes("GunbondGame00000000");
                List<byte> byteList = new List<byte>();
                byteList.AddRange(msg);
                byteList.Add(252); //<start_code>
                byteList.AddRange(Encoding.ASCII.GetBytes(PeerID)); //<peer_id>
                byteList.AddRange(Encoding.ASCII.GetBytes(room_id)); //<room_id>
                msg = byteList.ToArray();
                lock (sendMsg) //add to Queue
                {
                    sendMsg.Enqueue(msg);
                }
            }
            else
            {
                Console.WriteLine("You are not connected");
            }
        }

        void sendMessage()
        {
            while (connected)
            {
                lock (sendMsg)
                {
                    while (sendMsg.Count > 0)
                    {
                        try
                        {
                            byte[] MsgToBeSent = sendMsg.Dequeue();
                            Console.WriteLine("Sending : " + Encoding.ASCII.GetString(MsgToBeSent));
                            sendDel printSendMsg = new sendDel(() =>
                            {
                                peerUI.textBox4.Text = Encoding.ASCII.GetString(MsgToBeSent);
                            });
                            peerUI.Invoke(printSendMsg);

                            int byteSent = sender.Send(MsgToBeSent); // kirim ke tracker

                            //Receive response from tracker
                            byte[] bytes = new byte[1024];
                            int byteRec = sender.Receive(bytes);
                            recvDel printRecvMsg = new recvDel(() =>
                            {
                                peerUI.textBox5.Text = Encoding.ASCII.GetString(bytes);
                            });
                            peerUI.Invoke(printRecvMsg);


                            Message mSent = new Message();
                            mSent.parseMe(MsgToBeSent);

                            Message m = new Message();
                            m.parseMe(bytes);

                            if (m.msgCode == Message.START)
                            { 
                                //start game here
                                playStatus = 1;
                                peerUI.Close();
                            }
                            else
                            if (m.msgCode == Message.SUCCESS)
                            {
                                if (mSent.msgCode == Message.START)
                                {
                                    //start game here
                                    playStatus = 2;
                                    peerUI.Close();
                                }
                                else if (mSent.msgCode == Message.CREATE_ROOM)
                                {
                                    //create room success 
                                    Console.WriteLine("Create Room Success");
                                    //masukin ke myRoom
                                    myRoom = mSent.Rooms[0];
                                    inRoom = true; //masuk ke room yang di create
                                    canQuit = false;
                                    Console.WriteLine("my Room : " + myRoom.getRoomID());
                                    peerList.Add(this.PeerID);

                                    sendDel printCurrentRoom = new sendDel(() =>
                                    {
                                        peerUI.textBox6.Text = myRoom.getRoomID();
                                    });
                                    peerUI.Invoke(printCurrentRoom);
                                }
                                else if (mSent.msgCode == Message.JOIN)
                                {
                                    //join success
                                    inRoom = true;

                                    sendDel printCurrentRoom = new sendDel(() =>
                                    {
                                        peerUI.textBox6.Text = joininRoom;
                                    });
                                    peerUI.Invoke(printCurrentRoom);

                                    //Print Room Member
                                    del printRoomMember = new del(() =>
                                    {
                                        for (int i = 0; i < peerList.Count; i++)
                                        {
                                            peerUI.richTextBox1.Text += peerList[i];
                                            peerUI.richTextBox1.Text += "\n";
                                        }

                                    });
                                    peerUI.Invoke(printRoomMember);

                                    Console.WriteLine("Join Room Success");
                                    //TO DO : koneksi dengan GameConnection
                                }
                                else if (mSent.msgCode == Message.KEEP_ALIVE)
                                {
                                    //keep alive success
                                    Console.WriteLine("Keep Alive Success");
                                }
                                else if (mSent.msgCode == Message.QUIT)
                                {

                                    //quit success
                                    inRoom = false;
                                    sendDel printCurrentRoom = new sendDel(() =>
                                    {
                                        peerUI.textBox6.Text = "-";
                                    });
                                    peerUI.Invoke(printCurrentRoom);
                                    Console.WriteLine("Quit Success");
                                }
                            }
                            else if (m.msgCode == Message.ROOM) {
                                //get list room
                                roomList.Clear();
                                roomList.AddRange(m.Rooms);
                                Console.WriteLine("Room List : ");
                                List<string> roomString = new List<string>();
                                del printRoomList = new del(() => {
                                    Console.WriteLine("Room List : ");
                                    for (int i = 0; i < roomList.Count; i++)
                                    {
                                        Console.WriteLine(roomList[i].getRoomID());
                                        roomString.Add(roomList[i].getRoomID());
                                    }
                                    peerUI.listBox1.DataSource = roomString;
                                });
                                peerUI.Invoke(printRoomList);                                
                            }
                            else if (m.msgCode == Message.FAILED)
                            {
                                if (mSent.msgCode == Message.CREATE_ROOM)
                                {
                                    //create room success 
                                    Console.WriteLine("Create Room FAILED");
                                }
                                else if (mSent.msgCode == Message.JOIN)
                                {
                                    //join success
                                    Console.WriteLine("Join Room FAILED");
                                }
                                else if (mSent.msgCode == Message.KEEP_ALIVE)
                                {
                                    //keep alive success
                                    Console.WriteLine("Keep Alive FAILED");
                                }
                                else if (mSent.msgCode == Message.QUIT)
                                {
                                    //quit success
                                    Console.WriteLine("Quit FAILED");
                                }
                            }
                            else if (m.msgCode == 100) //Check if myRoom masih muat
                            {                                
                                Console.WriteLine("jumlah peer : " + peerList.Count());
                                Console.WriteLine("max player : " + myRoom.getMaxPlayer());
                                if (peerList.Count() < myRoom.getMaxPlayer())
                                {
                                    Console.WriteLine("Masih bisa join");
                                    byte[] msg = Encoding.ASCII.GetBytes("GunbondGame00000000");
                                    List<byte> byteList = new List<byte>();
                                    byteList.AddRange(msg);
                                    byteList.Add(Message.SUCCESS);
                                    msg = byteList.ToArray();
                                    int byteSent_ = sender.Send(msg);

                                    //Receive response from tracker
                                    byte[] join_msg = new byte[1024];
                                    int byteRecs = sender.Receive(join_msg);
                                    Message m_ = new Message();
                                    m_.parseMe(join_msg);
                                    if (m_.msgCode == Message.SUCCESS) 
                                    {
                                        peerList.Add(m.msgPeerID);

                                        //Print Room Member
                                        del printRoomMember = new del(() =>
                                        {
                                            Console.WriteLine("RoomMember : ");
                                            for (int i = 0; i < peerList.Count; i++)
                                            {
                                                peerUI.richTextBox1.Text += peerList[i];
                                                peerUI.richTextBox1.Text += "\n";
                                                Console.WriteLine(peerList[i]);
                                            }

                                        });
                                        peerUI.Invoke(printRoomMember);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Sudah join room lain");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Room already full");
                                    byte[] msg = Encoding.ASCII.GetBytes("GunbondGame00000000");
                                    List<byte> byteList = new List<byte>();
                                    byteList.AddRange(msg);
                                    byteList.Add(Message.FAILED);
                                    msg = byteList.ToArray();
                                    int byteSent_ = sender.Send(msg);
                                }
                            }
                            else if (m.msgCode == Message.QUIT) {
                                peerList.Remove(m.msgPeerID);
                                Console.WriteLine("Peer " + m.msgPeerID + " quit from your room. Boo!");
                                byte[] msg = Encoding.ASCII.GetBytes("GunbondGame00000000");
                                List<byte> byteList = new List<byte>();
                                byteList.AddRange(msg);
                                byteList.Add(Message.SUCCESS);
                                msg = byteList.ToArray();
                                int byteSent_ = sender.Send(msg);

                                //Print Room Member
                                del printRoomMember = new del(() =>
                                {
                                    Console.WriteLine("RoomMember : ");
                                    peerUI.richTextBox1.Text = "";
                                    for (int i = 0; i < peerList.Count; i++)
                                    {
                                        peerUI.richTextBox1.Text += peerList[i];
                                        peerUI.richTextBox1.Text += "\n";
                                        Console.WriteLine(peerList[i]);
                                    }

                                });
                                peerUI.Invoke(printRoomMember);
                            }
                        }
                        catch (SocketException se)
                        {
                            Console.WriteLine(se.ToString());
                            if (se.SocketErrorCode == SocketError.ConnectionAborted)
                            {
                                connected = false;
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }
                    }
                }
            }
        }

        void processMessage()
        {
            while (connected)
            {
                lock (receivedMsg)
                {
                    while (receivedMsg.Count > 0)
                    {
                        byte[] msg = receivedMsg.Dequeue();
                        Console.WriteLine("Processing : " + Encoding.ASCII.GetString(msg));
                    }
                }
            }
        }

        void KeepAlive()
        {
            while (connected)
            {
                Thread.Sleep(10000);
                byte[] bytes = new byte[1024];
                byte[] msg = Encoding.ASCII.GetBytes("GunbondGame00000000");
                List<byte> byteList = new List<byte>();
                byteList.AddRange(msg);
                byteList.Add(182);
                byteList.AddRange(Encoding.ASCII.GetBytes(PeerID));
                msg = byteList.ToArray();

                lock (sendMsg)
                {
                    sendMsg.Enqueue(msg);
                }

            }
        }
    }
}

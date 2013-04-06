using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace UNOS_Sister
{
    class GameClient
    {
        Peer peer;
        Socket sender;
        Queue<byte[]> sendMsg;
        Queue<byte[]> receivedMsg;

        Thread senderThread;
        Thread processThread;

        bool connected; 

        public GameClient(Peer peer_)
        {
            peer = peer_;
            sendMsg = new Queue<byte[]>();
            receivedMsg = new Queue<byte[]>();

            senderThread = new Thread(sendMessage);
            processThread = new Thread(processMessage);
            connected = false;
        }

        //Connect ke game server
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
                    String replyMsg;
                    int byteRec = sender.Receive(bytes);
                    replyMsg = Encoding.ASCII.GetString(bytes, 0, byteRec);
                    Console.WriteLine("Reply Msg : " + replyMsg);

                    if (replyMsg != "OK")
                    {
                        //ok
                        connected = true;
                        senderThread.Start();
                        processThread.Start();
                    }
                    else
                    {
                        //fail
                        connected = false;
                    }
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

        public void quitRoom()
        {
            if (connected == true)
            {
                //<pstr><reserved><quit_code><peer_id>
                byte[] bytes = new byte[1024];
                byte[] msg = Encoding.ASCII.GetBytes("GunbondGame00000000");
                List<byte> byteList = new List<byte>();
                byteList.AddRange(msg);
                byteList.Add(235); //<quit_code>
                byteList.AddRange(Encoding.ASCII.GetBytes(peer.PeerID)); //<peer_id>
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

        public void sendMessage()
        {
            if (connected == true)
            {
                lock (sendMsg)
                {
                    while (sendMsg.Count > 0)
                    {
                        try
                        {
                            byte[] MsgToBeSent = sendMsg.Dequeue();
                            Console.WriteLine("Sending : " + Encoding.ASCII.GetString(MsgToBeSent));
                            //peerUI.textBox4.Text = Encoding.ASCII.GetString(MsgToBeSent);

                            int byteSent = sender.Send(MsgToBeSent); // kirim ke tracker

                            byte[] bytes = new byte[1024];
                            int byteRec = sender.Receive(bytes);
                            lock (receivedMsg)
                            {
                                receivedMsg.Enqueue(bytes);
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
                        //peerUI.textBox5.Text = Encoding.ASCII.GetString(msg);
                    }
                }
            }
        }

    }
}

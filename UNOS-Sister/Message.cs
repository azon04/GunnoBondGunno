using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UNOS_Sister
{
    
    class Message
    {
        private string msgDefault = "GunbondGame00000000";
        public string IP;
        public byte msgCode; // 1 Byte type message
        public string msgPeerID; // 4 byte ID peer, contoh P001
        private Room dummyCreateRoom; // dummy Create Room message
        public List<Room> Rooms;
        private int iterate;

        public static byte HANDSHAKE = 135;
        public static byte KEEP_ALIVE = 182;
        public static byte CREATE_ROOM = 255;
        public static byte LIST = 254;
        public static byte ROOM = 200;
        public static byte SUCCESS = 127;
        public static byte FAILED = 128;
        public static byte JOIN = 253;
        public static byte START = 252;
        public static byte QUIT = 235;
        public static byte CHECK = 100;
        public static byte NONE = 1;

        public Message() { // constructor
            msgCode = 1;
            msgPeerID = "9999";
            Rooms = new List<Room>();
        }

        public byte getMsgCode() {
            return msgCode;
        }

        public void printMsg() {
            Console.WriteLine(msgCode.ToString());
            if (msgCode == 135){
                if (msgPeerID != "9999"){
                    Console.WriteLine("msg handshake resp : " + msgDefault + msgCode.ToString() + msgPeerID); // handshake response
                } else {
                    Console.WriteLine("msg handshake : " + msgDefault + msgCode.ToString()); // handshake
                }
            } else  if (msgCode == 182){
                Console.WriteLine("msg keep alive : " + msgDefault + msgCode.ToString() + msgPeerID); // keep alive
            } else if (msgCode == 255) {
                //Console.WriteLine("msg create : " + msgDefault + msgCode.ToString() + msgPeerID + dummyCreateRoom.getMaxPlayer() + dummyCreateRoom.getRoomID());
            } else if (msgCode == 254) {
                Console.WriteLine("msg list request : " + msgDefault + msgCode.ToString() + msgPeerID);
            } else if (msgCode == 127) {
                Console.WriteLine("msg success : " + msgDefault + msgCode.ToString());
            } else if (msgCode == 128) {
                Console.WriteLine("msg failed : " + msgDefault + msgCode.ToString());
            } else if (msgCode == 253) {
                Console.WriteLine("msg join room : " + msgDefault + msgCode.ToString() + msgPeerID + dummyCreateRoom.getRoomID());
            } else if (msgCode == 252) {
                Console.WriteLine("msg start game : " + msgDefault + msgCode.ToString() + msgPeerID + dummyCreateRoom.getRoomID());
            } else if (msgCode == 235) {
                Console.WriteLine("msg quit room : " + msgDefault + msgCode.ToString() + msgPeerID);
            } else if (msgCode == 200) {
                Console.WriteLine("msg room list : " + msgDefault + msgCode.ToString() + iterate.ToString() + dummyCreateRoom.getPeerID() + dummyCreateRoom.getMaxPlayer() + dummyCreateRoom.getRoomName());
            }
        }

        /// <summary>
        /// Parsing byte[] to Message Type
        /// </summary>
        /// <param name="iMsg">byte[] message</param>
        public void parseMe(byte[] iMsg) {
            if (validMsg(iMsg))
            {
                msgCode = iMsg[19];
                Console.WriteLine(msgCode.ToString());

                if (msgCode != 127 || msgCode != 128 || msgCode != 200) { // bukan message sukses, gagal, atau room
                    if ((msgCode == 135 && iMsg.Length == 24) || msgCode == 182 || msgCode == 254 || msgCode == 235) // Handshake response, keep alive, list, quit
                    {
                        msgPeerID = Encoding.ASCII.GetString(SubBytes(iMsg, 20, 4));
                    } else if (msgCode == 253 || msgCode == 252) { // message join dan start
                        msgPeerID = Encoding.ASCII.GetString(SubBytes(iMsg, 20, 4));
                        dummyCreateRoom = new Room();
                        dummyCreateRoom.setRoomID(SubBytes(iMsg,24, 50));
                        Rooms.Clear();
                        Rooms.Add(dummyCreateRoom);
                    } else if (msgCode == 255) { // message create room
                        msgPeerID = Encoding.ASCII.GetString(SubBytes(iMsg, 20, 4));
                        dummyCreateRoom = new Room();
                        dummyCreateRoom.setPeerID(SubBytes(iMsg, 20, 4));
                        dummyCreateRoom.setMaxPlayer(iMsg[24]);
                        dummyCreateRoom.setRoomID(SubBytes(iMsg, 25, 50));
                        Rooms.Clear();
                        Rooms.Add(dummyCreateRoom);
                    } else if (msgCode == 200) { // message room
                        iterate = iMsg[20];
                        Rooms.Clear();
                        for (int i = 0; i < iterate ; i++) {
                            dummyCreateRoom = new Room();
                            dummyCreateRoom.setPeerID(SubBytes(iMsg, (21 + (i * 55)), 4));
                            dummyCreateRoom.setMaxPlayer(iMsg[(25 + (i * 55))]);
                            dummyCreateRoom.setRoomID(SubBytes(iMsg, (26 + (i * 55)), 50));
                            Console.WriteLine("Msg : " + Encoding.ASCII.GetString(SubBytes(iMsg, (26 + (i * 55)), 50)));
                            Rooms.Add(dummyCreateRoom);
                        }
                    }
                    else if (msgCode == 100)
                    {
                        msgPeerID = Encoding.ASCII.GetString(SubBytes(iMsg, 20, 4));
                        Console.WriteLine("message Length : " + iMsg.Count());
                        IP = Encoding.ASCII.GetString(SubBytes(iMsg,24,iMsg.Count()-24));
                        Console.WriteLine(IP);
                    }
                }
            } else {
                Console.WriteLine("Message Tidak Valid");
            }
        }

        public bool validMsg(byte[] iMsg) {
            byte[] defaultBytes = Encoding.ASCII.GetBytes(msgDefault);
            if (iMsg.Length < defaultBytes.Length) return false;
            for (int i = 0; i < defaultBytes.Length; i++)
            {
                if (defaultBytes[i] != iMsg[i]) return false;
            }
            return true;
        }

        private byte[] SubBytes(byte[] source, int startIndex, int count)
        {
            byte[] temp = new byte[count];
            for (int i = startIndex; i < startIndex + count; i++)
            {
                temp[i-startIndex] = source[i];
            }
            return temp;
        }

        /// <summary>
        /// Construct byte[] from Message type
        /// </summary>
        /// <returns>byte[] message</returns>
        public byte[] Construct()
        {
            Console.WriteLine(msgCode.ToString());
            List<byte> tempList = new List<byte>();

            tempList.AddRange(Encoding.ASCII.GetBytes(msgDefault));
            tempList.Add(msgCode);

            if ((msgCode == 135 && msgPeerID != "9999") || msgCode == 182 || msgCode == 254 || msgCode == 235)
            {
                tempList.AddRange(Encoding.ASCII.GetBytes(msgPeerID));
            }
            else if (msgCode == 253 || msgCode == 252)
            {
                tempList.AddRange(Encoding.ASCII.GetBytes(msgPeerID));
                tempList.AddRange(Encoding.ASCII.GetBytes(dummyCreateRoom.getRoomID()));
            }
            else if (msgCode == 255)
            {
                tempList.Add((byte)dummyCreateRoom.getMaxPlayer());
                tempList.AddRange(Encoding.ASCII.GetBytes(dummyCreateRoom.getRoomName()));
            }
            else if (msgCode == 200)
            {
                tempList.Add((byte)Rooms.Count());
                for (int i = 0; i < Rooms.Count(); i++)
                {
                    tempList.AddRange(Encoding.ASCII.GetBytes(Rooms[i].getPeerID()));
                    tempList.Add((byte)Rooms[i].getMaxPlayer());
                    tempList.AddRange(Encoding.ASCII.GetBytes(Rooms[i].getRoomID()));
                    int n = 50 - Encoding.ASCII.GetBytes(Rooms[i].getRoomID()).Count();
                    if (Encoding.ASCII.GetBytes(Rooms[i].getRoomID()).Count() < 50)
                    {
                        byte[] byteToAdd = new byte[n];
                        tempList.AddRange(byteToAdd);
                    }
                }

            }
            else if (msgCode == 100)
            {
                tempList.AddRange(Encoding.ASCII.GetBytes(msgPeerID));
                tempList.AddRange(Encoding.ASCII.GetBytes(IP));
            } 

            return tempList.ToArray();
        }

    }
}

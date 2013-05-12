using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GunBond.Connection
{
    public class Message
    {
        public string PeerID; //4 byte peer ID
        public byte msgCode; //1 byte tipe message
        static string tag = "GunBond";

        public List<string> list;

        //buat posisi player
        public Vector2 playerPos; //posisi player
        public float playerRot; //angle tembak player
        public int playerOrt; //orientasi player
        
        //buat keepalive
        public int HP; //healthpoint player
        
        //buat next player
        public string nextPlayer;

        //buat fire skenario1
        public Vector2 bulletV0;

        //buat fire skenario2
        public float elapsedTime;

        //buat fire skenario3
        public Vector2 bulletPos;

        //buat init
        public int playerTexture;
        public Vector2 playerPos0;

        public static byte POS = 0;
        public static byte KEEP_ALIVE = 1;
        public static byte FIRE = 2;
        public static byte NEXT_PLAYER = 3;
        public static byte INIT = 4;
        public static byte PEERTABLE = 7;

        public Message() //ctor
        {
            msgCode = 100;
            PeerID = "9999";
            list = new List<string>();
        }

        public void Parse(byte[] iMsg) {
            if (validMsg(iMsg)) {
                //format msg = <tag><peerid><kodemsg><data>
                Console.WriteLine(Encoding.ASCII.GetString(iMsg));
                if (iMsg.Count() < 11) return;
                PeerID = Encoding.ASCII.GetString(SubBytes(iMsg, 7, 4));
                msgCode = iMsg[11];
                Console.WriteLine(msgCode.ToString());

                if (msgCode == 0) {
                    //process POS msg
                    String msgData = Encoding.ASCII.GetString(SubBytes(iMsg, 13, iMsg.Length-13));
                    msgData = msgData.Trim();
                    Console.WriteLine("Datanya loh : " + msgData);
                    String[] info = msgData.Split('|');
                    String[] VectorPos = info[0].Split(',');
                    playerPos.X = float.Parse(VectorPos[0]);
                    playerPos.Y = float.Parse(VectorPos[1]);
                    playerRot = float.Parse(info[1]);
                    playerOrt = int.Parse(info[2]);

                } else if (msgCode == 1) {
                    //process KEEP_ALIVE msg
                    HP = iMsg[12];
                } else if (msgCode == 2) {
                    //process FIRE msg
                    String msgData = Encoding.ASCII.GetString(SubBytes(iMsg, 12, iMsg.Length - 12));
                    String[] info = msgData.Split('|');
                    String[] VectorPos = info[0].Split(',');
                    Console.WriteLine("ini errornya : " + msgData + ", sus dulu...");
                    bulletV0 = new Vector2(float.Parse(VectorPos[0]), float.Parse(VectorPos[1]));
                    VectorPos = info[1].Split(',');
                    playerPos.X = float.Parse(VectorPos[0]);
                    playerPos.Y = float.Parse(VectorPos[1]);
                    playerRot = float.Parse(info[2]);
                } else if (msgCode == 3) {
                    //process NEXT_PLAYER msg
                    nextPlayer = Encoding.ASCII.GetString(SubBytes(iMsg, 12, 4));
                } else if (msgCode == 4) {
                    //process INIT msg
                    playerTexture = iMsg[12];
                    String msgData = Encoding.ASCII.GetString(SubBytes(iMsg, 13, iMsg.Length - 13));
                    Console.WriteLine("Isi Message Init - " + msgData);
                    String[] VectorPos0 = msgData.Split(',');
                    Console.WriteLine("VectorPos - " + VectorPos0[0]);
                    Console.WriteLine("VectorPos - " + VectorPos0[1]);
                    playerPos0.X = float.Parse(VectorPos0[0]);
                    playerPos0.Y = float.Parse(VectorPos0[1]);
                }
                else if (msgCode == 7)
                {
                    String msgData = Encoding.ASCII.GetString(SubBytes(iMsg, 12, iMsg.Length - 12));
                    string[] ips = msgData.Split(',');
                    list = new List<string>(ips);
                }
                else
                {
                    Console.WriteLine("Message Tidak Valid");
                }

            }   
        }

        public static Message[] ParseStream(byte[] iMsg)
        {
            List<Message> temp = new List<Message>();
            string s = Encoding.ASCII.GetString(iMsg);
            int i = 0;
            while (i < s.Length && i >= 0)
            {
                int j = s.IndexOf(tag, i+1);
                if (j <= 0) j = s.Length;
                Message newMessage = new Message();
                System.Diagnostics.Debug.WriteLine(s.Substring(i, j - i));
                newMessage.Parse(Encoding.ASCII.GetBytes(s.Substring(i, j - i)));
                temp.Add(newMessage);
                i = j;
            }
            return temp.ToArray();
        }
       
        public string GetString()
        {
            if (msgCode == 0) // position
            {
                return ("Kode : " + msgCode.ToString() + ", PeerID : " + PeerID + ", posisi : x = " + playerPos.X + ", y = " + playerPos.Y + ", orientasi : " + playerOrt + "angle : " + playerRot);
            } 
            else if (msgCode == 1) // update HP & keep alive
            {
                return ("Kode : " + msgCode.ToString() + ", PeerID : " + PeerID + ", HP : " + HP);
            }
            else if (msgCode == 2) // position + bullet position awal
            {
                return ("Kode : " + msgCode.ToString() + ", PeerID : " + PeerID + ", posisi : x = " + playerPos.X + ", y = " + playerPos.Y + ", orientasi : " + playerOrt + "angle : " + playerRot + "posisi awal bullet : x = " + bulletV0.X + ", y = " + bulletV0.Y);
            }
            else if(msgCode == 3) // message next player
            {
                return ("Kode : " + msgCode.ToString() + ", PeerID yang ngirim : " + PeerID + ", isi message : next player");
            }
            else if (msgCode == 4) // message INIT
            {
                return ("Kode : " + msgCode.ToString() + ", PeerID : " + PeerID + ", isi message : INIT");
            }
            return "Kode : " + msgCode.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj is Message)
            {
                Message msg = obj as Message;
                return Encoding.ASCII.GetString(this.Construct()).Equals(Encoding.ASCII.GetString(msg.Construct()));
            }
            return base.Equals(obj);
        }

        public bool validMsg(byte[] iMsg) {
            byte[] defaultBytes = Encoding.ASCII.GetBytes(tag);
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

        public byte[] Construct() {
            List<byte> tempList = new List<byte>();

            //format msg = <tag><peerid><kodemsg><data>
            tempList.AddRange(Encoding.ASCII.GetBytes(tag));
            tempList.AddRange(Encoding.ASCII.GetBytes(PeerID));
            tempList.Add(msgCode);

            if (msgCode == 0)
            {
                tempList.Add(POS);
                tempList.AddRange(Encoding.ASCII.GetBytes("" + playerPos.X.ToString("0.00") + "," + playerPos.Y.ToString("0.00") + "|"));
                tempList.AddRange(Encoding.ASCII.GetBytes(playerRot.ToString("0.000") + "|"));
                tempList.AddRange(Encoding.ASCII.GetBytes(playerOrt.ToString()));
            }
            else if (msgCode == 1)
            {
                tempList.Add(KEEP_ALIVE);
                tempList.AddRange(BitConverter.GetBytes(HP));
                //tempList.AddRange(Encoding.ASCII.GetBytes(HP.ToString()));
            }
            else if (msgCode == 2)
            {
                tempList.Add(FIRE);

                
                //skenario1
                tempList.AddRange(Encoding.ASCII.GetBytes("" + bulletV0.X + "," + bulletV0.Y + "|")); //ambil jadi string
                tempList.AddRange(Encoding.ASCII.GetBytes("" + playerPos.X + "," + playerPos.Y + "|"));
                tempList.AddRange(Encoding.ASCII.GetBytes(playerRot.ToString("0.000")));
                 


                /*
                //skenario2
                tempList.AddRange(Encoding.ASCII.GetBytes(elapsedTime.ToString("0.000")));
                */
                /*
                //skenario3
                tempList.AddRange(Encoding.ASCII.GetBytes(bulletPos)); //
                 */
            }
            else if (msgCode == 3)
            {
                tempList.Add(NEXT_PLAYER);
                tempList.AddRange(Encoding.ASCII.GetBytes(nextPlayer));
            }
            else if (msgCode == 4)
            {
                tempList.Add((byte)playerTexture);
                tempList.AddRange(Encoding.ASCII.GetBytes("" + playerPos0.X.ToString("0.000") + "," + playerPos0.Y.ToString("0.000")));
            }
            else if (msgCode == 7)
            {
                String s = "";
                foreach (string sid in list)
                {
                    s += sid + ",";
                }
                s = s.Substring(0, s.Length - 1);
                tempList.AddRange(Encoding.ASCII.GetBytes(s));
            }

            return tempList.ToArray();
        }
    }
}

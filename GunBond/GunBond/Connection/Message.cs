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
        string tag = "GunBond";
        


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
        
        public Message() //ctor
        {
            msgCode = 1;
            PeerID = "9999";
        }

        public void Parse(byte[] iMsg) {
            if (validMsg(iMsg)) {
                //format msg = <tag><peerid><kodemsg><data>
                PeerID = Encoding.ASCII.GetString(SubBytes(iMsg, 7, 4));
                msgCode = iMsg[11];
                Console.WriteLine(msgCode.ToString());

                if (msgCode == 0) {
                    //process POS msg
                    String msgData = Encoding.ASCII.GetString(SubBytes(iMsg, 12, iMsg.Length-12));
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
                    bulletV0 = new Vector2(float.Parse(VectorPos[0]), float.Parse(VectorPos[1]));
                    VectorPos = info[1].Split(',');
                    playerPos.X = float.Parse(VectorPos[0]);
                    playerPos.Y = float.Parse(VectorPos[1]);
                    playerRot = float.Parse(info[1]);
                } else if (msgCode == 3) {
                    //process NEXT_PLAYER msg
                    nextPlayer = Encoding.ASCII.GetString(SubBytes(iMsg, 12, 4));
                } else if (msgCode == 4) {
                    //process INIT msg
                    playerTexture = iMsg[12];
                    String msgData = Encoding.ASCII.GetString(SubBytes(iMsg, 13, iMsg.Length - 13));
                    System.Diagnostics.Debug.WriteLine(msgData);
                    Console.WriteLine("Sesuatu -" + msgData);
                    String[] VectorPos0 = msgData.Split(',');
                    Console.WriteLine(msgData);
                    playerPos0.X = float.Parse(VectorPos0[0]);
                    playerPos0.Y = float.Parse(VectorPos0[1]);

                } else {
                    Console.WriteLine("Message Tidak Valid");
                }

            }   
        }

        public string ToString()
        {
            return null;
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
                tempList.AddRange(Encoding.ASCII.GetBytes("" + playerPos.X + "," + playerPos.Y + "|"));
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
                tempList.AddRange(Encoding.ASCII.GetBytes("" + playerPos0.X + "," + playerPos0.Y));
            }

            return tempList.ToArray();
        }
    }
}

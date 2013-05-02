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

        public static byte POS = 0;
        public static byte KEEP_ALIVE = 1;
        public static byte FIRE = 2;
        public static byte NEXT_PLAYER = 3;
        
        public Message() //ctor
        {
            msgCode = 1;
            PeerID = "9999";
        }

        public void Parse(byte[] iMsg) {
            if (validMsg(iMsg)) {
                msgCode = iMsg[11];
                Console.WriteLine(msgCode.ToString());

                if (msgCode == 0) {
                    //process POS msg

                } else if (msgCode == 1) {
                    //process KEEP_ALIVE msg
                } else if (msgCode == 2) {
                    //process FIRE msg
                } else if (msgCode == 3) {
                    //process NEXT_PLAYER msg
                } else {
                    Console.WriteLine("Message Tidak Valid");
                }

            }   
        }

        public string ToString()
        {
            return null;
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

            if (msgCode == 0) {
                tempList.Add(POS);
                tempList.AddRange(Encoding.ASCII.GetBytes("" + playerPos.X + "," + playerPos.Y + "|"));
                tempList.AddRange(Encoding.ASCII.GetBytes(playerRot.ToString() + "|"));
                tempList.AddRange(Encoding.ASCII.GetBytes(playerOrt));
            } else if (msgCode == 1) {
                tempList.Add(KEEP_ALIVE);
                tempList.AddRange(Encoding.ASCII.GetBytes(HP));
            } else if (msgCode == 2) {
                tempList.Add(FIRE);

                /*
                //skenario1
                tempList.AddRange(Encoding.ASCII.GetBytes(bulletV0)); //ambil jadi string
                tempList.AddRange(Encoding.ASCII.GetBytes(playerRot));
                 */

                
                //skenario2
                tempList.AddRange(Encoding.ASCII.GetBytes(elapsedTime));
                
                /*
                //skenario3
                tempList.AddRange(Encoding.ASCII.GetBytes(bulletPos)); //
                 */
            } else if(msgCode == 3) {
                tempList.Add(Encoding.ASCII.GetBytes(NEXT_PLAYER));
                tempList.AddRange(Encoding.ASCII.GetBytes(nextPlayer));
            }

            return tempList.ToArray();
        }
    }
}

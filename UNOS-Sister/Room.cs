using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UNOS_Sister
{
    class Room
    {
        private byte[] RoomID;
        private byte[] PeerID;
        private string RoomName;
        private int max_player_num;
        List<string> Peers; // Pemain2 yang ada dalam Room

        public Room() {
            RoomID = Encoding.ASCII.GetBytes("R0001");
            PeerID = Encoding.ASCII.GetBytes("P0001");
            RoomName = "qwertyuiopqwertyuiopqwertyuiopqwertyuiopqwertyuiop";
            max_player_num = 2;
            Peers = new List<string>();
        }
        
        // setter
        public void setRoomID(byte[] s) {
            RoomID = s;
        }
        public void setPeerID(byte[] s) {
            PeerID = s;
        }

        public void setRoomName(byte[] s)
        {
            RoomName = Encoding.ASCII.GetString(s);
        }

        public void setMaxPlayer(byte i) {
            max_player_num = i;
        }
        
        // getter
        public string getRoomID() {
            return Encoding.ASCII.GetString(RoomID); 
        }
        public string getPeerID() {
            return Encoding.ASCII.GetString(PeerID); 
        }
        public string getRoomName() {
            return RoomName; 
        }
        public int getMaxPlayer() {
            return max_player_num; 
        }

    }
}

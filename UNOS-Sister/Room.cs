﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UNOS_Sister
{
    public class Room
    {
        private byte[] RoomID;
        private byte[] PeerID; // ID pemilik
        private string RoomName;
        private int max_player_num;
        public List<string> PeerIDs; // Pemain2 yang ada dalam Room

        public Room() {
            RoomID = Encoding.ASCII.GetBytes("R0001");
            PeerID = Encoding.ASCII.GetBytes("P001");
            RoomName = "qwertyuiopqwertyuiopqwertyuiopqwertyuiopqwertyuiop";
            max_player_num = 2;
            PeerIDs = new List<string>();
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
            if (PeerIDs.Count > 0)
            {
                return PeerIDs[0];
            }
            else
            {
                return null;
            }
        }
        public string getRoomName() {
            return RoomName; 
        }
        public int getMaxPlayer() {
            return max_player_num; 
        }

    }
}

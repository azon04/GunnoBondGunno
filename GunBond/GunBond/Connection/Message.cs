using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GunBond.Connection
{
    public class Message
    {
        public string PeerID;
        public byte messageCode;
        string tag = "GunBond";

        public Message()
        {
            
        }

        public byte[] GetBytes()
        {
            return null;
        }

        public string ToString()
        {
            return null;
        }
    }
}

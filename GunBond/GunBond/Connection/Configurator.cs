using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GunBond.Connection
{
    public class Configurator
    {
        public enum State { start, starting, done };

        public State Status = State.start; 
        List<string> IPTable;
        string IP;
        string tag = "GundBond";

        public Configurator(string IPAddress)
        {
            IPTable = new List<string>();
            IP = IPAddress;
        }

        public Configurator(string IPAddress, List<string> Table)
        {
            Console.WriteLine("Create Configurator");
            IPTable = Table;
            IP = IPAddress;
        }

        public List<string> IPToConnect()
        {
            List<string> temp = new List<string>();
            int idx = IPTable.IndexOf(IP);
            if (IPTable.Count == 2)
            {
                temp.Add(IPTable[(idx + 1) % 2]);
            }
            else if (IPTable.Count == 4)
            {
                temp.Add(IPTable[(idx + 1) % 4]);
            }
            else if (IPTable.Count == 6)
            {
                if (idx < 3)
                {
                    temp.Add(IPTable[(idx + 1) % 3]);
                    temp.Add(IPTable[idx + 3]);
                }
                else
                {
                    if (idx == 5)
                        temp.Add(IPTable[3]);
                    else
                        temp.Add(IPTable[(idx + 1)]);
                }
            }
            else if (IPTable.Count == 8)
            {
                if (idx < 4)
                {
                    temp.Add(IPTable[(idx + 1) % 4]);
                    temp.Add(IPTable[idx + 4]);
                }
                else
                {
                    if (idx == 7)
                        temp.Add(IPTable[4]);
                    else
                        temp.Add(IPTable[(idx + 1)]);
                }
            }
            return temp;
        }

        public byte[] ConstructMessageConfig()
        {
            List<byte> tempList = new List<byte>();
            tempList.AddRange(Encoding.ASCII.GetBytes(tag));
            tempList.Add(5);
            string s = "";
            foreach (string ip in IPTable)
            {
                s += "," + ip;
            }
            tempList.AddRange(Encoding.ASCII.GetBytes(s));
            return tempList.ToArray();
        }

        public void Parse(byte[] bytes) {
            string s = Encoding.ASCII.GetString(bytes);
            string[] sres = s.Split(',');
            IPTable.Clear();
            for (int i = 1; i < sres.Count(); i++)
            {
                IPTable.Add(sres[i]);
            }
        }
    }
}

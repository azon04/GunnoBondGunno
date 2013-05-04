using System;
using UNOS_Sister;
using System.Windows.Forms;
using GunBond.Connection;
using System.Collections.Generic;

namespace GunBond
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            PeerUI peer = new PeerUI();
            do
            {
                peer.ShowDialog();

                if (peer.peer.playStatus != 0)
                {
                    GameConnection gameConnection = null;
                    peer.peer.DisconnectFromServer();
                    if (peer.peer.playStatus == 2)
                    {   
                        gameConnection = new GameConnection(peer.peer.PeerID);
                        gameConnection.PeerIDs = peer.peer.peerList;
                        System.Diagnostics.Debug.WriteLine("Crator Peer : " + peer.peer.IPTable[peer.peer.PeerID]);
                        List<string> ipAddress = new List<string>();
                        foreach (string ip in peer.peer.IPTable.Values)
                        {
                            ipAddress.Add(ip);
                        }
                        gameConnection.StartConfig(ipAddress);
                        gameConnection.WaitConfigComplete();
                    }
                    else if (peer.peer.playStatus == 1)
                    {
                        gameConnection = new GameConnection(peer.peer.PeerID);
                        System.Diagnostics.Debug.WriteLine("Peer : " + peer.peer.IPTable.Count);
                        gameConnection.WaitConfig();
                    }
                    using (Game1 game = new Game1(gameConnection))
                    {
                        game.IsCreator = (peer.peer.playStatus == 2);
                        game.Run();
                    }
                    gameConnection.Close();
                }
            } while (peer.peer.playStatus != 0);
        }
    }
#endif
}


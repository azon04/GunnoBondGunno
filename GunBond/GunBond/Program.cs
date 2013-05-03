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
            peer.ShowDialog();

            if (peer.peer.playStatus != 0)
            {
                GameConnection gameConnection = null;
                if (peer.peer.playStatus == 2)
                {
                    gameConnection = new GameConnection(peer.peer.PeerID);
                    System.Diagnostics.Debug.WriteLine("Crator Peer : " + peer.peer.IPTable.Count);
                    gameConnection.StartConfig(new List<string>(peer.peer.IPTable.Values));
                }
                else if (peer.peer.playStatus == 1)
                {
                    gameConnection = new GameConnection(peer.peer.PeerID);
                    System.Diagnostics.Debug.WriteLine("Peer : " + peer.peer.IPTable.Count);
                }
                using (Game1 game = new Game1(gameConnection))
                {
                    game.Run();
                }
            }
        }
    }
#endif
}


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

            if (true)
            {
                GameConnection gameConnection = new GameConnection();
                System.Diagnostics.Debug.WriteLine(peer.peer.IPTable.Count);
                gameConnection.StartConfig(new List<string>(peer.peer.IPTable.Values));
                using (Game1 game = new Game1())
                {
                    game.Run();
                }
            }
        }
    }
#endif
}


using System;
using UNOS_Sister;
using System.Windows.Forms;

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
            
            using (Game1 game = new Game1())
            {
                game.Run();
            }
        }
    }
#endif
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UNOS_Sister
{
    class Program
    {
        [STAThread]
        public static int Main(String[] args)
        {            
            
            if (args.Length > 0 && args[0] == "-s")
            {
                TrackerUI trackerui = new TrackerUI();
                Application.Run(trackerui);
            } 
            else
            { 
                PeerUI peerui = new PeerUI();
                Application.Run(peerui);
            }
            return 0;
        }
    }
}

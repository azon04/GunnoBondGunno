using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UNOS_Sister
{
    public partial class TrackerUI : Form
    {
        Tracker tracker = null;

        public TrackerUI()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (tracker == null)
            {
                tracker = new Tracker();
                button2.Enabled = false;
                IPText.Text = tracker.IP;

                MaxPeerNumber.Value = tracker.max_peer;
                MaxRoomNumber.Value = tracker.max_room;
                LogCheck.Checked = tracker.log;
                tracker.LogText = LogText;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (tracker != null)
            {
                tracker.Close();
                tracker = null;
                button2.Enabled = true;
            }
        }

        private void MaxPeerNumber_ValueChanged(object sender, EventArgs e)
        {
            if (tracker != null)
            {
                tracker.max_peer = (int)MaxPeerNumber.Value;
            }
        }

        private void MaxRoomNumber_ValueChanged(object sender, EventArgs e)
        {
            if (tracker != null)
            {
                tracker.max_room = (int)MaxRoomNumber.Value;
            }
        }

        private void LogCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (tracker != null)
            {
                tracker.log = LogCheck.Checked;
            }
        }
    }
}

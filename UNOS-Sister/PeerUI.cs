using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Threading;

namespace UNOS_Sister
{
    public partial class PeerUI : Form
    {
        Peer peer; 
        public PeerUI()
        {
            peer = new Peer(this);
            InitializeComponent();
        }

        //Handshake
        private void button1_Click(object sender, EventArgs e)
        {
            peer.ConnectToServer(textBox2.Text);
        }

        //Create Room(room_id, max_player_num)
        private void button2_Click(object sender, EventArgs e)
        {
            peer.createRoom(textBox1.Text, comboBox1.Text);
        }

        //Get Room List
        private void button3_Click(object sender, EventArgs e)
        {
            peer.GetRoomList();
        }

        //Join Room(room_id)
        private void button5_Click(object sender, EventArgs e)
        {
            peer.joinRoom(textBox3.Text);
        }

        //Quit Room
        private void button6_Click(object sender, EventArgs e)
        {
            peer.quitRoom();
        }

        //Start Game(room_id)
        private void button7_Click(object sender, EventArgs e)
        {
            peer.startGame(textBox3.Text);
        }

        //Disconnect
        private void button4_Click(object sender, EventArgs e)
        {
            peer.DisconnectFromServer();
        }

        /*
        [STAThread]
        
        public static int Main(String[] args)
        {
            PeerUI peerUI = new PeerUI();
            Application.Run(peerUI);
            return 0;
        } */

        private void textBox2_TextChanged(object sender, EventArgs e) {}
        private void textBox1_TextChanged(object sender, EventArgs e) {}
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {}
        private void textBox3_TextChanged(object sender, EventArgs e) {}
        private void textBox4_TextChanged(object sender, EventArgs e) {} //Status Sending
        private void textBox5_TextChanged(object sender, EventArgs e) { } //Status Processing
        private void richTextBox1_TextChanged(object sender, EventArgs e) {} //Room List 
    }
}

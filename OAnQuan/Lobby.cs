using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OAnQuan
{
    public partial class Lobby : Form
    {
        public Lobby lobby;
        public List<Label> PlayerName = new List<Label>();
        public int playerCount = 0;
        public Lobby()
        {
            InitializeComponent();

            CheckForIllegalCrossThreadCalls = false;

            lobby = this;

            btnStart.Hide();

            PlayerName.Add(label1);
            PlayerName.Add(label2);
        }

        public void DisplayPlayer(string name)
        {
            playerCount++;
            switch(playerCount)
            {
                case 1:
                    label1.Text = name;
                    break;
                case 2:
                    label2.Text = name;
                    break;
                default:
                    break;
            }
        }

        public void ShowbtnStart()
        {
            btnStart.Show();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            ClientSocket.dataHeader = "START";
            ClientSocket.SendData("");
        }

        public void Changetextbox(string text)
        {
            richTextBox1.Text += text + "\n";
        }
    }
}

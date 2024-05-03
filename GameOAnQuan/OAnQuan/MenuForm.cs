using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Threading;
using WindowsFormsApp1;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Net;

namespace OAnQuan
{
    public partial class MenuForm : Form
    {
        public static Lobby lobby;

        public MenuForm()
        {
            InitializeComponent();

        }

        private void btnJoin_Click(object sender, EventArgs e)
        {
            string name = tbName.Text;
            string ip = tbIP.Text;

            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), 9999);
            ClientSocket.dataHeader = "CONNECT";
            ClientSocket.Connect(iPEndPoint);

            lobby = new Lobby();
            ClientSocket.SendData(name);
            Player2.name = name;
            lobby.FormClosed += new FormClosedEventHandler(lobby_FormClosed);
            this.Hide();
            lobby.Show();
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            string name = tbName.Text;
            string ip = tbIP.Text;

            IPEndPoint serverEP = new IPEndPoint(IPAddress.Parse(ip), 9999);
            ClientSocket.dataHeader = "CREATE";
            ClientSocket.Connect(serverEP);

            lobby = new Lobby();
            ClientSocket.SendData(name);
            Player1.name = name;
            lobby.FormClosed += new FormClosedEventHandler(lobby_FormClosed);
            this.Hide();
            lobby.Show();
        }

        public void lobby_FormClosed(object sender, EventArgs e)
        {
            ClientSocket.dataHeader = "DISCONNECT";
            ClientSocket.SendData(Player1.name);
            ClientSocket.clientSocket.Shutdown(SocketShutdown.Both);
            ClientSocket.clientSocket.Close();
            this.Show();
        }

        private void Menu_Shown(object sender, EventArgs e)
        {
            tbIP.Text = GetIPv4(NetworkInterfaceType.Wireless80211);

            if (string.IsNullOrEmpty(tbIP.Text))
            {
                tbIP.Text = GetIPv4(NetworkInterfaceType.Ethernet);
            }
        }

        private string GetIPv4(NetworkInterfaceType type)
        {
            string output = "";
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.NetworkInterfaceType == type && item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            output = ip.Address.ToString();
                        }
                    }
                }
            }
            return output;
        }
    }
}
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
        // Biến tĩnh lưu thông tin phòng chơi
        public static Lobby lobby;

        public MenuForm()
        {
            InitializeComponent();

        }
        // Button Join
        private void btnJoin_Click(object sender, EventArgs e)
        {
            // Lấy tên và IP từ giao diện
            string name = tbName.Text;
            string ip = tbIP.Text;
            // Tạo 1 IPEndPoint với ip vừa nhập và cổng 9999
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), 9999);
            // lập header dữ liệu giao tiếp với máy chủ
            // Header dữ liệu là CONNECT
            ClientSocket.dataHeader = "CONNECT";
            // gửi yêu cầu kết nối đến máy chủ thông qua lớp ClientSocket, sau đó hiển thị form Lobby.
            ClientSocket.Connect(iPEndPoint);
            lobby = new Lobby();
            ClientSocket.SendData(name);
            Player2.name = name;
            lobby.FormClosed += new FormClosedEventHandler(lobby_FormClosed);
            this.Hide();
            lobby.Show();
        }

        // Tạo phòng
        private void btnCreate_Click(object sender, EventArgs e)
        {
            // Lấy tên và IP từ giao diện người dùng
            string name = tbName.Text;
            string ip = tbIP.Text;
            // Tạo 1 đối tượng IPEndPoint với ip vừa nhập và cổng 9999
            IPEndPoint serverEP = new IPEndPoint(IPAddress.Parse(ip), 9999);
            // Header giao tiếp với máy chủ với header dữ liệu là CREATE
            ClientSocket.dataHeader = "CREATE";
            // gửi yêu cầu kết nối đến máy chủ thông qua lớp ClientSocket, sau đó hiển thị form Lobby.
            ClientSocket.Connect(serverEP);
            lobby = new Lobby();
            ClientSocket.SendData(name);// ClientSocket sẽ gửi thông tin tên người chơi đến máy chủ
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

       // Gọi phương thức GetIPv4 với tham số NetworkInterfaceType.Wireless80211, đại diện cho giao diện mạng không dây (Wi-Fi).
       // Đặt kết quả trả về(địa chỉ IP) vào thuộc tính Text của TextBox tbIP.
        private void Menu_Shown(object sender, EventArgs e)
        {
            tbIP.Text = GetIPv4(NetworkInterfaceType.Wireless80211);
            // Kiểm tra xem IP đã được lấy chưa
            if(string.IsNullOrEmpty(tbIP.Text))
            {
                // Nếu chưa, chuyển Wifi thành Ethernet (mạng dây) và đặt kết quả trả về vào Textbox
                tbIP.Text = GetIPv4(NetworkInterfaceType.Ethernet);
            }
        }
        // GetIPv4 dùng để nhận địa chỉ IP của máy tính (dây hoặc ko dây)
        // Nếu có, trả về chuỗi IP, ngược lại chuỗi rỗng
        private string GetIPv4(NetworkInterfaceType type)
        {
            string output = "";
            // Duyệt qua các mạng có sẵn trong hệ thống
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                // Chỗ này kiểm tra các giao diện mạng (type là Wifi or Ethernet), có đang hoạt động hay ko?
                if (item.NetworkInterfaceType == type && item.OperationalStatus == OperationalStatus.Up)
                {
                    // Duyệt qua tất cả IP của giao diện mạng
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        // Nếu IP hiện tại = IPv4 thì gán nó vào output
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            output = ip.Address.ToString();
                        }
                    }
                }
            }
            return output;
        }

        private void btnHowToPlay_Click(object sender, EventArgs e)
        {
            HowToPlay a = new HowToPlay();
            a.Show();
        }
    }
}

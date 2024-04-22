using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Numerics;

namespace Server
{
    class Program
    {
        // Đối tượng Socket lắng nghe kết nối từ Client
        private static Socket serverSocket;
        // Đối tượng Socket đại diện cho các Client khi kết nối
        private static Socket client;
        // Luồng xử lý dữ liệu từ Client
        private static Thread clientThread;
        // Một danh sách đại diện cho các người chơi đã kết nối
        private static List<Player> connectedPlayers = new List<Player>();

        static void Main(string[] args)
        {
            // Tạo một máy chủ lắng nghe các kết nối đến cổng 9999
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // Gắn Socket với địa chỉ và cổng
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, 9999));
            // Bắt đầu lắng nghe các kết nối, ở đây cụ thể là 2
            serverSocket.Listen(2);
            Console.WriteLine("Wating for player");

            // Đây là một vòng lặp vô hạn đợi các kết nối từ Client
            while (true)
            {
                // Trả về một đối tượng Socket (client) đại diện cho kết nối mới được chấp nhận
                client = serverSocket.Accept();
                // In ra thông điệp bao gồm IP và cổng 
                Console.WriteLine("Player connected from " + client.RemoteEndPoint.ToString());
                // Một luồng mới được tạo để xử lý Client vừa kết nối, đồng thời truyền một hàm readClientSocket để xử lý dữ liệu từ Client này
                clientThread = new Thread(() => readClientSocket(client));
                // Nếu luồng chính của chương trình kết thúc, luồng này cũng sẽ kết thúc mà không cần đợi đến khi hoàn thành nhiệm vụ
                clientThread.IsBackground = true;
                // Luồng mới được tạo, bắt đầu thực thi hàm
                clientThread.Start();
            }
        }
        // Hàm readClientSocket để xử lý dữ liệu từ Client
        public static void readClientSocket(Socket client)
        {
            // Một player được tạo đại diện cho Client vừa kết nối
            Player player = new Player();
            // Socket của Client được gán cho thuộc tính playerSocket của Player
            player.playerSocket = client;
            // Thêm player vào danh sách connectedPlayer
            connectedPlayers.Add(player);
            // Khởi tạo một mảng byte
            byte[] bytes = new byte[1024];
            // Vòng lặp đọc dữ liệu từ player, nó sẽ chạy cho đến khi Player bị đóng hoặc bị ngắt
            while (player.playerSocket.Connected)
            {
                // Kiểm tra có dữ liệu mới từ Client hay không bằng cách dùng Available
                if (player.playerSocket.Available > 0)
                {
                    // Khởi tạo biến msg
                    string msg = "";

                    while (player.playerSocket.Available > 0)
                    {
                        // Nếu có một dữ liệu mới, máy chủ sẽ đọc dữ liệu đó và ghi vào một mảng bytes
                        int bytesRec = player.playerSocket.Receive(bytes);
                        // Dữ liệu đó sẽ được mã hóa thành chuỗi ASCII, lưu vào biến msg
                        msg += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    }
                    // Sau khi nhận đc thông điệp từ Player, nó sẽ in ra thông điệp kèm theo IP và Port
                    Console.WriteLine(player.playerSocket.RemoteEndPoint.ToString() + " : " + msg);
                    // Gọi hàm AnalyzeMessage để xử lý và phân tích thông điệp từ Client
                    AnalyzeMessage(msg, player);
                }
            }
        }

        public static void AnalyzeMessage(string msg, Player player)
        {
            // msg là thông điệp được gưi từ Client đến máy chủ
            string[] PayLoad = msg.Split('|');
            // Loại thông điệp của Client
            switch (PayLoad[0])
            {
                // Payload[0] là CREATE, tên người chơi được lấy từ Payload1, thông điệp có dạng CREATED|PlayerName sẽ được gửi lại cho player để xác nhận rằng đã tạo người chơi thành công
                case "CREATE":
                    {
                        player.name = PayLoad[1];

                        byte[] data = Encoding.UTF8.GetBytes("CREATED|" + player.name);

                        player.playerSocket.Send(data);
                    }
                    break;
                // Payload[0] là CONNECT, tên người chơi được lấy từ Payload[1]
                case "CONNECT":
                    {
                        player.name = PayLoad[1];
                        // Gửi thông điệp của người chơi đã kết nối cho người chơi mới
                        foreach (var p in connectedPlayers)
                        {
                            byte[] data = Encoding.UTF8.GetBytes("CONNECTED|" + p.name);
                            player.playerSocket.Send(data);
                            Thread.Sleep(100);
                        }

                        // Gửi thông điệp của người chơi mới cho người chơi đã kết nối
                        foreach (var p in connectedPlayers)
                        {
                            if (player.playerSocket != p.playerSocket)
                            {
                                byte[] data = Encoding.UTF8.GetBytes("CONNECTED|" + player.name);
                                p.playerSocket.Send(data);
                                Thread.Sleep(100);
                            }
                        }
                    }
                    break;
                case "DISCONNECT":
                    {
                        //Tạo ra 1 bản sao của danh sách các connectedPlayers bằng cách sử dụng phương thức ToList()
                        foreach (var p in connectedPlayers.ToList())
                        {
                            // Nếu tên của Client trùng với tên của Client trong thông điệp Disconnect ko
                            if (p.name == PayLoad[1])
                            {
                                // Nếu có, đóng kết nối từ Client và đến Client
                                p.playerSocket.Shutdown(SocketShutdown.Both);
                                // Đóng kết nối hoàn toàn bằng Close
                                p.playerSocket.Close();
                                // Xóa nó khỏi danh sách các connectedPlayers
                                connectedPlayers.Remove(p);
                            }
                        }
                    }
                    break;
                case "START":
                    {
                        foreach (var p in connectedPlayers)
                        {
                            // Gửi thông điệp INIT đến mỗi Client được kết nối
                            // Với mỗi Client trong danh sách ConnectedPlayer, một chuỗi message được tạo với nội dung INIT| để bắt đầu (Khởi tạo)
                            // Vòng lặp duyệt qua từng Client để thêm vào chuối message 
                            // Nó được mã hóa bằng Encoding UTF8
                            // Mảng bytes này sẽ được gởi đến các Client thông qua Socket mỗi CLient
                            // Cuối cùng, một dòng thông báo Sendback: + message được đưa ra màn hình Console
                            // Lưu ý, dừng 100ms để đảm bảo quá trình gửi thông điệp
                            string message = "INIT|";
                            foreach (var p2 in connectedPlayers)
                            {
                                message += p2.name + "|";
                            }
                            byte[] data = Encoding.UTF8.GetBytes(message);
                            p.playerSocket.Send(data);
                            Console.WriteLine("Sendback: " + message);
                            Thread.Sleep(100);
                        }
                    }
                    break;

                // Thông điệp là RIGHT, LEFT là dịch phải, trái, nó chỉ dịch chuyển khi tên của Client không cùng với tên được chỉ định trong thông điệp
                // Thông điệp sẽ có dạng RIGHT/LEFT|Giá trị|Vị trí|player1turn
                // Mã hóa thông điệp dạng UTF8
                // Cuối cùng, gửi thông điệp đó đến Client tương ứng
                case "RIGHT":
                    {
                        string name = PayLoad[4];
                        foreach (var p in connectedPlayers)
                        {
                            if (p.name != name)
                            {
                                string position = PayLoad[2];
                                string value = PayLoad[1];
                                string player1turn = PayLoad[3];
                                string message = "RIGHT|" + value + "|" + position + "|" + player1turn;
                                byte[] data = Encoding.UTF8.GetBytes(message);
                                p.playerSocket.Send(data);
                            }
                        }
                    }
                    break;
                case "LEFT":
                    {
                        string name = PayLoad[4];
                        foreach (var p in connectedPlayers)
                        {
                            if (p.name != name)
                            {
                                string position = PayLoad[2];
                                string value = PayLoad[1];
                                //Console.WriteLine("Value: " + PayLoad[3]);
                                string player1turn = PayLoad[3];
                                string message = "LEFT|" + value + "|" + position + "|" + player1turn;
                                byte[] data = Encoding.UTF8.GetBytes(message);
                                p.playerSocket.Send(data);
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
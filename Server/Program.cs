using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace Server
{
    class Program
    {
        // Socket lắng nghe kết nối từ Client
        private static Socket serverSocket;
        // Đại diện cho mỗi Client kết nối
        private static Socket client;
        // Thread xử lý dữ liệu từ Client
        private static Thread clientThread;
        // Danh sách các Client đã kết nối
        private static List<Player> connectedPlayers = new List<Player>();
        // Danh sách các Phòng
        private static List<Lobby> lobbies = new List<Lobby>();
        // Chuối lưu IP của máy chủ
        public static string host;
        // Main dùng để tạo 1 đối tượng serverSocket 
        static void Main(string[] args)
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // Tạo xong, bind vào ("Mọi địa chỉ IP mà máy chủ có","9999")
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, 9999));
            // Tiến hành lắng nghe với backlog=2
            serverSocket.Listen(2);
            Console.WriteLine("Wating for player");

            // Vòng lặp vô hạn để chờ đợi kết nối từ Client và khi có kết nối mới, tạo 1 luồng xử lý kết nối đó
            while (true)
            {
                // Kết nối được chấp nhận bởi server (Accept()), mội client mới được tạo ra
                client = serverSocket.Accept();
                // Thông điệp cùng IP:Port
                Console.WriteLine("Player connected from " + client.RemoteEndPoint.ToString());
                // Một luồng mới được tạo ra để xử lý các kết nối từ Client, điều này giúp xử lý nhiều client cho server
                clientThread = new Thread(() => readClientSocket(client));
                clientThread.IsBackground = true;
                clientThread.Start();
            }
        }

        public static void readClientSocket(Socket client)
        {
            // Đối tượng Player đại diện cho CLient
            Player player = new Player();
            // Phòng chờ mà Client đó tham gia
            Lobby lobby = new Lobby();
            // Gán socket của client vào thuộc tính playerSocket
            player.playerSocket = client;
            // Lấy IP của client từ RemoteEndPoint của Socket và gán vào IP của Lobby
            lobby.ip = (((IPEndPoint)client.RemoteEndPoint).Address).ToString();
            // Thêm lobby vào danh sách lobbies để theo dõi phòng chờ mà người chơi tham gia
            lobbies.Add(lobby);
            // Thêm player vào danh sách Connected player để theo dõi các client đang kết nối
            connectedPlayers.Add(player);
            // Tạo 1 mảng bytes có kích thước 1024 để lưu dữ liệu nhận được từ client
            byte[] bytes = new byte[1024];
            // while dùng kiểm tra client còn kết nói hay không
            while (player.playerSocket.Connected)
            {
                // Kiểm tra xem có dữ liệu được gửi từ client hay không
                if (player.playerSocket.Available > 0)
                {
                    string msg = "";
                    // Nếu có
                    while (player.playerSocket.Available > 0)
                    {
                        // Sử dụng phương thức Receive để nhận dữ liệu từ client và lưu vào mảng bytes
                        int bytesRec = player.playerSocket.Receive(bytes);
                        // Chuyển thành chuối ASCII, lưu vào biến msg
                        msg += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    }
                    // In ra màn hình thông điệu cho biết dữ liệu nhận được từ Client nào (IP:Port) cùng msg
                    Console.WriteLine(player.playerSocket.RemoteEndPoint.ToString() + " : " + msg);
                    // Hàm này dùng phân tích và xử lý tin nhắn nhận đc
                    AnalyzeMessage(msg, player,lobby);
                }
            }
        }
        // Tìm kiếm 1 đối tượng trong danh sách lobbies dựa nào ip của client
        // Tham số đầu vào là 1 Lobby cần đc tìm kiếm
        // Đầu ra trả về 1 đối tượng lobby nếu tìm thấy trong ds, ngược lại trả về NULL
        // Sử dụng phương thức Find của List để tìm 1 đối tượng Lobby trong ds Lobby sao cho IP trùng với IP của Lobby đang đc xem xét
        public static Lobby findLobby(Lobby lobby)
        {
            return lobbies.Find(x => x.ip == lobby.ip);
        }
        // Phan tích tin nhắn từ Client: chia thành các phần sử dụng ký tự |, phần sử đầu tiên là Loại tin nhắn
        // (CREATE, CONNECT, DISCONNECT, START, TIMEOUT, CLOSE, RIGHT, LEFT,...)
        //
        public static void AnalyzeMessage(string msg, Player player,Lobby lobby)
        {
            string[] PayLoad = msg.Split('|');

            switch (PayLoad[0])
            {
                case "CREATE":
                    {
                        // Tìm phòng tương ứng với địa chỉ IP của Client
                        Lobby lobby1 = findLobby(lobby);
                        // Nếu phòng đã tồn tại
                        if(lobby1!=null)
                        {
                            string ip = lobby1.ip;
                            // Nếu phòng chưa có host, client hiện tại sẽ trở thành host của phòng
                            if (lobby1.isHost == false)
                            {
                                player.name = PayLoad[1];
                                lobby1.isHost = true;
                                lobby1.Host = player;
                                // Server sẽ gửi tin nhắn CREATED cho client kèm tên người chơi
                                byte[] data = Encoding.UTF8.GetBytes("CREATED|" + player.name);
                                player.playerSocket.Send(data);
                                // Xử lý trùng khớp:
                                // Nếu IP của lobby trùng với IP của lobby1, lobby đó sẽ được loại khỏ danh sách lobby và thêm lại vào lobby1
                                foreach (var lb in lobbies.ToList())
                                {
                                    if (lb.ip == lobby1.ip)
                                    {
                                        lobbies.Remove(lb);
                                        lobbies.Add(lobby1);
                                    }
                                }
                            }
                            else
                            {
                                // Còn nếu phòng đã có Host, server báo EXISTED cho client báo phòng tồn tại và có người chơi làm host
                                Console.WriteLine("Existed");
                                byte[] data = Encoding.UTF8.GetBytes("EXISTED|");
                                player.playerSocket.Send(data);
                            }
                        }
                    }
                    break;
                case "CONNECT":
                    {
                        // Tìm phòng chơi tương ứng với địa chỉ IP của Client
                        Lobby lobby1 = findLobby(lobby);
                        // Nếu phòng chơi tồn tại
                        if (lobby1 != null)
                        {
                            // Nếu đã có host nhưng chưa có Guest
                            if (lobby1.isHost == true && lobby1.isGuest == false)
                            {
                                player.name = PayLoad[1];
                                lobby1.isGuest = true;
                                lobby1.Guest = player;
                                // Server sẽ gửi tin nhắn cho Client về tên của Host
                                byte[] data = Encoding.UTF8.GetBytes("CONNECTED|" + lobby1.Host.name);
                                player.playerSocket.Send(data);
                                Thread.Sleep(100);
                                // Client sẽ gửi cho Server về tên cả Client
                                data = Encoding.UTF8.GetBytes("CONNECTED|" + player.name);
                                player.playerSocket.Send(data);
                                lobby1.Host.playerSocket.Send(data);
                                // Nếu tồn tại 1 IP của lobby trùng với IP của lobby, xóa trong lobby, thêm vào lobby1
                                foreach (var lb in lobbies.ToList())
                                {
                                    if (lb.ip == lobby1.ip)
                                    {
                                        lobbies.Remove(lb);
                                        lobbies.Add(lobby1);
                                    }
                                }
                            }
                            // Ngược lại nếu phòng chơi chưa có host, tức là phòng chưa được tạo, Server sẽ gửi thông báo cho Client NOTEXISTED
                            else if(lobby1.isHost == false)
                            {
                                Console.WriteLine("Not Existed");
                                byte[] data = Encoding.UTF8.GetBytes("NOTEXISTED|");
                                player.playerSocket.Send(data);
                            }
                            else if(lobby1.isHost == true && lobby1.isGuest == true)
                            {
                                // Nếu đã tồn tại cả Host và Guest, gửi thông báo phòng đã đầy
                                Console.WriteLine("Full");
                                byte[] data = Encoding.UTF8.GetBytes("FULL|");
                                player.playerSocket.Send(data);
                            }
                        }
                    }
                    break;
                // SocketShutdown.Both: hai phần đọc và ghi của kết nối sẽ được đóng
                //
                case "DISCONNECT":
                    {
                        // Tìm phòng chơi tương ứng với địa chỉ IP của Client 
                        Lobby lobby1 = findLobby(lobby);
                        if(lobby1 != null)
                        {
                            // Nếu là Host
                            if(player.name == lobby1.Host.name)
                            {
                                //Ngắt kết nối mạng của Host
                                lobby1.Host.playerSocket.Shutdown(SocketShutdown.Both);
                                // Đóng socket bằng cách gọi hàm Close
                                lobby1.Host.playerSocket.Close();
                                // Không còn Host nữa
                                lobby1.isHost = false;
                                // Loại host khỏi danh sách người chơi đã kết nối trong connectedPlayers
                                connectedPlayers.Remove(lobby1.Host);
                            }
                            // Guest làm tương tự Host
                            else if(player.name == lobby1.Guest.name)
                            {
                                lobby1.Guest.playerSocket.Shutdown(SocketShutdown.Both);
                                lobby1.Guest.playerSocket.Close();
                                lobby1.isGuest = false;
                                connectedPlayers.Remove(lobby1.Guest);
                            }
                        }
                    }
                    break;
                case "START":
                    {
                        // Tìm lobby ứng với trận đấu sắp bắt đầu dựa trên Ip của CLient
                        Lobby lobby1 = findLobby(lobby);
                        if (lobby1 != null)
                        {
                            // Thông điệp gởi đến Host: thông điệp chứa tên host và guest
                            string message = "HOST|" + lobby1.Host.name + "|" + lobby1.Guest.name;
                            byte[] data = Encoding.UTF8.GetBytes(message);
                            lobby1.Host.playerSocket.Send(data);
                            Console.WriteLine("Sendback: " + message);
                            Thread.Sleep(100);
                            // Guest: tương tự host
                            message = "INIT|" + lobby1.Host.name + "|" + lobby1.Guest.name;
                            data = Encoding.UTF8.GetBytes(message);
                            lobby1.Guest.playerSocket.Send(data);
                            Console.WriteLine("Sendback: " + message);
                            // Đảm bảo rằng thông điệp được gửi đến đúng địa chỉ và được xử lý bởi client.
                            Thread.Sleep(100);
                        }                   
                    }
                    break;
                //  Server chỉ in ra một thông báo cho biết có timeout và hiển thị tên của người chơi liên quan.
                case "TIMEOUT":
                    {
                        Console.WriteLine("Timeout: " + PayLoad[1]);
                    }
                    break;
                case "CLOSE":
                    {
                        // Tìm lobby ứng với client muốn đóng kết nối
                        Lobby lobby1 = findLobby(lobby);
                        if(lobby1 != null)
                        {
                            Console.WriteLine("Close: " + PayLoad[1]);
                            // Server gửi một tin nhắn thông báo đóng kết nối tới người chơi muốn đóng kết nối.
                            string message = "CLOSED|" + PayLoad[1];
                            byte[] data = Encoding.UTF8.GetBytes(message);
                            if(lobby1.Host.name == PayLoad[1])
                            {
                                lobby1.Host.playerSocket.Send(data);
                            }
                            else if(lobby1.Guest.name == PayLoad[1])
                            {
                                lobby1.Guest.playerSocket.Send(data);
                            }
                        }
                    }
                    break;
                // Trường hợp này xảy ra khi một người chơi di chuyển sang phải hoặc trái.
                // Server gửi thông điệp đến cả host và guest với các thông tin về vị trí mới, giá trị và lượt chơi của người chơi.
                case "RIGHT":
                    {
                        Lobby lobby1 = findLobby(lobby);
                        if(lobby1 != null)
                        {
                            string position = PayLoad[2];
                            string value = PayLoad[1];
                            string player1turn = PayLoad[3];
                            string message = "GORIGHT|" + value + "|" + position + "|" + player1turn;
                            byte[] data = Encoding.UTF8.GetBytes(message);
                            lobby1.Guest.playerSocket.Send(data);
                            lobby1.Host.playerSocket.Send(data);
                        }
                    }
                    break;
                case "LEFT":
                    {
                        Lobby lobby1 = findLobby(lobby);
                        if (lobby1 != null)
                        {
                            string position = PayLoad[2];
                            string value = PayLoad[1];
                            string player1turn = PayLoad[3];
                            string message = "GOLEFT|" + value + "|" + position + "|" + player1turn;
                            byte[] data = Encoding.UTF8.GetBytes(message);
                            lobby1.Guest.playerSocket.Send(data);
                            lobby1.Host.playerSocket.Send(data);
                        }
                    }
                    break;
                // Server chỉ in ra tên của người chiến thắng.
                case "WINNER":
                    {
                        Console.WriteLine("Winner: " + PayLoad[1]);
                    }
                    break;
                // khi trò chơi kết thúc với kết quả hòa. Server chỉ in ra một thông báo nói rằng trò chơi kết thúc với kết quả hòa.
                case "DRAW":
                    {
                        Console.WriteLine("Draw");
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
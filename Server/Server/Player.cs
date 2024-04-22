using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Server
{
    // Lớp chứa thông tin người chơi
    internal class Player
    {
        public string name { get; set; } // Tên Player
        public int isHost { get; set; } // Xác định nó có phải là máy chủ ko
        public int turn { get; set; } // Xác định lượt chơi, 1 nếu là lượt chơi của mình, 0 nếu ko là lượt chơi của mình 
        public Socket playerSocket { get; set; } //Kết nối socket của người chơi với server, giao tiếp, gửi và nhận dữ liệu của server với client
    }
}

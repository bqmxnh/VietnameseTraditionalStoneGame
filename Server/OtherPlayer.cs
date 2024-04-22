using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class OtherPlayer
    {
        public string name { get; set; }
        public int isHost { get; set; }
        public int turn { get; set; }
        public Socket playerSocket { get; set; }
    }
}

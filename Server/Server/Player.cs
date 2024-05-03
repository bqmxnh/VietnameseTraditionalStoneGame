using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Server
{
    internal class Player
    {
        public string name { get; set; }
        public int isHost { get; set; }
        public int turn { get; set; }
        public Socket playerSocket { get; set; }
    }
}
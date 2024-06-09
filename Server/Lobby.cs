using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class Lobby
    {
        public string ip { get; set; }
        
        public bool isHost = false;

        public bool isGuest = false;
        public Player Host { get; set; }
        public Player Guest { get; set; }
    }
}

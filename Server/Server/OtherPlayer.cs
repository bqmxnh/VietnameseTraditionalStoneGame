using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class OtherPlayer
    {
        public string Name { set; get; } 
        public string isHost { set; get; }      
        public string turn { set; get; }
        public int playerSocket { set; get; }
    }
}

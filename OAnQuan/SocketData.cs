using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAnQuan
{
    internal class SocketData
    {
        private int action;

        public int Action
        {
            get { return action; }
            set { action = value; }
        }

        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        public SocketData(int action, string message)
        {
            this.Action = action;
            this.Message = message;
        }
    }
    public enum SocketCommand
    {
        Left,
        Right,
        Undo,
    }
}

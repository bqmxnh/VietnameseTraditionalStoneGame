using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OAnQuan
{
    internal class SocketManager
    {
        #region Client
        Socket client;
        public bool ConnectServer()
        {
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(IP), Port);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                client.Connect(iPEndPoint);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region Server
        Socket server;
        public bool connected = false;
        public void CreateServer()
        {
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(IP), Port);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            server.Bind(iPEndPoint);
            server.Listen(10);

            Thread acceptClient = new Thread(() =>
            {
                client = server.Accept();
            });
            acceptClient.IsBackground = true;
            acceptClient.Start();
        }

        #endregion

        #region Data
        public string IP = "127.0.0.1";
        public int Port = 9999;
        public const int BUFFER = 2048;
        public bool isServer = true;

        public bool Send(object o)
        {
            byte[] data = SerializeData(o);
            return SendData(client, data);
        }

        public object Receive()
        {
            byte[] data = new byte[BUFFER];
            bool isOK = ReceiveData(client, data);

            return DeserializeData(data);
        }

        private bool SendData(Socket socket, byte[] data)
        {
            return socket.Send(data) == 1 ? true : false;
        }

        private bool ReceiveData(Socket socket, byte[] data)
        {
            return socket.Receive(data) == 1 ? true : false;
        }

        public byte[] SerializeData(object o)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bm = new BinaryFormatter();
            bm.Serialize(ms, o);
            return ms.ToArray();
        }

        public object DeserializeData(byte[] data)
        {
            MemoryStream ms = new MemoryStream(data);
            BinaryFormatter bm = new BinaryFormatter();
            ms.Position = 0;
            return bm.Deserialize(ms);
        }

        public string GetIPv4(NetworkInterfaceType _type)
        {
            string output = "";
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up)
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
        #endregion

    }
}
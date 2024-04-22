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
        private static Socket serverSocket;
        private static Socket client;
        private static Thread clientThread;
        private static List<Player> connectedPlayers = new List<Player>();

        static void Main(string[] args)
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, 9999));
            serverSocket.Listen(2);
            Console.WriteLine("Wating for player");


            while (true)
            {
                client = serverSocket.Accept();
                Console.WriteLine("Player connected from " + client.RemoteEndPoint.ToString());
                clientThread = new Thread(() => readClientSocket(client));
                clientThread.IsBackground = true;
                clientThread.Start();
            }
        }

        public static void readClientSocket(Socket client)
        {
            Player player = new Player();
            player.playerSocket = client;
            connectedPlayers.Add(player);

            byte[] bytes = new byte[1024];

            while (player.playerSocket.Connected)
            {
                if (player.playerSocket.Available > 0)
                {
                    string msg = "";

                    while (player.playerSocket.Available > 0)
                    {
                        int bytesRec = player.playerSocket.Receive(bytes);
                        msg += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    }

                    Console.WriteLine(player.playerSocket.RemoteEndPoint.ToString() + " : " + msg);
                    AnalyzeMessage(msg, player);
                }
            }
        }

        public static void AnalyzeMessage(string msg, Player player)
        {
            string[] PayLoad = msg.Split('|');

            switch (PayLoad[0])
            {
                case "CREATE":
                    {
                        player.name = PayLoad[1];

                        byte[] data = Encoding.UTF8.GetBytes("CREATED|" + player.name); 

                        player.playerSocket.Send(data);
                    }
                    break;
                case "CONNECT":
                    {
                        player.name = PayLoad[1];
                        // Send connected players to the new player
                        foreach (var p in connectedPlayers)
                        {
                            byte[] data = Encoding.UTF8.GetBytes("CONNECTED|" + p.name);
                            player.playerSocket.Send(data);
                            Thread.Sleep(100);
                        }

                        // Send the new player to the connected players
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
                        // Create a copy of the list to avoid the exception of modifying the list while iterating
                        foreach (var p in connectedPlayers.ToList())
                        {
                            if (p.name == PayLoad[1])
                            {
                                p.playerSocket.Shutdown(SocketShutdown.Both);
                                p.playerSocket.Close();
                                connectedPlayers.Remove(p);
                            }
                        }
                    }
                    break;
                case "START":
                    {
                        foreach(var p in connectedPlayers)
                        {
                            string message = "INIT|";
                            foreach(var p2 in connectedPlayers)
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
                            if(p.name != name)
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
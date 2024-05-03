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
        private static List<Lobby> lobbies = new List<Lobby>();
        public static string host;

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
            Lobby lobby = new Lobby();
            player.playerSocket = client;
            lobby.ip = (((IPEndPoint)client.RemoteEndPoint).Address).ToString();
            lobbies.Add(lobby);
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
                    AnalyzeMessage(msg, player, lobby);
                }
            }
        }

        public static Lobby findLobby(Lobby lobby)
        {
            return lobbies.Find(x => x.ip == lobby.ip);
        }

        public static void AnalyzeMessage(string msg, Player player, Lobby lobby)
        {
            string[] PayLoad = msg.Split('|');

            switch (PayLoad[0])
            {
                case "CREATE":
                    {

                        Lobby lobby1 = findLobby(lobby);
                        if (lobby1 != null)
                        {
                            string ip = lobby1.ip;
                            if (lobby1.isHost == false)
                            {
                                player.name = PayLoad[1];
                                lobby1.isHost = true;
                                lobby1.Host = player;
                                byte[] data = Encoding.UTF8.GetBytes("CREATED|" + player.name);
                                player.playerSocket.Send(data);
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
                                Console.WriteLine("Existed");
                                byte[] data = Encoding.UTF8.GetBytes("EXISTED|");
                                player.playerSocket.Send(data);
                            }
                        }
                    }
                    break;
                case "CONNECT":
                    {
                        Lobby lobby1 = findLobby(lobby);
                        if (lobby1 != null)
                        {
                            if (lobby1.isHost == true && lobby1.isGuest == false)
                            {
                                player.name = PayLoad[1];
                                lobby1.isGuest = true;
                                lobby1.Guest = player;
                                byte[] data = Encoding.UTF8.GetBytes("CONNECTED|" + lobby1.Host.name);
                                player.playerSocket.Send(data);
                                Thread.Sleep(100);
                                data = Encoding.UTF8.GetBytes("CONNECTED|" + player.name);
                                player.playerSocket.Send(data);
                                lobby1.Host.playerSocket.Send(data);
                                foreach (var lb in lobbies.ToList())
                                {
                                    if (lb.ip == lobby1.ip)
                                    {
                                        lobbies.Remove(lb);
                                        lobbies.Add(lobby1);
                                    }
                                }
                            }
                            else if (lobby1.isHost == false)
                            {
                                Console.WriteLine("Not Existed");
                                byte[] data = Encoding.UTF8.GetBytes("NOTEXISTED|");
                                player.playerSocket.Send(data);
                            }
                            else if (lobby1.isHost == true && lobby1.isGuest == true)
                            {
                                Console.WriteLine("Full");
                                byte[] data = Encoding.UTF8.GetBytes("FULL|");
                                player.playerSocket.Send(data);
                            }
                        }
                    }
                    break;
                case "DISCONNECT":
                    {
                        Lobby lobby1 = findLobby(lobby);
                        if (lobby1 != null)
                        {
                            if (player.name == lobby1.Host.name)
                            {
                                lobby1.Host.playerSocket.Shutdown(SocketShutdown.Both);
                                lobby1.Host.playerSocket.Close();
                                lobby1.isHost = false;
                                connectedPlayers.Remove(lobby1.Host);
                            }
                            else if (player.name == lobby1.Guest.name)
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
                        Lobby lobby1 = findLobby(lobby);
                        if (lobby1 != null)
                        {
                            string message = "HOST|" + lobby1.Host.name + "|" + lobby1.Guest.name;
                            byte[] data = Encoding.UTF8.GetBytes(message);
                            lobby1.Host.playerSocket.Send(data);
                            Console.WriteLine("Sendback: " + message);
                            Thread.Sleep(100);

                            message = "INIT|" + lobby1.Host.name + "|" + lobby1.Guest.name;
                            data = Encoding.UTF8.GetBytes(message);
                            lobby1.Guest.playerSocket.Send(data);
                            Console.WriteLine("Sendback: " + message);
                            Thread.Sleep(100);
                        }
                    }
                    break;
                case "TIMEOUT":
                    {
                        Console.WriteLine("Timeout: " + PayLoad[1]);
                    }
                    break;
                case "CLOSE":
                    {
                        Lobby lobby1 = findLobby(lobby);
                        if (lobby1 != null)
                        {
                            Console.WriteLine("Close: " + PayLoad[1]);
                            string message = "CLOSED|" + PayLoad[1];
                            byte[] data = Encoding.UTF8.GetBytes(message);
                            if (lobby1.Host.name == PayLoad[1])
                            {
                                lobby1.Host.playerSocket.Send(data);
                            }
                            else if (lobby1.Guest.name == PayLoad[1])
                            {
                                lobby1.Guest.playerSocket.Send(data);
                            }
                        }
                    }
                    break;
                case "RIGHT":
                    {
                        Lobby lobby1 = findLobby(lobby);
                        if (lobby1 != null)
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
                case "WINNER":
                    {
                        Console.WriteLine("Winner: " + PayLoad[1]);
                    }
                    break;
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
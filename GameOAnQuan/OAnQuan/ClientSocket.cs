using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1;

namespace OAnQuan
{
    internal class ClientSocket
    {
        public static Socket clientSocket;
        public static Thread thread;
        public static string dataHeader = "";

        public static void Connect(IPEndPoint ipEP)
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(ipEP);
            thread = new Thread(() => Receive());
            thread.IsBackground = true;
            thread.Start();
        }

        public static void SendData(string data)
        {
            string msg = dataHeader + "|" + data;
            byte[] bytes = Encoding.UTF8.GetBytes(msg);
            clientSocket.Send(bytes);
        }

        public static void Receive()
        {
            byte[] bytes = new byte[1024];

            while (clientSocket.Connected)
            {
                if (clientSocket.Available > 0)
                {
                    string msg = "";
                    while (clientSocket.Available > 0)
                    {
                        int bytesRec = clientSocket.Receive(bytes);
                        msg += Encoding.UTF8.GetString(bytes, 0, bytesRec);
                    }

                    AnalyzeMessage(msg);
                    MenuForm.lobby.Changetextbox(msg);
                }
            }
        }

        public static MainForm mainForm;
        public static bool player1Turn;

        public static void AnalyzeMessage(string msg)
        {
            string[] PayLoad = msg.Split('|');

            switch (PayLoad[0])
            {
                case "EXISTED":
                    {
                        MenuForm.lobby.MsgExisted();
                        MenuForm.lobby.closeForm();
                    }
                    break;
                case "CREATED":
                    {
                        MenuForm.lobby.ShowbtnStart();
                        MenuForm.lobby.DisplayPlayer(PayLoad[1]);
                    }
                    break;
                case "NOTEXISTED":
                    {
                        MenuForm.lobby.MsgNotExisted();
                        MenuForm.lobby.closeForm();
                    }
                    break;
                case "FULL":
                    {
                        MenuForm.lobby.Text = "FULL";
                        MenuForm.lobby.MsgFull();
                        MenuForm.lobby.closeForm();
                    }
                    break;
                case "CONNECTED":
                    MenuForm.lobby.DisplayPlayer(PayLoad[1]);
                    break;
                case "HOST":
                    {
                        mainForm = new MainForm(PayLoad[1], PayLoad[2]);
                        MenuForm.lobby.Invoke((MethodInvoker)delegate
                        {
                            //MenuForm.lobby.Hide();
                            mainForm.Text = "Host";
                            mainForm.changeMode(true);
                            mainForm.Show();
                        });
                    }
                    break;
                case "INIT":
                    {
                        mainForm = new MainForm(PayLoad[1], PayLoad[2]);
                        MenuForm.lobby.Invoke((MethodInvoker)delegate
                        {
                            //MenuForm.lobby.Hide();
                            mainForm.Text = "Client";
                            mainForm.changeMode(false);
                            mainForm.Show();
                        });
                    }
                    break;
                case "CLOSED":
                    {
                        MessageBox.Show("Player " + PayLoad[1] + " has left the game");
                        mainForm.FormClosed -= mainForm.MainForm_FormClosed;
                        mainForm.Close();
                    }
                    break;
                case "GORIGHT":
                    {
                        mainForm.Invoke((MethodInvoker)async delegate
                        {
                            (mainForm.Controls["button" + PayLoad[2]] as Button).Text = "0";
                            if (PayLoad[3].ToLower() == "true")
                                player1Turn = true;
                            else
                                player1Turn = false;
                            mainForm.ResetTime();
                            await mainForm.goright(int.Parse(PayLoad[1]), int.Parse(PayLoad[2]), player1Turn);
                            if (player1Turn)
                                mainForm.StartPlayer2Turn();
                            else
                                mainForm.StartPlayer1Turn();
                            mainForm.changeturn();
                        });
                    }
                    break;
                case "GOLEFT":
                    {
                        mainForm.Invoke((MethodInvoker)async delegate
                        {
                            (mainForm.Controls["button" + PayLoad[2]] as Button).Text = "0";
                            if (PayLoad[3].ToLower() == "true")
                                player1Turn = true;
                            else
                                player1Turn = false;
                            mainForm.ResetTime();
                            await mainForm.goleft(int.Parse(PayLoad[1]), int.Parse(PayLoad[2]), player1Turn);
                            if (player1Turn)
                                mainForm.StartPlayer2Turn();
                            else
                                mainForm.StartPlayer1Turn();
                            mainForm.changeturn();
                        });
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
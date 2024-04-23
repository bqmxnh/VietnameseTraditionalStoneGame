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

            while(clientSocket.Connected)
            {
                if(clientSocket.Available > 0)
                {
                    string msg = "";
                    while(clientSocket.Available > 0)
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
                case "CREATED":
                    {
                        MenuForm.lobby.ShowbtnStart();
                        MenuForm.lobby.DisplayPlayer(PayLoad[1]);
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
                case "CLICKED":
                    {
                        mainForm.Invoke((MethodInvoker)delegate
                        {
                            Button btn = mainForm.Controls["button" + PayLoad[2]] as Button;
                            btn.Focus();
                        });
                    }
                    break;
                case "GORIGHT":
                    {
                        mainForm.Invoke((MethodInvoker)async delegate
                        {
                            (mainForm.Controls["button" + PayLoad[2]] as Button).Text = "0";
                            if (PayLoad[3].ToLower() == "true")
                            {
                                player1Turn = true;
                                mainForm.StartPlayer1Turn();
                            }
                            else
                            {
                                player1Turn = false;
                                mainForm.StartPlayer2Turn();
                            }
                            await mainForm.goright(int.Parse(PayLoad[1]), int.Parse(PayLoad[2]),player1Turn);
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
                            {
                                player1Turn = true;
                                mainForm.StartPlayer1Turn();
                            }
                            else
                            {
                                player1Turn = false;
                                mainForm.StartPlayer2Turn();
                            }
                            await mainForm.goleft(int.Parse(PayLoad[1]), int.Parse(PayLoad[2]),player1Turn);
                            mainForm.changeturn();
                        });
                    }
                    break;
                //case "TIME":
                //    {

                //    }
                default:
                    break;
            }
        }
    }
}

using OAnQuan;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp1
{
    public partial class MainForm : Form
    {
        // Khai báo biến toàn cục để lưu giá trị và vị trí của nút được chọn
        int value;
        int position;
        bool quan6, quan12;
        bool player1Turn = true;

        System.Windows.Forms.Timer player1Timer;
        System.Windows.Forms.Timer player2Timer;
        int player1TimeLeft = 15; // Thời gian mặc định cho mỗi lượt của người chơi 1
        int player2TimeLeft = 15; // Thời gian mặc định cho mỗi lượt của người chơi 2

        public MainForm(string player1,string player2)
        {
            InitializeComponent();
            InitializeTimers();

            quan12 = quan6 = true;
            foreach (Control control in this.Controls)
            {
                if (control is Button)
                {
                    Button button = control as Button;;
                    int value = int.Parse(button.Text);
                    // Nhóm các button từ 1 đến 5
                    if (button.Name == "button1" || button.Name == "button2" || button.Name == "button3" || button.Name == "button4" || button.Name == "button5")
                        button.Image = imageListButton1to5.Images[value];
                    // Nhóm các button từ 7 đến 11
                    else if (button.Name == "button7" || button.Name == "button8" || button.Name == "button9" || button.Name == "button10" || button.Name == "button11")
                        button.Image = imageListButton7to11.Images[value];
                    // Button 6
                    else if (button.Name == "button6")
                        button.Image = imageListButton6.Images[value]; // Thay "image6" bằng tên hình ảnh của bạn
                                                                     // Button 12
                    else if (button.Name == "button12")
                        button.Image = imageListButton12.Images[value]; // Thay "image12" bằng tên hình ảnh của bạn
                }
            }
            lb1name.Text = player1;
            lb2name.Text = player2;
            player1Timer.Start(); // Start the timer for player 1
        }

        private void InitializeTimers()
        {
            player1Timer = new System.Windows.Forms.Timer();
            player1Timer.Interval = 1000; // 1 second
            player1Timer.Tick += Player1Timer_Tick;

            player2Timer = new System.Windows.Forms.Timer();
            player2Timer.Interval = 1000; // 1 second
            player2Timer.Tick += Player2Timer_Tick;
        }

        private void Player1Timer_Tick(object sender, EventArgs e)
        {
            player1TimeLeft--; // Decrease player 1's remaining time
            textBox3.Text = player1TimeLeft.ToString(); // Update TextBox with player 1's remaining time

            if (player1TimeLeft == 0) // Check if time has run out
            {
                player1Timer.Stop(); // Stop the timer for player 1
                MessageBox.Show("Player 1 ran out of time. Game Over.");
                //player1Turn = false; // Player 1 loses turn
                //StartPlayer2Turn(); // Start player 2's turn
                //player1TimeLeft = 0; // Ensure time left is set to 0
                //textBox3.Text = "0"; // Update TextBox with player 1's remaining time
            }
        }

        private void Player2Timer_Tick(object sender, EventArgs e)
        {
            player2TimeLeft--; // Decrease player 2's remaining time
            textBox4.Text = player2TimeLeft.ToString(); // Update TextBox with player 2's remaining time

            if (player2TimeLeft == 0) // Check if time has run out
            {
                player2Timer.Stop(); // Stop the timer for player 2
                MessageBox.Show("Player 2 ran out of time. Game Over.");
                //player1Turn = true; // Player 2 loses turn
                //StartPlayer1Turn(); // Start player 1's turn
                //player2TimeLeft = 0; // Ensure time left is set to 0
                //textBox4.Text = "0"; // Update TextBox with player 2's remaining time
            }
        }

        private void ResetTime()
        {
            player1TimeLeft = 15; // Đặt lại thời gian của người chơi 1
            player2TimeLeft = 15; // Đặt lại thời gian của người chơi 2
            textBox3.Text = player1TimeLeft.ToString(); // Cập nhật TextBox với thời gian còn lại của người chơi 1
            textBox4.Text = player2TimeLeft.ToString(); // Cập nhật TextBox với thời gian còn lại của người chơi 2
        }
        public void StartPlayer1Turn()
        {
            ResetTime(); // Đặt lại thời gian cho cả hai người chơi
            player1Timer.Start(); // Bắt đầu bộ đếm thời gian cho người chơi 1
            player2Timer.Stop(); // Dừng bộ đếm thời gian cho người chơi 2
        }

        public void StartPlayer2Turn()
        {
            ResetTime(); // Đặt lại thời gian cho cả hai người chơi
            player2Timer.Start(); // Bắt đầu bộ đếm thời gian cho người chơi 1
            player1Timer.Stop(); // Dừng bộ đếm thời gian cho người chơi 2
        }



        // Sự kiện click nút cho tất cả các nút có giá trị
        private void button_Click(object sender, EventArgs e)
        {
            if (player1Turn)
                StartPlayer1Turn();

            else
            {
                player1Turn = false;
                StartPlayer2Turn();
            }

            // Nếu nút có giá trị là 0, không làm gì cả
            if ((sender as Button).Text == "0")
                return;
            // Nếu không, lưu giá trị và vị trí của nút được chọn
            // Hiển thị panel để chọn hướng di chuyển
            else
            {
                value = int.Parse((sender as Button).Text);
                position = int.Parse((sender as Button).Name.Substring(6));
                if (player1Turn && position <= 5) // Kiểm tra lượt của player 1
                    panel2.Visible = true; // Hiển thị panel cho player 1
                else if (!player1Turn && position >= 7) // Kiểm tra lượt của player 2
                    panel1.Visible = true; // Hiển thị panel cho player 2
            }

        }


        //check 2 buttons after the button chosen.
        private void getscoreright(int position)
        {
            Button btn, btn2;
            //If the button is button1, next button to check is button12
            //if don't do this, the position-1=0, not exist
            if (position == 1)
            {
                position = 12;
                btn = this.Controls["button" + position.ToString()] as Button;
            }
            else
                btn = this.Controls["button" + (position - 1).ToString()] as Button;

            if (btn.Text == "0")
            {
                //if the button is button2, next to it is button1, the second button is button12
                //if don't do this, the position-2=0, not exist
                if (position == 2)
                {
                    position = 12;
                    btn2 = this.Controls["button" + position.ToString()] as Button;
                }
                else
                    btn2 = this.Controls["button" + (position - 2).ToString()] as Button;
                //if the second button is not 0, add the value to the score and set the second button to 0
                //then check the next button
                if (btn2.Text != "0")
                {
                    if (btn2.Name == "button6")
                    {
                        if (player1Turn)
                            textBox1.Text = (int.Parse(textBox1.Text) + int.Parse(btn2.Text) + 9).ToString();
                        else
                            textBox2.Text = (int.Parse(textBox2.Text) + int.Parse(btn2.Text) + 9).ToString();
                        MessageBox.Show("Người chơi ăn được quan.");
                        quan6 = false;
                    }
                    else if (btn2.Name == "button12")
                    {
                        if (player1Turn)
                            textBox1.Text = (int.Parse(textBox1.Text) + int.Parse(btn2.Text) + 9).ToString();
                        else
                            textBox2.Text = (int.Parse(textBox2.Text) + int.Parse(btn2.Text) + 9).ToString();
                        MessageBox.Show("Người chơi ăn được quan.");
                        quan12 = false;
                    }
                    else
                    {
                        if (player1Turn)
                            textBox1.Text = (int.Parse(textBox1.Text) + int.Parse(btn2.Text)).ToString();
                        else
                            textBox2.Text = (int.Parse(textBox2.Text) + int.Parse(btn2.Text)).ToString();
                    }
                    btn2.Text = "0";
                    getscoreright(position - 2);
                }
                else
                    return;
            }
            else
                return;
        }

        //same ideas as getscoreright
        private void getscoreleft(int position)
        {
            Button btn, btn2;
            //If the button is button12, next button to check is button1
            //if don't do this, the position+1=13, not exist
            if (position == 12)
            {
                position = 1;
                btn = this.Controls["button" + position.ToString()] as Button;
            }
            else
                btn = this.Controls["button" + (position + 1).ToString()] as Button;
            if (btn.Text == "0")
            {
                //if the button is button11, next to it is button12, the second button is button1
                //if don't do this, the position+2=13, not exist
                if (position == 11)
                {
                    position = 1;
                    btn2 = this.Controls["button" + position.ToString()] as Button;
                }
                else
                    btn2 = this.Controls["button" + (position + 2).ToString()] as Button;
                if (btn2.Text != "0")
                {
                    if (btn2.Name == "button6")
                    {
                        if (player1Turn)
                            textBox1.Text = (int.Parse(textBox1.Text) + int.Parse(btn2.Text) + 9).ToString();
                        else
                            textBox2.Text = (int.Parse(textBox2.Text) + int.Parse(btn2.Text) + 9).ToString();
                        MessageBox.Show("Người chơi ăn được quan.");
                        quan6 = false;
                    }
                    else if (btn2.Name == "button12")
                    {
                        if (player1Turn)
                            textBox1.Text = (int.Parse(textBox1.Text) + int.Parse(btn2.Text) + 9).ToString();
                        else
                            textBox2.Text = (int.Parse(textBox2.Text) + int.Parse(btn2.Text) + 9).ToString();
                        MessageBox.Show("Người chơi ăn được quan.");
                        quan12 = false;
                    }
                    else
                    {
                        if (player1Turn)
                            textBox1.Text = (int.Parse(textBox1.Text) + int.Parse(btn2.Text)).ToString();
                        else
                            textBox2.Text = (int.Parse(textBox2.Text) + int.Parse(btn2.Text)).ToString();
                    }
                    btn2.Text = "0";
                    getscoreleft(position);
                }
                else
                    return;
            }
            else
                return;
        }

        //recursive function to move the value to the left until meet the QUAN or meet a button with 0 value
        public async Task goleft(int value, int position)
        {
            for (int i = value; i != 0; i--)
            {
                await Task.Delay(500); // Delay for 0.5 seconds
                position++;
                if (position == 12)
                {
                    foreach (Control c in this.Controls)
                    {
                        if (c is Button)
                            if (c.Name == "button" + position.ToString())
                                c.Text = (int.Parse(c.Text) + 1).ToString();
                    }
                    position = 0;

                }
                else
                {
                    foreach (Control c in this.Controls)
                    {
                        if (c is Button)
                            if (c.Name == "button" + position.ToString())
                                c.Text = (int.Parse(c.Text) + 1).ToString();
                    }
                }
            }

            if (position == 5 || position == 11)
                return;

            Button nextBtn = this.Controls["button" + (position + 1).ToString()] as Button;
            if (nextBtn.Text != "0")
            {
                value = int.Parse(nextBtn.Text);
                nextBtn.Text = "0";
                await goleft(value, position + 1);
            }
            else
                getscoreleft(position);

            if (endgame())
            {
                MessageBox.Show("Game Over");
                return;
            }

            if (checkrowabove())
            {
                updaterow(1);
                if (checkrowbelow())
                    updaterow(2);
            }
            else if (checkrowbelow())
                updaterow(2);
            else
                return;
        }

        //same ideas as goleft
        public async Task goright(int value, int position)
        {
            for (int i = value; i != 0; i--)
            {
                await Task.Delay(500); // Delay for 0.5 seconds
                position--;
                if (position > 0)
                {
                    foreach (Control c in this.Controls)
                    {
                        if (c is Button)
                            if (c.Name == "button" + position.ToString())
                                c.Text = (int.Parse(c.Text) + 1).ToString();
                    }
                }
                else
                {
                    position = 12;
                    foreach (Control c in this.Controls)
                    {
                        if (c is Button)
                            if (c.Name == "button" + position.ToString())
                                c.Text = (int.Parse(c.Text) + 1).ToString();
                    }
                }
            }

            if (position == 7 || position == 1)
                return;

            Button nextBtn = this.Controls["button" + (position - 1).ToString()] as Button;
            if (nextBtn.Text != "0")
            {
                value = int.Parse(nextBtn.Text);
                nextBtn.Text = "0";
                await goright(value, position - 1);
            }
            else
            {
                getscoreright(position);
            }

            if (endgame())
            {
                MessageBox.Show("Game Over");
                return;
            }

            if (checkrowabove())
                updaterow(1);
            else if (checkrowbelow())
                updaterow(2);
            else
                return;
        }

        private bool checkrowabove()
        {
            for (int i = 1; i < 6; i++)
            {
                if ((this.Controls["button" + i.ToString()] as Button).Text != "0")
                    return false;
            }
            return true;
        }

        //check if the below row is empty or not
        private bool checkrowbelow()
        {
            for (int i = 7; i < 12; i++)
            {
                if ((this.Controls["button" + i.ToString()] as Button).Text != "0")
                    return false;
            }
            return true;
        }

        //update the row by setting all buttons in the row to 1 and decrease the score by 5
        private void updaterow(int row)
        {
            if (row == 1)
            {
                for (int i = 1; i < 6; i++)
                {
                    this.Controls["button" + i.ToString()].Text = "1";
                }
                textBox1.Text = (int.Parse(textBox1.Text) - 5).ToString();
            }
            else
            {
                for (int i = 7; i < 12; i++)
                {
                    this.Controls["button" + i.ToString()].Text = "1";
                }
                textBox2.Text = (int.Parse(textBox1.Text) - 5).ToString();
            }            
        }

        //check if the game is over or not
        private bool endgame()
        {
            if (button6.Text == "0" && button12.Text == "0")
                return true;
            return false;
        }

        // Sự kiện khi giá trị Text của nút được thay đổi
        // Sự kiện button_TextChanged
        private void button_TextChanged(object sender, EventArgs e)
        {
            Button button = sender as Button;
            int value = int.Parse(button.Text);

            // Nhóm các button từ 1 đến 5
            if (button.Name == "button1" || button.Name == "button2" || button.Name == "button3" || button.Name == "button4" || button.Name == "button5")
                button.Image = imageListButton1to5.Images[value];
            // Nhóm các button từ 7 đến 11
            else if (button.Name == "button7" || button.Name == "button8" || button.Name == "button9" || button.Name == "button10" || button.Name == "button11")
                button.Image = imageListButton7to11.Images[value];
            // Button 6
            else if (button.Name == "button6")
            {
                if (!quan6)
                    button.Image = imageListButton6.Images[value]; // Thay "image6" bằng tên hình ảnh của bạn
                else
                    button.Image = imageListQuan6.Images[value];
            }
            else if (button.Name == "button12")
            {
                if (!quan12)
                    button.Image = imageListButton12.Images[value]; // Thay "image12" bằng tên hình ảnh của bạn
                else
                    button.Image = imageListQuan12.Images[value];
            }
        }


        // Sự kiện click nút cho các nút trong panel của player 1
        private async void buttonOpt_Click(object sender, EventArgs e)
        {
            player1Timer.Stop(); // Stop the timer for player 1
            player2Timer.Stop(); // Stop the timer for player 2
            string option = (sender as Button).Text;
            panel2.Visible = false;
            if (option == "Return")
                return;
            if (option == "Right")
            {
                Button button = this.Controls["button" + position.ToString()] as Button;
                button.Text = "0";
                await goright(value, position);
                // goright(value, position);
                ClientSocket.dataHeader = "RIGHT";
                string data = value.ToString() + "|" + position.ToString() + "|" + player1Turn.ToString() + "|" + lb1name.Text;
                ClientSocket.SendData(data);
            }
            else if (option == "Left")
            {
                Button button = this.Controls["button" + position.ToString()] as Button;
                button.Text = "0";
                //goleft(value, position);
                await goleft(value, position);
                ClientSocket.dataHeader = "LEFT";
                string data = value.ToString() + "|" + position.ToString() + "|" + player1Turn.ToString() + "|" + lb1name.Text;
                ClientSocket.SendData(data);
            }
            player1Turn = !player1Turn; // Chuyển lượt cho player 2
            player2Timer.Start();
        }

        
        // Sự kiện click nút cho các nút trong panel của player 2
        private async void buttonOpt_Click_Player2(object sender, EventArgs e)
        {
            player1Timer.Stop(); // Stop the timer for player 1
            player2Timer.Stop(); // Stop the timer for player 2

            string option = (sender as Button).Text;
            panel1.Visible = false;
            if (option == "Return")
                return;
            if (option == "Right")
            {
                Button button = this.Controls["button" + position.ToString()] as Button;
                button.Text = "0";
                // goright(value, position);
                await goright(value, position);
                ClientSocket.dataHeader = "RIGHT";
                string data = value.ToString() + "|" + position.ToString() + "|" + player1Turn.ToString() + "|" + lb2name.Text;
                ClientSocket.SendData(data);
            }
            else if (option == "Left")
            {
                Button button = this.Controls["button" + position.ToString()] as Button;
                button.Text = "0";
                //goleft(value, position);
                await goleft(value, position);
                ClientSocket.dataHeader = "LEFT";
                string data = value.ToString() + "|" + position.ToString() + "|" + player1Turn.ToString() + "|" + lb2name.Text;
                ClientSocket.SendData(data);
            }
            player1Turn = !player1Turn; // Chuyển lượt cho player 1
            player1Timer.Start();
        }        
    }
}

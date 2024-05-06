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
using System.IO;

namespace WindowsFormsApp1
{
    public partial class MainForm : Form
    {
        // Create global virables for value and position of the chosen button
        // Create global virables to check turn of player 1 or player 2
        // Create global virables to check if quan6 or quan12 is still exist
        int value;
        int position;
        bool quan6, quan12;
        bool player1Turn = true;


        System.Windows.Forms.Timer player1Timer;
        System.Windows.Forms.Timer player2Timer;
        int currentPlayerTimeLeft = 15;


        public MainForm(string player1,string player2)
        {
            InitializeComponent();
            InitializeTimers();

            quan12 = quan6 = true;
            foreach (Control control in this.Controls)
            {
                if (control is Button)
                {
                    Button button = control as Button;
                    int value = int.Parse(button.Text);
                    // Button 1 to 5
                    if (button.Name == "button1" || button.Name == "button2" || button.Name == "button3" || button.Name == "button4" || button.Name == "button5")
                        button.Image = imageListButton1to5.Images[value];
                    // Button 7 to 11
                    else if (button.Name == "button7" || button.Name == "button8" || button.Name == "button9" || button.Name == "button10" || button.Name == "button11")
                        button.Image = imageListButton7to11.Images[value];
                    // Button 6
                    else if (button.Name == "button6")
                        button.Image = imageListQuan6.Images[value]; 
                    else if (button.Name == "button12")
                        button.Image = imageListQuan12.Images[value]; 
                }
            }
            lb1name.Text = player1;
            lb2name.Text = player2;
        }

        #region Timer
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
            currentPlayerTimeLeft--;
            progressBar1.Value = currentPlayerTimeLeft;

            if (currentPlayerTimeLeft == 0)
            {
                player1Timer.Stop();
                MessageBox.Show("Player 1 ran out of time. Switch turns");
                ClientSocket.dataHeader = "TIMEOUT";
                string data = lb1name.Text;
                ClientSocket.SendData(data);
                ChangeTurn();
            }
        }

        private void Player2Timer_Tick(object sender, EventArgs e)
        {
            currentPlayerTimeLeft--;
            progressBar2.Value = currentPlayerTimeLeft;

            if (currentPlayerTimeLeft == 0)
            {
                player2Timer.Stop();
                MessageBox.Show("Player 2 ran out of time. Switch turns");
                ClientSocket.dataHeader = "TIMEOUT";
                string data = lb2name.Text;
                ClientSocket.SendData(data);
                ChangeTurn();
            }
        }


        private void ChangeTurn()
        {
            if (player1Turn)
                player1Timer.Stop();
            else
                player2Timer.Stop();

            player1Turn = !player1Turn;

            if (player1Turn)
                turnLabel.Text = "It's " + lb1name.Text + "'s Turn";
            else
                turnLabel.Text = "It's " + lb2name.Text + "'s Turn";

            ResetTime();
            if (player1Turn)
                StartPlayer1Turn();
            else
                StartPlayer2Turn();
        }




        public void ResetTime()
        {
            currentPlayerTimeLeft = 15;
            progressBar1.Value = progressBar2.Value = currentPlayerTimeLeft;

            // Stop both timers
            player1Timer.Stop();
            player2Timer.Stop();

           
        }





        public void StartPlayer1Turn()
        {
            ResetTime();
            player2Timer.Stop(); // Dừng đồng hồ đếm của người chơi 2
            player1Timer.Start(); // Bắt đầu đếm ngược cho người chơi 1
            turnLabel.Text = "It's " + lb1name.Text + "'s Turn"; // Cập nhật Label để hiển thị lượt chơi
        }

        public void StartPlayer2Turn()
        {
            ResetTime();
            player2Timer.Start();
            player1Timer.Stop();
            turnLabel.Text = "It's " + lb2name.Text + "'s Turn"; // Cập nhật Label để hiển thị lượt chơi
        }
        #endregion

        #region Button Click Events
        // Click event for all buttons except button 6 and 12
        private void button_Click(object sender, EventArgs e)
        {
            // If the value of the button is 0, do nothing
            if ((sender as Button).Text == "0")
                return;
            // Save the value and position of the chosen button
            // Show the panel for player
            else
            {
                value = int.Parse((sender as Button).Text);
                position = int.Parse((sender as Button).Name.Substring(6));
                // Check the turn of player 1 or player 2
                // Show the panel for player 1 or player 2
                if (player1Turn && position <= 5) 
                    panel2.Visible = true; 
                else if (!player1Turn && position >= 7) 
                    panel1.Visible = true; 
                ClientSocket.dataHeader = "CLICK";
                string data = value.ToString() + "|" + position.ToString();
                ClientSocket.SendData(data);
            }

        }

        // Click event for all buttons in panel 1
        public void buttonOpt_Click(object sender, EventArgs e)
        {
            // Stop the timer for both players
            player1Timer.Stop();
            player2Timer.Stop();
            string option = (sender as Button).Text;
            panel2.Visible = false;
            if (option == "Return")
                return;
            if (option == "Right")
            {
                Button button = this.Controls["button" + position.ToString()] as Button;
                button.Text = "0";
                ClientSocket.dataHeader = "RIGHT";
                string data = value.ToString() + "|" + position.ToString() + "|" + player1Turn.ToString() + "|" + lb1name.Text;
                ClientSocket.SendData(data);
            }
            else if (option == "Left")
            {
                Button button = this.Controls["button" + position.ToString()] as Button;
                button.Text = "0";
                ClientSocket.dataHeader = "LEFT";
                string data = value.ToString() + "|" + position.ToString() + "|" + player1Turn.ToString() + "|" + lb1name.Text;
                ClientSocket.SendData(data);
            }
        }

        // Click event for all buttons in panel 2
        public void buttonOpt_Click_Player2(object sender, EventArgs e)
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
                ClientSocket.dataHeader = "RIGHT";
                string data = value.ToString() + "|" + position.ToString() + "|" + player1Turn.ToString() + "|" + lb2name.Text;
                ClientSocket.SendData(data);
            }
            else if (option == "Left")
            {
                Button button = this.Controls["button" + position.ToString()] as Button;
                button.Text = "0";
                ClientSocket.dataHeader = "LEFT";
                string data = value.ToString() + "|" + position.ToString() + "|" + player1Turn.ToString() + "|" + lb2name.Text;
                ClientSocket.SendData(data);
            }
        }
        #endregion

        #region Game Logic
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
                    updatescore(btn2);
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
                    updatescore(btn2);
                    btn2.Text = "0";
                    getscoreleft(position);
                }
                else
                    return;
            }
            else
                return;
        }

        public void changeturn()
        {
            player1Turn = !player1Turn;
        }

        private void updatescore(object sender)
        {
            Button btn2 = sender as Button;
            //if the button is button6 or button12 and the quan is still exist, add 9 to the score
            if (btn2.Name == "button6" && quan6)
            {
                if (player1Turn)
                    textBox1.Text = (int.Parse(textBox1.Text) + int.Parse(btn2.Text) + 9).ToString();
                else
                    textBox2.Text = (int.Parse(textBox2.Text) + int.Parse(btn2.Text) + 9).ToString();
                MessageBox.Show("Người chơi ăn được quan.");
                quan6 = false;
            }
            else if (btn2.Name == "button12" && quan12)
            {
                if (player1Turn)
                    textBox1.Text = (int.Parse(textBox1.Text) + int.Parse(btn2.Text) + 9).ToString();
                else
                    textBox2.Text = (int.Parse(textBox2.Text) + int.Parse(btn2.Text) + 9).ToString();
                MessageBox.Show("Người chơi ăn được quan.");
                quan12 = false;
            }
            //if the button is not button6 or button12, add the value to the score
            else
            {
                if (player1Turn)
                    textBox1.Text = (int.Parse(textBox1.Text) + int.Parse(btn2.Text)).ToString();
                else
                    textBox2.Text = (int.Parse(textBox2.Text) + int.Parse(btn2.Text)).ToString();
            }
        }

        //recursive function to move the value to the left until meet the QUAN or meet a button with 0 value
        public async Task goleft(int value, int position,bool playerturn)
        {
            panel1.Visible = false;
            panel2.Visible = false;
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
                await goleft(value, position + 1,playerturn);
            }
            else
                getscoreleft(position);

            if (endgame())
            {
                totalScore();
                Winner();
                DisableBtn();
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
        public async Task goright(int value, int position, bool playerturn)
        {
            panel1.Visible = false;
            panel2.Visible = false;
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
                await goright(value, position - 1,playerturn);
            }
            else
            {
                getscoreright(position);
            }

            if (endgame())
            {
                totalScore();
                Winner();
                changeMode(playerturn);
                DisableBtn();
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

        //check if the above row is empty or not
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

        //calculate the total score 
        private void totalScore()
        {
            for(int i =1;i<= 5; i++)
            {
                textBox1.Text = (int.Parse(textBox1.Text) + int.Parse(this.Controls["button" + i.ToString()].Text)).ToString();
                this.Controls["button" + i.ToString()].Text = "0";
            }
            for (int i = 7; i <= 11; i++)
            {
                textBox2.Text = (int.Parse(textBox2.Text) + int.Parse(this.Controls["button" + i.ToString()].Text)).ToString();
                this.Controls["button" + i.ToString()].Text = "0";
            }
        }

        //show the winner
        private void Winner()
        {
            int score1 = int.Parse(textBox1.Text);
            int score2 = int.Parse(textBox2.Text);
            ClientSocket.dataHeader = "WINNER";
            string data = "";

            if (score1 > score2)
            {
                MessageBox.Show(lb1name.Text + " win");
                data = lb1name.Text;
            }
            else if (score1<score2)
            {
                MessageBox.Show(lb2name + " win");
                data = lb2name.Text;
            }
            else
            {
                MessageBox.Show("Draw");
                ClientSocket.dataHeader = "DRAW";
            }
            ClientSocket.SendData(data);
        }
        #endregion

        // TextChanged event for all buttons
        // Change the image of the button based on the value of the button
        private void button_TextChanged(object sender, EventArgs e)
        {
            Button button = sender as Button;
            int value = int.Parse(button.Text);

            // Button 1 to 5
            if (button.Name == "button1" || button.Name == "button2" || button.Name == "button3" || button.Name == "button4" || button.Name == "button5")
                button.Image = imageListButton1to5.Images[value];
            // Button 7 to 11
            else if (button.Name == "button7" || button.Name == "button8" || button.Name == "button9" || button.Name == "button10" || button.Name == "button11")
                button.Image = imageListButton7to11.Images[value];
            // Button 6
            else if (button.Name == "button6")
            {
                if (!quan6)
                    button.Image = imageListButton6.Images[value]; 
                else
                    button.Image = imageListQuan6.Images[value];
            }
            // Button 12
            else if (button.Name == "button12")
            {
                if (!quan12)
                    button.Image = imageListButton12.Images[value]; 
                else
                    button.Image = imageListQuan12.Images[value];
            }
        }        

        private void MainForm_Shown(object sender, EventArgs e)
        {
            StartPlayer1Turn();
        }

        // If a player closes the game, send a message to the other player
        public void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            player1Timer.Stop();
            player2Timer.Stop();
            if (endgame())
                return;
            ClientSocket.dataHeader = "CLOSE";
            string data = "";
            if (this.Text == "Host")
            {
                data = lb1name.Text;
            }
            else
            {
                data = lb2name.Text;
            }
            ClientSocket.SendData(data);

        }

        //Check side of the player and disable the button of the oppsite side
        public void changeMode(bool player1Turn)
        {
            if(player1Turn)
            {
                foreach(Control button in this.Controls)
                {
                    if (button is Button)
                    {
                        for (int i = 7; i <= 11; i++)
                        {
                            if (button.Name == "button" + i.ToString())
                                button.Click -= button_Click;
                        }
                    }
                }
            }
            else if(!player1Turn)
            {
                foreach (Control button in this.Controls)
                {
                    if (button is Button)
                    {
                        for (int i = 1; i <= 5; i++)
                        {
                            if (button.Name == "button" + i.ToString())
                                button.Click -= button_Click;
                        }
                    }
                }
            }
            
        }

       


        //Remove all click events of the buttons
        private void DisableBtn()
        {
            foreach (Control c in this.Controls)
            {
                if (c is Button)
                    c.Click -= button_Click;
            }
        }
    }
}

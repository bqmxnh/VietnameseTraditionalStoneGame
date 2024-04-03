using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //create global variables to store the value and position of the button chosen
        int value;
        int position;

        //create a button click event for all buttons with value
        private void button_Click(object sender, EventArgs e)
        {
            //if the button is 0, do nothing
            if ((sender as Button).Text == "0")
                return;
            //if the button is not 0, store the value and position of the button
            //show the panel to choose the direction
            else
            {
                value = int.Parse((sender as Button).Text);
                position = int.Parse((sender as Button).Name.Substring(6));
                panel1.Visible = true;
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
                    textBox1.Text = (int.Parse(textBox1.Text) + int.Parse(btn2.Text)).ToString();
                    btn2.Text = "0";
                    getscoreright(position-2);
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
                    textBox1.Text = (int.Parse(textBox1.Text) + int.Parse(btn2.Text)).ToString();
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
        private void goleft(int value, int position)
        {
            //loop till the value of the chosen button (LINH) is 0
            for(int i = value; i != 0; i--)
            {
                //move to the next button
                position++;
                //if the position is 12, +1 to its value and move to the first button
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
                //if the position is not 12, +1 to its value
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

            //if the next button is QUAN, stop
            if (position == 5 || position == 11)
                return;            
            
            //if the next button is not QUAN, move the value to the next button
            Button btn = this.Controls["button" + (position + 1).ToString()] as Button;
            //if the next button is not 0, store its value (get new LINH) and set it to 0
            if (btn.Text != "0")
            {
                value = int.Parse(btn.Text);
                btn.Text = "0";
                goleft(value, position + 1);
            }
            //if the next button is 0, check if we can update score or not 
            else
                getscoreleft(position);

            //check if the game is over or not
            if (endgame())
            {
                MessageBox.Show("Game Over");
                return;
            }

            //check if the row or all empty or not
            //if yes, update the row, decrease the score by 5
            if (checkrowabove())
            {
                updaterow(1);
                if(checkrowbelow())
                    updaterow(2);
            }
            else if (checkrowbelow()) 
                updaterow(2);
            else
                return;
        }

        //same ideas as goleft
        private void goright( int value,  int position)
        {
            for(int i = value; i != 0; i--)
            {
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
            
            Button btn = this.Controls["button" + (position - 1).ToString()] as Button;
            if (btn.Text != "0")
            {
                value = int.Parse(btn.Text);
                btn.Text = "0";
                goright(value, position - 1);
            }
            else
            {
                getscoreright(position);
            }
            
            if(endgame())
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

        //check if the above row is empty or not
        private bool checkrowabove()
        {
            for(int i = 1;i<6;i++)
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
            }
            else
            {
                for (int i = 7; i < 12; i++)
                {
                    this.Controls["button" + i.ToString()].Text = "1";
                }
            }
            textBox1.Text = (int.Parse(textBox1.Text)-5).ToString();
        }

        //check if the game is over or not
        private bool endgame()
        {
            Button btn, btn2;
            btn = this.Controls["button" + 12.ToString()] as Button;
            btn2 = this.Controls["button" + 6.ToString()] as Button;
            if (btn.Text == "0" && btn2.Text == "0")
                return true;
            return false;
        }

        //create button click event for all buttons in the panel
        private void buttonOpt_Click(object sender, EventArgs e)
        {
            //store the option chosen
            string option = (sender as Button).Text;
            panel1.Visible = false;
            //if the option is Return, do nothing
            if (option == "Return")
                return;
            //if the option is Right or Left, move the value to the right or left, set the chosen button to 0
            if (option == "Right")
            {
                Button button = this.Controls["button" + position.ToString()] as Button;
                button.Text = "0";
                goright(value, position);

            }
            else if (option == "Left")
            {
                Button button = this.Controls["button" + position.ToString()] as Button;
                button.Text = "0";
                goleft(value, position);
            }
        }
    }
}

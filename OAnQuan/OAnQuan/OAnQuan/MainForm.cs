using System;
using System.Linq;
using System.Windows.Forms;

namespace OAnQuan
{
    public partial class MainForm : Form
    {
        private int numberOfDân = 5; // Mặc định là 5
        public MainForm()
        {
            InitializeComponent();
        }

        // Chọn số dân là 5 hoặc 10 tại toolStripComboBox1, sau đó cập nhật số dân cho mỗi button Dân
        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Cập nhật số lượng dân
            numberOfDân = int.Parse(toolStripComboBox1.SelectedItem.ToString());
            UpdateButtons(); // Cập nhật nút dân
        }
        // Hàm này dùng để cập nhật số dân cho mỗi button, trừ button quân là o6 và o12
        private void UpdateButtons()
        {
            // Duyệt qua tất cả các button dân và cập nhật văn bản của chúng
            foreach (Control control in Controls)
            {
                if (control is Button button)
                {
                    if (button.Name.StartsWith("o") && button.Name != "o6" && button.Name != "o12")
                    {
                        button.Text = numberOfDân.ToString();
                    }
                }
            }
        }
        // Sau khi nhấn vào ô dân, hiện mũi tên để di chuyển trái phải

        private void Dan_Click(object sender, EventArgs e)
        {
            turnleft.Visible = true;
            turnright.Visible = true;
        }
    }

}

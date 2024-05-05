namespace OAnQuan
{
    partial class MenuForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MenuForm));
            this.btnJoin = new System.Windows.Forms.Button();
            this.tbIP = new System.Windows.Forms.TextBox();
            this.btnCreate = new System.Windows.Forms.Button();
            this.tbName = new System.Windows.Forms.TextBox();
            this.btnHowToPlay = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnJoin
            // 
            this.btnJoin.BackColor = System.Drawing.Color.Transparent;
            this.btnJoin.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnJoin.FlatAppearance.BorderSize = 0;
            this.btnJoin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnJoin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnJoin.Location = new System.Drawing.Point(680, 441);
            this.btnJoin.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnJoin.Name = "btnJoin";
            this.btnJoin.Size = new System.Drawing.Size(186, 48);
            this.btnJoin.TabIndex = 0;
            this.btnJoin.UseVisualStyleBackColor = false;
            this.btnJoin.Click += new System.EventHandler(this.btnJoin_Click);
            // 
            // tbIP
            // 
            this.tbIP.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbIP.Font = new System.Drawing.Font("Elephant", 15F, System.Drawing.FontStyle.Bold);
            this.tbIP.ForeColor = System.Drawing.Color.DarkKhaki;
            this.tbIP.Location = new System.Drawing.Point(564, 351);
            this.tbIP.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.tbIP.Multiline = true;
            this.tbIP.Name = "tbIP";
            this.tbIP.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tbIP.Size = new System.Drawing.Size(189, 45);
            this.tbIP.TabIndex = 1;
            this.tbIP.Text = "127.0.0.1";
            this.tbIP.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnCreate
            // 
            this.btnCreate.BackColor = System.Drawing.Color.Transparent;
            this.btnCreate.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnCreate.FlatAppearance.BorderSize = 0;
            this.btnCreate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCreate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnCreate.Location = new System.Drawing.Point(454, 441);
            this.btnCreate.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(195, 48);
            this.btnCreate.TabIndex = 0;
            this.btnCreate.UseVisualStyleBackColor = false;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // tbName
            // 
            this.tbName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbName.Font = new System.Drawing.Font("Elephant", 15F, System.Drawing.FontStyle.Bold);
            this.tbName.ForeColor = System.Drawing.Color.DarkKhaki;
            this.tbName.Location = new System.Drawing.Point(564, 224);
            this.tbName.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.tbName.Multiline = true;
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(199, 44);
            this.tbName.TabIndex = 1;
            this.tbName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnHowToPlay
            // 
            this.btnHowToPlay.BackColor = System.Drawing.Color.Transparent;
            this.btnHowToPlay.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnHowToPlay.FlatAppearance.BorderSize = 0;
            this.btnHowToPlay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHowToPlay.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnHowToPlay.Location = new System.Drawing.Point(26, 441);
            this.btnHowToPlay.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnHowToPlay.Name = "btnHowToPlay";
            this.btnHowToPlay.Size = new System.Drawing.Size(99, 85);
            this.btnHowToPlay.TabIndex = 2;
            this.btnHowToPlay.UseVisualStyleBackColor = false;
            this.btnHowToPlay.Click += new System.EventHandler(this.btnHowToPlay_Click);
            // 
            // MenuForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(954, 524);
            this.Controls.Add(this.btnHowToPlay);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.tbIP);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.btnJoin);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.MaximumSize = new System.Drawing.Size(976, 580);
            this.MinimumSize = new System.Drawing.Size(976, 580);
            this.Name = "MenuForm";
            this.Text = "Menu";
            this.Shown += new System.EventHandler(this.Menu_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnJoin;
        private System.Windows.Forms.TextBox tbIP;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.Button btnHowToPlay;
    }
}
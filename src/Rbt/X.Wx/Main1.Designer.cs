namespace X.Wx
{
    partial class Main
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.xPanel1 = new X.Wx.Controls.XPanel();
            this.lb_name = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pb_head = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.lb_nickname = new System.Windows.Forms.Label();
            this.tb_msg = new System.Windows.Forms.TextBox();
            this.bt_send = new System.Windows.Forms.Button();
            this.bt_sendimg = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_head)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.panel1.Controls.Add(this.xPanel1);
            this.panel1.Controls.Add(this.lb_name);
            this.panel1.Controls.Add(this.pictureBox2);
            this.panel1.Controls.Add(this.pb_head);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(280, 620);
            this.panel1.TabIndex = 5;
            // 
            // xPanel1
            // 
            this.xPanel1.Current = null;
            this.xPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.xPanel1.Location = new System.Drawing.Point(0, 69);
            this.xPanel1.Name = "xPanel1";
            this.xPanel1.Size = new System.Drawing.Size(280, 551);
            this.xPanel1.TabIndex = 6;
            // 
            // lb_name
            // 
            this.lb_name.AutoSize = true;
            this.lb_name.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lb_name.ForeColor = System.Drawing.Color.White;
            this.lb_name.Location = new System.Drawing.Point(63, 23);
            this.lb_name.Name = "lb_name";
            this.lb_name.Size = new System.Drawing.Size(74, 22);
            this.lb_name.TabIndex = 4;
            this.lb_name.Text = "橙子兄弟";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::X.Wx.Properties.Resources.m1;
            this.pictureBox2.Location = new System.Drawing.Point(237, 19);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(30, 30);
            this.pictureBox2.TabIndex = 5;
            this.pictureBox2.TabStop = false;
            // 
            // pb_head
            // 
            this.pb_head.BackColor = System.Drawing.Color.White;
            this.pb_head.Image = global::X.Wx.Properties.Resources.no_u;
            this.pb_head.Location = new System.Drawing.Point(12, 12);
            this.pb_head.Name = "pb_head";
            this.pb_head.Size = new System.Drawing.Size(45, 45);
            this.pb_head.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pb_head.TabIndex = 3;
            this.pb_head.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.Controls.Add(this.textBox1);
            this.panel2.Location = new System.Drawing.Point(280, 396);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(8, 0, 8, 8);
            this.panel2.Size = new System.Drawing.Size(680, 224);
            this.panel2.TabIndex = 6;
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Location = new System.Drawing.Point(8, 0);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(664, 216);
            this.textBox1.TabIndex = 8;
            // 
            // lb_nickname
            // 
            this.lb_nickname.AutoSize = true;
            this.lb_nickname.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lb_nickname.Location = new System.Drawing.Point(593, 13);
            this.lb_nickname.Name = "lb_nickname";
            this.lb_nickname.Size = new System.Drawing.Size(74, 21);
            this.lb_nickname.TabIndex = 7;
            this.lb_nickname.Text = "橙子兄弟";
            // 
            // tb_msg
            // 
            this.tb_msg.Location = new System.Drawing.Point(286, 368);
            this.tb_msg.Name = "tb_msg";
            this.tb_msg.Size = new System.Drawing.Size(563, 21);
            this.tb_msg.TabIndex = 8;
            // 
            // bt_send
            // 
            this.bt_send.Location = new System.Drawing.Point(895, 367);
            this.bt_send.Name = "bt_send";
            this.bt_send.Size = new System.Drawing.Size(57, 23);
            this.bt_send.TabIndex = 9;
            this.bt_send.Text = "发送";
            this.bt_send.UseVisualStyleBackColor = true;
            this.bt_send.Click += new System.EventHandler(this.bt_send_Click);
            // 
            // bt_sendimg
            // 
            this.bt_sendimg.Location = new System.Drawing.Point(855, 367);
            this.bt_sendimg.Name = "bt_sendimg";
            this.bt_sendimg.Size = new System.Drawing.Size(34, 23);
            this.bt_sendimg.TabIndex = 9;
            this.bt_sendimg.Text = "+";
            this.bt_sendimg.UseVisualStyleBackColor = true;
            this.bt_sendimg.Click += new System.EventHandler(this.bt_sendimg_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(960, 620);
            this.Controls.Add(this.bt_sendimg);
            this.Controls.Add(this.bt_send);
            this.Controls.Add(this.tb_msg);
            this.Controls.Add(this.lb_nickname);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Main";
            this.Load += new System.EventHandler(this.Main_Load);
            this.Controls.SetChildIndex(this.panel1, 0);
            this.Controls.SetChildIndex(this.panel2, 0);
            this.Controls.SetChildIndex(this.lb_nickname, 0);
            this.Controls.SetChildIndex(this.tb_msg, 0);
            this.Controls.SetChildIndex(this.bt_send, 0);
            this.Controls.SetChildIndex(this.bt_sendimg, 0);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_head)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lb_name;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pb_head;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label lb_nickname;
        private System.Windows.Forms.TextBox tb_msg;
        private System.Windows.Forms.Button bt_send;
        private System.Windows.Forms.Button bt_sendimg;
    }
}
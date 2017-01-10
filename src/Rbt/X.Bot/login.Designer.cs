namespace X.Bot
{
    partial class Login
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
            this.pb_headimg = new System.Windows.Forms.PictureBox();
            this.lb_tip = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pb_headimg)).BeginInit();
            this.SuspendLayout();
            // 
            // lb_title
            // 
            this.lb_title.Location = new System.Drawing.Point(8, 8);
            this.lb_title.Size = new System.Drawing.Size(90, 21);
            this.lb_title.Text = "微信机器人";
            // 
            // pb_headimg
            // 
            this.pb_headimg.BackColor = System.Drawing.SystemColors.Control;
            this.pb_headimg.ImageLocation = "";
            this.pb_headimg.Location = new System.Drawing.Point(40, 84);
            this.pb_headimg.Name = "pb_headimg";
            this.pb_headimg.Size = new System.Drawing.Size(130, 130);
            this.pb_headimg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pb_headimg.TabIndex = 1;
            this.pb_headimg.TabStop = false;
            // 
            // lb_tip
            // 
            this.lb_tip.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lb_tip.Location = new System.Drawing.Point(12, 246);
            this.lb_tip.Name = "lb_tip";
            this.lb_tip.Size = new System.Drawing.Size(187, 49);
            this.lb_tip.TabIndex = 3;
            this.lb_tip.Text = "扫码登陆你的帐号";
            this.lb_tip.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.linkLabel1.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLabel1.LinkColor = System.Drawing.Color.Gray;
            this.linkLabel1.Location = new System.Drawing.Point(66, 310);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(78, 17);
            this.linkLabel1.TabIndex = 5;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "© 80xc.com";
            // 
            // Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(211, 344);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.lb_tip);
            this.Controls.Add(this.pb_headimg);
            this.Name = "Login";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "微信机器人";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Login_FormClosing);
            this.Load += new System.EventHandler(this.Login_Load);
            this.Controls.SetChildIndex(this.pb_headimg, 0);
            this.Controls.SetChildIndex(this.lb_tip, 0);
            this.Controls.SetChildIndex(this.lb_title, 0);
            this.Controls.SetChildIndex(this.linkLabel1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pb_headimg)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pb_headimg;
        private System.Windows.Forms.Label lb_tip;
        private System.Windows.Forms.LinkLabel linkLabel1;
    }
}
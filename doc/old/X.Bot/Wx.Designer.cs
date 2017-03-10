﻿namespace X.Bot
{
    partial class Wx
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Wx));
            this.pb_head = new System.Windows.Forms.PictureBox();
            this.pb_close = new System.Windows.Forms.PictureBox();
            this.sp_c = new System.Windows.Forms.SplitContainer();
            this.tv_contact = new System.Windows.Forms.TreeView();
            this.sp_p = new System.Windows.Forms.SplitContainer();
            this.gp_msg = new System.Windows.Forms.GroupBox();
            this.pb_msg_close = new System.Windows.Forms.PictureBox();
            this.bt_send = new System.Windows.Forms.Button();
            this.bt_send_pic = new System.Windows.Forms.Button();
            this.tb_msg = new System.Windows.Forms.TextBox();
            this.tb_log = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pb_head)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_close)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sp_c)).BeginInit();
            this.sp_c.Panel1.SuspendLayout();
            this.sp_c.Panel2.SuspendLayout();
            this.sp_c.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sp_p)).BeginInit();
            this.sp_p.Panel1.SuspendLayout();
            this.sp_p.Panel2.SuspendLayout();
            this.sp_p.SuspendLayout();
            this.gp_msg.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_msg_close)).BeginInit();
            this.SuspendLayout();
            // 
            // lb_title
            // 
            this.lb_title.AutoEllipsis = true;
            this.lb_title.AutoSize = false;
            this.lb_title.Location = new System.Drawing.Point(49, 13);
            this.lb_title.Size = new System.Drawing.Size(95, 25);
            this.lb_title.Text = "查看日志";
            this.lb_title.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lb_title.DoubleClick += new System.EventHandler(this.lb_title_DoubleClick);
            // 
            // pb_head
            // 
            this.pb_head.BackColor = System.Drawing.Color.White;
            this.pb_head.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pb_head.Image = ((System.Drawing.Image)(resources.GetObject("pb_head.Image")));
            this.pb_head.Location = new System.Drawing.Point(8, 8);
            this.pb_head.Name = "pb_head";
            this.pb_head.Size = new System.Drawing.Size(35, 35);
            this.pb_head.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pb_head.TabIndex = 5;
            this.pb_head.TabStop = false;
            this.pb_head.Click += new System.EventHandler(this.pb_head_Click);
            // 
            // pb_close
            // 
            this.pb_close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pb_close.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pb_close.Image = global::X.Bot.Properties.Resources.x;
            this.pb_close.Location = new System.Drawing.Point(395, 8);
            this.pb_close.Name = "pb_close";
            this.pb_close.Size = new System.Drawing.Size(30, 30);
            this.pb_close.TabIndex = 8;
            this.pb_close.TabStop = false;
            this.pb_close.Click += new System.EventHandler(this.pb_close_Click);
            // 
            // sp_c
            // 
            this.sp_c.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sp_c.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.sp_c.Location = new System.Drawing.Point(8, 49);
            this.sp_c.Name = "sp_c";
            // 
            // sp_c.Panel1
            // 
            this.sp_c.Panel1.Controls.Add(this.tv_contact);
            this.sp_c.Panel1Collapsed = true;
            // 
            // sp_c.Panel2
            // 
            this.sp_c.Panel2.Controls.Add(this.sp_p);
            this.sp_c.Size = new System.Drawing.Size(417, 269);
            this.sp_c.SplitterDistance = 132;
            this.sp_c.TabIndex = 9;
            // 
            // tv_contact
            // 
            this.tv_contact.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tv_contact.Location = new System.Drawing.Point(0, 0);
            this.tv_contact.Name = "tv_contact";
            this.tv_contact.Size = new System.Drawing.Size(132, 100);
            this.tv_contact.TabIndex = 12;
            this.tv_contact.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tv_contact_NodeMouseDoubleClick);
            // 
            // sp_p
            // 
            this.sp_p.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sp_p.Location = new System.Drawing.Point(0, 0);
            this.sp_p.Name = "sp_p";
            this.sp_p.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // sp_p.Panel1
            // 
            this.sp_p.Panel1.Controls.Add(this.gp_msg);
            this.sp_p.Panel1Collapsed = true;
            // 
            // sp_p.Panel2
            // 
            this.sp_p.Panel2.Controls.Add(this.tb_log);
            this.sp_p.Size = new System.Drawing.Size(417, 269);
            this.sp_p.SplitterDistance = 85;
            this.sp_p.TabIndex = 0;
            // 
            // gp_msg
            // 
            this.gp_msg.Controls.Add(this.pb_msg_close);
            this.gp_msg.Controls.Add(this.bt_send);
            this.gp_msg.Controls.Add(this.bt_send_pic);
            this.gp_msg.Controls.Add(this.tb_msg);
            this.gp_msg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gp_msg.Location = new System.Drawing.Point(0, 0);
            this.gp_msg.Name = "gp_msg";
            this.gp_msg.Size = new System.Drawing.Size(150, 85);
            this.gp_msg.TabIndex = 0;
            this.gp_msg.TabStop = false;
            this.gp_msg.Text = "橙子兄弟";
            // 
            // pb_msg_close
            // 
            this.pb_msg_close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pb_msg_close.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pb_msg_close.Image = global::X.Bot.Properties.Resources.x;
            this.pb_msg_close.Location = new System.Drawing.Point(133, -3);
            this.pb_msg_close.Name = "pb_msg_close";
            this.pb_msg_close.Size = new System.Drawing.Size(20, 20);
            this.pb_msg_close.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pb_msg_close.TabIndex = 10;
            this.pb_msg_close.TabStop = false;
            this.pb_msg_close.Click += new System.EventHandler(this.pb_msg_close_Click);
            // 
            // bt_send
            // 
            this.bt_send.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bt_send.Location = new System.Drawing.Point(84, 56);
            this.bt_send.Name = "bt_send";
            this.bt_send.Size = new System.Drawing.Size(60, 23);
            this.bt_send.TabIndex = 2;
            this.bt_send.Text = "发送";
            this.bt_send.UseVisualStyleBackColor = true;
            this.bt_send.Click += new System.EventHandler(this.bt_send_Click);
            // 
            // bt_send_pic
            // 
            this.bt_send_pic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bt_send_pic.Location = new System.Drawing.Point(6, 56);
            this.bt_send_pic.Name = "bt_send_pic";
            this.bt_send_pic.Size = new System.Drawing.Size(24, 23);
            this.bt_send_pic.TabIndex = 2;
            this.bt_send_pic.Text = "+";
            this.bt_send_pic.UseVisualStyleBackColor = true;
            this.bt_send_pic.Click += new System.EventHandler(this.bt_send_pic_Click);
            // 
            // tb_msg
            // 
            this.tb_msg.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_msg.Location = new System.Drawing.Point(6, 20);
            this.tb_msg.Multiline = true;
            this.tb_msg.Name = "tb_msg";
            this.tb_msg.Size = new System.Drawing.Size(138, 30);
            this.tb_msg.TabIndex = 1;
            // 
            // tb_log
            // 
            this.tb_log.AutoWordSelection = true;
            this.tb_log.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tb_log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tb_log.Location = new System.Drawing.Point(0, 0);
            this.tb_log.Name = "tb_log";
            this.tb_log.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
            this.tb_log.Size = new System.Drawing.Size(417, 269);
            this.tb_log.TabIndex = 11;
            this.tb_log.Text = "";
            this.tb_log.WordWrap = false;
            // 
            // Wx
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(433, 327);
            this.Controls.Add(this.sp_c);
            this.Controls.Add(this.pb_close);
            this.Controls.Add(this.pb_head);
            this.Name = "Wx";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "查看日志";
            this.Controls.SetChildIndex(this.pb_head, 0);
            this.Controls.SetChildIndex(this.lb_title, 0);
            this.Controls.SetChildIndex(this.pb_close, 0);
            this.Controls.SetChildIndex(this.sp_c, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pb_head)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_close)).EndInit();
            this.sp_c.Panel1.ResumeLayout(false);
            this.sp_c.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.sp_c)).EndInit();
            this.sp_c.ResumeLayout(false);
            this.sp_p.Panel1.ResumeLayout(false);
            this.sp_p.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.sp_p)).EndInit();
            this.sp_p.ResumeLayout(false);
            this.gp_msg.ResumeLayout(false);
            this.gp_msg.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_msg_close)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PictureBox pb_head;
        private System.Windows.Forms.PictureBox pb_close;
        private System.Windows.Forms.SplitContainer sp_c;
        private System.Windows.Forms.TreeView tv_contact;
        private System.Windows.Forms.SplitContainer sp_p;
        private System.Windows.Forms.RichTextBox tb_log;
        private System.Windows.Forms.GroupBox gp_msg;
        private System.Windows.Forms.TextBox tb_msg;
        private System.Windows.Forms.Button bt_send;
        private System.Windows.Forms.Button bt_send_pic;
        private System.Windows.Forms.PictureBox pb_msg_close;
    }
}
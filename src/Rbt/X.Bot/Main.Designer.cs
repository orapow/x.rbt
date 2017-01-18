namespace X.Bot
{
    partial class Main
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.ni_tip = new System.Windows.Forms.NotifyIcon(this.components);
            this.cms_notify = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cms_mi_newwx = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.cms_mi_showmain = new System.Windows.Forms.ToolStripMenuItem();
            this.cms_mi_manager = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.cms_mi_setting = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.cms_mi_exit = new System.Windows.Forms.ToolStripMenuItem();
            this.ss_bottom = new System.Windows.Forms.StatusStrip();
            this.tsb_cp = new System.Windows.Forms.ToolStripStatusLabel();
            this.fp_wxs = new System.Windows.Forms.FlowLayoutPanel();
            this.pb_newwx = new System.Windows.Forms.PictureBox();
            this.cms_user = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cms_user_name = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.cm_us_exit = new System.Windows.Forms.ToolStripMenuItem();
            this.pb_head = new System.Windows.Forms.PictureBox();
            this.lb_vip = new System.Windows.Forms.Label();
            this.tip_info = new System.Windows.Forms.ToolTip(this.components);
            this.cms_notify.SuspendLayout();
            this.ss_bottom.SuspendLayout();
            this.fp_wxs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_newwx)).BeginInit();
            this.cms_user.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_head)).BeginInit();
            this.SuspendLayout();
            // 
            // lb_title
            // 
            this.lb_title.AutoEllipsis = true;
            this.lb_title.AutoSize = false;
            this.lb_title.Location = new System.Drawing.Point(59, 11);
            this.lb_title.Size = new System.Drawing.Size(103, 21);
            this.lb_title.Text = "微信机器人";
            // 
            // ni_tip
            // 
            this.ni_tip.ContextMenuStrip = this.cms_notify;
            this.ni_tip.Icon = ((System.Drawing.Icon)(resources.GetObject("ni_tip.Icon")));
            this.ni_tip.Text = "微信机器人";
            this.ni_tip.Visible = true;
            // 
            // cms_notify
            // 
            this.cms_notify.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cms_mi_newwx,
            this.toolStripSeparator3,
            this.cms_mi_showmain,
            this.cms_mi_manager,
            this.toolStripSeparator2,
            this.cms_mi_setting,
            this.toolStripSeparator5,
            this.cms_mi_exit});
            this.cms_notify.Name = "cms_user";
            this.cms_notify.Size = new System.Drawing.Size(142, 132);
            // 
            // cms_mi_newwx
            // 
            this.cms_mi_newwx.Name = "cms_mi_newwx";
            this.cms_mi_newwx.Size = new System.Drawing.Size(141, 22);
            this.cms_mi_newwx.Text = "登陆微信号";
            this.cms_mi_newwx.Click += new System.EventHandler(this.pb_newwx_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(138, 6);
            // 
            // cms_mi_showmain
            // 
            this.cms_mi_showmain.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cms_mi_showmain.Name = "cms_mi_showmain";
            this.cms_mi_showmain.Size = new System.Drawing.Size(141, 22);
            this.cms_mi_showmain.Text = "主界面(&M)";
            this.cms_mi_showmain.Click += new System.EventHandler(this.cms_mi_showmain_Click);
            // 
            // cms_mi_manager
            // 
            this.cms_mi_manager.Name = "cms_mi_manager";
            this.cms_mi_manager.Size = new System.Drawing.Size(141, 22);
            this.cms_mi_manager.Text = "管理中心(&G)";
            this.cms_mi_manager.Click += new System.EventHandler(this.bt_usercenter_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(138, 6);
            // 
            // cms_mi_setting
            // 
            this.cms_mi_setting.Name = "cms_mi_setting";
            this.cms_mi_setting.Size = new System.Drawing.Size(141, 22);
            this.cms_mi_setting.Text = "设置(&S)";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(138, 6);
            // 
            // cms_mi_exit
            // 
            this.cms_mi_exit.Name = "cms_mi_exit";
            this.cms_mi_exit.Size = new System.Drawing.Size(141, 22);
            this.cms_mi_exit.Text = "退出(&Q)";
            this.cms_mi_exit.Click += new System.EventHandler(this.cms_mi_exit_Click);
            // 
            // ss_bottom
            // 
            this.ss_bottom.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsb_cp});
            this.ss_bottom.Location = new System.Drawing.Point(0, 335);
            this.ss_bottom.Name = "ss_bottom";
            this.ss_bottom.Size = new System.Drawing.Size(242, 22);
            this.ss_bottom.SizingGrip = false;
            this.ss_bottom.TabIndex = 4;
            // 
            // tsb_cp
            // 
            this.tsb_cp.BackColor = System.Drawing.Color.Transparent;
            this.tsb_cp.IsLink = true;
            this.tsb_cp.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.tsb_cp.LinkColor = System.Drawing.Color.Silver;
            this.tsb_cp.Name = "tsb_cp";
            this.tsb_cp.Size = new System.Drawing.Size(227, 17);
            this.tsb_cp.Spring = true;
            this.tsb_cp.Text = "© 80xc.com";
            this.tsb_cp.ToolTipText = "欢迎访问官网";
            // 
            // fp_wxs
            // 
            this.fp_wxs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fp_wxs.Controls.Add(this.pb_newwx);
            this.fp_wxs.Location = new System.Drawing.Point(8, 56);
            this.fp_wxs.Margin = new System.Windows.Forms.Padding(0);
            this.fp_wxs.Name = "fp_wxs";
            this.fp_wxs.Size = new System.Drawing.Size(234, 265);
            this.fp_wxs.TabIndex = 1;
            this.fp_wxs.MouseDown += new System.Windows.Forms.MouseEventHandler(this.fp_wxs_MouseDown);
            // 
            // pb_newwx
            // 
            this.pb_newwx.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pb_newwx.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pb_newwx.Image = global::X.Bot.Properties.Resources._;
            this.pb_newwx.Location = new System.Drawing.Point(0, 0);
            this.pb_newwx.Margin = new System.Windows.Forms.Padding(0, 0, 5, 5);
            this.pb_newwx.Name = "pb_newwx";
            this.pb_newwx.Size = new System.Drawing.Size(45, 45);
            this.pb_newwx.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pb_newwx.TabIndex = 0;
            this.pb_newwx.TabStop = false;
            this.tip_info.SetToolTip(this.pb_newwx, "点击登陆微信号");
            this.pb_newwx.Click += new System.EventHandler(this.pb_newwx_Click);
            // 
            // cms_user
            // 
            this.cms_user.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cms_user_name,
            this.toolStripMenuItem1,
            this.toolStripSeparator4,
            this.cm_us_exit});
            this.cms_user.Name = "cms_user";
            this.cms_user.Size = new System.Drawing.Size(139, 76);
            this.cms_user.Opening += new System.ComponentModel.CancelEventHandler(this.cms_user_Opening);
            // 
            // cms_user_name
            // 
            this.cms_user_name.Enabled = false;
            this.cms_user_name.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cms_user_name.Name = "cms_user_name";
            this.cms_user_name.Size = new System.Drawing.Size(138, 22);
            this.cms_user_name.Text = "昵称";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(138, 22);
            this.toolStripMenuItem1.Text = "打开界面(&F)";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(135, 6);
            // 
            // cm_us_exit
            // 
            this.cm_us_exit.Name = "cm_us_exit";
            this.cm_us_exit.Size = new System.Drawing.Size(138, 22);
            this.cm_us_exit.Text = "退出(&O)";
            this.cm_us_exit.Click += new System.EventHandler(this.cm_us_exit_Click);
            // 
            // pb_head
            // 
            this.pb_head.Image = global::X.Bot.Properties.Resources.no_u;
            this.pb_head.Location = new System.Drawing.Point(8, 8);
            this.pb_head.Name = "pb_head";
            this.pb_head.Size = new System.Drawing.Size(45, 45);
            this.pb_head.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pb_head.TabIndex = 8;
            this.pb_head.TabStop = false;
            this.pb_head.Click += new System.EventHandler(this.pb_head_Click);
            // 
            // lb_vip
            // 
            this.lb_vip.AutoSize = true;
            this.lb_vip.ForeColor = System.Drawing.Color.Gray;
            this.lb_vip.Location = new System.Drawing.Point(61, 36);
            this.lb_vip.Name = "lb_vip";
            this.lb_vip.Size = new System.Drawing.Size(101, 12);
            this.lb_vip.TabIndex = 9;
            this.lb_vip.Text = "到期：2016-09-09";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(242, 357);
            this.Controls.Add(this.lb_vip);
            this.Controls.Add(this.pb_head);
            this.Controls.Add(this.fp_wxs);
            this.Controls.Add(this.ss_bottom);
            this.Name = "Main";
            this.Padding = new System.Windows.Forms.Padding(0, 45, 0, 0);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "微信机器人";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.Controls.SetChildIndex(this.ss_bottom, 0);
            this.Controls.SetChildIndex(this.fp_wxs, 0);
            this.Controls.SetChildIndex(this.pb_head, 0);
            this.Controls.SetChildIndex(this.lb_title, 0);
            this.Controls.SetChildIndex(this.lb_vip, 0);
            this.cms_notify.ResumeLayout(false);
            this.ss_bottom.ResumeLayout(false);
            this.ss_bottom.PerformLayout();
            this.fp_wxs.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pb_newwx)).EndInit();
            this.cms_user.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pb_head)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon ni_tip;
        private System.Windows.Forms.StatusStrip ss_bottom;
        private System.Windows.Forms.ToolStripStatusLabel tsb_cp;
        private System.Windows.Forms.FlowLayoutPanel fp_wxs;
        private System.Windows.Forms.PictureBox pb_newwx;
        private System.Windows.Forms.ContextMenuStrip cms_user;
        private System.Windows.Forms.ToolStripMenuItem cm_us_exit;
        private System.Windows.Forms.ContextMenuStrip cms_notify;
        private System.Windows.Forms.ToolStripMenuItem cms_mi_showmain;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem cms_mi_exit;
        private System.Windows.Forms.ToolStripMenuItem cms_mi_manager;
        private System.Windows.Forms.ToolStripMenuItem cms_mi_newwx;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.PictureBox pb_head;
        private System.Windows.Forms.ToolStripMenuItem cms_user_name;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.Label lb_vip;
        private System.Windows.Forms.ToolTip tip_info;
        private System.Windows.Forms.ToolStripMenuItem cms_mi_setting;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
    }
}


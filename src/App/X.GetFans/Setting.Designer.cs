namespace X.GetFans
{
    partial class Setting
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
            this.components = new System.ComponentModel.Container();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.pb_shpic = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tb_shtxt = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_audit = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tb_intxt = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.bt_ok = new System.Windows.Forms.Button();
            this.cb_debug = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tb_tosec = new System.Windows.Forms.TextBox();
            this.tb_newct = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tb_gname = new System.Windows.Forms.TextBox();
            this.tb_full_txt = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.tb_full_ct = new System.Windows.Forms.TextBox();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_shpic)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lb_title
            // 
            this.lb_title.Location = new System.Drawing.Point(7, 7);
            this.lb_title.Size = new System.Drawing.Size(50, 21);
            this.lb_title.Text = "Rules";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.pb_shpic);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.tb_shtxt);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.tb_audit);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.tb_full_txt);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.tb_intxt);
            this.groupBox4.Location = new System.Drawing.Point(11, 103);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(419, 362);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "素材设置";
            // 
            // pb_shpic
            // 
            this.pb_shpic.Location = new System.Drawing.Point(210, 130);
            this.pb_shpic.Name = "pb_shpic";
            this.pb_shpic.Size = new System.Drawing.Size(203, 155);
            this.pb_shpic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pb_shpic.TabIndex = 10;
            this.pb_shpic.TabStop = false;
            this.pb_shpic.Click += new System.EventHandler(this.pb_shpic_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(210, 114);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "转发图片：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(208, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 8;
            this.label1.Text = "转发文字：";
            // 
            // tb_shtxt
            // 
            this.tb_shtxt.Location = new System.Drawing.Point(210, 32);
            this.tb_shtxt.Multiline = true;
            this.tb_shtxt.Name = "tb_shtxt";
            this.tb_shtxt.Size = new System.Drawing.Size(203, 79);
            this.tb_shtxt.TabIndex = 9;
            this.tb_shtxt.Text = "我决定立即加入【21天完美瘦身计划】，组队对搞惰性，在新的一年里，每天30分钟。养成运动好习惯，成为期待中的自己！\r\n点开图图，长按扫描，立即报名！（限80人）";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 8;
            this.label2.Text = "审核文案：";
            // 
            // tb_audit
            // 
            this.tb_audit.Location = new System.Drawing.Point(8, 32);
            this.tb_audit.Multiline = true;
            this.tb_audit.Name = "tb_audit";
            this.tb_audit.Size = new System.Drawing.Size(196, 79);
            this.tb_audit.TabIndex = 9;
            this.tb_audit.Text = "@[发送人] 截图已经收到，系统会在2小时内检测朋友圈海报，未转发或秒删将会被清理出群哦！";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 114);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 8;
            this.label6.Text = "活动规则：";
            // 
            // tb_intxt
            // 
            this.tb_intxt.Location = new System.Drawing.Point(8, 130);
            this.tb_intxt.Multiline = true;
            this.tb_intxt.Name = "tb_intxt";
            this.tb_intxt.Size = new System.Drawing.Size(196, 155);
            this.tb_intxt.TabIndex = 9;
            this.tb_intxt.Text = "【活动规则】欢迎大家报名[活动名称]，一起组队对搞惰性，每天30分钟，恢复运动活力。\r\n【如何报名】请复制下方的文字和图片，分享到自己的朋友友圈，发布运动宣言。最" +
    "后将成朋友圈的截图发到群里。\r\n【活动奖励】群满80人后我们会核对大家是否转发，审核通过的朋友将获得留群一起交流学习的机会。\r\n----------------" +
    "-----\r\n朋友圈转发内容及配图如下↓↓↓";
            this.toolTip1.SetToolTip(this.tb_intxt, "内容识别成功时回复给经纪人，为空时不回复");
            // 
            // bt_ok
            // 
            this.bt_ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bt_ok.Location = new System.Drawing.Point(337, 8);
            this.bt_ok.Name = "bt_ok";
            this.bt_ok.Size = new System.Drawing.Size(57, 30);
            this.bt_ok.TabIndex = 4;
            this.bt_ok.Text = "确定";
            this.toolTip1.SetToolTip(this.bt_ok, "保存并更新所有配置");
            this.bt_ok.UseVisualStyleBackColor = true;
            this.bt_ok.Click += new System.EventHandler(this.bt_ok_Click);
            // 
            // cb_debug
            // 
            this.cb_debug.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cb_debug.AutoSize = true;
            this.cb_debug.Location = new System.Drawing.Point(260, 16);
            this.cb_debug.Name = "cb_debug";
            this.cb_debug.Size = new System.Drawing.Size(72, 16);
            this.cb_debug.TabIndex = 9;
            this.cb_debug.Text = "调试模式";
            this.toolTip1.SetToolTip(this.cb_debug, "开启调式模式将记录所有日志");
            this.cb_debug.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tb_tosec);
            this.groupBox1.Controls.Add(this.tb_full_ct);
            this.groupBox1.Controls.Add(this.tb_newct);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Location = new System.Drawing.Point(11, 471);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(419, 82);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "发送设置";
            // 
            // tb_tosec
            // 
            this.tb_tosec.Location = new System.Drawing.Point(232, 24);
            this.tb_tosec.Name = "tb_tosec";
            this.tb_tosec.Size = new System.Drawing.Size(41, 21);
            this.tb_tosec.TabIndex = 16;
            // 
            // tb_newct
            // 
            this.tb_newct.Location = new System.Drawing.Point(90, 24);
            this.tb_newct.Name = "tb_newct";
            this.tb_newct.Size = new System.Drawing.Size(41, 21);
            this.tb_newct.TabIndex = 17;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(279, 28);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(29, 12);
            this.label9.TabIndex = 12;
            this.label9.Text = "秒钟";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(137, 28);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(89, 12);
            this.label7.TabIndex = 13;
            this.label7.Text = "人或时间超过：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(18, 28);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 14;
            this.label5.Text = "新成员满：";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tb_gname);
            this.groupBox2.Location = new System.Drawing.Point(11, 44);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(419, 53);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "群名称";
            // 
            // tb_gname
            // 
            this.tb_gname.Location = new System.Drawing.Point(6, 20);
            this.tb_gname.Name = "tb_gname";
            this.tb_gname.Size = new System.Drawing.Size(407, 21);
            this.tb_gname.TabIndex = 0;
            // 
            // tb_full_txt
            // 
            this.tb_full_txt.Location = new System.Drawing.Point(8, 304);
            this.tb_full_txt.Multiline = true;
            this.tb_full_txt.Name = "tb_full_txt";
            this.tb_full_txt.Size = new System.Drawing.Size(405, 52);
            this.tb_full_txt.TabIndex = 9;
            this.tb_full_txt.Text = "请大家尽快按流程获取报名资格，群已满80人，我们现在开始审核，稍后要清理未转发的人哦～谢谢配合～";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 288);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "满员文案：";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(18, 55);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 12);
            this.label8.TabIndex = 14;
            this.label8.Text = "满员人数：";
            // 
            // tb_full_ct
            // 
            this.tb_full_ct.Location = new System.Drawing.Point(90, 51);
            this.tb_full_ct.Name = "tb_full_ct";
            this.tb_full_ct.Size = new System.Drawing.Size(41, 21);
            this.tb_full_ct.TabIndex = 17;
            this.tb_full_ct.Text = "80";
            // 
            // Setting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(438, 560);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cb_debug);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.bt_ok);
            this.Name = "Setting";
            this.Text = "Rules";
            this.Load += new System.EventHandler(this.Rules_Load);
            this.Controls.SetChildIndex(this.bt_ok, 0);
            this.Controls.SetChildIndex(this.groupBox4, 0);
            this.Controls.SetChildIndex(this.cb_debug, 0);
            this.Controls.SetChildIndex(this.lb_title, 0);
            this.Controls.SetChildIndex(this.groupBox1, 0);
            this.Controls.SetChildIndex(this.groupBox2, 0);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_shpic)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button bt_ok;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tb_intxt;
        private System.Windows.Forms.CheckBox cb_debug;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tb_audit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tb_shtxt;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox pb_shpic;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tb_tosec;
        private System.Windows.Forms.TextBox tb_newct;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox tb_gname;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tb_full_txt;
        private System.Windows.Forms.TextBox tb_full_ct;
        private System.Windows.Forms.Label label8;
    }
}
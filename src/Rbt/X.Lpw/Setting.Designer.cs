namespace X.Lpw
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Setting));
            this.bt_save = new System.Windows.Forms.Button();
            this.bt_remove = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lb_rules = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tb_gname = new System.Windows.Forms.TextBox();
            this.tb_bname = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tb_api = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lb_looks = new System.Windows.Forms.ListBox();
            this.bt_select_contact = new System.Windows.Forms.Button();
            this.tb_keys = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cb_send_on_fail = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tb_id_succ = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.tb_online = new System.Windows.Forms.TextBox();
            this.tb_tpl = new System.Windows.Forms.TextBox();
            this.tb_send_succ = new System.Windows.Forms.TextBox();
            this.tb_id_fail = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.bt_ok = new System.Windows.Forms.Button();
            this.cb_debug = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.cb_c = new System.Windows.Forms.ComboBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.tb_sms = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // lb_title
            // 
            this.lb_title.Location = new System.Drawing.Point(7, 7);
            this.lb_title.Size = new System.Drawing.Size(50, 21);
            this.lb_title.Text = "Rules";
            // 
            // bt_save
            // 
            this.bt_save.Location = new System.Drawing.Point(206, 20);
            this.bt_save.Name = "bt_save";
            this.bt_save.Size = new System.Drawing.Size(39, 21);
            this.bt_save.TabIndex = 4;
            this.bt_save.Text = "保存";
            this.toolTip1.SetToolTip(this.bt_save, "保存当前规则");
            this.bt_save.UseVisualStyleBackColor = true;
            this.bt_save.Click += new System.EventHandler(this.bt_save_Click);
            // 
            // bt_remove
            // 
            this.bt_remove.Location = new System.Drawing.Point(206, 47);
            this.bt_remove.Name = "bt_remove";
            this.bt_remove.Size = new System.Drawing.Size(39, 21);
            this.bt_remove.TabIndex = 4;
            this.bt_remove.Text = "删除";
            this.toolTip1.SetToolTip(this.bt_remove, "删除当前规则");
            this.bt_remove.UseVisualStyleBackColor = true;
            this.bt_remove.Click += new System.EventHandler(this.bt_remove_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lb_rules);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.tb_gname);
            this.groupBox1.Controls.Add(this.tb_bname);
            this.groupBox1.Controls.Add(this.bt_save);
            this.groupBox1.Controls.Add(this.bt_remove);
            this.groupBox1.Location = new System.Drawing.Point(639, 44);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(252, 304);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "转发规则";
            // 
            // lb_rules
            // 
            this.lb_rules.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_rules.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.lb_rules.FormattingEnabled = true;
            this.lb_rules.IntegralHeight = false;
            this.lb_rules.ItemHeight = 24;
            this.lb_rules.Location = new System.Drawing.Point(8, 74);
            this.lb_rules.Name = "lb_rules";
            this.lb_rules.Size = new System.Drawing.Size(237, 220);
            this.lb_rules.TabIndex = 7;
            this.toolTip1.SetToolTip(this.lb_rules, "规则列表，双击可快速删除");
            this.lb_rules.Click += new System.EventHandler(this.lb_rules_Click);
            this.lb_rules.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lb_rules_DrawItem);
            this.lb_rules.DoubleClick += new System.EventHandler(this.bt_remove_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "转发到：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "楼盘名：";
            // 
            // tb_gname
            // 
            this.tb_gname.Location = new System.Drawing.Point(65, 47);
            this.tb_gname.Name = "tb_gname";
            this.tb_gname.Size = new System.Drawing.Size(135, 21);
            this.tb_gname.TabIndex = 0;
            this.tb_gname.Enter += new System.EventHandler(this.tb_gname_Enter);
            // 
            // tb_bname
            // 
            this.tb_bname.Location = new System.Drawing.Point(65, 20);
            this.tb_bname.Name = "tb_bname";
            this.tb_bname.Size = new System.Drawing.Size(135, 21);
            this.tb_bname.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tb_api);
            this.groupBox2.Location = new System.Drawing.Point(11, 44);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(253, 52);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "网关配置：";
            // 
            // tb_api
            // 
            this.tb_api.Enabled = false;
            this.tb_api.Location = new System.Drawing.Point(6, 20);
            this.tb_api.Name = "tb_api";
            this.tb_api.Size = new System.Drawing.Size(239, 21);
            this.tb_api.TabIndex = 7;
            this.tb_api.Text = "http://ldgl.loupan.com:8093";
            this.toolTip1.SetToolTip(this.tb_api, "数据提交地址");
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lb_looks);
            this.groupBox3.Controls.Add(this.bt_select_contact);
            this.groupBox3.Controls.Add(this.tb_keys);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Location = new System.Drawing.Point(11, 163);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(253, 243);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "采集设置";
            // 
            // lb_looks
            // 
            this.lb_looks.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_looks.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.lb_looks.FormattingEnabled = true;
            this.lb_looks.IntegralHeight = false;
            this.lb_looks.ItemHeight = 24;
            this.lb_looks.Location = new System.Drawing.Point(6, 107);
            this.lb_looks.Name = "lb_looks";
            this.lb_looks.Size = new System.Drawing.Size(241, 128);
            this.lb_looks.TabIndex = 7;
            this.toolTip1.SetToolTip(this.lb_looks, "只监控以下选中的群或好友的消息。\\r\\n双击可删除");
            this.lb_looks.Click += new System.EventHandler(this.lb_rules_Click);
            this.lb_looks.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lb_looks_DrawItem);
            this.lb_looks.DoubleClick += new System.EventHandler(this.lb_looks_DoubleClick);
            // 
            // bt_select_contact
            // 
            this.bt_select_contact.BackColor = System.Drawing.Color.Transparent;
            this.bt_select_contact.Location = new System.Drawing.Point(227, 80);
            this.bt_select_contact.Name = "bt_select_contact";
            this.bt_select_contact.Size = new System.Drawing.Size(20, 21);
            this.bt_select_contact.TabIndex = 9;
            this.bt_select_contact.Text = "+";
            this.toolTip1.SetToolTip(this.bt_select_contact, "保存当前规则");
            this.bt_select_contact.UseVisualStyleBackColor = false;
            this.bt_select_contact.Click += new System.EventHandler(this.bt_select_contact_Click);
            // 
            // tb_keys
            // 
            this.tb_keys.Location = new System.Drawing.Point(65, 17);
            this.tb_keys.Multiline = true;
            this.tb_keys.Name = "tb_keys";
            this.tb_keys.Size = new System.Drawing.Size(182, 57);
            this.tb_keys.TabIndex = 7;
            this.tb_keys.Text = "客户电话 经纪人姓名 业务员 手机号码";
            this.toolTip1.SetToolTip(this.tb_keys, "遇到以下关键字之一就采集，多个关键字用空格隔开");
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 84);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "监控群或用户：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "关键字：";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cb_send_on_fail);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.tb_id_succ);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.textBox1);
            this.groupBox4.Controls.Add(this.tb_online);
            this.groupBox4.Controls.Add(this.tb_tpl);
            this.groupBox4.Controls.Add(this.tb_send_succ);
            this.groupBox4.Controls.Add(this.tb_id_fail);
            this.groupBox4.Location = new System.Drawing.Point(270, 44);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(363, 362);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "回复规则";
            // 
            // cb_send_on_fail
            // 
            this.cb_send_on_fail.AutoSize = true;
            this.cb_send_on_fail.Location = new System.Drawing.Point(285, 77);
            this.cb_send_on_fail.Name = "cb_send_on_fail";
            this.cb_send_on_fail.Size = new System.Drawing.Size(72, 16);
            this.cb_send_on_fail.TabIndex = 9;
            this.cb_send_on_fail.Text = "发送模板";
            this.toolTip1.SetToolTip(this.cb_send_on_fail, "识别失败时发送此模板");
            this.cb_send_on_fail.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 17);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 8;
            this.label6.Text = "识别成功：";
            // 
            // tb_id_succ
            // 
            this.tb_id_succ.Location = new System.Drawing.Point(6, 32);
            this.tb_id_succ.Multiline = true;
            this.tb_id_succ.Name = "tb_id_succ";
            this.tb_id_succ.Size = new System.Drawing.Size(172, 43);
            this.tb_id_succ.TabIndex = 9;
            this.tb_id_succ.Text = "@[发送人] 您的客户 [客户姓名] 已经识别，正在报备。[胜利]";
            this.toolTip1.SetToolTip(this.tb_id_succ, "内容识别成功时回复给经纪人，为空时不回复");
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 78);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 12);
            this.label7.TabIndex = 5;
            this.label7.Text = "报备成功：";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 139);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(77, 12);
            this.label8.TabIndex = 5;
            this.label8.Text = "机器人上线：";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(184, 78);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 12);
            this.label9.TabIndex = 5;
            this.label9.Text = "格式模板：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(184, 17);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 5;
            this.label5.Text = "识别失败：";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(6, 203);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(351, 151);
            this.textBox1.TabIndex = 7;
            this.textBox1.Text = resources.GetString("textBox1.Text");
            // 
            // tb_online
            // 
            this.tb_online.Location = new System.Drawing.Point(6, 154);
            this.tb_online.Multiline = true;
            this.tb_online.Name = "tb_online";
            this.tb_online.Size = new System.Drawing.Size(172, 43);
            this.tb_online.TabIndex = 7;
            this.tb_online.Text = "@[所有人] [城市]机器人已经上线，开始接受报备";
            // 
            // tb_tpl
            // 
            this.tb_tpl.Location = new System.Drawing.Point(184, 93);
            this.tb_tpl.Multiline = true;
            this.tb_tpl.Name = "tb_tpl";
            this.tb_tpl.Size = new System.Drawing.Size(171, 104);
            this.tb_tpl.TabIndex = 7;
            this.toolTip1.SetToolTip(this.tb_tpl, "识别失败后要发送的模板");
            // 
            // tb_send_succ
            // 
            this.tb_send_succ.Location = new System.Drawing.Point(6, 93);
            this.tb_send_succ.Multiline = true;
            this.tb_send_succ.Name = "tb_send_succ";
            this.tb_send_succ.Size = new System.Drawing.Size(172, 43);
            this.tb_send_succ.TabIndex = 7;
            this.tb_send_succ.Text = "@[发送人] 您的客户[客户姓名]，已经成功报备。[玫瑰][玫瑰][奋斗][奋斗]";
            this.toolTip1.SetToolTip(this.tb_send_succ, "已经转发到开发商时回复给经纪人，为空时不回复");
            // 
            // tb_id_fail
            // 
            this.tb_id_fail.Location = new System.Drawing.Point(184, 32);
            this.tb_id_fail.Multiline = true;
            this.tb_id_fail.Name = "tb_id_fail";
            this.tb_id_fail.Size = new System.Drawing.Size(171, 42);
            this.tb_id_fail.TabIndex = 7;
            this.tb_id_fail.Text = "@[发送人] 您的报备信息未能识别，请重新报备，错误信息：[错误信息]。[难过]";
            this.toolTip1.SetToolTip(this.tb_id_fail, "内容识别失败时回复给经纪人，为空时不回复");
            // 
            // bt_ok
            // 
            this.bt_ok.Location = new System.Drawing.Point(798, 8);
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
            this.cb_debug.AutoSize = true;
            this.cb_debug.Location = new System.Drawing.Point(720, 16);
            this.cb_debug.Name = "cb_debug";
            this.cb_debug.Size = new System.Drawing.Size(72, 16);
            this.cb_debug.TabIndex = 9;
            this.cb_debug.Text = "调试模式";
            this.toolTip1.SetToolTip(this.cb_debug, "开启调式模式将记录所有日志");
            this.cb_debug.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.cb_c);
            this.groupBox5.Location = new System.Drawing.Point(11, 102);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(253, 52);
            this.groupBox5.TabIndex = 6;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "城市：";
            // 
            // cb_c
            // 
            this.cb_c.FormattingEnabled = true;
            this.cb_c.Location = new System.Drawing.Point(6, 20);
            this.cb_c.Name = "cb_c";
            this.cb_c.Size = new System.Drawing.Size(120, 20);
            this.cb_c.TabIndex = 0;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.tb_sms);
            this.groupBox6.Location = new System.Drawing.Point(639, 354);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(253, 52);
            this.groupBox6.TabIndex = 6;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "警报通知";
            // 
            // tb_sms
            // 
            this.tb_sms.Enabled = false;
            this.tb_sms.Location = new System.Drawing.Point(6, 20);
            this.tb_sms.Name = "tb_sms";
            this.tb_sms.Size = new System.Drawing.Size(239, 21);
            this.tb_sms.TabIndex = 7;
            this.tb_sms.Text = "18073113871";
            this.toolTip1.SetToolTip(this.tb_sms, "通知电话号码，多个用“;”隔开");
            // 
            // Setting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(899, 414);
            this.Controls.Add(this.cb_debug);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.bt_ok);
            this.Controls.Add(this.groupBox1);
            this.Name = "Setting";
            this.Text = "Rules";
            this.Load += new System.EventHandler(this.Rules_Load);
            this.Controls.SetChildIndex(this.groupBox1, 0);
            this.Controls.SetChildIndex(this.bt_ok, 0);
            this.Controls.SetChildIndex(this.groupBox2, 0);
            this.Controls.SetChildIndex(this.groupBox6, 0);
            this.Controls.SetChildIndex(this.groupBox5, 0);
            this.Controls.SetChildIndex(this.groupBox3, 0);
            this.Controls.SetChildIndex(this.groupBox4, 0);
            this.Controls.SetChildIndex(this.lb_title, 0);
            this.Controls.SetChildIndex(this.cb_debug, 0);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button bt_save;
        private System.Windows.Forms.Button bt_remove;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tb_bname;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox tb_api;
        private System.Windows.Forms.ListBox lb_rules;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tb_keys;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tb_id_fail;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tb_send_succ;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button bt_ok;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.ComboBox cb_c;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tb_online;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tb_tpl;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tb_id_succ;
        private System.Windows.Forms.CheckBox cb_send_on_fail;
        private System.Windows.Forms.CheckBox cb_debug;
        private System.Windows.Forms.Button bt_select_contact;
        private System.Windows.Forms.TextBox tb_gname;
        private System.Windows.Forms.ListBox lb_looks;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TextBox tb_sms;
    }
}
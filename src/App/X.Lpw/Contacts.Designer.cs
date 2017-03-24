namespace X.Lpw
{
    partial class Contacts
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
            this.lb_list = new System.Windows.Forms.ListBox();
            this.lb_key = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.bt_ok = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lb_title
            // 
            this.lb_title.Size = new System.Drawing.Size(77, 21);
            this.lb_title.Text = "Contacts";
            // 
            // lb_list
            // 
            this.lb_list.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_list.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.lb_list.FormattingEnabled = true;
            this.lb_list.IntegralHeight = false;
            this.lb_list.ItemHeight = 24;
            this.lb_list.Location = new System.Drawing.Point(12, 73);
            this.lb_list.Name = "lb_list";
            this.lb_list.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lb_list.Size = new System.Drawing.Size(249, 284);
            this.lb_list.TabIndex = 3;
            this.toolTip1.SetToolTip(this.lb_list, "双击选定当前项，按Ctrl多选");
            this.lb_list.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lb_list_DrawItem);
            this.lb_list.DoubleClick += new System.EventHandler(this.lb_list_DoubleClick);
            // 
            // lb_key
            // 
            this.lb_key.Location = new System.Drawing.Point(12, 46);
            this.lb_key.Name = "lb_key";
            this.lb_key.Size = new System.Drawing.Size(249, 21);
            this.lb_key.TabIndex = 4;
            this.toolTip1.SetToolTip(this.lb_key, "输入关键字进行检索");
            this.lb_key.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lb_key_KeyUp);
            // 
            // bt_ok
            // 
            this.bt_ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bt_ok.Location = new System.Drawing.Point(172, 8);
            this.bt_ok.Name = "bt_ok";
            this.bt_ok.Size = new System.Drawing.Size(57, 30);
            this.bt_ok.TabIndex = 5;
            this.bt_ok.Text = "确定";
            this.toolTip1.SetToolTip(this.bt_ok, "保存并更新所有配置");
            this.bt_ok.UseVisualStyleBackColor = true;
            this.bt_ok.Click += new System.EventHandler(this.bt_ok_Click);
            // 
            // Contacts
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(273, 369);
            this.Controls.Add(this.bt_ok);
            this.Controls.Add(this.lb_key);
            this.Controls.Add(this.lb_list);
            this.Name = "Contacts";
            this.Text = "Contacts";
            this.Controls.SetChildIndex(this.lb_title, 0);
            this.Controls.SetChildIndex(this.lb_list, 0);
            this.Controls.SetChildIndex(this.lb_key, 0);
            this.Controls.SetChildIndex(this.bt_ok, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lb_list;
        private System.Windows.Forms.TextBox lb_key;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button bt_ok;
    }
}
namespace X.Wx
{
    partial class Akey
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
            this.label1 = new System.Windows.Forms.Label();
            this.tb_key = new System.Windows.Forms.TextBox();
            this.bt_get = new System.Windows.Forms.Button();
            this.bt_ok = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lb_title
            // 
            this.lb_title.Size = new System.Drawing.Size(69, 21);
            this.lb_title.Text = "获取Key";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label1.Location = new System.Drawing.Point(12, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(191, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "此Key用于加密你的数据和使用凭据";
            // 
            // tb_key
            // 
            this.tb_key.Location = new System.Drawing.Point(12, 62);
            this.tb_key.Multiline = true;
            this.tb_key.Name = "tb_key";
            this.tb_key.Size = new System.Drawing.Size(259, 86);
            this.tb_key.TabIndex = 4;
            // 
            // bt_get
            // 
            this.bt_get.Location = new System.Drawing.Point(12, 154);
            this.bt_get.Name = "bt_get";
            this.bt_get.Size = new System.Drawing.Size(60, 28);
            this.bt_get.TabIndex = 5;
            this.bt_get.Text = "获取";
            this.bt_get.UseVisualStyleBackColor = true;
            this.bt_get.Click += new System.EventHandler(this.bt_get_Click);
            // 
            // bt_ok
            // 
            this.bt_ok.Location = new System.Drawing.Point(211, 154);
            this.bt_ok.Name = "bt_ok";
            this.bt_ok.Size = new System.Drawing.Size(60, 28);
            this.bt_ok.TabIndex = 5;
            this.bt_ok.Text = "确定";
            this.bt_ok.UseVisualStyleBackColor = true;
            this.bt_ok.Click += new System.EventHandler(this.bt_ok_Click);
            // 
            // Akey
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(279, 194);
            this.Controls.Add(this.bt_ok);
            this.Controls.Add(this.bt_get);
            this.Controls.Add(this.tb_key);
            this.Controls.Add(this.label1);
            this.Name = "Akey";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "获取Key";
            this.Controls.SetChildIndex(this.lb_title, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.tb_key, 0);
            this.Controls.SetChildIndex(this.bt_get, 0);
            this.Controls.SetChildIndex(this.bt_ok, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tb_key;
        private System.Windows.Forms.Button bt_get;
        private System.Windows.Forms.Button bt_ok;
    }
}
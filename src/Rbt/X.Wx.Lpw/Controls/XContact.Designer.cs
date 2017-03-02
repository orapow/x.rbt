namespace X.Wx.Controls
{
    partial class XContact
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.lb_name = new System.Windows.Forms.Label();
            this.lb_msg = new System.Windows.Forms.Label();
            this.lb_bag = new System.Windows.Forms.Label();
            this.lb_dot = new System.Windows.Forms.Label();
            this.pb_image = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pb_image)).BeginInit();
            this.SuspendLayout();
            // 
            // lb_name
            // 
            this.lb_name.AutoSize = true;
            this.lb_name.BackColor = System.Drawing.Color.Transparent;
            this.lb_name.Font = new System.Drawing.Font("微软雅黑 Light", 12F);
            this.lb_name.ForeColor = System.Drawing.Color.White;
            this.lb_name.Location = new System.Drawing.Point(58, 12);
            this.lb_name.Name = "lb_name";
            this.lb_name.Size = new System.Drawing.Size(74, 21);
            this.lb_name.TabIndex = 1;
            this.lb_name.Text = "橙子兄弟";
            // 
            // lb_msg
            // 
            this.lb_msg.AutoSize = true;
            this.lb_msg.BackColor = System.Drawing.Color.Transparent;
            this.lb_msg.ForeColor = System.Drawing.Color.Gray;
            this.lb_msg.Location = new System.Drawing.Point(58, 37);
            this.lb_msg.Name = "lb_msg";
            this.lb_msg.Size = new System.Drawing.Size(125, 12);
            this.lb_msg.TabIndex = 2;
            this.lb_msg.Text = "[5条] @橙子兄弟 你好";
            // 
            // lb_bag
            // 
            this.lb_bag.BackColor = System.Drawing.Color.Transparent;
            this.lb_bag.ForeColor = System.Drawing.Color.White;
            this.lb_bag.Image = global::X.Wx.Properties.Resources.bag;
            this.lb_bag.Location = new System.Drawing.Point(41, 2);
            this.lb_bag.Margin = new System.Windows.Forms.Padding(0);
            this.lb_bag.Name = "lb_bag";
            this.lb_bag.Size = new System.Drawing.Size(20, 20);
            this.lb_bag.TabIndex = 3;
            this.lb_bag.Text = "99";
            this.lb_bag.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lb_bag.Visible = false;
            // 
            // lb_dot
            // 
            this.lb_dot.BackColor = System.Drawing.Color.Transparent;
            this.lb_dot.ForeColor = System.Drawing.Color.White;
            this.lb_dot.Image = global::X.Wx.Properties.Resources.dot;
            this.lb_dot.Location = new System.Drawing.Point(45, 6);
            this.lb_dot.Margin = new System.Windows.Forms.Padding(0);
            this.lb_dot.Name = "lb_dot";
            this.lb_dot.Size = new System.Drawing.Size(12, 12);
            this.lb_dot.TabIndex = 3;
            this.lb_dot.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lb_dot.Visible = false;
            // 
            // pb_image
            // 
            this.pb_image.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pb_image.ErrorImage = global::X.Wx.Properties.Resources.no_u;
            this.pb_image.Location = new System.Drawing.Point(12, 12);
            this.pb_image.Name = "pb_image";
            this.pb_image.Size = new System.Drawing.Size(40, 40);
            this.pb_image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pb_image.TabIndex = 0;
            this.pb_image.TabStop = false;
            // 
            // Contact
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lb_bag);
            this.Controls.Add(this.lb_dot);
            this.Controls.Add(this.lb_msg);
            this.Controls.Add(this.lb_name);
            this.Controls.Add(this.pb_image);
            this.Name = "Contact";
            this.Padding = new System.Windows.Forms.Padding(12);
            this.Size = new System.Drawing.Size(282, 62);
            ((System.ComponentModel.ISupportInitialize)(this.pb_image)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pb_image;
        private System.Windows.Forms.Label lb_name;
        private System.Windows.Forms.Label lb_msg;
        private System.Windows.Forms.Label lb_bag;
        private System.Windows.Forms.Label lb_dot;
    }
}

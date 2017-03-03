namespace X.Js
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
            ((System.ComponentModel.ISupportInitialize)(this.pb_headimg)).BeginInit();
            this.SuspendLayout();
            // 
            // lb_title
            // 
            this.lb_title.Text = "登陆";
            // 
            // pb_headimg
            // 
            this.pb_headimg.BackColor = System.Drawing.Color.Transparent;
            this.pb_headimg.Image = global::X.Js.Properties.Resources.loading;
            this.pb_headimg.Location = new System.Drawing.Point(38, 64);
            this.pb_headimg.Name = "pb_headimg";
            this.pb_headimg.Size = new System.Drawing.Size(130, 130);
            this.pb_headimg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pb_headimg.TabIndex = 1;
            this.pb_headimg.TabStop = false;
            // 
            // lb_tip
            // 
            this.lb_tip.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lb_tip.Location = new System.Drawing.Point(10, 213);
            this.lb_tip.Name = "lb_tip";
            this.lb_tip.Size = new System.Drawing.Size(186, 45);
            this.lb_tip.TabIndex = 2;
            this.lb_tip.Text = "正在加载二维码";
            this.lb_tip.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(206, 284);
            this.Controls.Add(this.lb_tip);
            this.Controls.Add(this.pb_headimg);
            this.MaximizeBox = false;
            this.Name = "Login";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "登陆";
            this.Controls.SetChildIndex(this.lb_title, 0);
            this.Controls.SetChildIndex(this.pb_headimg, 0);
            this.Controls.SetChildIndex(this.lb_tip, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pb_headimg)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pb_headimg;
        private System.Windows.Forms.Label lb_tip;
    }
}
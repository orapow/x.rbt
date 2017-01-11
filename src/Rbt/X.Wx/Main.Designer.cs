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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.pb_head = new System.Windows.Forms.PictureBox();
            this.ni_tip = new System.Windows.Forms.NotifyIcon(this.components);
            this.tb_log = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pb_head)).BeginInit();
            this.SuspendLayout();
            // 
            // lb_title
            // 
            this.lb_title.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lb_title.AutoEllipsis = true;
            this.lb_title.AutoSize = false;
            this.lb_title.Location = new System.Drawing.Point(54, 17);
            this.lb_title.Size = new System.Drawing.Size(542, 21);
            this.lb_title.Text = "查看日志";
            // 
            // pb_head
            // 
            this.pb_head.BackColor = System.Drawing.Color.White;
            this.pb_head.Image = ((System.Drawing.Image)(resources.GetObject("pb_head.Image")));
            this.pb_head.Location = new System.Drawing.Point(8, 8);
            this.pb_head.Name = "pb_head";
            this.pb_head.Size = new System.Drawing.Size(40, 40);
            this.pb_head.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pb_head.TabIndex = 5;
            this.pb_head.TabStop = false;
            // 
            // ni_tip
            // 
            this.ni_tip.Text = "橙子兄弟";
            this.ni_tip.Visible = true;
            // 
            // tb_log
            // 
            this.tb_log.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_log.AutoWordSelection = true;
            this.tb_log.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tb_log.Location = new System.Drawing.Point(8, 54);
            this.tb_log.Name = "tb_log";
            this.tb_log.ReadOnly = true;
            this.tb_log.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.tb_log.Size = new System.Drawing.Size(624, 301);
            this.tb_log.TabIndex = 6;
            this.tb_log.Text = "";
            this.tb_log.WordWrap = false;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 362);
            this.Controls.Add(this.tb_log);
            this.Controls.Add(this.pb_head);
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "查看日志";
            this.Load += new System.EventHandler(this.Main_Load);
            this.Controls.SetChildIndex(this.pb_head, 0);
            this.Controls.SetChildIndex(this.lb_title, 0);
            this.Controls.SetChildIndex(this.tb_log, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pb_head)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PictureBox pb_head;
        private System.Windows.Forms.NotifyIcon ni_tip;
        private System.Windows.Forms.RichTextBox tb_log;
    }
}
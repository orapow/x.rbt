namespace Rbt.Svr
{
    partial class Form1
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
            this.bt_start = new System.Windows.Forms.Button();
            this.bt_stop = new System.Windows.Forms.Button();
            this.tb_log = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // bt_start
            // 
            this.bt_start.Location = new System.Drawing.Point(12, 12);
            this.bt_start.Name = "bt_start";
            this.bt_start.Size = new System.Drawing.Size(66, 34);
            this.bt_start.TabIndex = 0;
            this.bt_start.Text = "启动";
            this.bt_start.UseVisualStyleBackColor = true;
            this.bt_start.Click += new System.EventHandler(this.bt_start_Click);
            // 
            // bt_stop
            // 
            this.bt_stop.Location = new System.Drawing.Point(84, 12);
            this.bt_stop.Name = "bt_stop";
            this.bt_stop.Size = new System.Drawing.Size(66, 34);
            this.bt_stop.TabIndex = 0;
            this.bt_stop.Text = "停止";
            this.bt_stop.UseVisualStyleBackColor = true;
            this.bt_stop.Click += new System.EventHandler(this.bt_stop_Click);
            // 
            // tb_log
            // 
            this.tb_log.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_log.Location = new System.Drawing.Point(12, 52);
            this.tb_log.Multiline = true;
            this.tb_log.Name = "tb_log";
            this.tb_log.Size = new System.Drawing.Size(440, 509);
            this.tb_log.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 573);
            this.Controls.Add(this.tb_log);
            this.Controls.Add(this.bt_stop);
            this.Controls.Add(this.bt_start);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bt_start;
        private System.Windows.Forms.Button bt_stop;
        private System.Windows.Forms.TextBox tb_log;
    }
}


namespace X.Wx.Controls
{
    partial class XPanel
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
            this.fp = new System.Windows.Forms.FlowLayoutPanel();
            this.vsb = new System.Windows.Forms.VScrollBar();
            this.SuspendLayout();
            // 
            // fp
            // 
            this.fp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fp.Location = new System.Drawing.Point(0, 0);
            this.fp.Name = "fp";
            this.fp.Size = new System.Drawing.Size(501, 356);
            this.fp.TabIndex = 0;
            // 
            // vsb
            // 
            this.vsb.Dock = System.Windows.Forms.DockStyle.Right;
            this.vsb.Location = new System.Drawing.Point(491, 0);
            this.vsb.Name = "vsb";
            this.vsb.Size = new System.Drawing.Size(10, 356);
            this.vsb.TabIndex = 1;
            this.vsb.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vsb_Scroll);
            // 
            // XPanel
            // 
            this.Controls.Add(this.vsb);
            this.Controls.Add(this.fp);
            this.Name = "XPanel";
            this.Size = new System.Drawing.Size(501, 356);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel fp;
        private System.Windows.Forms.VScrollBar vsb;
    }
}

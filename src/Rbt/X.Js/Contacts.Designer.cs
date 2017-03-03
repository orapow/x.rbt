namespace X.Js
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
            this.lb_list.Size = new System.Drawing.Size(177, 284);
            this.lb_list.TabIndex = 3;
            this.toolTip1.SetToolTip(this.lb_list, "双击选定当前项");
            this.lb_list.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lb_list_DrawItem);
            this.lb_list.DoubleClick += new System.EventHandler(this.lb_list_DoubleClick);
            // 
            // lb_key
            // 
            this.lb_key.Location = new System.Drawing.Point(12, 46);
            this.lb_key.Name = "lb_key";
            this.lb_key.Size = new System.Drawing.Size(177, 21);
            this.lb_key.TabIndex = 4;
            this.toolTip1.SetToolTip(this.lb_key, "输入关键字进行检索");
            this.lb_key.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lb_key_KeyUp);
            // 
            // Contacts
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(201, 369);
            this.Controls.Add(this.lb_key);
            this.Controls.Add(this.lb_list);
            this.Name = "Contacts";
            this.Text = "Contacts";
            this.Controls.SetChildIndex(this.lb_title, 0);
            this.Controls.SetChildIndex(this.lb_list, 0);
            this.Controls.SetChildIndex(this.lb_key, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lb_list;
        private System.Windows.Forms.TextBox lb_key;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}
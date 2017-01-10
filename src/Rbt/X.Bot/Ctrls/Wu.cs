using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace X.Bot.Ctrls
{
    public partial class Wu : PictureBox
    {
        public string uin { get; set; }
        public string nickname { get; set; }

        public Wu(ContextMenuStrip cms, string uin)
        {
            InitializeComponent();
            SizeMode = PictureBoxSizeMode.StretchImage;
            Width = Height = 45;
            ContextMenuStrip = cms;
            Margin = new Padding(0, 0, 5, 5);
            this.uin = uin;
            Name = "id:" + uin;
            Cursor = Cursors.Hand;
        }
    }
}

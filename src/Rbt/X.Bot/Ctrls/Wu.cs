using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using X.Bot.App;

namespace X.Bot.Ctrls
{
    public partial class Wu : PictureBox
    {
        public string uin { get; set; }
        public string nickname { get; set; }
        public Wc wx { get; set; }

        public Wu(ContextMenuStrip cms, string uin)
        {
            InitializeComponent();
            SizeMode = PictureBoxSizeMode.CenterImage;
            Width = Height = 45;
            ContextMenuStrip = cms;
            Image = Properties.Resources.loading;
            Margin = new Padding(0, 0, 5, 5);
            this.uin = uin;
            Name = "id:" + uin;
            Cursor = Cursors.Hand;
        }
    }
}

using System.Windows.Forms;
using X.Bot.App;

namespace X.Bot.Ctrls
{
    public partial class Wu : PictureBox
    {
        public string uin { get; private set; }
        public string nickname { get; set; }
        public Wc wx { get; private set; }

        public Wu(ContextMenuStrip cms, Wc wx)
        {
            InitializeComponent();
            SizeMode = PictureBoxSizeMode.CenterImage;
            Width = Height = 45;
            ContextMenuStrip = cms;
            Image = Properties.Resources.loading;
            Margin = new Padding(0, 0, 5, 5);
            Cursor = Cursors.Hand;

            this.wx = wx;
            this.wx.SetCode += Wx_SetCode;

        }

        private void Wx_SetCode(string uin)
        {
            this.uin = uin;
            Name = "id:" + uin;
        }

        private void Wu_DoubleClick(object sender, System.EventArgs e)
        {
            if (wx != null) wx.Show();
        }
    }
}

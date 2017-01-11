using System;
using System.Collections.Generic;
using System.Drawing;

namespace X.Wx
{
    public partial class Main : Base
    {
        public Main(App.Wc w, App.Wc.Contact u, Image hd) : base(w)
        {
            InitializeComponent();
            User = u;
            pb_head.Image = hd;
            Text = "运行日志：" + u.NickName;
        }

        private void Main_Load(object sender, EventArgs e)
        {
            Text = User.NickName;
        }

        public void OutLog(string log)
        {
            try
            {
                Invoke((Action)(() =>
                {
                    if (tb_log.Lines.Length > 65535) tb_log.Text = "";
                    tb_log.AppendText(log + "\r\n");
                }));
            }
            catch { }
        }
    }
}

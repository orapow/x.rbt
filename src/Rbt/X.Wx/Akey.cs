using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace X.Wx
{
    public partial class Akey : Base
    {
        public string key { get; private set; }
        public Akey()
        {
            InitializeComponent();
        }

        private void bt_get_Click(object sender, EventArgs e)
        {
            Process.Start("http://rbt.80xc.com/login--L3VzZXIvYWtleS5odG1s.html");
        }

        private void bt_ok_Click(object sender, EventArgs e)
        {
            key = tb_key.Text;
            if (string.IsNullOrEmpty(key)) MessageBox.Show("Key不能为空，请点击 获取 按钮。");
            else
            {
                isquit = false;
                Close();
            }
        }
    }
}

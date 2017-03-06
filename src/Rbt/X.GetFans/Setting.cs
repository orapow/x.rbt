using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using X.GetFans.App;

namespace X.GetFans
{
    public partial class Setting : Base
    {
        public Setting()
        {
            InitializeComponent();
        }

        private void Rules_Load(object sender, EventArgs e)
        {
            Text = "软件配置";
            Rbt.LoadConfig();
            pb_shpic.Image = base64ToImage(Rbt.cfg.sh_pic);
            tb_audit.Text = Rbt.cfg.audit_txt;
            tb_intxt.Text = Rbt.cfg.in_txt;
            tb_shtxt.Text = Rbt.cfg.sh_txt;
            tb_tosec.Text = Rbt.cfg.tosec + "";
            tb_newct.Text = Rbt.cfg.newct + "";
            tb_gname.Text = Rbt.cfg.gpname;
            cb_debug.Checked = Rbt.cfg.isdebug;
            tb_full_txt.Text = Rbt.cfg.full_txt;
            tb_full_ct.Text = Rbt.cfg.full_ct + "";
        }

        private void bt_ok_Click(object sender, EventArgs e)
        {
            Rbt.cfg.sh_pic = imageToBase64(pb_shpic.Image.Clone() as Image);
            Rbt.cfg.audit_txt = tb_audit.Text;
            Rbt.cfg.in_txt = tb_intxt.Text;
            Rbt.cfg.sh_txt = tb_shtxt.Text;
            Rbt.cfg.tosec = int.Parse(tb_tosec.Text);
            Rbt.cfg.newct = int.Parse(tb_newct.Text);
            Rbt.cfg.gpname = tb_gname.Text;
            Rbt.cfg.isdebug = cb_debug.Checked;
            Rbt.cfg.full_ct = int.Parse(tb_full_ct.Text);
            Rbt.cfg.full_txt = tb_full_txt.Text;
            Rbt.SaveConfig();
            Close();
        }

        string imageToBase64(Image img)
        {
            using (var ms = new MemoryStream())
            {
                img.Save(ms, ImageFormat.Png);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        private void pb_shpic_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "图片文件(*.jpg;*.png;*.jpeg;)|*.jpg;*.png;*.jpeg;";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                pb_shpic.Image = Image.FromFile(ofd.FileName).Clone() as Image;
            }
        }
    }
}




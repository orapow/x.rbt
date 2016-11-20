using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace X.Wx
{
    public partial class Form1 : Form
    {
        App.Wx wx = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void Wx_OutLog(string log)
        {
            BeginInvoke((Action)delegate ()
            {
                if (textBox1.Lines.Length > 2000) textBox1.Clear();
                textBox1.AppendText(log + "\r\n");
            });
        }

        private void Wx_NewMsg(App.Wx.Msg m, string uk)
        {

        }

        private void Wx_Logout(string uk)
        {
            Application.Exit();
        }

        private void Wx_Loged(App.Wx w)
        {

        }

        private void Wx_Scaned(string hdimg)
        {
            Image img = null;
            using (var ms = new MemoryStream())
            {
                var data = Convert.FromBase64String(hdimg.Replace("data:img/jpg;base64,", ""));
                ms.Write(data, 0, data.Length);
                img = Image.FromStream(ms);
            }
            BeginInvoke((Action)delegate ()
            {
                pictureBox1.Image = img;
            });
        }

        private void Wx_LoadQr(string qrcode)
        {
            Image img = null;
            using (var ms = new MemoryStream())
            {
                var data = Convert.FromBase64String(qrcode.Replace("data:img/jpg;base64,", ""));
                ms.Write(data, 0, data.Length);
                img = Image.FromStream(ms);
            }
            BeginInvoke((Action)delegate ()
            {
                pictureBox1.Image = img;
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int id = int.Parse(listBox1.SelectedItem + "");
            ((Action)delegate ()
            {
                wx = new App.Wx(id);
                wx.LoadQr += Wx_LoadQr;
                wx.Scaned += Wx_Scaned;
                wx.Loged += Wx_Loged;
                wx.Logout += Wx_Logout;
                wx.NewMsg += Wx_NewMsg;
                wx.OutLog += Wx_OutLog;
                wx.Run();
            }).BeginInvoke(null, null);
        }
    }
}

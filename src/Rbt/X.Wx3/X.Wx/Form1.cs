using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using X.Core.Utility;
using X.Wx.App;

namespace X.Wx
{
    public partial class Form1 : Form
    {
        List<Tcp> tcps = null;
        bool stop = false;
        TcpListener svr = null;

        public Form1()
        {
            InitializeComponent();
            tcps = new List<Tcp>();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            ((Action)(delegate ()
            {
                var c = new Wc();
                c.Run();

            })).BeginInvoke(null, null);

        }

        private void Tcp_Closed(Tcp tc)
        {
            lock (tcps) tcps.Remove(tc);
        }

        private void Tcp_NewMsg(msg m, Tcp tc)
        {
            if (m.act == "login") tc.code = m.body;

            Invoke((Action)(() =>
            {
                if (m.act == "qrcode") pictureBox1.ImageLocation = m.body;
                else if (m.act == "headimg") pictureBox1.Image = base64ToImage(m.body);
                else if (m.act == "setuser") label1.Text = m.body;
            }));

            Debug.WriteLine(m.act + "->" + m.body);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="basecode"></param>
        /// <returns></returns>
        public Image base64ToImage(string basecode)
        {
            if (string.IsNullOrEmpty(basecode)) return null;
            basecode = basecode.Replace("data:img/jpg;base64,", "");
            var data = Convert.FromBase64String(basecode);
            var ms = new MemoryStream(data);
            var img = Image.FromStream(ms);
            ms.Close();
            ms.Dispose();
            return img;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ((Action)(delegate ()
            {
                svr = new TcpListener(IPAddress.Parse("127.0.0.1"), 10900);
                svr.Start();

                while (!stop)
                {
                    var tcp = new Tcp(svr.AcceptTcpClient());
                    tcp.NewMsg += Tcp_NewMsg; ;
                    tcp.Closed += Tcp_Closed; ;
                    tcp.Start();
                    lock (tcps) tcps.Add(tcp);
                }

            })).BeginInvoke(null, null);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            stop = true;
            if (svr != null) svr.Stop();
        }
    }

}

using Rbt.Svr.App;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace Rbt.Svr
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void bt_start_Click(object sender, EventArgs e)
        {
            Service.Start();
        }

        private void bt_stop_Click(object sender, EventArgs e)
        {
            Service.Stop();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Service.Stop();
        }
    }
}

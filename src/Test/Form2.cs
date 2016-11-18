using Rbt.Svr;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Test
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            var dt = DateTime.Now;
            for (var i = 0; i < 100; i++)
            {
                new Thread(o =>
                {
                    var wc = new Wc(null);
                    Debug.WriteLine(o + "->" + (DateTime.Now - dt).TotalMilliseconds + "->" + wc.GetStr("http://www.jd.com").msg);
                }).Start(i);
            }
        }
    }
}

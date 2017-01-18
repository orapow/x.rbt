using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.Bot.App;

namespace X.Bot
{
    public class Wc1
    {
        public string headimg { get; private set; }
        public string nickname { get; private set; }
        public string uin { get; private set; }
        public Tcp tc { get; set; }

        public void Run()
        {
            var pi = new ProcessStartInfo("cl.exe", "script.js");
            var p = new Process();
            p.StartInfo = pi;
            p.Start();
            p.WaitForExit();
        }

        public void Quit()
        {
            tc.Send(new msg() { act = "quit" });
        }

    }
}

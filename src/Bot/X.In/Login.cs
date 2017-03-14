using System.Collections.Generic;
using System.ServiceProcess;

namespace X.In
{
    public partial class Login : ServiceBase
    {
        List<Tcp> tcps = null;
        bool stop = true;
        Queue<wxlog> newwx = null;
        int tcp_port = 0;
        public Login()
        {
            InitializeComponent();
            tcps = new List<Tcp>();
            int.TryParse(ConfigurationManager.AppSettings.Get("tcp_port"), out tcp_port);
        }

        protected override void OnStart(string[] args)
        {

        }

        protected override void OnStop()
        {

        }
    }
}

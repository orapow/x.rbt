using System;
using System.Linq;
using System.Text;
using X.App.Views.wx;
using X.Core.Plugin;
using X.Core.Utility;
using X.Data;

namespace X.App.Views.wx.bw
{
    public class _us : _wx
    {
        public override int needwx
        {
            get
            {
                return 1;
            }
        }
        protected x_wx_user cu = null;

        protected override void InitView()
        {
            base.InitView();

            cu = DB.x_wx_user.FirstOrDefault(o => o.openid == opid);

            if (cu == null)
            {
                cu = new x_wx_user()
                {
                    openid = opid,
                    upid = 0,
                    ctime = DateTime.Now,
                    etime = DateTime.Now,
                    nickname = "会员"
                };

                DB.x_wx_user.InsertOnSubmit(cu);
            }

            if (string.IsNullOrEmpty(cu.city))
            {
                var ip = Tools.GetClientIP();
                if (!string.IsNullOrEmpty(ip))
                {
                    var c = Tools.GetHttpData("http://int.dpool.sina.com.cn/iplookup/iplookup.php?ip=" + ip, Encoding.GetEncoding("GB2312")); //1 - 1 - 1  中国 上海  上海
                    Loger.Info("get_city->" + ip + ":" + c);
                    if (!string.IsNullOrEmpty(c) && c[0] == '1' && c.Length >= 6) cu.city = c.TrimEnd('\t').Split('\t').LastOrDefault();
                }
            }

            cu.last_time = DateTime.Now;
            SubmitDBChanges();

            dict.Add("cu", cu);
        }
    }
}

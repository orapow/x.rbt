using System;
using System.IO;
using System.Linq;
using System.Text;
using X.App.Com;
using X.Core.Cache;
using X.Core.Utility;
using X.Web;

namespace X.App.Views.wx
{
    public class receive : xview
    {
        protected override int needus
        {
            get
            {
                return 0;
            }
        }
        /// <summary>
        /// 动作
        /// auth 授权消息
        /// msg 处理消息
        /// </summary>
        public string act { get; set; }
        /// <summary>
        /// 公众号id
        /// 公众号消息时有此参数
        /// </summary>
        public string appid { get; set; }

        string ts = "";
        string nonce = "";
        string msign = "";

        protected override string GetParmNames
        {
            get
            {
                return "act-appid";
            }
        }

        public override byte[] GetResponse()
        {
            InitView();

            var sr = new StreamReader(Context.Request.InputStream);
            var xml = sr.ReadToEnd();
            Console.WriteLine(xml);

            ts = GetReqParms("timestamp");
            nonce = GetReqParms("nonce");
            msign = GetReqParms("msg_signature");

            var back = "";
            switch (act)
            {
                case "open":
                    back = ProcOpen(xml);
                    break;
                case "mp":
                    back = ProcMsg(xml);
                    break;
            }

            return Encoding.UTF8.GetBytes(back);
        }

        private string ProcMsg(string xml)
        {
            var mg = Wx.Msg.Get(Wx.Open.appid, xml, msign, nonce, ts);
            if (mg == null) return "";

            var txt = "";
            switch (mg.MsgType)
            {
                //case "text":
                //    txt = getMsgForText(mg.GetString("Content"));
                //    break;
                case "event":
                    txt = getMsgForEvent(mg.GetString("Event"), mg.GetString("EventKey"));
                    break;
            }

            var msg = new Wx.Msg.MsgObj("");
            msg.AddValue("Content", txt);
            msg.AddValue("MsgType", "text");
            msg.AddValue("CreateTime", Tools.GetGreenTime(""));
            msg.AddValue("FromUserName", mg.ToUserName);
            msg.AddValue("ToUserName", mg.FromUserName);
            return msg.ToXml(Wx.Open.appid);

        }

        //string getMsgForText(string t)
        //{
        //    if (t == "提现") return getLink("");
        //    return "";
        //}
        string getMsgForEvent(string e, string key)
        {
            switch (e.ToLower())
            {
                case "subscribe":
                case "scan":
                    return getLink(key.Replace("qrscene_", ""));
            }
            return "";
        }
        string getLink(string k)
        {
            return "<a href='http://" + cfg.domain + "/wx/user/cash-" + k + ".html'>点我提现</a>";
        }

        private string ProcOpen(string xml)
        {
            var push = Wx.Open.Revice(xml, msign, nonce, ts);
            if (push == null) throw new XExcep("0x0030");

            switch (push.InfoType)
            {
                case "component_verify_ticket":
                    Wx.Open.SetVerify_Ticket(push.GetValue("ComponentVerifyTicket"));
                    break;
                case "unauthorized":
                    var mp = DB.x_wxmp.SingleOrDefault(o => o.appid == push.GetValue("AuthorizerAppid"));
                    if (mp != null)
                    {
                        DB.x_wxmp.DeleteOnSubmit(mp);
                        SubmitDBChanges();
                    }
                    break;
                case "updateauthorized":
                case "authorized":
                    mp = DB.x_wxmp.SingleOrDefault(o => o.appid == push.GetValue("AuthorizerAppid"));
                    if (mp != null)
                    {
                        mp.access_token = push.GetValue("AuthorizationCode");
                        mp.expires_in = int.Parse(push.GetValue("AuthorizationCodeExpiredTime"));
                        mp.token_time = DateTime.Now;
                        SubmitDBChanges();
                    }
                    break;
            }
            return "success";
        }
    }
}

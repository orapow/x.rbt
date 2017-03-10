using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace wxBot
{
    public class BaseRequest
    {
        public BaseRequest()
        {
            DeviceID = "e84617712" + DateTime.Now.ToString("fffffff");
        }
        public long Uin { get; set; }
        public string Sid { get; set; }
        public string Skey { get; set; }
        public string DeviceID { get; set; }
    }

    public class SyncKey
    {
        public class KeyValuePair
        {
            public int Key { get; set; }
            public int Val { get; set; }
        }

        public int Count { get; set; }
        public IList<KeyValuePair> List { get; set; }
    }

    public class Group
    {
        public string UserName { get; set; }
        public string NickName { get; set; }
        public string EncryChatRoomId { get; set; }
    }

    public class Contact
    {
        public long Uin { get; set; }
        public string UserName { get; set; }
        public string NickName { get; set; }
    }

    public class Msg
    {
        public string MsgId { get; set; }
        public string FromUserName { get; set; }
        public string ToUserName { get; set; }
        public int MsgType { get; set; }
        public string Content { get; set; }
    }

    public class BotManager
    {
        private log4net.ILog _log = log4net.LogManager.GetLogger("WXBot");
        private WebUtils _webUtils = new WebUtils();

        private static readonly DateTime BaseTime = new DateTime(1970, 1, 1);
        private static string GetCurrentSeconds()
        {
            long s = (long)(DateTime.UtcNow - BaseTime).TotalMilliseconds;
            return s.ToString();
        }

        public string Uuid { get; private set; }
        public string RedirectUrl { get; private set; }
        public BaseRequest BaseRequest { get; private set; }
        public string PassTicket { get; private set; }
        public string Gateway { get; private set; }
        public string UserName { get; private set; }
        public System.Net.CookieContainer _cookie = new CookieContainer();
        public IList<Group> GroupList = new List<Group>();
        public string TargetGroup { get; set; }
        public IList<Contact> UserList = new List<Contact>();
        public string MediaId { get; set; }

        public SyncKey _syncKey;
        public string SyncKey
        {
            get
            {
                string val = String.Empty;

                foreach (var item in _syncKey.List)
                    val += String.Format("{0}_{1}|", item.Key, item.Val);

                return val.TrimEnd('|');
            }
        }

        public string GetUUID()
        {
            const string url = "https://login.weixin.qq.com/jslogin";

            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("appid", "wx782c26e4c19acffb");
            dict.Add("fun", "new");
            dict.Add("lang", "zh_CN");
            dict.Add("_", GetCurrentSeconds());

            string resp = _webUtils.DoPost(url, dict);
            Regex reg = new Regex(@"window.QRLogin.code = 200; window.QRLogin.uuid = ""(\S+?)""");
            Uuid = reg.Match(resp).Groups[1].Value;
            return Uuid;
        }

        public Image GetQRCode()
        {
            string url = String.Format("https://login.weixin.qq.com/qrcode/{0}?t=webwx&_={1}", Uuid, GetCurrentSeconds());

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

            req.ServicePoint.Expect100Continue = false;
            req.Method = "GET";
            req.KeepAlive = true;

            req.ContentType = "image/png";
            HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();

            System.IO.Stream stream = null;

            try
            {
                // 以字符流的方式读取HTTP响应
                stream = rsp.GetResponseStream();

                return Image.FromStream(stream);
            }
            catch
            {
                return null;
            }
            finally
            {
                // 释放资源
                if (stream != null) stream.Close();
                if (rsp != null) rsp.Close();
            }
        }

        public void GetUserAvatar()
        {
            string url = String.Format("https://login.weixin.qq.com/cgi-bin/mmwebwx-bin/login?loginicon=true&uuid={0}&tip=0&r=-1160614525&_={1}", Uuid, GetCurrentSeconds());

            string resp = _webUtils.DoGet(url, null);

            if (!resp.Contains("userAvatar"))
                return;

            var tmpArr = resp.Split(',');

            byte[] bytes = Convert.FromBase64String(tmpArr[1].TrimEnd(new char[] { '\'', ';' }));
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                ms.Write(bytes, 0, bytes.Length);

                Image.FromStream(ms).Save("avatar.png");
            }
        }

        public void WaitForLogin()
        {
            string url = String.Format("https://login.weixin.qq.com/cgi-bin/mmwebwx-bin/login?tip=0&uuid={0}&_={1}", Uuid, GetCurrentSeconds());

            string resp = _webUtils.DoGet(url, null);

            Regex reg = new Regex(@"window.redirect_uri=""(\S+?)""");

            RedirectUrl = reg.Match(resp).Groups[1].Value;

            if (!String.IsNullOrEmpty(RedirectUrl))
                Gateway = "https://" + new Uri(RedirectUrl).Host;
        }

        public void Login()
        {
            string xml = _webUtils.DoGet(RedirectUrl + "&fun=new", null, _cookie);

            Regex reg = new Regex(@"<skey>(\S+?)</skey><wxsid>(\S+?)</wxsid><wxuin>(\d+)</wxuin><pass_ticket>(\S+?)</pass_ticket>");

            PassTicket = String.Empty;
            BaseRequest = new BaseRequest();
            if (reg.IsMatch(xml))
            {
                Match m = reg.Match(xml);
                BaseRequest.Skey = m.Groups[1].Value;
                BaseRequest.Sid = m.Groups[2].Value;
                BaseRequest.Uin = Convert.ToInt64(m.Groups[3].Value);
                PassTicket = System.Web.HttpUtility.UrlDecode(m.Groups[4].Value, Encoding.UTF8);
            }
        }

        public string wxInit()
        {
            string url = String.Format("{0}/cgi-bin/mmwebwx-bin/webwxinit?pass_ticket={1}&skey={2}&r={3}", Gateway, PassTicket, BaseRequest.Skey, GetCurrentSeconds());

            string json = JsonConvert.SerializeObject(new { BaseRequest = BaseRequest });

            string resp = _webUtils.DoPost(url, json);

            JObject o = JObject.Parse(resp);

            UserName = (string)o.SelectToken("User.UserName");

            string token = JsonConvert.SerializeObject(o.SelectToken("SyncKey"));

            _syncKey = JsonConvert.DeserializeObject<SyncKey>(token);

            return resp;
        }

        public string wxStatusNotify()
        {
            string url = String.Format("{0}/cgi-bin/mmwebwx-bin/webwxstatusnotify?lang=zh_CN&pass_ticket={1}", Gateway, PassTicket);

            var o = new
            {
                BaseRequest = BaseRequest,
                Code = 3,
                FromUserName = UserName,
                ToUserName = UserName,
                ClientMsgId = GetCurrentSeconds()
            };

            string json = JsonConvert.SerializeObject(o);

            string resp = _webUtils.DoPost(url, json);

            return resp;
        }

        public IList<Group> GetContact()
        {
            string url = String.Format("{0}/cgi-bin/mmwebwx-bin/webwxgetcontact?pass_ticket={1}&skey={2}&r={3}", Gateway, PassTicket, BaseRequest.Skey, GetCurrentSeconds());

            string resp = _webUtils.DoPost(url, "{}", _cookie);

            JObject o = JObject.Parse(resp);

            string token = JsonConvert.SerializeObject(o.SelectToken("MemberList"));

            IList<Group> contact = JsonConvert.DeserializeObject<IList<Group>>(token);

            foreach (Group item in contact)
            {
                if (item.UserName.StartsWith("@@"))
                    this.GroupList.Add(item);
            }

            return this.GroupList;
        }

        public IList<Contact> BatchGetContact()
        {
            string url = String.Format("{0}/cgi-bin/mmwebwx-bin/webwxbatchgetcontact?type=ex&pass_ticket={1}&r={2}", Gateway, PassTicket, GetCurrentSeconds());

            var j = new
            {
                BaseRequest = BaseRequest,
                Count = GroupList.Count,
                List = GroupList
            };

            string json = JsonConvert.SerializeObject(j);

            string resp = _webUtils.DoPost(url, json, _cookie);

            JObject o = JObject.Parse(resp);

            string token = JsonConvert.SerializeObject(o.SelectToken("ContactList[0].MemberList"));

            this.UserList = JsonConvert.DeserializeObject<IList<Contact>>(token);

            return this.UserList;
        }

        public string SyncCheck()
        {
            string url = String.Format("{0}/cgi-bin/mmwebwx-bin/synccheck", Gateway);

            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("sid", BaseRequest.Sid);
            dict.Add("uin", BaseRequest.Uin.ToString());
            dict.Add("skey", BaseRequest.Skey);
            dict.Add("deviceid", BaseRequest.DeviceID);
            dict.Add("synckey", SyncKey);
            dict.Add("r", GetCurrentSeconds());
            dict.Add("_", GetCurrentSeconds());

            string resp = _webUtils.DoGet(url, dict, _cookie);

            _log.Info(resp);

            return resp;
        }

        public void wxSync()
        {
            string url = String.Format("{0}/cgi-bin/mmwebwx-bin/webwxsync?sid={1}&skey={2}&pass_ticket={3}", Gateway, BaseRequest.Sid, BaseRequest.Skey, PassTicket);

            var o = new
            {
                BaseRequest = BaseRequest,
                SyncKey = _syncKey,
                rr = GetCurrentSeconds()
            };

            string json = JsonConvert.SerializeObject(o);

            string resp = _webUtils.DoPost(url, json, _cookie);

            _log.Info(resp);

            JObject jo = JObject.Parse(resp);

            string token = JsonConvert.SerializeObject(jo.SelectToken("SyncKey"));
            _syncKey = JsonConvert.DeserializeObject<SyncKey>(token);

            if (0 != (int)jo.SelectToken("ModContactCount"))
            {
                token = JsonConvert.SerializeObject(jo.SelectToken("ModContactList[0].MemberList"));
                UserList = JsonConvert.DeserializeObject<IList<Contact>>(token);

                Console.WriteLine(token);
                foreach (var item in UserList)
                {
                    Console.WriteLine(item.Uin.ToString() + " " + item.NickName + " " + item.UserName);
                }


                string group = (string)jo.SelectToken("ModContactList[0].UserName");
                Console.WriteLine(group);
                if (!String.IsNullOrEmpty(group) && group == GroupList[0].UserName)
                {
                    Console.WriteLine("sendMsg");

                    foreach (var item in UserList)
                    {
                        //Console.WriteLine( wxSendMsg(GroupList[0].UserName, "打招呼" + item.NickName));
                        if (item.UserName != UserName)
                            Console.WriteLine(wxSendMsg(GroupList[0].UserName, this.MediaId, 3));
                    }
                }
            }

            return;
            token = JsonConvert.SerializeObject(jo.SelectToken("AddMsgList"));

            IList<Msg> msg = JsonConvert.DeserializeObject<IList<Msg>>(token);
            foreach (Msg item in msg)
            {
                if (item.FromUserName != UserName && item.FromUserName == "aaaaaaaaaaaaaaaa")
                {
                    if (item.FromUserName.StartsWith("@@"))
                        item.Content = item.Content.Substring(66);
                    wxSendMsg(item.FromUserName, "自动回复" + item.Content);

                    Console.WriteLine("收到新消息。已经自动回复...");
                }
            }

            //Console.WriteLine(JsonConvert.SerializeObject(msg));
        }

        public string wxSendMsg(string ToUserName, string Content)
        {
            string url = String.Format("{0}/cgi-bin/mmwebwx-bin/webwxsendmsg?pass_ticket={1}", Gateway, PassTicket);
            Content = Content.Replace("<br/>", "\n");
            var o = new
            {
                BaseRequest = BaseRequest,
                Msg = new
                {
                    Type = 1,
                    Content = Content,
                    FromUserName = UserName,
                    ToUserName = ToUserName,
                    LocalID = GetCurrentSeconds(),
                    ClientMsgId = GetCurrentSeconds()
                }
            };

            string json = JsonConvert.SerializeObject(o);

            return _webUtils.DoPost(url, json, _cookie);
        }

        public string wxSendMsg(string ToUserName, string Content, int Type)
        {
            string url = String.Format("{0}/cgi-bin/mmwebwx-bin/webwxsendmsgimg?fun=async&f=json&pass_ticket={1}", Gateway, PassTicket);
            var o = new
            {
                BaseRequest = BaseRequest,
                Msg = new
                {
                    Type = 3,
                    MediaId = Content,
                    FromUserName = UserName,
                    ToUserName = ToUserName,
                    LocalID = GetCurrentSeconds(),
                    ClientMsgId = GetCurrentSeconds()
                },
                Scene = 0
            };

            string json = JsonConvert.SerializeObject(o);

            return _webUtils.DoPost(url, json, _cookie);
        }

        public string wxUpload(string webwx_data_ticket)
        {
            const string url = "https://file.wx2.qq.com/cgi-bin/mmwebwx-bin/webwxuploadmedia?f=json";

            var o = new
            {
                //UploadType = 2,
                //FromUserName = UserName,
                //ToUserName = GroupList[0].UserName,
                //FileMd5 = "",
                BaseRequest = BaseRequest,
                ClientMediaId = GetCurrentSeconds(),
                TotalLen = 121782,
                StartPos = 0,
                DataLen = 121782,
                MediaType = 4
            };
            Console.WriteLine(JsonConvert.SerializeObject(o));
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("id", "WU_FILE_0");
            dict.Add("name", "img.jpg");
            dict.Add("type", "image/jpeg");
            dict.Add("lastModifiedDate", "Wed Sep 07 2016 10:38:12 GMT+0800");
            dict.Add("size", "121782");
            dict.Add("mediatype", "pic");
            dict.Add("uploadmediarequest", JsonConvert.SerializeObject(o));
            dict.Add("webwx_data_ticket", webwx_data_ticket);
            //dict.Add("pass_ticket", "");

            Top.Api.Util.FileItem file = new Top.Api.Util.FileItem(@"D:\Pro\wxbot\doc\wxBot\wxBot\wxBot\bin\Debug\img.jpg");
            Dictionary<string, Top.Api.Util.FileItem> fileList = new Dictionary<string, Top.Api.Util.FileItem>();
            fileList.Add("filename", file);

            string resp = new Top.Api.Util.WebUtils().DoPost(url, dict, fileList);

            _log.Info(resp);

            JObject jo = JObject.Parse(resp);

            this.MediaId = (string)jo["MediaId"];

            return this.MediaId;
        }
    }
}

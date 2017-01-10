using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Core.Utility;
using X.Data;
using X.Web.Com;

namespace X.App.Apis.contact
{
    public class sync : xapi
    {
        [ParmsAttr(name = "uin", min = 1)]
        public long uin { get; set; }

        [ParmsAttr(name = "data", req = true)]
        public string data { get; set; }

        protected override XResp Execute()
        {
            var list = Serialize.FromJson<List<Contact>>(Context.Server.HtmlDecode(data));
            if (list == null) return new XResp();

            var cts = cu.x_contact.Where(o => o.uin == uin);

            tocontact(list.ToList(), cts.ToList());

            return new XResp();
        }

        void tocontact(List<Contact> list, List<x_contact> cts)
        {
            foreach (var c in cts)
            {
                Contact u = null;

                if (!string.IsNullOrEmpty(c.wxno)) u = list.FirstOrDefault(o => c.wxno == o.Alias);
                else if (!string.IsNullOrEmpty(u.RemarkName)) u = list.FirstOrDefault(o => c.remarkname == o.RemarkName && c.nickname == o.NickName);
                else u = list.FirstOrDefault(o => o.NickName == c.nickname);

                if (u == null) DB.x_contact.DeleteOnSubmit(c);
                else
                {
                    c.imgurl = u.HeadImgUrl;
                    c.membercount = u.MemberCount;
                    c.nickname = u.NickName;
                    c.remarkname = u.RemarkName;
                    c.roomid = u.EncryChatRoomId;
                    c.signature = u.Signature;
                    c.username = u.UserName;
                    c.wxno = u.Alias;

                    if (u.MemberList != null)
                    {
                        tocontact(u.MemberList, cts.Where(o => o.group_id == c.contact_id).ToList());
                    }

                    list.Remove(u);
                }
            }

            foreach (var u in list)
            {
                var c = new x_contact() { user_id = cu.user_id, uin = uin };
                c.imgurl = u.HeadImgUrl;
                c.membercount = u.MemberCount;
                c.nickname = u.NickName;
                c.remarkname = u.RemarkName;
                c.roomid = u.EncryChatRoomId;
                c.signature = u.Signature;
                c.username = u.UserName;
                c.wxno = u.Alias;
            }
        }

        /// <summary>
        /// 联系人实体
        /// </summary>
        class Contact
        {
            public long Uin { get; set; }
            public string Alias { get; set; }
            public string UserName { get; set; }
            public string NickName { get; set; }
            public string KeyWord { get; set; }
            public int MemberCount { get; set; }
            public List<Contact> MemberList { get; set; }
            public string Signature { get; set; }
            public string RemarkName { get; set; }
            public string DisplayName { get; set; }
            public string HeadImgUrl { get; set; }
            public string EncryChatRoomId { get; set; }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rbt.Svr.App
{
    public class Msg
    {
        public string FromUserName { get; set; }
        public string ToUserName { get; set; }
        public string MsgId { get; set; }
        public string Content { get; set; }
        public string CreateTime { get; set; }

        public override string ToString()
        {
            return Com.ToJson(this);
        }

    }
}

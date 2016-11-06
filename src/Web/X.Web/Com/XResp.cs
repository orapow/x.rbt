using System;
using System.Collections.Generic;
using System.Linq;

namespace X.Web.Com
{
    /// <summary>
    /// 返回数据基本类
    /// </summary>
    public class XResp
    {
        public bool issucc { get; set; }
        public string msg { get; set; }
        public string code { get; set; }
        public XResp(string code, string msg)
        {
            this.msg = msg;
            this.code = code;
            issucc = false;
        }
        public XResp()
        {
            issucc = true;
        }
    }

    /// <summary>
    /// 列表反回统一格式
    /// </summary>
    public class Resp_List : XResp
    {
        public int page { get; set; }
        public int count { get; set; }
        public object items { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Core.Utility;
using X.Web.Com;

namespace X.Web.Apis
{
    public abstract class Api : XFace
    {
        /// <summary>
        /// 执行接口
        /// </summary>
        /// <returns></returns>
        protected virtual XResp Execute()
        {
            return new XResp();
        }

        protected virtual void InitApi()
        {
            SetParms(Context.Request.Form);
        }

        public override byte[] GetResponse()
        {
            InitApi();
            Context.Response.ContentType = "text/json";
            var json = Serialize.ToJson(Execute());
            json = json.Replace(":null", ":\"\"");
            return Encoding.UTF8.GetBytes(json);
        }
    }
}

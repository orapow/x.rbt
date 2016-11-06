using Rbt.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rbt.Web
{
    /// <summary>
    /// App 的摘要说明
    /// </summary>
    public class App : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using X.App.Com;
using X.Core.Cache;
using X.Data;
using X.Web;
using X.Web.Com;
using X.Web.Views;

namespace X.App.Views.mgr
{
    public class xmg : xview
    {
        protected x_mgr mg = null;

        /// <summary>
        /// 功能权限码
        /// #是默认码
        /// 为空说明不需要验证
        /// </summary>
        protected virtual string powercode
        {
            get
            {
                return this.GetType().FullName.ToLower();
            }
        }

        /// <summary>
        /// 是否有权限
        /// </summary>
        public bool HasPower(string code)
        {
            return true;
        }

        /// <summary>
        /// 验证权限
        /// </summary>
        private void ValidPower()
        {
            if (!HasPower(powercode))
            {
                throw new XExcep("T当前用户没有此权限");
            }
        }

        protected override void InitDict()
        {
            base.InitDict();

            //var id = GetReqParms("ad");// Context.Request.Cookies["ad"];
            //if (string.IsNullOrEmpty("ad")) throw new XExcep("0x0006");

            mg = DB.x_mgr.FirstOrDefault(o=>o.mgr_id==1); //CacheHelper.Get<x_mgr>("mgr." + id);
            if (mg == null) throw new XExcep("0x0006");

            ValidPower();

            //CacheHelper.Save("mgr." + id, mg, 60 * 60);
            dict.Add("mg", mg);
        }
    }
}

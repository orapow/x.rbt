using System;
using System.Collections.Generic;
using System.Linq;
using X.Core.Cache;
using X.Data;
using X.Web;

namespace X.App.Apis.mch
{
    public class xmg : xapi
    {
        protected x_mch mg = null;
        /// <summary>
        /// 功能权限码
        /// #是默认码
        /// 为空说明不需要验证
        /// </summary>
        protected virtual string powercode
        {
            get
            {
                return "#";
            }
        }
        protected override void InitApi()
        {
            base.InitApi();

            //var id = GetReqParms("ad");
            //if (string.IsNullOrEmpty(id)) throw new XExcep("0x0006");

            mg = DB.x_mch.FirstOrDefault(o => o.mch_id == 1);// CacheHelper.Get<x_mgr>("mch." + id);
            if (mg == null) throw new XExcep("0x0006");

            //CacheHelper.Save("mgr." + id, mg, 60 * 60);

            ValidPower();
        }

        /// <summary>
        /// 是否有权限
        /// </summary>
        private bool HasPower(string code)
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
                throw new XExcep("T当前用户没有权限");
            }
        }
    }
}

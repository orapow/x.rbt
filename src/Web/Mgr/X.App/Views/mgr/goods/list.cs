using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace X.App.Views.mgr.goods
{
    public class list : xmg
    {
        /// <summary>
        /// 1、商品
        /// 2、团购
        /// 3、积分
        /// </summary>
        [ParmsAttr(req = true, min = 1, name = "商品类型")]
        public int tp { get; set; }
        protected override string GetParmNames
        {
            get
            {
                return "tp";
            }
        }
    }
}

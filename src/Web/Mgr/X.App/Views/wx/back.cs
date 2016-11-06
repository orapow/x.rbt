using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Core.Cache;
using X.Core.Utility;
using X.Web;
using X.Web.Com;

namespace X.App.Views.wx
{
    public class back : wx
    {
        /// <summary>
        /// 卡号
        /// </summary>
        public string no { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string card_pwd { get; set; }


        protected override void InitDict()
        {
            base.InitDict();

            if (!string.IsNullOrEmpty(no) && !string.IsNullOrEmpty(card_pwd))
            {
                var card = DB.x_rebate_card.SingleOrDefault(o => o.no == no);

                if (card == null || card.card_pwd != card_pwd) throw new XExcep("卡号或密码不正确");

                dict.Add("card_id", card.rebate_card_id);
            }
        }
    }
}

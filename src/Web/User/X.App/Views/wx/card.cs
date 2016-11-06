using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web;

namespace X.App.Views.wx
{
    public class card : wx
    {
        public string no { get; set; }
        public override int needwx
        {
            get
            {
                return 2;
            }
        }
        protected override string GetParmNames
        {
            get
            {
                return "no";
            }
        }
        protected override void Validate()
        {
            base.Validate();
            Validator.Require("no", no);
        }
        protected override void InitDict()
        {
            base.InitDict();
            var c = DB.x_rebate_card.SingleOrDefault(o => o.no == no && o.status == 2);
            if (c == null) throw new XExcep("T此卡不存在或暂不能兑换。");
            dict.Add("card", c);
            dict.Add("ag", DB.x_agent.SingleOrDefault(o => o.agent_id == c.agent));
        }
    }
}

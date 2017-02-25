using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace X.App.Views.wx.user
{
    public class balance : _wx
    {
        protected override void InitDict()
        {
            base.InitDict();
            dict.Add("count", DB.x_red_get.Count(o => o.get_op == opid));
        }
    }
}

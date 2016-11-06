using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web.Com;

namespace X.App.Apis.mgr.cate
{
    public class list : xmg
    {
        public string key { get; set; }
        protected override Web.Com.XResp Execute()
        {
            var tree = new XTree();
            tree.LoadList += tree_LoadList;
            tree.InitTree("");
            var r = new Resp_List();
            r.items = tree.OutTree();
            return r;
        }

        List<TreeNode> tree_LoadList(object id)
        {
            var q = from m in DB.x_dict
                    where m.code == "goods.cate" && m.upval == id + ""
                    select new item()
                    {
                        name = m.name,
                        id = m.value,
                        cid = m.dict_id,
                        img = m.img,
                        jp = m.jp
                    };
            if (!string.IsNullOrEmpty(key)) q = q.Where(o => o.name.Contains(key) || o.jp.Contains(key));
            return q.ToList<TreeNode>();
        }

        public class item : TreeNode
        {
            public long cid { get; set; }
            public string img { get; set; }
            public string jp { get; set; }
            public item() : base("") { }
        }

    }
}

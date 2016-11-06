using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;

namespace X.App.Views.mgr.cate
{
    public class select : xmg
    {
        private List<item> data = new List<item>();
        protected override void InitDict()
        {
            var tree = new XTree();
            tree.LoadList += tree_LoadList;
            tree.InitTree("");
            var list = tree.OutTree();
            list.Insert(0, new item() { id = "0", name = "顶级分类" });
            dict.Add("dict", list);
        }

        private void tree_OutNode(TreeNode n)
        {
            throw new NotImplementedException();
        }

        List<TreeNode> tree_LoadList(object id)
        {
            var q = from m in DB.x_dict
                    where m.code == "goods.cate" && m.upval == id + ""
                    select new item()
                    {
                        name = m.name,
                        img = m.img,
                        id = m.value
                    };
            return q.ToList<TreeNode>();
        }

        public class item : TreeNode
        {
            public long cid { get; set; }
            public string img { get; set; }
            public string value { get { return id + ""; } }
            public string jp { get; set; }
            public item() : base("") { }
        }

        public override string GetTplFile()
        {
            return "com/dict";
        }
    }
}

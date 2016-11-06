using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.goods
{
    public class save : xmg
    {
        public int id { get; set; }
        /// <summary>
        /// 1、商品
        /// 2、团购
        /// 3、积分
        /// </summary>
        [ParmsAttr(req = true, name = "商品类型")]
        public int tp { get; set; }
        public string action { get; set; }
        public string cate { get; set; }//分类
        public int mch_id { get; set; }//商户
        public int type { get; set; }//商品类型
        public string name { get; set; }//名称
        public string no { get; set; }//编号
        public string alias { get; set; }//别名
        public string remark { get; set; }//简介
        public string tags { get; set; }//标签
        public string cover { get; set; }//封面
        public string imgs { get; set; }//图集
        public int sort { get; set; }//排序
        public string desc { get; set; }//描述
        public int stock { get; set; }//库存
        public int limit { get; set; }//限购
        public decimal op { get; set; }//原价
        public decimal np { get; set; }//现价
        public int re_it { get; set; }//返积分
        public int red { get; set; }//支持退款
        public int rnd { get; set; }//是否配送

        protected override Web.Com.XResp Execute()
        {
            x_goods ent = null;
            if (action == "1")
            {
                ent = new x_goods() { ctme = DateTime.Now, type = tp };
            }
            else if (id > 0)
            {
                ent = DB.x_goods.SingleOrDefault(o => o.goods_id == id);
                if (ent == null) throw new XExcep("0x0005");
            }
            else ent = new x_goods() { ctme = DateTime.Now, type = tp };

            ent.cate_id = cate;
            ent.mch_id = mch_id;
            ent.type = type;
            ent.name = name;
            ent.no = no;
            ent.alias = alias;
            ent.remark = remark;
            ent.tags = tags;
            ent.cover = (string.IsNullOrEmpty(cover) && !string.IsNullOrEmpty(imgs)) ? imgs.Split(',')[0] : cover;
            ent.imgs = imgs;
            ent.sort = sort;
            ent.desc = desc;
            ent.stock = stock;
            ent.limit = limit;
            ent.old_price = op;
            ent.new_price = np;
            ent.return_exp = re_it;
            ent.refunded = red == 1;
            ent.sended = rnd == 1;

            if (ent.goods_id == 0) DB.x_goods.InsertOnSubmit(ent);
            if (action == "1") DB.x_goods.InsertOnSubmit(ent);

            SubmitDBChanges();

            return new XResp();
        }
    }
}

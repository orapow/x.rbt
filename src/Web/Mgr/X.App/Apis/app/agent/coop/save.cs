using System;
using System.Collections.Generic;
using System.Linq;
using X.App.Com;
using X.Core.Plugin;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.app.agent.coop
{
    public class save : xag
    {
        public int id { get; set; }//房源ID
        public string house { get; set; }//楼盘

        public int room { get; set; }//室
        public int hall { get; set; }//厅
        public int toilet { get; set; }//卫
        public int livein { get; set; }//可住人数

        public string intime { get; set; }//入住时间
        public string point { get; set; }

        public string cover { get; set; }
        public string img_sn { get; set; }
        public string img_xq { get; set; }
        public string img_fx { get; set; }

        public decimal area { get; set; }//面积
        public int pay_way { get; set; }//付款方式
        public int lea_way { get; set; }//出租方式
        public int lea_room { get; set; }//出租居室
        public decimal price { get; set; }//价格
        public int unit { get; set; }//单位
        public int type { get; set; }//房屋类型
        public int toward { get; set; }//朝向
        public int floor { get; set; }//总楼层
        public int onfloor { get; set; }//所在楼层
        public int decorate { get; set; }//装修
        public string build_age { get; set; }
        public string dno { get; set; }//门牌
        public string hno { get; set; }//楼号
        public string uno { get; set; }//单元号
        public string more { get; set; }//补充

        public string ms { get; set; }
        public string cfgs { get; set; }
        public string fys { get; set; }

        public int region { get; set; }//区域
        public int businessarea { get; set; }//商圈

        protected override void Validate()
        {
            base.Validate();

            Validator.Require("house", house);
            Validator.CheckRange("region", region, 0, null);
            Validator.CheckRange("businessarea", businessarea, 0, null);

            Validator.Require("point", point);

            Validator.Require("hno", hno);
            Validator.Require("dno", dno);

            Validator.CheckRange("price", price, 0, null);

            Validator.CheckRange("room", room, 0, null);
            if (lea_way == 2) Validator.CheckRange("lea_room", lea_room, 0, null);

            Validator.CheckRange("lea_way", lea_way, 0, null);

        }

        protected override XResp Execute()
        {
            x_coop coop = new x_coop();
            if (id > 0)
            {
                coop = DB.x_coop.SingleOrDefault(o => o.coop_id == id);
                if (coop == null) throw new XExcep("0x0005");
            }

            coop.house = house;

            coop.room = room;
            coop.hall = hall;
            coop.toilet = toilet;
            coop.livein = livein;
            coop.build_age = build_age;
            coop.door_no = hno.Trim() + " " + (string.IsNullOrEmpty(uno) ? "" : uno.Trim()) + " " + dno.Trim();
            coop.intime = intime;

            if (string.IsNullOrEmpty(coop.images)) cover = "";
            else if (string.IsNullOrEmpty(cover)) cover = coop.images.Split(',')[0];
            coop.cover = cover;

            coop.img_fx = img_fx;
            coop.img_sn = img_sn;
            coop.img_xq = img_xq;

            coop.area = area;
            coop.pay_way = pay_way;
            coop.lea_way = lea_way;
            coop.price = price;
            coop.unit = unit;
            coop.type = type;
            coop.toward = toward;
            coop.floor = floor;
            coop.onfloor = onfloor;
            coop.decorate = decorate;

            if (!string.IsNullOrEmpty(point))
            {
                var pt = point.Split(',');
                if (pt.Length == 2)
                {
                    coop.longitude = decimal.Parse(pt[0]);
                    coop.latitude = decimal.Parse(pt[1]);
                }
            }

            coop.lea_room = lea_room;

            if (!string.IsNullOrEmpty(cfgs))
            {
                var cs = "0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0".Split(',');
                foreach (var c in cfgs.Split(','))
                {
                    if (string.IsNullOrEmpty(c)) continue;
                    var i = int.Parse(c) - 1;
                    cs[i] = "1";
                }
                coop.config = string.Join(",", cs);// cs.Join(',');
            }

            if (!string.IsNullOrEmpty(fys))
            {
                var cs = "0,0,0,0,0,0".Split(',');
                foreach (var c in fys.Split(','))
                {
                    if (string.IsNullOrEmpty(c)) continue;
                    var i = int.Parse(c) - 1;
                    cs[i] = "1";
                }
                coop.feiyong = string.Join(",", cs);// cs.Join(',');
            }

            if (!string.IsNullOrEmpty(ms))
            {
                var cs = "0,0,0,0,0".Split(',');
                foreach (var c in ms.Split(','))
                {
                    if (string.IsNullOrEmpty(c)) continue;
                    var i = int.Parse(c) - 1;
                    cs[i] = "1";
                }
                coop.remark = string.Join(",", cs);// cs.Join(',');
            }

            coop.more = more;

            coop.time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            coop.region = region;
            coop.businessarea = businessarea;

            if (coop.id == 0)
            {
                coop.agent_id = cag.id;
                coop.status = 1;
                coop.up_time = coop.time;
                DB.x_coop.InsertOnSubmit(coop);

                SubmitDBChanges();

                var rt = new x_rent()
                {
                    coop_id = coop.id,
                    z_time = coop.time,// DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    status = 1,
                    e_time = intime,
                    z_price = price
                };

                DB.x_rent.InsertOnSubmit(rt);

            }
            else
            {
                var ret = coop.x_rent.LastOrDefault();
                if (ret != null) ret.e_time = coop.intime;
            }

            var zf_id = ZufangSdk.PushCoop(coop);
            if (zf_id == 0) Loger.Info("app.agent.coop.save->租房数据推送失败");
            coop.zf_id = zf_id;

            SubmitDBChanges();

            return new XResp();
        }
    }
}

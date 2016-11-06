using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.App.Com;
using X.Core.Plugin;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.coop
{
    public class save : xmg
    {
        public int id { get; set; }//房源ID
        public int agid { get; set; }//经纪人id
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

        public int region { get; set; }//区域
        public int businessarea { get; set; }//商圈
        public int subwayline { get; set; }//地铁线
        public int subwaystation { get; set; }//地铁站

        protected override void Validate()
        {
            base.Validate();

            Validator.Require("house", house);
            Validator.CheckRange("region", region, 0, null);
            Validator.CheckRange("businessarea", businessarea, 0, null);
            Validator.CheckRange("agid", agid, 0, null);

            Validator.Require("point", point);

            Validator.Require("hno", hno);
            Validator.Require("dno", dno);

            Validator.CheckRange("price", price, 0, null);

            Validator.CheckRange("room", room, 0, null);
            if (lea_way == 2) Validator.CheckRange("lea_room", lea_room, 0, null);

            Validator.CheckRange("lea_way", lea_way, 0, null);

        }

        protected override Web.Com.XResp Execute()
        {
            x_coop coop = new x_coop();
            if (id > 0)
            {
                coop = DB.x_coop.SingleOrDefault(o => o.coop_id == id);
                if (coop == null) throw new XExcep("0x0005");
            }
            coop.agent_id = agid;
            coop.house = house;

            coop.room = room;
            coop.hall = hall;
            coop.toilet = toilet;
            coop.livein = livein;
            coop.build_age = build_age;
            coop.door_no = hno.Trim() + " " + (string.IsNullOrEmpty(uno) ? "" : uno.Trim()) + " " + dno.Trim();
            coop.intime = intime;

            coop.img_fx = img_fx;
            coop.img_sn = img_sn;
            coop.img_xq = img_xq;

            if (string.IsNullOrEmpty(coop.images)) cover = "";
            else if (string.IsNullOrEmpty(cover)) cover = coop.images.Split(',')[0];

            coop.cover = cover;

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

            coop.config =
                GetReqParms("c1") + "," +
                GetReqParms("c2") + "," +
                GetReqParms("c3") + "," +
                GetReqParms("c4") + "," +
                GetReqParms("c5") + "," +
                GetReqParms("c6") + "," +
                GetReqParms("c7") + "," +
                GetReqParms("c8") + "," +
                GetReqParms("c9") + "," +
                GetReqParms("c10") + "," +
                GetReqParms("c11") + "," +
                GetReqParms("c12") + "," +
                GetReqParms("c13") + "," +
                GetReqParms("c14") + "," +
                GetReqParms("c15") + "," +
                GetReqParms("c16") + "," +
                GetReqParms("c17") + "," +
                GetReqParms("c18") + "," +
                GetReqParms("c19") + "," +
                GetReqParms("c20");

            coop.feiyong =
                GetReqParms("d1") + "," +
                GetReqParms("d2") + "," +
                GetReqParms("d3") + "," +
                GetReqParms("d4") + "," +
                GetReqParms("d5") + "," +
                GetReqParms("d6");

            coop.remark =
                GetReqParms("t1") + "," +
                GetReqParms("t2") + "," +
                GetReqParms("t3") + "," +
                GetReqParms("t4") + "," +
                GetReqParms("t5");

            coop.more = more;

            coop.time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            coop.region = region;
            coop.businessarea = businessarea;

            if (coop.id == 0)
            {
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

            var zf_id = ZufangSdk.PushCoop(coop);
            if (zf_id == 0) Loger.Info("mgr.coop.save->租房数据推送失败");
            coop.zf_id = zf_id;

            SubmitDBChanges();

            return new XResp();
        }
    }
}

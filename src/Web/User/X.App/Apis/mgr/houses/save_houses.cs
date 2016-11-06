using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.houses
{
    public class save_houses : xapi
    {
        public int id { get; set; }//

        public string name { get; set; }//名称
        public string quanpin { get; set; }//全拼
        public string jianpin { get; set; }//简拼

        public int region { get; set; }//区域
        public int businessarea { get; set; }//商圈

        public int subwayline { get; set; }//地铁线
        public int subwaystation { get; set; }//地铁站

        public string address { get; set; }//详细地址

        public string volume { get; set; }//容积
        public string parkingspace { get; set; }//停车位
        public string greenrate { get; set; }//绿化率

        public string propertycompany { get; set; }//物业公司
        public string propertytype { get; set; }//物业类型
        public string propertyfee { get; set; }//物业费用

        public string developers { get; set; }//开发商

        public string intro { get; set; }//简介

        //public string longitude { get; set; }//经度
        //public string latitude { get; set; }//纬度
        public string point { get; set; }//坐标

        public string opentime { get; set; }//开盘时间
        public string checkintime { get; set; }//入住时间
        //public string updatetime { get; set; }//更新时间

        public int state { get; set; }//状态(初始:1,通过:2,不通过:3)

        public string time { get; set; }//时间

        public static int page = 0;
        public static int limit = 50;

        public string key { get; set; }//搜索关键字

        protected override void Validate()
        {
            base.Validate();

            Validator.Require("name", name);
            //Validator.Require("quanpin", quanpin);
            //Validator.Require("jianpin", jianpin);

            Validator.CheckRange("region", region, 0, null);
            Validator.CheckRange("businessarea", businessarea, 0, null);

            Validator.CheckRange("subwayline", subwayline, 0, null);
            Validator.CheckRange("subwaystation", subwaystation, 0, null);

            Validator.CheckRange("state", state, 0, null);
        }

        protected override Web.Com.XResp Execute()
        {
            var r = new Resp_List();
            r.page = page;

            var houses = new x_houses();
            if (id > 0)
            {
                houses = DB.x_houses.SingleOrDefault(o => o.houses_id == id);
                if (houses == null) throw new XExcep("0x0005");
            }

            if (DB.x_houses.Count(o => o.name == name && o.region == region && o.businessarea == businessarea && o.houses_id != id) > 0) throw new XExcep("T楼盘已经存在");

            houses.name = name;
            houses.quanpin = quanpin;
            houses.jianpin = jianpin;

            houses.region = region;
            houses.businessarea = businessarea;
            houses.subwayline = subwayline;
            houses.subwaystation = subwaystation;

            houses.address = address;

            houses.volume = volume;
            houses.parkingspace = parkingspace;
            houses.greenrate = greenrate;

            houses.propertycompany = propertycompany;
            houses.propertytype = propertytype;
            houses.propertyfee = propertyfee;

            houses.developers = developers;

            houses.intro = intro;

            if (!string.IsNullOrEmpty(point))
            {
                var pt = point.Split(',');
                if (pt.Length == 2)
                {
                    houses.longitude = decimal.Parse(pt[0]);//经度
                    houses.latitude = decimal.Parse(pt[1]);//纬度
                }
            }

            houses.opentime = opentime;
            houses.checkintime = checkintime;
            houses.updatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            houses.state = 2;//状态(初始:1,通过:2,不通过:3)

            houses.time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            if (id == 0) DB.x_houses.InsertOnSubmit(houses);//插入新数据

            SubmitDBChanges();

            return new XResp();
        }
    }
}

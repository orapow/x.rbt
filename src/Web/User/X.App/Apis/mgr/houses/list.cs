using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.houses
{
    /// <summary>
    /// 楼盘管理列表
    /// </summary>
    public class list : xmg
    {
        public int agid { get; set; }
        public int region { get; set; }//区域
        public int businessarea { get; set; }//商圈
        public int subwayline { get; set; }//地铁线
        public int subwaystation { get; set; }//地铁站
        public int page { get; set; }
        public int limit { get; set; }
        public string key { get; set; }

        protected override Web.Com.XResp Execute()
        {
            var r = new Resp_List();
            r.page = page;

            var q = from houses in DB.x_houses
                    orderby houses.time descending
                    select new
                    {
                        houses.id,//ID
                        houses.houses_id,//ID

                        houses.name,//名称
                        houses.quanpin,//全拼
                        houses.jianpin,//简拼

                        houses.region,//区域
                        region_name = GetDictName("coop.qy", houses.region),
                        houses.businessarea,//商圈
                        businessarea_name = GetDictName("coop.qy", houses.businessarea),
                        houses.subwayline,//地铁线
                        subwayline_name = GetDictName("coop.dt", houses.subwayline),
                        houses.subwaystation,//地铁站
                        subwaystation_name = GetDictName("coop.dt", houses.subwaystation),

                        houses.address,//详细地址

                        houses.volume,//容积
                        houses.parkingspace,//停车位
                        houses.greenrate,//绿化率

                        houses.propertycompany,//物业公司
                        houses.propertytype,//物业类型
                        houses.propertyfee,//物业费用

                        houses.developers,//开发商

                        houses.intro,//简介

                        houses.longitude,//经度
                        houses.latitude,//纬度

                        houses.opentime,//开盘时间
                        houses.checkintime,//入住时间
                        houses.updatetime,//更新时间

                        houses.state,//状态(待审核:1,通过:2,不通过:3)
                        houses.state_name,

                        houses.time//时间
                    };

            if (!string.IsNullOrEmpty(key)) q = q.Where(o => o.name.Contains(key) || o.quanpin.Contains(key) || o.jianpin.Contains(key) || o.address.Contains(key));
            if (region > 0) q = q.Where(o => o.region == region);
            if (businessarea > 0) q = q.Where(o => o.businessarea == businessarea);
            if (subwayline > 0) q = q.Where(o => o.subwayline == subwayline);
            if (subwaystation > 0) q = q.Where(o => o.subwaystation == subwaystation);

            r.items = q.Skip((page - 1) * limit).Take(limit).ToList();
            r.count = q.Count();

            return r;
        }

    }
}

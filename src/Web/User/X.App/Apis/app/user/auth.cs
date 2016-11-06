using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web.Com;

namespace X.App.Apis.app.user
{
    public class auth : xu
    {
        /// <summary>
        /// 头像
        /// </summary>
        public string head { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 身份证正面
        /// </summary>
        public string card_img { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        public string card_no { get; set; }

        protected override bool need_user
        {
            get
            {
                return true;
            }
        }

        protected override void Validate()
        {
            base.Validate();
            Validator.Require("head", head);
            Validator.Require("name", name);
            Validator.Require("card_img", card_img);
            Validator.Require("card_no", card_no);
        }

        protected override Web.Com.XResp Execute()
        {
            us.card_no = card_no;
            us.card_img = card_img;
            us.name = name;
            us.image = head;

            SubmitDBChanges();

            return new XResp();
        }
    }
}

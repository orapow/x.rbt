using System.Linq;
using X.Core.Utility;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.user
{
    /// <summary>
    /// 删除用户
    /// </summary>
    public class setpwd : xmg
    {
        public int id { get; set; }

        protected override void Validate()
        {
            base.Validate();
            Validator.CheckRange("id", id, 0, null);
        }

        protected override XResp Execute()
        {
            var u = DB.x_user.FirstOrDefault(o => o.user_id == id);
            if (u == null) throw new XExcep("T用户不存在");

            u.pwd = Secret.MD5("123456");

            SubmitDBChanges();

            return new XResp();
        }

    }
}

using System.Linq;
using X.Web;

namespace X.App.Views.wx.bw
{
    public class detail : _us
    {
        public int id { get; set; }

        protected override void Validate()
        {
            base.Validate();
            Validator.CheckRange("id", id, 1, null);
        }

        protected override string GetParmNames
        {
            get
            {
                return "id";
            }
        }

        protected override void InitDict()
        {
            base.InitDict();

            var cp = DB.x_ocoop.FirstOrDefault(o => o.ocoop_id == id);
            if (cp == null) throw new XExcep("T房源不存在");

            cp.hits++;
            SubmitDBChanges();

            dict.Add("cp", cp);

            if (cp.x_tel_log.Count(o => o.user_id == cu.user_id) > 0 || !cu.iset) dict.Add("show_tel", true);

            dict.Add("call_times", DB.x_tel_log.Count(o => o.ocoop_id == id));
            dict.Add("my_call_times", cp.x_tel_log.Count(o => o.user_id == cu.user_id));

        }
    }
}

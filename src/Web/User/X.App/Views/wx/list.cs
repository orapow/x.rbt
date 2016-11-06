namespace X.App.Views.wx
{
    public class list : _wx
    {
        public string key { get; set; }
        protected override string GetParmNames
        {
            get
            {
                return "key";
            }
        }
        protected override void InitDict()
        {
            base.InitDict();
            dict.Add("lea_rooms", GetDictList("coop.lea_room", "0"));
            dict["key"] = Context.Server.UrlDecode(key);
        }
    }
}

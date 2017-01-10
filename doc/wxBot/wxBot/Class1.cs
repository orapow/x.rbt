using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace wxBot
{
    class Class1
    {
        public void MethodName()
        {
            System.IO.StreamReader sr = new System.IO.StreamReader(@"logs\2016-09\a.log");

            string json = sr.ReadToEnd();

            sr.Close();

            JObject o = JObject.Parse(json);

            int count = (int)o.SelectToken("ModContactCount");

            Console.WriteLine(count);

            string a = "@@8c294da50f172625697ee02273a5a563d8b5d7a80d52621c6110b2a5d9a5995a";
            IList<JToken> tk= o.SelectToken("ModContactList").Children().ToList();
            foreach (JToken item in tk)
            {
                string name = (string)item["UserName"];
                Console.WriteLine(name);

                if (a == name)
                {
                    string token = JsonConvert.SerializeObject(item["MemberList"]);

                    Console.WriteLine(token);
                    Console.WriteLine("==================================================");

                    //UserList = JsonConvert.DeserializeObject<IList<Contact>>(token);

                    
                    IList<Contact> list = JsonConvert.DeserializeObject<IList<Contact>>(item["MemberList"].ToString());
                    Console.WriteLine("==================================================");
                    Console.WriteLine(JsonConvert.SerializeObject(list));
                    //Console.WriteLine(json);
                }        
            }
        }
    }
}

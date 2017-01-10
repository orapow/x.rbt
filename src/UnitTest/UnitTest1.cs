using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using X.Core.Utility;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using X.Net;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        static long getcurrentseconds()
        {
            return (long)(DateTime.UtcNow - DateTime.MinValue).TotalMilliseconds;
        }
        [TestMethod]
        public void TestMethod1()
        {
            //var dt = Encry.Encode(Encoding.UTF8.GetBytes("<title style=\"asdfqewrqweq: sadfa;\">C#正则删除HTML标签 - HOT SUMMER - 博客园</title>"));
            ////Thread.Sleep(60 * 1000);
            //dt = Encry.Decode(dt);
            //Console.WriteLine("dt->" + Encoding.UTF8.GetString(dt));
            //Console.WriteLine(Tools.RemoveHtml("<title style=\"asdfqewrqweq: sadfa;\">C#正则删除HTML标签 - HOT SUMMER - 博客园</title>"));
        }


        class Secret
        {
            static byte[] getKey()
            {
                var dt = DateTime.Now;
                if (dt.Second >= 50) dt = dt.AddSeconds(-30);
                var key = X.Core.Utility.Secret.MD5(dt.ToString("yyyymmddHHMMmm") + "+80xc.com+");
                Console.WriteLine("key->" + key);
                return Encoding.UTF8.GetBytes(key);
            }

            public static byte[] Decode(byte[] data)
            {
                var k1 = getKey();
                var k2 = data.Take(32);
                if (!Enumerable.SequenceEqual(k1, k2)) throw new Exception("Decode Error");

                var i = 0;
                for (var j = 0; j < data.Length; j++) data[j] = (byte)(data[j] ^ k1[i++ % k1.Length]);
                
                return data.Skip(32).ToArray();
            }

            public static byte[] Encode(byte[] data)
            {
                var key = getKey();
                var i = 0;
                var dts = data.ToList();
                for (var j = 0; j < dts.Count; j++) dts[j] = (byte)(dts[j] ^ key[i++ % key.Length]);
                dts.InsertRange(0, key.ToList());
                return dts.ToArray();
            }
        }
    }
}

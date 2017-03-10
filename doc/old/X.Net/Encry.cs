using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace X.Net
{
    public class Encry
    {
        #region 
        static byte[] getKey(string k)
        {
            var key = Core.Utility.Secret.MD5(Encoding.UTF8.GetString(new byte[] { 43, 56, 48, 120, 99, 46, 99, 111, 109, 43 }) + k);
            Console.WriteLine("key->" + key);
            return Encoding.UTF8.GetBytes(key);
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="data"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static byte[] Decode(byte[] data, string k)
        {
            var k1 = getKey(k);
            var k2 = data.Take(32);
            if (!Enumerable.SequenceEqual(k1, k2)) throw new Exception("Decode Error");

            var dt = data.Skip(32).ToArray();

            var i = 0;
            for (var j = 0; j < dt.Length; j++) dt[j] = (byte)(dt[j] ^ k1[i++ % k1.Length]);

            return dt;
        }
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="data"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static byte[] Encode(byte[] data, string k)
        {
            var key = getKey(k);
            var i = 0;
            var dts = data.ToList();
            for (var j = 0; j < dts.Count; j++) dts[j] = (byte)(dts[j] ^ key[i++ % key.Length]);
            dts.InsertRange(0, key.ToList());
            return dts.ToArray();
        }

        #endregion
    }
}

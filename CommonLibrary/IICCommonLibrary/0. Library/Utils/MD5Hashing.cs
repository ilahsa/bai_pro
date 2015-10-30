using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Imps.Services.CommonV4
{
    public sealed class MD5Hashing
    {
        private static MD5 md5 = MD5.Create();
        //私有化构造函数  
        private MD5Hashing()
        {
        }
        /// <summary>  
        /// 使用utf8编码将字符串散列  
        /// </summary>  
        /// <param name="sourceString">要散列的字符串</param>  
        /// <returns>散列后的字符串</returns>  
        public static string HashString(string sourceString)
        {
            byte[] source = md5.ComputeHash(Encoding.UTF8.GetBytes(sourceString));
            return StrUtils.ToHexString(source);
        }
    }
}

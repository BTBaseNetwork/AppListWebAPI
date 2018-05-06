using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CSharp;
using BahamutCommon.Encryption;

namespace BahamutCommon.Utils
{
    public class StringUtil
    {
        public static string Md5String(string originString)
        {
            if (originString == null)
            {
                return null;
            }
            return MD5.ComputeMD5Hash(originString);
        }
    }

    public class TokenUtil
    {
        public static string GenerateToken(string appkey,string uniqueId)
        {
            var guid = Guid.NewGuid();
            var code = string.Format("{0}{1}{2}{3}", guid.ToString(), appkey, uniqueId, DateTime.UtcNow.Ticks);
            return SHA.ComputeSHA1Hash(code);
        }

        public static string GenerateKeyOfToken(string appkey,string uniqueId,string token)
        {
            var code = string.Format("{0}#{1}#{2}",appkey, uniqueId, token);
            return MD5.ComputeMD5Hash(code);
        }
    }
}

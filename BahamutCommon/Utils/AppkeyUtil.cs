using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BahamutCommon.Encryption;

namespace BahamutCommon.Utils
{
    public class AppkeyUtil
    {
        /// <summary>
        /// Generate a string length less than 128
        /// </summary>
        /// <param name="appName"></param>
        /// <returns></returns>
        public static string GenerateAppkey(string appName)
        {
            var guid = Guid.NewGuid();
            var code = string.Format("{0}_{1}_{2}", guid.ToString(), appName,DateTime.UtcNow.Ticks);
            
            return SHA.ComputeSHA1Hash(code);
        }
    }

    public class KeyUtil
    {
        static public string GenerateListKeyOfId<T>(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }
            return string.Format("List<{0}>:{1}", typeof(T).FullName, id);
        }
    }
}

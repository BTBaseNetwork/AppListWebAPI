using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BahamutCommon.Encryption
{
    public class Base64
    {
        public static string DecodeString(string value)
        {
            return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(value));
        }

        public static string EncodeString(string value)
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(value));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace BahamutCommon.Utils
{
    public class SignatureUtil
    {
        public static bool TestStringParametersSignature(string signature, params string[] parameters)
        {
            return signature == GenerateSignature(parameters);
        }

        public static string GenerateSignature(params string[] parameters)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < parameters.Length; i++)
            {
                sb.Append(parameters[i]);
            }
            return BahamutCommon.Encryption.MD5.ComputeMD5Hash(sb.ToString());
        }
    }
}
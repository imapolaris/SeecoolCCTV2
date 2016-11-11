using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace UserPlugin
{
    internal static class MD5Encryptor
    {
        public static string GetMD5Hash(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return "";
            }
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input.Trim()));
            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }
    }
}

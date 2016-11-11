using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketHelper
{
    public static class EncodeHelper
    {
        public static string GetString(byte[] bytes)
        {
            if (bytes == null)
                return null;
            if (bytes.Length == 0)
                return "";
            return Encoding.UTF8.GetString(bytes);
        }

        public static string GetString(byte[] bytes, int startIndex, int len)
        {
            if (bytes == null)
                return null;
            if (bytes.Length == 0)
                return "";
            return Encoding.UTF8.GetString(bytes, startIndex, len);
        }

        public static byte[] GetBytes(string content)
        {
            if (content == null)
                return null;
            return Encoding.UTF8.GetBytes(content);
        }
    }
}

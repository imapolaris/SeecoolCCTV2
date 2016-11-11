using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoStreamModels.Util
{
    public class BytesHelper
    {
        public static byte[] AppendPrefix(int code, byte[] bytes)
        {
            byte[] cBytes = BitConverter.GetBytes(code);
            if (bytes == null)
                bytes = new byte[0];
            List<byte> li = new List<byte>(cBytes);
            li.AddRange(bytes);
            return li.ToArray();
        }

        public static byte[] SubArray(byte[] bytes, int start, int len)
        {
            byte[] arr = new byte[len];
            Array.Copy(bytes, start, arr, 0, len);
            return arr;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketHelper
{
    internal static  class BytesHelper
    {
        public static byte[] SubArray(byte[] bytes,int start,int len)
        {
            byte[] arr = new byte[len];
            Array.Copy(bytes, start, arr, 0, len);
            return arr;
        }

        public static byte[] Combine(byte[] src1,byte[] src2)
        {
            byte[] arr = new byte[src1.Length + src2.Length];
            Array.Copy(src1, arr, src1.Length);
            Array.Copy(src2, 0, arr, src1.Length, src2.Length);
            return arr;
        }
    }
}

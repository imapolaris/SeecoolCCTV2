using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCCTVStreamCmd.H264
{
    public static class BytesHelper
    {
        public static byte[] SubBytes(byte[] data, int startIndex)
        {
            int length = data.Length - startIndex;
            return SubBytes(data, startIndex, length);
        }

        public static byte[] SubBytes(byte[] data, int startIndex, int length)
        {
            if (startIndex >= data.Length || startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex), "起始索引位置超出了数组范围。");
            if (startIndex + length > data.Length)
                throw new ArgumentException("截取长度超出了数组范围。", nameof(length));
            byte[] buf = new byte[length];
            Array.Copy(data, startIndex, buf, 0, length);
            return buf;
        }
    }
}

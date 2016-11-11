using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CCTVStreamCmd
{
    public static class DataConverter
    {
        public static byte[] ToByteArray(IntPtr buf, int bufsize)
        {
            byte[] buffer = new byte[bufsize];
            Marshal.Copy(buf, buffer, 0, bufsize);
            return buffer;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoStreamModels.Util
{
    internal class StreamHelper
    {
        public static byte[] ReadBytes(BinaryReader br)
        {
            int len = br.ReadInt32();
            return br.ReadBytes(len);
        }

        public static void WriteBytes(BinaryWriter bw,byte[] bytes)
        {
            bw.Write(bytes.Length);
            bw.Write(bytes);
        }

        public static string ReadString(BinaryReader br)
        {
            int len = br.ReadInt32();
            byte[] strB = br.ReadBytes(len);
            return Encoding.UTF8.GetString(strB);
        }

        public static void WriteString(BinaryWriter bw,string str)
        {
            bw.Write(str.Length);
            byte[] strB = Encoding.UTF8.GetBytes(str);
            bw.Write(strB);
        }

        public static DateTime ReadTime(BinaryReader br)
        {
            long ticks = br.ReadInt64();
            return new DateTime(ticks);
        }

        public static void WriteTime(BinaryWriter bw,DateTime time)
        {
            bw.Write(time.Ticks);
        }
    }
}

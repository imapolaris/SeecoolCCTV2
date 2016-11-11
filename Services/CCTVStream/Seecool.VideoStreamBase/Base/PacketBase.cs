using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seecool.VideoStreamBase
{
    public class PacketBase
    {
        public static bool ReadBool(Stream ms)
        {
            byte[] buffer = ReadByteArray(ms, 1);
            return BitConverter.ToBoolean(buffer, 0);
        }

        public static int ReadInt(Stream ms)
        {
            byte[] buffer = ReadByteArray(ms, 4);
            return BitConverter.ToInt32(buffer, 0);
        }

        public static long ReadLong(Stream ms)
        {
            byte[] buffer = ReadByteArray(ms, 8);
            return BitConverter.ToInt64(buffer, 0);
        }

        public static DateTime ReadTime(Stream ms)
        {
            return new DateTime(ReadLong(ms));
        }

        public static Guid ReadGuid(Stream ms)
        {
            byte[] buffer = ReadByteArray(ms, 16);
            return new Guid(buffer);
        }

        public static string ReadString(Stream ms)
        {
            int videoIdLen = ReadInt(ms);
            if (videoIdLen < 0)
                return null;
            byte[] bytes = ReadByteArray(ms, videoIdLen);
            return Encoding.UTF8.GetString(bytes);
        }

        public static byte[] ReadByteArray(Stream ms, int length)
        {
            byte[] buffer = new byte[length];
            ms.Read(buffer, 0, length);
            return buffer;
        }

        public static void WriteBytes(Stream stream, bool data)
        {
            WriteBytes(stream, BitConverter.GetBytes(data));
        }

        public static void WriteBytes(Stream stream, int data)
        {
            WriteBytes(stream, BitConverter.GetBytes(data));
        }

        public static void WriteBytes(Stream stream, long data)
        {
            WriteBytes(stream, BitConverter.GetBytes(data));
        }

        public static byte[] GetBytes(DateTime time)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                WriteBytes(ms, time);
                return ms.ToArray();
            }
        }

        public static byte[] GetBytes(string str)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                WriteBytes(ms, str);
                return ms.ToArray();
            }
        }

        public static void WriteBytes(Stream stream, DateTime time)
        {
            WriteBytes(stream, time.Ticks);
        }

        public static void WriteBytes(MemoryStream ms, Guid guid)
        {
            WriteBytes(ms, guid.ToByteArray());
        }

        public static void WriteBytes(Stream stream, string str)
        {
            if (str == null)
            {
                WriteBytes(stream, -1);
            }
            else
            {
                byte[] objBytes = Encoding.UTF8.GetBytes(str);
                WriteBytes(stream, BitConverter.GetBytes(objBytes.Length));
                WriteBytes(stream, objBytes);
            }
        }

        public static void WriteBytes(Stream stream, byte[] bytes)
        {
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}

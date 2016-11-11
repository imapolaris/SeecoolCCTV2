using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CenterStorageCmd
{
    public class VideoBasePacket
    {
        public DateTime Time { get; private set; }
        public byte[] Header { get; private set; }
        public long Length { get; private set; }
        public VideoBasePacket(byte[] header, DateTime time, long length)
        {
            Time = time;
            Header = header;
            Length = length;
        }

        public static byte[] Encode(VideoBasePacket packet)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                PacketBase.WriteBytes(ms, packet.Time);
                PacketBase.WriteBytes(ms, packet.Header.Length);
                PacketBase.WriteBytes(ms, packet.Header);
                PacketBase.WriteBytes(ms, packet.Length);
                return ms.ToArray();
            }
        }

        public static VideoBasePacket Decode(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                return Decode(ms);
            }
        }

        public static VideoBasePacket Decode(Stream ms)
        {
            DateTime time = PacketBase.ReadTime(ms);
            int headLen = PacketBase.ReadInt(ms);
            byte[] header = PacketBase.ReadByteArray(ms, headLen);
            long length = PacketBase.ReadLong(ms);
            return new VideoBasePacket(header, time, length);
        }
    }
}
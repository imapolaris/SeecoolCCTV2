using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageCmd
{
    public class HikHeaderPacket
    {
        public int StreamId { get; private set; }
        public string StreamName { get; private set; }
        public string StreamUrl { get; private set; }
        public int Type { get; private set; }
        public byte[] Header { get; private set; }
        public HikHeaderPacket(int streamId, string streamName, string streamUrl, int type, byte[] header)
        {
            StreamId = streamId;
            StreamName = streamName;
            StreamUrl = streamUrl;
            Type = type;
            Header = header;
        }

        public static byte[] Encode(HikHeaderPacket packet)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                PacketBase.WriteBytes(ms, (int)VideoType.Hik);
                PacketBase.WriteBytes(ms, packet.StreamId);
                PacketBase.WriteBytes(ms, packet.StreamName);
                PacketBase.WriteBytes(ms, packet.StreamUrl);
                PacketBase.WriteBytes(ms, packet.Type);
                PacketBase.WriteBytes(ms, packet.Header.Length);
                PacketBase.WriteBytes(ms, packet.Header);
                return ms.ToArray();
            }
        }

        public static HikHeaderPacket Decode(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                return Decode(ms);
            }
        }

        public static HikHeaderPacket Decode(MemoryStream ms)
        {
            int videoType = PacketBase.ReadInt(ms);
            if (videoType != (int)VideoType.Hik)
                return null;
            int streamId = PacketBase.ReadInt(ms);
            string streamName = PacketBase.ReadString(ms);
            string streamUrl = PacketBase.ReadString(ms);
            int type = PacketBase.ReadInt(ms);
            int length = PacketBase.ReadInt(ms);
            byte[] buffer = PacketBase.ReadByteArray(ms, length);
            return new HikHeaderPacket(streamId, streamName, streamUrl, type, buffer);
        }
    }
}

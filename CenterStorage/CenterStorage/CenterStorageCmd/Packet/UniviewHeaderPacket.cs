using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageCmd.Packet
{
    public class UniviewHeaderPacket
    {
        public int StreamId { get; private set; }
        public string StreamName { get; private set; }
        public string StreamUrl { get; private set; }

        public UniviewHeaderPacket(int streamId, string streamName, string streamUrl)
        {
            StreamId = streamId;
            StreamName = streamName;
            StreamUrl = streamUrl;
        }

        public static byte[] Encode(UniviewHeaderPacket packet)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                PacketBase.WriteBytes(ms, (int)VideoType.Uniview);
                PacketBase.WriteBytes(ms, packet.StreamId);
                PacketBase.WriteBytes(ms, packet.StreamName);
                PacketBase.WriteBytes(ms, packet.StreamUrl);
                return ms.ToArray();
            }
        }

        public static UniviewHeaderPacket Decode(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                return Decode(ms);
            }
        }

        public static UniviewHeaderPacket Decode(MemoryStream ms)
        {
            int videoType = PacketBase.ReadInt(ms);
            if (videoType != (int)VideoType.Uniview)
                return null;
            int streamId = PacketBase.ReadInt(ms);
            string streamName = PacketBase.ReadString(ms);
            string streamUrl = PacketBase.ReadString(ms);
            return new UniviewHeaderPacket(streamId, streamName, streamUrl);
        }
    }
}

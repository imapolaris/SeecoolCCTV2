using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seecool.VideoStreamBase
{
    public class VideoHeaderPacket
    {
        public static byte[] Encoder(IHeaderPacket packet)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                if (packet is HikHeaderPacket)
                {
                    PacketBase.WriteBytes(ms, (int)VideoPacketType.HikvisionHeader);
                    PacketBase.WriteBytes(ms, HikHeaderPacket.Encode(packet as HikHeaderPacket));
                }
                else if (packet is StandardHeaderPacket)
                {
                    PacketBase.WriteBytes(ms, (int)VideoPacketType.FfmpegHeader);
                    PacketBase.WriteBytes(ms, StandardHeaderPacket.Encode(packet as StandardHeaderPacket));
                }
                return ms.ToArray();
            }
        }

        public static IHeaderPacket Decode(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                return Decode(ms);
            }
        }

        public static IHeaderPacket Decode(MemoryStream ms)
        {
            VideoPacketType type = (VideoPacketType)PacketBase.ReadInt(ms);
            IHeaderPacket packet = null;
            switch(type)
            {
                case VideoPacketType.HikvisionHeader:
                    packet= HikHeaderPacket.Decode(ms);
                    break;
                case VideoPacketType.FfmpegHeader:
                    packet = StandardHeaderPacket.Decode(ms);
                    break;
            }
            return packet;
        }
    }
}

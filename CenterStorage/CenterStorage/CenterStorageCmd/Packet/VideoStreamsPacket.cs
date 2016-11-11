using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageCmd
{
    public class VideoStreamsPacket: IComparable<VideoStreamsPacket>
    {
        public TimePeriodPacket TimePeriod { get; private set; }
        public StreamPacket[] VideoStreams { get; private set; }
        public VideoStreamsPacket(TimePeriodPacket timePeriod, StreamPacket[] streamDatas)
        {
            TimePeriod = timePeriod;
            VideoStreams = streamDatas;
        }

        public static byte[] Encode(VideoStreamsPacket packet)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                PacketBase.WriteBytes(ms, TimePeriodPacket.Encode(packet.TimePeriod));
                PacketBase.WriteBytes(ms, packet.VideoStreams.Length);
                for (int i = 0; i < packet.VideoStreams.Length; i++)
                {
                    PacketBase.WriteBytes(ms, StreamPacket.Encode(packet.VideoStreams[i]));
                }
                return ms.ToArray();
            }
        }

        public static VideoStreamsPacket Decode(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                return Decode(ms);
            }
        }

        public static VideoStreamsPacket Decode(MemoryStream ms)
        {
            TimePeriodPacket ti = TimePeriodPacket.Decode(ms);
            int length = PacketBase.ReadInt(ms);
            StreamPacket[] spList = new StreamPacket[length];
            for (int i = 0; i < length; i++)
            {
                spList[i] = StreamPacket.Decode(ms);
            }
            return new VideoStreamsPacket(ti, spList);
        }

        public int CompareTo(VideoStreamsPacket other)
        {
            if (other == null) return 1;

            return TimePeriod.CompareTo(other.TimePeriod);
        }
    }
}

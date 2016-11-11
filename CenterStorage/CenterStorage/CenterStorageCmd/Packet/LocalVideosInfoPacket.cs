using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageCmd
{
    public class LocalVideosInfoPacket
    {
        public ITimePeriod TimePeriod { get; private set; }
        public VideoTimePeriodsPacket[] ValidTimePeriods { get; private set; }

        public LocalVideosInfoPacket(ITimePeriod timePeriod, VideoTimePeriodsPacket[] packet)
        {
            TimePeriod = timePeriod;
            ValidTimePeriods = packet;
        }

        public static byte[] Encode(LocalVideosInfoPacket packet)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                PacketBase.WriteBytes(ms, TimePeriodPacket.Encode(packet.TimePeriod));
                PacketBase.WriteBytes(ms, packet.ValidTimePeriods.Length);
                for (int i = 0; i < packet.ValidTimePeriods.Length; i++)
                    PacketBase.WriteBytes(ms, VideoTimePeriodsPacket.Encode(packet.ValidTimePeriods[i]));
                return ms.ToArray();
            }
        }

        public static LocalVideosInfoPacket Decode(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                return Decode(ms);
            }
        }

        public static LocalVideosInfoPacket Decode(Stream ms)
        {
            ITimePeriod tp = TimePeriodPacket.Decode(ms);
            int length = PacketBase.ReadInt(ms);
            VideoTimePeriodsPacket[] vis = new VideoTimePeriodsPacket[length];
            for (int i = 0; i < length; i++)
                vis[i] = VideoTimePeriodsPacket.Decode(ms);
            return new LocalVideosInfoPacket(tp, vis);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageCmd
{
    public class VideoTimePeriodsPacket: VideoInfo
    {
        public TimePeriodPacket[] TimePeriods { get; private set; }
        public VideoTimePeriodsPacket(IVideoInfo videoInfo, TimePeriodPacket[] tp)
            :base(videoInfo)
        {
            TimePeriods = tp;
        }

        public static byte[] Encode(VideoTimePeriodsPacket packet)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                PacketBase.WriteBytes(ms, VideoInfo.Encode(packet));
                PacketBase.WriteBytes(ms, TimePeriodPacket.EncodeArray(packet.TimePeriods));
                return ms.ToArray();
            }
        }

        public static VideoTimePeriodsPacket Decode(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream(buffer))
                return Decode(ms);
        }

        public static new VideoTimePeriodsPacket Decode(Stream ms)
        {
            VideoInfo videoInfo = VideoInfo.Decode(ms);
            TimePeriodPacket[] packets = TimePeriodPacket.DecodeArray(ms);
            return new VideoTimePeriodsPacket(videoInfo, packets);
        }
    }
}

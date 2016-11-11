using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageCmd
{
    public class VideoDataParam: VideoInfo
    {
        public DateTime Time { get; private set; }
        public VideoDataParam(IVideoInfo videoInfo, DateTime time):base(videoInfo)
        {
            Time = time;
        }

        public static byte[] Encode(VideoDataParam param)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                PacketBase.WriteBytes(ms, VideoInfo.Encode(param as VideoInfo));
                PacketBase.WriteBytes(ms, param.Time.Ticks);
                return ms.ToArray();
            }
        }

        public static VideoDataParam Decode(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                return Decode(ms);
            }
        }

        public static VideoDataParam Decode(MemoryStream ms)
        {
            VideoInfo videoInfo = VideoInfo.Decode(ms);
            DateTime time = PacketBase.ReadTime(ms);
            return new VideoDataParam(videoInfo, time);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageCmd
{
    public class VideoBaseInfomParam: VideoInfo, IVideoBaseInfom
    {
        public DateTime BeginTime { get; private set; }
        public DateTime EndTime { get; private set; }

        public VideoBaseInfomParam(string videoId, int streamId, string videoName, DateTime beginTime, DateTime endTime):base(videoId, streamId, videoName)
        {
            BeginTime = beginTime;
            EndTime = endTime;
        }

        public VideoBaseInfomParam(IVideoInfo videoInfo, ITimePeriod ti) 
            : this(videoInfo.VideoId, videoInfo.StreamId, videoInfo.VideoName, ti.BeginTime, ti.EndTime)
        {
        }

        public static byte[] Encode(IVideoBaseInfom param)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                PacketBase.WriteBytes(ms, VideoInfo.Encode(param));
                PacketBase.WriteBytes(ms, TimePeriodPacket.Encode(param));
                return ms.ToArray();
            }
        }

        public static VideoBaseInfomParam Decode(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                return Decode(ms);
            }
        }

        public static new VideoBaseInfomParam Decode(Stream ms)
        {
            IVideoInfo vi = VideoInfo.Decode(ms);
            ITimePeriod ti = TimePeriodPacket.Decode(ms);
            return new VideoBaseInfomParam(vi, ti);
        }
    }
}
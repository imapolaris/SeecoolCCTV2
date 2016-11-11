using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageCmd
{
    public class VideoDataInfoParam : ISourceInfo, ITimePeriod
    {
        public string SourceIp { get; private set; }
        public int SourcePort { get; private set; }
        public DateTime BeginTime { get; private set; }
        public DateTime EndTime { get; private set; }
        public VideoInfo[] VideoInfos { get; private set; }
        
        public VideoDataInfoParam(string srcIp, int srcPort, VideoInfo[] vis,DateTime begin,DateTime end)
        {
            SourceIp = srcIp;
            SourcePort = srcPort;
            VideoInfos = vis;
            BeginTime = begin;
            EndTime = end;
        }

        public static byte[] Encode(VideoDataInfoParam param)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                PacketBase.WriteBytes(ms, param.SourceIp);
                PacketBase.WriteBytes(ms, param.SourcePort);
                PacketBase.WriteBytes(ms, param.VideoInfos.Length);
                foreach (VideoInfo vi in param.VideoInfos)
                {
                    PacketBase.WriteBytes(ms, vi.VideoId);
                    PacketBase.WriteBytes(ms, vi.StreamId);
                }
                PacketBase.WriteBytes(ms, param.BeginTime);
                PacketBase.WriteBytes(ms, param.EndTime);
                return ms.ToArray();
            }
        }

        public static VideoDataInfoParam Decode(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                return Decode(ms);
            }
        }

        public static VideoDataInfoParam Decode(MemoryStream ms)
        {
            string sourceIp = PacketBase.ReadString(ms);
            int sourcePort = PacketBase.ReadInt(ms);

            int count = PacketBase.ReadInt(ms);
            VideoInfo[] vis = new VideoInfo[count];
            for (int i = 0; i < count; i++)
            {
                string videoId = PacketBase.ReadString(ms);
                int streamId = PacketBase.ReadInt(ms);
                vis[i] = new VideoInfo(videoId, streamId);
            }
            DateTime beginTime = PacketBase.ReadTime(ms);
            DateTime endTime = PacketBase.ReadTime(ms);
            return new VideoDataInfoParam(sourceIp, sourcePort, vis, beginTime, endTime);
        }
    }
}
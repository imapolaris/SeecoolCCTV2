using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageCmd
{
    public class DownloadInfoParam : VideoBaseInfomParam, IDownloadInfo
    {
        public string SourceIp { get; private set; }
        public int SourcePort { get; private set; }
        public string DownloadPath { get; private set; }

        public DownloadInfoParam(DownloadInfoParam param)
            :this(param as ISourceInfo, param as ITimePeriod, param as IVideoInfo, param.DownloadPath)
        {
        }

        public DownloadInfoParam(ISourceInfo sourceInfo, ITimePeriod tp, IVideoInfo vi, string downPath)
            :this(sourceInfo.SourceIp, sourceInfo.SourcePort, tp.BeginTime, tp.EndTime, vi.VideoId, vi.StreamId, downPath, vi.VideoName)
        {
        }
        public DownloadInfoParam(string ip, int port, VideoBaseInfomParam baseInfo, string downPath)
            : this(ip, port, baseInfo.BeginTime, baseInfo.EndTime, baseInfo.VideoId, baseInfo.StreamId, downPath, baseInfo.VideoName)
        { }

        public DownloadInfoParam(string ip, int port, DateTime begin, DateTime end, string videoId, int streamId,string downPath, string videoName)
            :base(videoId, streamId, videoName, begin, end)
        {
            SourceIp = ip;
            SourcePort = port;
            UpdatePath(downPath);
        }

        public void UpdatePath(string path)
        {
            DownloadPath = path;
        }

        public static bool AreEqualIgnorePath(IDownloadInfo info1, IDownloadInfo info2)
        {
            if (info1 == info2)
                return true;
            if (info1 == null || info2 == null)
                return false;
            return info1.SourceIp == info2.SourceIp && info1.SourcePort == info2.SourcePort
                && info1.VideoId == info2.VideoId && info1.StreamId == info2.StreamId
                && info1.BeginTime == info2.BeginTime && info1.EndTime == info2.EndTime;
        }

        public static bool AreEqual(IDownloadInfo info1, IDownloadInfo info2)
        {
            return AreEqualIgnorePath(info1, info2) && info1?.DownloadPath == info2?.DownloadPath;
        }

        public static byte[] Encode(IDownloadInfo param)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                PacketBase.WriteBytes(ms, param.SourceIp);
                PacketBase.WriteBytes(ms, param.SourcePort);
                PacketBase.WriteBytes(ms, param.DownloadPath);
                PacketBase.WriteBytes(ms, VideoBaseInfomParam.Encode(param as IVideoBaseInfom));
                return ms.ToArray();
            }
        }

        public static new DownloadInfoParam Decode(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                return Decode(ms);
            }
        }

        public static new DownloadInfoParam Decode(Stream stream)
        {
            string sourceIp = PacketBase.ReadString(stream);
            int sourcePort = PacketBase.ReadInt(stream);
            string downloadPath = PacketBase.ReadString(stream);
            var param = VideoBaseInfomParam.Decode(stream);
            return new DownloadInfoParam(sourceIp, sourcePort, param, downloadPath);
        }

        public static byte[] EncodeArray(IDownloadInfo[] infos)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                int len = infos != null ? infos.Length : 0;
                PacketBase.WriteBytes(ms, len);
                for (int i = 0; i < len; i++)
                    PacketBase.WriteBytes(ms, Encode(infos[i]));
                return ms.ToArray();
            }
        }

        public static new IDownloadInfo[] DecodeArray(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                return DecodeArray(ms);
            }
        }

        public static new IDownloadInfo[] DecodeArray(Stream stream)
        {
            int len = PacketBase.ReadInt(stream);
            IDownloadInfo[] infos = new IDownloadInfo[len];
            for (int i = 0; i < len; i++)
                infos[i] = Decode(stream);
            return infos;
        }
    }
}

using System;
using System.IO;

namespace CenterStorageCmd
{
    public class VideoInfo: IVideoInfo
    {
        public string VideoId { get; private set; }
        public int StreamId { get; private set; }
        public string VideoName { get; private set; }

        public VideoInfo(string videoId, int streamId, string videoName = null)
        {
            VideoId = videoId;
            StreamId = streamId;
            VideoName = videoName;
        }

        public VideoInfo(IVideoInfo videoInfo):this(videoInfo.VideoId, videoInfo.StreamId, videoInfo.VideoName)
        {
        }

        public static byte[] Encode(IVideoInfo info)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                PacketBase.WriteBytes(ms, info.VideoId);
                PacketBase.WriteBytes(ms, info.StreamId);
                PacketBase.WriteBytes(ms, info.VideoName);
                return ms.ToArray();
            }
        }

        public static VideoInfo Decode(Stream sm)
        {
            string videoId = PacketBase.ReadString(sm);
            int streamId = PacketBase.ReadInt(sm);
            string videoName = PacketBase.ReadString(sm);
            return new VideoInfo(videoId, streamId, videoName);
        }

        public static byte[] EncodeArray(IVideoInfo[] storageFlags)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                int len = storageFlags != null ? storageFlags.Length : 0;
                PacketBase.WriteBytes(ms, len);
                for (int i = 0; i < len; i++)
                    PacketBase.WriteBytes(ms, VideoInfo.Encode(storageFlags[i]));
                return ms.ToArray();
            }
        }

        public static VideoInfo[] DecodeArray(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                return DecodeArray(ms);
            }
        }

        public static VideoInfo[] DecodeArray(Stream stream)
        {
            int len = PacketBase.ReadInt(stream);
            VideoInfo[] vInfos = new VideoInfo[len];
            for(int i = 0; i < len; i++)
                vInfos[i] = VideoInfo.Decode(stream);
            return vInfos;
        }
    }
}
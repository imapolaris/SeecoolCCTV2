using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageCmd
{
    public class FfmpegHeaderPacket
    {
        public int StreamId { get; private set; }
        public string StreamName { get; private set; }
        public string StreamUrl { get; private set; }

        public int CodecID { get; private set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public FfmpegHeaderPacket(int streamId, string streamName, string streamUrl, int codecID, int width, int height)
        {
            StreamId = streamId;
            StreamName = streamName;
            StreamUrl = streamUrl;
            CodecID = codecID;
            Width = width;
            Height = height;
        }

        public static byte[] Encode(FfmpegHeaderPacket packet)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                PacketBase.WriteBytes(ms, (int)VideoType.Ffmpeg);
                PacketBase.WriteBytes(ms, packet.StreamId);
                PacketBase.WriteBytes(ms, packet.StreamName);
                PacketBase.WriteBytes(ms, packet.StreamUrl);
                PacketBase.WriteBytes(ms, packet.CodecID);
                PacketBase.WriteBytes(ms, packet.Width);
                PacketBase.WriteBytes(ms, packet.Height);
                return ms.ToArray();
            }
        }

        public static FfmpegHeaderPacket Decode(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                return Decode(ms);
            }
        }

        public static FfmpegHeaderPacket Decode(MemoryStream ms)
        {
            int videoType = PacketBase.ReadInt(ms);
            if (videoType != (int)VideoType.Ffmpeg)
                return null;
            int streamId = PacketBase.ReadInt(ms);
            string streamName = PacketBase.ReadString(ms);
            string streamUrl = PacketBase.ReadString(ms);
            int codecId = PacketBase.ReadInt(ms);
            int width = PacketBase.ReadInt(ms);
            int height = PacketBase.ReadInt(ms);
            return new FfmpegHeaderPacket(streamId, streamName, streamUrl, codecId, width, height);
        }
    }
}

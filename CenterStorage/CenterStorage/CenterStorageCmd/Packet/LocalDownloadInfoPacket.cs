using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageCmd
{
    public class LocalDownloadInfoPacket
    {
        public IVideoInfo Info { get; private set; }
        public string Path { get; private set; }

        public LocalDownloadInfoPacket(IVideoInfo videoInfo, string path)
        {
            Info = videoInfo;
            Path = path;
        }

        public static byte[] Encode(LocalDownloadInfoPacket packet)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                PacketBase.WriteBytes(ms, VideoInfo.Encode(packet.Info));
                PacketBase.WriteBytes(ms, packet.Path);
                return ms.ToArray();
            }
        }

        public static LocalDownloadInfoPacket Decode(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                return Decode(ms);
            }
        }

        public static LocalDownloadInfoPacket Decode(Stream ms)
        {
            VideoInfo vi = VideoInfo.Decode(ms);
            string path = PacketBase.ReadString(ms);
            return new LocalDownloadInfoPacket(vi, path);
        }
    }
}

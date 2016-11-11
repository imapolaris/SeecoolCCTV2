using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageCmd
{
    public class StorageFlagParam: VideoInfo, IVideoInfo
    {
        public bool StorageOn { get; private set; }
        public StorageFlagParam(IVideoInfo vi, bool isStartUpStorage)
            :base(vi)
        {
            StorageOn = isStartUpStorage;
        }

        public static byte[] Encode(StorageFlagParam packet)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                PacketBase.WriteBytes(ms, VideoInfo.Encode(packet as IVideoInfo));
                PacketBase.WriteBytes(ms, packet.StorageOn);
                return ms.ToArray();
            }
        }

        public static StorageFlagParam Decode(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                return Decode(ms);
            }
        }

        public new static StorageFlagParam Decode(Stream stream)
        {
            IVideoInfo vi = VideoInfo.Decode(stream);
            bool isStartUp = PacketBase.ReadBool(stream);
            return new StorageFlagParam(vi, isStartUp);
        }
    }
}

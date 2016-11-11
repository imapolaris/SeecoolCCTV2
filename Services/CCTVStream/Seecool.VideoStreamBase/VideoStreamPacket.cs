using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seecool.VideoStreamBase
{
    public class VideoStreamPacket: IStreamPacket
    {
        private byte[] buf;
        private bool _isKeyFrame;
        private DateTime now;

        public byte[] Buffer { get; private set; }
        public CCTVFrameType FrameType { get; private set; }
        public DateTime Time { get; private set; }

        public VideoStreamPacket(byte[] buffer, CCTVFrameType frameType, DateTime time)
        {
            Buffer = buffer;
            FrameType = frameType;
            Time = time;
        }

        public VideoStreamPacket(byte[] buf, bool _isKeyFrame, DateTime now)
        {
            this.buf = buf;
            this._isKeyFrame = _isKeyFrame;
            this.now = now;
        }

        public static byte[] Encode(VideoStreamPacket packet)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                PacketBase.WriteBytes(ms, packet.Buffer.Length);
                PacketBase.WriteBytes(ms, packet.Buffer);
                PacketBase.WriteBytes(ms, (int)packet.FrameType);
                PacketBase.WriteBytes(ms, packet.Time);
                return ms.ToArray();
            }
        }

        public static VideoStreamPacket Decode(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                return Decode(ms);
            }
        }

        private static VideoStreamPacket Decode(MemoryStream ms)
        {
            int length = PacketBase.ReadInt(ms);
            byte[] buffer = PacketBase.ReadByteArray(ms, length);
            CCTVFrameType type = (CCTVFrameType)PacketBase.ReadInt(ms);
            DateTime time = PacketBase.ReadTime(ms);
            return new VideoStreamPacket(buffer, type, time);
        }
    }
}

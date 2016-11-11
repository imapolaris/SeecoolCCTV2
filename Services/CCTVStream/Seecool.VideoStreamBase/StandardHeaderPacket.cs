using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seecool.VideoStreamBase
{
    public class StandardHeaderPacket: IHeaderPacket
    {
        public int CodecID { get; private set; }
        public byte[] Buffer { get; private set; }
        public StandardHeaderPacket(int codecId, byte[] buffer)
        {
            CodecID = codecId;
            Buffer = buffer;
        }

        public static byte[] Encode(StandardHeaderPacket packet)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                PacketBase.WriteBytes(ms, packet.CodecID);
                PacketBase.WriteBytes(ms, packet.Buffer.Length);
                PacketBase.WriteBytes(ms, packet.Buffer);
                return ms.ToArray();
            }
        }

        public static StandardHeaderPacket Decode(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                return Decode(ms);
            }
        }

        public static StandardHeaderPacket Decode(Stream ms)
        {
            int codecId = PacketBase.ReadInt(ms);
            int length = PacketBase.ReadInt(ms);
            byte[] buf = PacketBase.ReadByteArray(ms, length);
            return new StandardHeaderPacket(codecId, buf);
        }
    }
}

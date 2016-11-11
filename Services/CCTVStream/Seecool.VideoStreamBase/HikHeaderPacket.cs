using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seecool.VideoStreamBase
{
    public class HikHeaderPacket: IHeaderPacket
    {
        public byte[] Buffer { get; private set; }
        public HikHeaderPacket(byte[] buffer)
        {
            Buffer = buffer;
        }

        public static byte[] Encode(HikHeaderPacket packet)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                PacketBase.WriteBytes(ms, packet.Buffer.Length);
                PacketBase.WriteBytes(ms, packet.Buffer);
                return ms.ToArray();
            }
        }

        public static HikHeaderPacket Decode(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                return Decode(ms);
            }
        }

        public static HikHeaderPacket Decode(MemoryStream ms)
        {
            int length = PacketBase.ReadInt(ms);
            byte[] buffer = PacketBase.ReadByteArray(ms, length);
            return new HikHeaderPacket(buffer);
        }
    }
}

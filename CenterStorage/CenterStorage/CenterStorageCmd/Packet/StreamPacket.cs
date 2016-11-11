using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageCmd
{
    public class StreamPacket
    {
        public DateTime Time { get; private set; }
        public DataType Type { get; private set; }
        public byte[] Buffer { get; private set; }
        public StreamPacket(DateTime time, DataType type, byte[] buffer)
        {
            Time = time;
            Type = type;
            Buffer = buffer;
        }

        public static byte[] Encode(StreamPacket data)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                PacketBase.WriteBytes(ms, data.Time);
                PacketBase.WriteBytes(ms, (int)data.Type);
                PacketBase.WriteBytes(ms, data.Buffer.Length);
                PacketBase.WriteBytes(ms, data.Buffer);
                return ms.ToArray();
            }
        }

        public static StreamPacket Decode(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                return Decode(ms);
            }
        }

        public static StreamPacket Decode(MemoryStream ms)
        {
            DateTime time = PacketBase.ReadTime(ms);
            DataType type = (DataType)PacketBase.ReadInt(ms);
            int length = PacketBase.ReadInt(ms);
            byte[] buf = PacketBase.ReadByteArray(ms,length);
            return new StreamPacket(time, type, buf);
        }
    }
}

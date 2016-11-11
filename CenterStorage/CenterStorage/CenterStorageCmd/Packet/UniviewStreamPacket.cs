using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageCmd.Packet
{
    public class UniviewStreamPacket : StreamPacket
    {
        public string DecodeTag { get; private set; }
        public UniviewStreamPacket(DateTime time, DataType type, byte[] buffer, string decodeTag) : base(time, type, buffer)
        {
            DecodeTag = decodeTag;
        }

        public static byte[] Encode(UniviewStreamPacket data)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                PacketBase.WriteBytes(ms, data.Time);
                PacketBase.WriteBytes(ms, (int)data.Type);
                PacketBase.WriteBytes(ms, data.DecodeTag);
                PacketBase.WriteBytes(ms, data.Buffer.Length);
                PacketBase.WriteBytes(ms, data.Buffer);
                return ms.ToArray();
            }
        }

        public new static UniviewStreamPacket Decode(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                return Decode(ms);
            }
        }

        public new static UniviewStreamPacket Decode(MemoryStream ms)
        {
            DateTime time = PacketBase.ReadTime(ms);
            DataType type = (DataType)PacketBase.ReadInt(ms);
            string dTag = PacketBase.ReadString(ms);
            int length = PacketBase.ReadInt(ms);
            byte[] buf = PacketBase.ReadByteArray(ms, length);
            return new UniviewStreamPacket(time, type, buf, dTag);
        }
    }
}

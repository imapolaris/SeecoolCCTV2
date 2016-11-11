using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageCmd
{
    public class MessagePacket
    {
        public MessageType Type { get; private set; }
        public string Message { get; private set; }
        public string Operate { get; private set; }
        public MessagePacket(MessageType type, string message, string operate)
        {
            Type = type;
            Message = message;
            Operate = operate;
        }

        public static byte[] Encode(MessagePacket packet)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                PacketBase.WriteBytes(ms, (int)packet.Type);
                PacketBase.WriteBytes(ms, packet.Message);
                PacketBase.WriteBytes(ms, packet.Operate);
                return ms.ToArray();
            }
        }

        public static MessagePacket Decode(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                return Decode(ms);
            }
        }

        public static MessagePacket Decode(Stream ms)
        {
            MessageType type = (MessageType)PacketBase.ReadInt(ms);
            string message = PacketBase.ReadString(ms);
            string operate = PacketBase.ReadString(ms);
            return new MessagePacket(type, message, operate);
        }
    }

    public enum MessageType
    {
        Infom = 1,
        Warn,
        Error
    }
}

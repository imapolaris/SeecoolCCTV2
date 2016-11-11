using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayModels.Param
{
    public class StringPacket : Packet
    {
        public int MessageCode { get; set; }
        public string Content { get; set; }

        public StringPacket()
        {

        }

        public StringPacket(int code, string content)
        {
            MessageCode = code;
            Content = content;
        }

        public override byte[] Serialize()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write(MessageCode);
                    WriteString(bw, Content);
                    return ms.ToArray();
                }
            }
        }

        public override void Deserialize(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    MessageCode = br.ReadInt32();
                    Content = ReadString(br);
                }
            }
        }

        public static StringPacket DeserializeObject(byte[] bytes)
        {
            StringPacket sp = new StringPacket();
            sp.Deserialize(bytes);
            return sp;
        }
    }
}

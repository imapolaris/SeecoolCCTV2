using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayModels.Param
{
    public class BoolPacket:Packet
    {
        public int MessageCode { get; set; }
        public bool Value{ get; set; }

        public BoolPacket()
        {

        }

        public BoolPacket(int code, bool value)
        {
            MessageCode = code;
            Value = value;
        }

        public override byte[] Serialize()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write(MessageCode);
                    bw.Write(Value);
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
                    Value = br.ReadBoolean();
                }
            }
        }

        public static BoolPacket DeserializeObject(byte[] bytes)
        {
            BoolPacket sp = new BoolPacket();
            sp.Deserialize(bytes);
            return sp;
        }
    }
}

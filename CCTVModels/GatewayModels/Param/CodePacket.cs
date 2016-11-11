using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayModels.Param
{
    public class CodePacket:Packet
    {
        public int MessageCode { get; set; }

        public CodePacket()
        {

        }

        public CodePacket(int code)
        {
            MessageCode = code;
        }

        public override byte[] Serialize()
        {
            return BitConverter.GetBytes(MessageCode);
        }

        public override void Deserialize(byte[] bytes)
        {
            MessageCode = BitConverter.ToInt32(bytes, 0);
        }

        public static CodePacket DeserializeObject(byte[] bytes)
        {
            CodePacket sp = new CodePacket();
            sp.Deserialize(bytes);
            return sp;
        }
    }
}

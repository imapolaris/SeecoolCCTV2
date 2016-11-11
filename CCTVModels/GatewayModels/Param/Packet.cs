using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayModels.Param
{
    public abstract class Packet
    {
        public abstract byte[] Serialize();
        public abstract void Deserialize(byte[] bytes);

        protected void WriteString(BinaryWriter bw, string str)
        {
            if (str != null)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(str);
                bw.Write(bytes.Length);
                bw.Write(bytes);
            }
            else
                bw.Write(0);
        }

        protected string ReadString(BinaryReader br)
        {
            int len = br.ReadInt32();
            if (len > 0)
            {
                byte[] strB = br.ReadBytes(len);
                return Encoding.UTF8.GetString(strB);
            }
            return null;
        }
    }
}

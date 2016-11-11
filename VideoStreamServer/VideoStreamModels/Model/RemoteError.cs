using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoStreamModels.Util;

namespace VideoStreamModels.Model
{
    public class RemoteError : IByteSerializer
    {
        public string ErrorMessage { get; set; }
        public RemoteError()
        {

        }

        public RemoteError(string errMsg)
        {
            ErrorMessage = errMsg;
        }

        public virtual void Deserialize(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    int pCode = br.ReadInt32();
                    ErrorMessage = StreamHelper.ReadString(br);
                }
            }
        }

        public virtual byte[] Serialize()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write(StreamEntityCode.StreamAddress);
                    StreamHelper.WriteString(bw, ErrorMessage);
                    return ms.ToArray();
                }
            }
        }

        public static RemoteError DeserializeTo(byte[] bytes)
        {
            RemoteError re = new RemoteError();
            re.Deserialize(bytes);
            return re;
        }
    }
}

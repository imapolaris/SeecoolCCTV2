using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoStreamModels.Util;

namespace VideoStreamModels.Model
{
    public class StreamAddress : IByteSerializer
    {
        public string PreferredServerIp { get; set; }
        public int PreferredServerPort { get; set; }
        public string Url { get; set; }

        public StreamAddress()
        {

        }

        public StreamAddress(string url, string preferredServerIp, int preferredServerPort)
        {
            Url = url;
            PreferredServerIp = preferredServerIp;
            PreferredServerPort = preferredServerPort;
        }

        public virtual void Deserialize(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    int pCode = br.ReadInt32();
                    PreferredServerPort = br.ReadInt32();
                    PreferredServerIp = StreamHelper.ReadString(br);
                    Url = StreamHelper.ReadString(br);
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
                    bw.Write(PreferredServerPort);
                    StreamHelper.WriteString(bw, PreferredServerIp);
                    StreamHelper.WriteString(bw, Url);
                    return ms.ToArray();
                }
            }
        }

        public static StreamAddress DeserializeTo(byte[] bytes)
        {
            StreamAddress uw = new StreamAddress();
            uw.Deserialize(bytes);
            return uw;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoStreamModels.Util;

namespace VideoStreamModels.Model
{
    public class StreamData : IByteSerializer
    {
        public byte[] Buffer { get; set; }
        public DateTime Time { get; set; }

        public StreamData()
        {

        }

        public StreamData(DateTime time,byte[] buf)
        {
            Time = time;
            Buffer = buf;
        }

        public virtual void Deserialize(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    int pCode = br.ReadInt32();
                    Time = StreamHelper.ReadTime(br);
                    Buffer = StreamHelper.ReadBytes(br);
                }
            }
        }

        public virtual byte[] Serialize()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write(StreamEntityCode.StreamData);
                    StreamHelper.WriteTime(bw,Time);
                    StreamHelper.WriteBytes(bw, Buffer);
                    return ms.ToArray();
                }
            }
        }

        public static StreamData DeserializeTo(byte[] bytes)
        {
            StreamData sd = new StreamData();
            sd.Deserialize(bytes);
            return sd;
        }
    }
}

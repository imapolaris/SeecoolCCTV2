using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoStreamModels.Util;

namespace VideoStreamModels.Model
{
    public class HikvStreamHeader : IStreamHeader
    {
        public VideoDeviceType DeviceType { get { return VideoDeviceType.Hikv; } }
        public byte[] Buffer { get; set; }
        public HikvStreamHeader()
        {

        }

        public HikvStreamHeader(byte[] buf)
        {
            Buffer = buf;
        }
        public virtual void Deserialize(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    int pCode = br.ReadInt32();
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
                    bw.Write(StreamEntityCode.HikvHeader);
                    StreamHelper.WriteBytes(bw, Buffer);
                    return ms.ToArray();
                }
            }
        }
        public static HikvStreamHeader DeserializeTo(byte[] bytes)
        {
            HikvStreamHeader hh = new HikvStreamHeader();
            hh.Deserialize(bytes);
            return hh;
        }
    }
}

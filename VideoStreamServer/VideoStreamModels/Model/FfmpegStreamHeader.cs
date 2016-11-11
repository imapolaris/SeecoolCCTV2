using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoStreamModels.Util;

namespace VideoStreamModels.Model
{
    public class FfmpegStreamHeader : IStreamHeader
    {
        public VideoDeviceType DeviceType { get { return VideoDeviceType.Ffmpeg; } }
        public int CodecID { get; set; }
        public byte[] Buffer { get; set; }

        public FfmpegStreamHeader()
        {

        }

        public FfmpegStreamHeader(int codeId, byte[] buf)
        {
            CodecID = codeId;
            Buffer = buf;
        }

        public virtual void Deserialize(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    int pCode = br.ReadInt32();
                    CodecID = br.ReadInt32();
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
                    bw.Write(StreamEntityCode.FfmpegHeader);
                    bw.Write(CodecID);
                    StreamHelper.WriteBytes(bw, Buffer);
                    return ms.ToArray();
                }
            }
        }

        public static FfmpegStreamHeader DeserializeTo(byte[] bytes)
        {
            FfmpegStreamHeader fh = new FfmpegStreamHeader();
            fh.Deserialize(bytes);
            return fh;
        }
    }
}

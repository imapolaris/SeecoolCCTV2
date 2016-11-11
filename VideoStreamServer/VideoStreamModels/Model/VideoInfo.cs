using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoStreamModels.Util;

namespace VideoStreamModels.Model
{
    public class VideoInfo : IByteSerializer
    {
        public string VideoId { get; set; }
        public int StreamIndex { get; set; }

        public VideoInfo()
        {

        }

        public VideoInfo(string videoId, int streamIndex)
        {
            VideoId = videoId;
            StreamIndex = streamIndex;
        }


        public virtual void Deserialize(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    int pCode = br.ReadInt32();
                    VideoId = StreamHelper.ReadString(br);
                    StreamIndex = br.ReadInt32();
                }
            }
        }

        public virtual byte[] Serialize()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write(StreamEntityCode.VideoInfo);
                    StreamHelper.WriteString(bw, VideoId);
                    bw.Write(StreamIndex);
                    return ms.ToArray();
                }
            }
        }

        public static VideoInfo DeserializeTo(byte[] bytes)
        {
            VideoInfo vi = new VideoInfo();
            vi.Deserialize(bytes);
            return vi;
        }
    }
}

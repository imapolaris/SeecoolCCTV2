using FFmpeg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoStreamClient.Entity
{
    public class FfmpegPackage
    {
        public Constants.AVCodecID CodecID { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Type { get; set; }
        public ulong Pts { get; set; }
        public byte[] Data { get; set; }
    }
}

using FFmpeg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoStreamClient.Entity
{
    public class FfmpegHeader
    {
        public Constants.AVCodecID CodecID { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}

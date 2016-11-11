using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoStreamClient.Entity;

namespace VideoStreamClient.Events
{
    public class FfmpegHeaderEventArgs : EventArgs
    {
        public int CodecId { get { return (int)Header.CodecID; } }
        public int Width { get { return Header.Width; } }
        public int Height { get { return Header.Height; } }
        public FfmpegHeader Header { get; private set; }

        public FfmpegHeaderEventArgs(FfmpegHeader header)
        {
            this.Header = header;
        }
    }
}

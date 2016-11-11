using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoStreamClient.Entity;

namespace VideoStreamClient.Events
{
    public class VideoFrameEventArgs:EventArgs
    {
        public VideoFrame Frame{ get; private set; }

        public VideoFrameEventArgs(VideoFrame frame)
        {
            this.Frame = frame;
        }
    }
}

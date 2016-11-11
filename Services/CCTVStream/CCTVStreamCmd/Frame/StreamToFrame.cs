using Media.Rtp;
using Media.Rtsp.Server.MediaTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVStreamCmd.Frame
{
    public static class StreamToFrame
    {
        public static RtpFrame GetRtpFrame(byte[] stream)
        {
            RFC6184Media.RFC6184Frame frame = new RFC6184Media.RFC6184Frame(96);
            frame.Packetize(stream);
            return frame;
        }
    }
}

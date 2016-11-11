using Seecool.VideoStreamBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Media.Rtp;
using CCTVStreamCmd.Frame;

namespace CCTVStreamCmd
{
    public abstract class StreamBase : IStream, IDisposable
    {
        public IHeaderPacket Header { get; private set; }
        public event Action<IHeaderPacket> HeaderEvent;
        public event Action<IStreamPacket> StreamEvent;
        public event Action<RtpFrame, CCTVFrameType> RtpFrameEvent;

        protected void onHeader(IHeaderPacket header)
        {
            Header = header;
            var handler = HeaderEvent;
            if (handler != null)
                handler(header);
        }

        protected void onStream(byte[] stream, CCTVFrameType type, DateTime time)
        {
            var handler = StreamEvent;
            if (handler != null)
                handler(new VideoStreamPacket(stream, type, time));
        }

        protected void onFrame(byte[] stream, CCTVFrameType frameType)
        {
            var handler = RtpFrameEvent;
            if (handler != null)
            {
                var frame = StreamToFrame.GetRtpFrame(stream);
                handler(frame, frameType);
            }
        }

        public abstract void Dispose();
    }
}

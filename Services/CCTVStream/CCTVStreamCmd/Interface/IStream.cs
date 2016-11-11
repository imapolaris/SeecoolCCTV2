using Media.Rtp;
using Seecool.VideoStreamBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCTVStreamCmd
{
    public interface IStream: IDisposable
    {
        IHeaderPacket Header { get; }
        event Action<IHeaderPacket> HeaderEvent;
        event Action<IStreamPacket> StreamEvent;
        event Action<RtpFrame, CCTVFrameType> RtpFrameEvent;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoNS.VideoDistribute;
using VideoStreamClient.Entity;

namespace VideoNS.VideoFlashback
{
    internal interface IFlashbackPlayer : IDisposable
    {
        DateTime StartTime { get; }
        DateTime EndTime { get;  } 
        DateTime CurrentTime { get; set; }
        double Speed { get; set; }
        bool Playing { get; set; }

        event Action<VideoFrame> VideoFrameEvent;
    }
}

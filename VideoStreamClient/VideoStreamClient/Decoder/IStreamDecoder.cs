using CenterStorageCmd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoStreamClient.Entity;

namespace VideoStreamClient
{
    public interface IStreamDecoder: IDisposable
    {
        Action<VideoFrame> VideoFrameEvent { get; set; }
        bool Update(StreamPacket packet);
        void PlayingSpeed(int fastTimes);
    }
}

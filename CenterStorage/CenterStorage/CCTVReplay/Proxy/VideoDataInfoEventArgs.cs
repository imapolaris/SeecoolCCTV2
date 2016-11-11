using CenterStorageCmd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVReplay.Proxy
{
    public class VideoDataInfoEventArgs:EventArgs
    {
        public string VideoId { get; private set; }
        public int StreamId { get; private set; }
        public TimePeriodPacket[] TimePeriods { get; private set; }

        public VideoDataInfoEventArgs(string videoId,int streamId,TimePeriodPacket[] timePeriods)
        {
            this.VideoId = videoId;
            this.StreamId = streamId;
            this.TimePeriods = timePeriods;
        }
    }
}

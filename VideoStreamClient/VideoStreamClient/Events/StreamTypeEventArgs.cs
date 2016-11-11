using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCTVModels;

namespace VideoStreamClient.Events
{
    public class StreamTypeEventArgs : EventArgs
    {
        public int StreamIndex { get { return StreamInfo.Index; } }
        public string StreamName { get { return StreamInfo.Name; } }

        public StreamInfo StreamInfo { get; private set; }

        public StreamTypeEventArgs(StreamInfo si)
        {
            StreamInfo = si;
        }

    }
}

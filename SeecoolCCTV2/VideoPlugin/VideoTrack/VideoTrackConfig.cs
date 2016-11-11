using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoNS.VideoTrack
{
    public class VideoTrackConfig
    {
        public string Handing { get; set; }
        public string Host { get; set; }
        public string SubPort { get; set; }
        public string RpcPort { get; set; }
        public VideoTrackConfig()
        {
            Handing = "视频跟踪";
            Host = "192.168.9.222";
            SubPort = "8061";
            RpcPort = "8068";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVModels
{
    public class CCTVVideoTrack
    {
        public string VideoId { get; set; }
        public string Ip { get; set; }
        public int SubPort { get; set; } = 8061;
        public int RpcPort { get; set; } = 8068;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVModels
{
    public class CCTVCameraLimits
    {
        public string VideoId { get; set; }
        public double UpLimit { get; set; }
        public double DownLimit { get; set; }
        public double LeftLimit { get; set; }
        public double RightLimit { get; set; }
        public double MaxViewPort { get; set; }
        public double MinViewPort { get; set; }
    }
}

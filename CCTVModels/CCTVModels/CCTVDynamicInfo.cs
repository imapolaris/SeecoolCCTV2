using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVModels
{
    public class CCTVDynamicInfo
    {
        public string VideoId { get; set; }
        public double Latitude { get; set; } = 91;
        public double Longitude { get; set; } = 181;
        public double SOG { get; set; } = 0;
        public double COG { get; set; } = 0;
        public double Altitude { get; set; }
        /// <summary>
        /// 水平朝向
        /// </summary>
        public double Heading { get; set; } = 511;
        /// <summary>
        /// 垂直朝向
        /// </summary>
        public double Tilt { get; set; } = 91;
        public double ViewPort { get; set; }
    }
}

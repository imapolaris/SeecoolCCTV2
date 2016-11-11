using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVModels
{
    public class CCTVTargetTrack
    {
        public string VideoId { get; set; }
        public double HorizontalTilt { get; set; }
        public double VerticalTilt { get; set; }
        public double LongitudeA { get; set; }
        public double LatitudeA { get; set; }
        public double PanA { get; set; }
        public double TiltA { get; set; }
        public double LongitudeB { get; set; }
        public double LatitudeB { get; set; }
        public double PanB { get; set; }
        public double TiltB { get; set; }
    }
}

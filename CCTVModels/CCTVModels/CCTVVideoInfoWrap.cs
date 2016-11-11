using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVModels
{
    public class CCTVVideoInfoWrap
    {
        public CCTVStaticInfo StaticInfo { get; set; }
        public CCTVControlConfig Control { get; set; }
        public CCTVCameraLimits CameraLimits { get; set; }
        public CCTVTargetTrack TargetTrack { get; set; }
        public CCTVVideoTrack VideoTrack { get; set; }
        public CCTVVideoAnalyze VideoAnalyze { get; set; }
        public CCTVDeviceInfo DeviceInfo { get; set; }
    }
}

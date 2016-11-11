using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCTVInfoAdapterOld
{
    public class CCTVStaticInfo
    {
        public string VideoId;
        public string Name;
        public string IP;
        public double Latitude = 91;
        public double Longitude = 181;
        public double Altitude;
        public double Heading = 511;
        public double ViewPort;
        public bool HighDef;
        public bool PanTilt;
        public bool Zoom;
        public SwitchInfo[] AuxSwitch = EmptySwitches;
        public bool VideoAnalyze;
        public bool TargetTrack;
        public TrackVideoInfo TrackInfo;
        public bool AbsolutePanTilt;
        public bool ImageTrack;
        public string ImageTrackHost;
        public StreamInfo[] Streams = EmptyStreamInfos;
        public DVRChannelInfo DvrChannelInfo;

        static public SwitchInfo[] EmptySwitches = new SwitchInfo[0];
        static public StreamInfo[] EmptyStreamInfos = new StreamInfo[0];
    }

    public struct SwitchInfo
    {
        public int Index;
        public string Name;
    }

    public class TrackVideoInfo
    {
        public double UpLimit;
        public double DownLimit;
        public double LeftLimit;
        public double RightLimit;
        public double MaxViewPort;
        public double MinViewPort;
    }

    public class StreamInfo
    {
        public int Index;
        public string Name;
        public string Url;
    }

    public class DVRChannelInfo
    {
        public enum DVRType { Unknown, HikVision, USNT };

        public DVRType Type = DVRType.Unknown;
        public string Host = "127.0.0.1";
        public int Port = 8000;
        public string User = "admin";
        public string Pass = "12345";
        public int Channel = 1;
    }
}

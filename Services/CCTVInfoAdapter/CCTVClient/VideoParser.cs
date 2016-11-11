using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CCTVClient
{
    public class VideoParser
    {
        public class State
        {
            public int ID;
            public string Text;
            public string Description;
            public string ServerHost;
            public string User;
            public ulong VideoId;
        }

        public static bool TryParseState(string s, out State state)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(State));
                state = (State)xs.Deserialize(new StringReader(s));
                return (null != state);
            }
            catch
            {
                state = null;
                return false;
            }
        }

        public class Mouse
        {
            public enum EventTypeEnum { Unknown = 0, LButtonDown, LButtonUp, LButtonDblClk, MouseMove, MouseWheel, RButtonDown, RButtonUp, RButtonDblClk }
            public EventTypeEnum Event = EventTypeEnum.Unknown;
            public int Flags = 0;
            public int X = 0;
            public int Y = 0;
            public int Delta = 0;
        }

        public static bool TryParseMouse(string s, out Mouse mouse)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(Mouse));
                mouse = (Mouse)xs.Deserialize(new StringReader(s));
                return (null != mouse);
            }
            catch
            {
                mouse = null;
                return false;
            }
        }

        public class Camera
        {
            public ulong Id;
            public struct _tagPointing
            {
                public double Pan;
                public double Tilt;
            }
            public _tagPointing Pointing;
            public double Zoom;
            public double Focus;
            public double ZoomPower;
            public double DZoomPower;
        }

        public class GPS
        {
            public ulong Id;
            public double Longitude = 181;
            public double Latitude = 91;
            public double SOG = 0;
            public double COG = 0;
            public string DataTime;
        }

        public static bool TryParseRealtime(string s, out Camera camera)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(Camera));
                camera = (Camera)xs.Deserialize(new StringReader(s));
                return (null != camera);
            }
            catch
            {
                camera = null;
                return false;
            }
        }

        public static bool TryParseRealtime(string s, out GPS gps)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(GPS));
                gps = (GPS)xs.Deserialize(new StringReader(s));
                return (null != gps);
            }
            catch
            {
                gps = null;
                return false;
            }
        }

        [System.Xml.Serialization.XmlInclude(typeof(Front))]
        [System.Xml.Serialization.XmlInclude(typeof(Server))]
        public class Node
        {
            public ulong Id;
            public string Name;
            public string Host;
            public bool Online;
        }

        public class Front : Node
        {
            public int Type = 0;
            public string User = "admin";
            public string Pass = "12345";
            public int Port = 8000;
            public List<Video> Childs = new List<Video>();
        }

        public class Server : Node
        {
            public List<Node> Childs = new List<Node>();
        }

        public class Video
        {
            public enum ParamFlags
            {
                Connect = 0x0001,
                Mpeg4 = 0x0002,
                H264 = 0x0004,
                Mpeg2 = 0x0008,
                Related = 0x0010,
                Tunnel = 0x0020,
                Brother = 0x0040,
                RawHd = 0x0080,
                HikHd = 0x0100,
                HighDef = RawHd | HikHd,
                FogPenetration = 0x0200,
                Analyze = 0x1000,
                Thermal = 0x2000,
            }

            public ulong Id;
            public string Name;
            public string Host;
            public bool D1Avail;
            public bool RelatedVideo;
            public bool VideoAnalyze;
            public bool HighDef;
            public UInt32 Param;
            public ParamFlags Flags { get { return (ParamFlags)(Param & 0xFFFF); } }
            public int FrontType { get { return (int)((Param >> 16) & 0xFFFF); } }
            public int DvrChannel = 0;
            public bool Online;
            public class _tagPanTiltUnit
            {
                public string Protocol;
                public int DeviceID;
                public bool EightDirections;
                public bool FourDirections;
                public bool MultiSpeeds;
                public bool Focus;
                public bool Zoom;
                public bool Iris;
                public bool Preset;
                public bool Trackable;
                public struct Switch
                {
                    public int Index;
                    public string Name;
                }
                public List<Switch> AuxSwitchs;
                public double Longitude = 181;
                public double Latitude = 91;
                public double Altitude = -1;
                public double LeftLimit;
                public double RightLimit;
                public double UpLimit;
                public double DownLimit;
                public double WideView;
                public double TeleView;
            }
            public _tagPanTiltUnit PanTiltUnit;
        }

        public static bool TryParseNode(string s, out Node node)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(Node));
                node = (Node)xs.Deserialize(new StringReader(s));
                return (null != node);
            }
            catch
            {
                node = null;
                return false;
            }
        }
    }
}

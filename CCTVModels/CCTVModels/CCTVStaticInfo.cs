using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVModels
{
    public enum CCTVPlatformType { CCTV1, CCTV2 }

    public class CCTVStaticInfo
    {
        public string VideoId { get; set; }
        public string Name { get; set; }
        public ImageType ImageType { get; set; } = ImageType.Unknow;
        public CCTVPlatformType Platform { get; set; } = CCTVPlatformType.CCTV2;
        public StreamInfo[] Streams { get; set; }
        //==================================//
        //方位信息
        public double Latitude { get; set; } = 91;
        public double Longitude { get; set; } = 181;
        public double Altitude { get; set; }
        /// <summary>
        /// 初始水平朝向。
        /// </summary>
        public double Heading { get; set; } = 511;
        /// <summary>
        /// 初始垂直朝向。
        /// </summary>
        public double Tilt { get; set; } = 91;
        /// <summary>
        /// 画面长宽比。
        /// </summary>
        public double SizeRatio { get; set; } = 16.0 / 9;
        public double ViewPort { get; set; }
        //==================================//
    }


    public enum StreamType { Main = 1, Sub = 2 }
    public class StreamInfo
    {
        /// <summary>
        /// 标记码流类别：主码流、子码流。
        /// </summary>
        public StreamType StreamType { get; set; }
        public int Index { get; set; }
        public string Name { get; set; }
        public int Channel { get; set; }
        public string Url { get; set; }
    }

    [Flags]
    public enum ImageType
    {
        Unknow = 0x0,
        HighDef = 0X1,
        NightVision = 0X2
    }
}

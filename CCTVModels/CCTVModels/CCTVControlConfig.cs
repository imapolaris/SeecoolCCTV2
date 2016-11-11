using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVModels
{
    public enum CCTVControlType { UnControl, DVR, TransDVR, TCP }
    public enum SerialType { ICU03 = 1, Bewator = 2 }
    public class CCTVControlConfig
    {
        public string VideoId { get; set; }
        public CCTVControlType Type { get; set; } = CCTVControlType.UnControl;
        public string Ip { get; set; } = "192.168.9.251";
        public int Port { get; set; } = 8000;
        /// <summary>
        /// 当Type= DVR 或 TransDVR时有效。
        /// </summary>
        public string UserName { get; set; } = "admin";
        /// <summary>
        /// 当Type= DVR 或 TransDVR时有效。
        /// </summary>
        public string Password { get; set; } = "admin12345";
        /// <summary>
        /// 当Type= DVR 或TransDVR(串口号为485)时有效。
        /// </summary>
        public int Channel { get; set; } = 0;
        /// <summary>
        /// 当Type= TransDVR 或 TCP 时有效。
        /// </summary>
        public byte CameraId { get; set; } = 1;
        /// <summary>
        /// 当Type= TransDVR 时有效。
        /// </summary>
        public int SerialPort { get; set; } = 1;//串口号：1－232串口；2－485串口
        /// <summary>
        /// 当Type= TransDVR 时有效。
        /// </summary>
        public SerialType SerialType { get; set; } = SerialType.ICU03;
        /// <summary>
        /// 当Type= TransDVR 或 TCP 时有效。
        /// </summary>
        public bool ReverseZoom { get; set; } = false;

        public SwitchInfo[] AuxSwitch { get; set; } = EmptySwitches;
        static public SwitchInfo[] EmptySwitches = new SwitchInfo[0];
    }

    public struct SwitchInfo
    {
        public int Index;
        public string Name;
    }
}

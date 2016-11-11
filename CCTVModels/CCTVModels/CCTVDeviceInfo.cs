using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVModels
{
    public enum DeviceType { Unknown, HikIP, HikAnalog, Axis };

    public class CCTVDeviceInfo
    {
        public string VideoId { get; set; }
        public DeviceType DeviceType { get; set; }
        
        public string PreferredServerId { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }
        public string User { get; set; } = "admin";
        public string Password { get; set; } = "12345";
    }
}

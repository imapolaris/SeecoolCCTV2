using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCTVModels;

namespace CCTV2InfoDeploy.Util
{
    internal class StreamUrlGenner
    {
        //rtsp主码流：rtsp://user:pass@192.168.9.156/axis-media/media.amp?streamprofile=Quality
        //rtsp子码流：rtsp://user:pass@192.168.9.156/axis-media/media.amp?streamprofile=Mobile
        private const string RtspUrl = "rtsp://{0}:{1}@{2}/axis-media/media.amp?streamprofile={3}";
        //hikv主码流:hikv://user:pass@192.168.9.156:8000/stream?channel=1&profile=main"
        //hikv子码流:hikv://user:pass@192.168.9.156:8000/stream?channel=1&profile=sub"
        private const string HikvUrl = "hikv://{0}:{1}@{2}:{3}/stream?channel={4}&profile={5}";
        public static void BuildStreamUrl(StreamInfo[] sis, CCTVDeviceInfo device)
        {
            if (sis == null || device == null)
                return;
            switch (device.DeviceType)
            {
                case DeviceType.HikIP:
                    {
                        foreach (StreamInfo si in sis)
                        {
                            si.Url = string.Format(HikvUrl, device.User, device.Password, device.Ip, device.Port, si.Channel, si.StreamType.ToString());
                        }
                    }
                    break;
                case DeviceType.Axis:
                    {
                        foreach (StreamInfo si in sis)
                        {
                            si.Url = string.Format(RtspUrl, device.User, device.Password, device.Ip, si.StreamType == StreamType.Main ? "Quality" : "Mobile");
                        }
                    }
                    break;
                case DeviceType.HikAnalog:
                case DeviceType.Unknown:
                default:
                    break;
            }

        }
    }
}

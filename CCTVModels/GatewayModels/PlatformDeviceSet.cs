using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayModels
{
    public class PlatformDeviceSet
    {
        public string PlatformId { get; set; }
        public string DeviceId { get; set; }
        public string Name { get; set; }
        public PlatformDevice[] Items { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GatewayModels;

namespace Persistence.GBT
{
    public class PlatformDevicePersistence : BasePersistence<PlatformDeviceSet>
    {
        public static PlatformDevicePersistence Instance { get; private set; }
        static PlatformDevicePersistence()
        {
            Instance = new PlatformDevicePersistence();
        }

        private PlatformDevicePersistence() : base("GBT28181/PlatformDevice")
        {
        }
    }
}

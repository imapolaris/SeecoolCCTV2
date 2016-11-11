using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCTVModels;

namespace Persistence
{
    public class DevicePersistence : BasePersistence<CCTVDeviceInfo>
    {
        public static DevicePersistence Instance { get; private set; }
        static DevicePersistence()
        {
            Instance = new DevicePersistence();
        }

        private DevicePersistence() : base("CCTVDeviceInfo")
        {
        }
    }
}

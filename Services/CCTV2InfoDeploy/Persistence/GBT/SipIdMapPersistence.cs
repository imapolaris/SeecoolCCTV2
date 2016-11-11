using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GatewayModels;
using Persistence;

namespace Persistence.GBT
{
    public class SipIdMapPersistence : BasePersistence<SipIdMap>
    {
        public static SipIdMapPersistence Instance { get; private set; }
        static SipIdMapPersistence()
        {
            Instance = new SipIdMapPersistence();
        }

        private SipIdMapPersistence() : base("GBT28181/DeviceIdMap")
        {
        }
    }
}

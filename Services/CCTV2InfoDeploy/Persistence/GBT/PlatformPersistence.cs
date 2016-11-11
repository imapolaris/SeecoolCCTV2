using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GatewayModels;
using Persistence;

namespace Persistence.GBT
{
    public class PlatformPersistence : BasePersistence<Platform>
    {
        public static PlatformPersistence Instance { get; private set; }
        static PlatformPersistence()
        {
            Instance = new PlatformPersistence();
        }

        private PlatformPersistence() : base("GBT28181/Platform")
        {
        }
    }
}

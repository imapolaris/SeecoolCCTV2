using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GatewayModels;
using GatewayModels.Util;
using Persistence;

namespace Persistence.GBT
{
    public class GatewayPersistence : BasePersistence<Gateway>
    {
        public static GatewayPersistence Instance { get; private set; }
        static GatewayPersistence()
        {
            Instance = new GatewayPersistence();
        }

        private GatewayPersistence() : base("GBT28181/Gateway")
        {
        }

        public Gateway Current
        {
            get
            {
                Gateway gw = GetInfo(Gateway.Key);
                if (gw == null)
                {
                    gw = new Gateway()
                    {
                        SipNumber = SipIdGenner.GenDeviceID()
                    };
                    GatewayPersistence.Instance.Put(Gateway.Key, gw);
                }
                return gw;
            }
        }
    }
}

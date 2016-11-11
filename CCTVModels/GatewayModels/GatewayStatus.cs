using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayModels
{
    public class GatewayStatus
    {
        public GatewayStatus()
        {

        }

        public GatewayStatus(Gateway gw)
        {
            SipNumber = gw.SipNumber;
            Port = gw.Port;
        }
        public string SipNumber { get; set; }
        public int Port { get; set; } = 6900;
        public bool IsStarted { get; set; }
    }
}

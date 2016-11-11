using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayModels
{
    public class Gateway
    {
        public Gateway()
        {

        }

        public Gateway(Gateway gw)
        {
            if(gw!=null)
            {
                SipNumber = gw.SipNumber;
                Port = gw.Port;
            }
        }
        public const string Key = "Default";
        public string SipNumber { get; set; }
        public int Port { get; set; } = 6090;

        public override bool Equals(object obj)
        {
            return Equals(obj as Gateway);
        }

        public bool Equals(Gateway gw)
        {
            if (gw == null)
                return false;
            if (gw == this)
                return true;
            return gw.SipNumber == this.SipNumber && gw.Port == this.Port;
        }
    }
}

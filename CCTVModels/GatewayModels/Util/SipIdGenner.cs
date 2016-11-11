using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayModels.Util
{
    public class SipIdGenner
    {
        public static string GenDeviceID()
        {
            DateTime dt = DateTime.Now;
            return dt.ToString("yyMMddHHmmssffffff");
        }
    }
}

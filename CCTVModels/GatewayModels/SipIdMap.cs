using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayModels
{
    public class SipIdMap
    {
        public SipIdMap()
        {

        }

        public SipIdMap(string staticId,string sipNum)
        {
            StaticId = staticId;
            SipNumber = sipNum;
        }
        public string StaticId { get; set; }
        public string SipNumber { get; set; }
    }
}

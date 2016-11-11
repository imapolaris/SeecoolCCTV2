using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using GBTModels.Global;

namespace GBTModels.Notify
{
    [XmlRoot("Notify")]
    public class KeepAlive : AbstractDeviceObject
    {
        public override string CmdType
        {
            get
            {
                return GBTCommandTypes.KeepAlive;
            }
            set
            {
            }
        }

        public string Status { get { return "OK"; } set { } }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using GBTModels.Global;

namespace GBTModels.Query
{
    [XmlRoot("Query")]
    public class DeviceStatus : AbstractDeviceObject
    {
        public override string CmdType
        {
            get
            {
                return GBTCommandTypes.DeviceStatus;
            }
            set
            {
            }
        }
    }
}

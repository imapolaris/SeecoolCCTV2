using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using GBTModels.Global;

namespace GBTModels.Query
{
    [XmlRoot("Query")]
    public class DeviceCatalog : AbstractDeviceObject
    {
        public sealed override string CmdType
        {
            get
            {
                return GBTCommandTypes.CataLog;
            }
            set
            {
            }
        }

        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}

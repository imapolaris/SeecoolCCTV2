using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IVEFModels
{
    [XmlRoot("Root")]
    public class ShipCollection
    {
        [XmlElement("MSG_IVEF")]
        public MSG_IVEF[] Ships { get; set; }
    }
}

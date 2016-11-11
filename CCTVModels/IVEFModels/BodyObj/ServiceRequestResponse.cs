using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IVEFModels.BodyObj
{
    public class ServiceRequestResponse
    {
        [XmlAttribute]
        public string Reason { get; set; }
        [XmlAttribute]
        public string ResponseOn { get; set; }
        [XmlAttribute]
        public int Result { get; set; }
    }
}

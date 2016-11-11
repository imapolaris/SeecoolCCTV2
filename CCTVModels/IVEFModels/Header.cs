using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IVEFModels
{
    public class Header
    {
        [XmlAttribute]
        public string MsgRefId { get; set; }
        [XmlAttribute]
        public string Version { get; set; }
    }
}

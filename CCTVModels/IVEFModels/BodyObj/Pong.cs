using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IVEFModels.BodyObj
{
    public class Pong
    {
        [XmlAttribute]
        public string ResponseOn { get; set; }
        [XmlAttribute]
        public int SourceId { get; set; }
        [XmlAttribute]
        public DateTime TimeStamp { get; set; }
    }
}

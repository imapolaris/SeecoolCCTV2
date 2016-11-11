using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IVEFModels.BodyObj
{
    public class ServerStatus
    {
        [XmlAttribute]
        public string ContactIdentity { get; set; }
        [XmlAttribute]
        public string Details { get; set; }
        /// <summary>
        /// Status of the server (ok / not ok)
        /// </summary>
        [XmlAttribute]
        public bool Status { get; set; }

    }
}

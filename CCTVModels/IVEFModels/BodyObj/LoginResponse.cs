using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IVEFModels.BodyObj
{
    public class LoginResponse
    {
        [XmlAttribute]
        public string Reason { get; set; }
        [XmlAttribute]
        public string ResponseOn { get; set; }
        /// <summary>
        /// 1 = Accepted 2 = Declined
        /// </summary>
        [XmlAttribute]
        public int Result { get; set; }
    }
}

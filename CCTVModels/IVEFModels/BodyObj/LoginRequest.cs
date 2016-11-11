using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IVEFModels.BodyObj
{
    public class LoginRequest
    {
        /// <summary>
        /// 1 = plain 2 - 6 the 5 most common encryption 0 = custom
        /// </summary>
        [XmlAttribute]
        public int Encryption { get; set; }
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string Password { get; set; }
    }
}

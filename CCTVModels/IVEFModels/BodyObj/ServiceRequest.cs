using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IVEFModels.BodyObj
{
    public class ServiceRequest
    {
        [XmlElement("Area")]
        public Area[] Areas { get; set; }
        public Transmission Transmission { get; set; }
        [XmlElement("Item")]
        public Item[] Items { get; set; }
        public Filter Filter { get; set; }
    }

    public class Area
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlElement("Pos")]
        public Position[] Postions { get; set; }
    }

    public class Transmission
    {
        [XmlAttribute]
        public int Type { get; set; }
        [XmlAttribute]
        public decimal Period { get; set; }
    }

    public class Item
    {
        [XmlAttribute]
        public int DataSelector { get; set; }
        [XmlAttribute]
        public string FieldSelector { get; set; }
    }

    public class Filter
    {
        [XmlAttribute]
        public string Predicate { get; set; }
    }
}

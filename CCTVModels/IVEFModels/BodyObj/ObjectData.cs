using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IVEFModels.BodyObj
{
    public class ObjectData
    {
        [XmlElement("TrackData")]
        public TrackData TrackData { get; set; }
        [XmlElement("VesselData")]
        public VesselData[] VesselDatas { get; set; }
        [XmlElement("VoyageData")]
        public VoyageData[] VoyageDatas { get; set; }
        [XmlElement("TaggedItem")]
        public TaggedItem[] TaggedItems { get; set; }
    }

    #region 【TrackData】
    public class TrackData
    {
        [XmlAttribute]
        public decimal COG { get; set; }
        [XmlAttribute]
        public decimal EstAccSOG { get; set; }
        [XmlAttribute]
        public decimal EstAccCOG { get; set; }
        [XmlAttribute]
        public int Id { get; set; }
        [XmlAttribute]
        public decimal Length { get; set; }
        [XmlAttribute]
        public decimal Heading { get; set; }
        [XmlAttribute]
        public decimal ROT { get; set; }
        [XmlAttribute]
        public decimal SOG { get; set; }
        [XmlAttribute]
        public string SourceId { get; set; }
        [XmlAttribute]
        public string SourceName { get; set; }
        [XmlAttribute]
        public DateTime UpdateTime { get; set; }
        [XmlAttribute]
        public int TrackStatus { get; set; }
        [XmlAttribute]
        public decimal Width { get; set; }
        public Position Pos { get; set; }
        public NavStatus NavStatus { get; set; }
    }

    public class NavStatus
    {
        [XmlAttribute]
        public int Value { get; set; }
    }
    #endregion 【TrackData】

    #region 【VesselData】
    public class VesselData
    {
        [XmlAttribute]
        public int Class { get; set; }
        [XmlAttribute]
        public bool BlackListed { get; set; }
        [XmlAttribute]
        public int Id { get; set; }
        [XmlAttribute]
        public string SpecialAttention { get; set; }
        [XmlAttribute]
        public string SourceId { get; set; }
        [XmlAttribute]
        public string SourceName { get; set; }
        [XmlAttribute]
        public int SourceType { get; set; }
        [XmlAttribute]
        public DateTime UpdateTime { get; set; }
        //[XmlIgnore]
        //public DateTime UpdateTime { get; set; }
        //[XmlAttribute("UpdateTime")]
        //public string UpdateTimeStr {
        //    get { return UpdateTime.ToUniversalTime().ToString("s") + "Z"; }
        //    set
        //    {
        //        string t = value.TrimEnd('Z');
        //        try
        //        {
        //            UpdateTime = DateTime.Parse(t).ToLocalTime();
        //        }
        //        catch (Exception e)
        //        {

        //        }
        //    }
        //}
        public Construction Construction { get; set; }
        public Identifier Identifier { get; set; }
    }

    public class Construction
    {
        [XmlAttribute]
        public string HullColor { get; set; }
        [XmlAttribute]
        public int HullType { get; set; }
        [XmlAttribute]
        public decimal DeadWeight { get; set; }
        [XmlAttribute]
        public decimal GrossWeight { get; set; }
        [XmlAttribute]
        public decimal Length { get; set; }
        [XmlAttribute]
        public int LloydsShipType { get; set; }
        [XmlAttribute]
        public int YearOfBuild { get; set; }
        [XmlAttribute]
        public decimal MaxAirDraught { get; set; }
        [XmlAttribute]
        public decimal MaxDraught { get; set; }
        [XmlAttribute]
        public int MaxPersonsOnBoard { get; set; }
        [XmlAttribute]
        public decimal MaxSpeed { get; set; }
        [XmlAttribute]
        public decimal Width { get; set; }
        public UnType UnType { get; set; }
    }

    public class UnType
    {
        [XmlAttribute]
        public string CodeA { get; set; }
        [XmlAttribute]
        public string CodeB { get; set; }
        [XmlAttribute]
        public int Mode { get; set; }
    }

    public class Identifier
    {
        [XmlAttribute]
        public string Callsign { get; set; }
        [XmlAttribute]
        public int IMO { get; set; }
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string FormerName { get; set; }
        [XmlAttribute]
        public string Flag { get; set; }
        [XmlAttribute]
        public string Owner { get; set; }
        [XmlAttribute]
        public int MMSI { get; set; }
        [XmlAttribute]
        public string LRIT { get; set; }

        [XmlElement("OtherId")]
        public OtherId[] OtherIds { get; set; }
        [XmlElement("OtherName")]
        public OtherName[] OtherNames { get; set; }
    }

    public class OtherId
    {
        [XmlAttribute]
        public string Id { get; set; }
        [XmlAttribute]
        public string Value { get; set; }
    }

    public class OtherName
    {
        [XmlAttribute]
        public string Lang { get; set; }
        [XmlAttribute]
        public string Name { get; set; }
    }
    #endregion 【VesselData】

    #region 【VoyageData】
    public class VoyageData
    {
        [XmlAttribute]
        public decimal AirDraught { get; set; }
        [XmlAttribute]
        public int Id { get; set; }
        [XmlAttribute]
        public int CargoTypeIMO { get; set; }
        [XmlAttribute]
        public string ContactIdentity { get; set; }
        [XmlAttribute]
        public string DestCode { get; set; }
        [XmlAttribute]
        public string DestName { get; set; }
        [XmlAttribute]
        public string DepartCode { get; set; }
        [XmlAttribute]
        public string DepartName { get; set; }
        [XmlAttribute]
        public decimal Draught { get; set; }
        [XmlAttribute]
        public DateTime ETA { get; set; }
        [XmlAttribute]
        public DateTime ATD { get; set; }
        [XmlAttribute]
        public decimal ISPSLevel { get; set; }
        [XmlAttribute]
        public decimal OverSizedLength { get; set; }
        [XmlAttribute]
        public decimal OverSizedWidth { get; set; }
        [XmlAttribute]
        public int PersonsOnBoard { get; set; }
        [XmlAttribute]
        public decimal Pilots { get; set; }
        [XmlAttribute]
        public bool RouteBound { get; set; }
        [XmlAttribute]
        public string SourceId { get; set; }
        [XmlAttribute]
        public string SourceName { get; set; }
        [XmlAttribute]
        public int SourceType { get; set; }
        [XmlAttribute]
        public int TankerStatus { get; set; }
        [XmlAttribute]
        public bool Tugs { get; set; }
        [XmlAttribute]
        public DateTime UpdateTime { get; set; }

        [XmlElement("Waypoint")]
        public Waypoint[] Waypoints { get; set; }
    }

    public class Waypoint
    {
        [XmlAttribute]
        public DateTime ATA { get; set; }
        [XmlAttribute]
        public DateTime ETA { get; set; }
        [XmlAttribute]
        public DateTime RTA { get; set; }
        [XmlAttribute]
        public string LoCode { get; set; }
        [XmlAttribute]
        public string Name { get; set; }

        public Position Pos { get; set; }
    }
    #endregion 【VoyageData】

    #region 【TaggedItem】
    public class TaggedItem
    {
        [XmlAttribute]
        public string Key { get; set; }
        /// <summary>
        /// 本可以是任意类型。
        /// </summary>
        [XmlAttribute]
        public string Value { get; set; }
    }
    #endregion 【TaggedItem】

    public class Position
    {
        [XmlAttribute]
        public decimal Altitude { get; set; }
        [XmlAttribute]
        public decimal EstAccAlt { get; set; }
        [XmlAttribute]
        public decimal EstAccLat { get; set; }
        [XmlAttribute]
        public decimal EstAccLong { get; set; }
        [XmlAttribute("Lat")]
        public decimal Latitude { get; set; }
        [XmlAttribute("Long")]
        public decimal Longitude { get; set; }
    }
}

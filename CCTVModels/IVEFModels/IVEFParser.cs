using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IVEFModels
{
    public class IVEFParser
    {
        private const string Template = "<?xml version=\"1.0\"?>\r\n<Root>\r\n{0}\r\n</Root>";
        private const string FileStart = "<?xml version=\"1.0\"?>\r\n";
        private const string StartLabel = "<MSG_IVEF";
        private const string EndLabel = "</MSG_IVEF>";
        public IVEFParser()
        {

        }

        public MSG_IVEF[] Parse(string xml)
        {
            if (isSingle(xml))
            {
                return new MSG_IVEF[] { parseSingle(xml) };
            }
            else
            {
                return parseGroup(xml).Ships;
            }
        }

        private bool isSingle(string xml)
        {
            int index = xml.IndexOf(EndLabel, StringComparison.OrdinalIgnoreCase);
            if (index < 0)
                throw new ArgumentException($"无效的IVEF数据内容，未找到{EndLabel}标签。", nameof(xml));
            if (index + EndLabel.Length == xml.Length)
                return true;
            return false;
        }

        private MSG_IVEF parseSingle(string xml)
        {
            int startIndex = xml.IndexOf(StartLabel, StringComparison.OrdinalIgnoreCase);
            if (startIndex > 0)
            {
                xml = xml.Substring(startIndex);
            }
            xml = FileStart + xml;
            xml = removeXmlns(xml);
            using (StringReader sr = new StringReader(xml))
            {
                XmlSerializer cpSer = new XmlSerializer(typeof(MSG_IVEF));
                cpSer.UnknownAttribute += CpSer_UnknownAttribute;
                cpSer.UnknownElement += CpSer_UnknownElement;
                cpSer.UnknownNode += CpSer_UnknownNode;
                MSG_IVEF temp = cpSer.Deserialize(sr) as MSG_IVEF;
                return temp;
            }
        }

        private ShipCollection parseGroup(string xml)
        {
            int startIndex = xml.IndexOf(StartLabel, StringComparison.OrdinalIgnoreCase);
            if (startIndex > 0)
            {
                xml = xml.Substring(startIndex);
            }

            xml = string.Format(Template, removeXmlns(xml));
            //只读出其中规定一个字段是可行的。
            using (StringReader sr = new StringReader(xml))
            {
                XmlSerializer cpSer = new XmlSerializer(typeof(ShipCollection));
                cpSer.UnknownAttribute += CpSer_UnknownAttribute;
                cpSer.UnknownElement += CpSer_UnknownElement;
                cpSer.UnknownNode += CpSer_UnknownNode;
                ShipCollection temp = cpSer.Deserialize(sr) as ShipCollection;
                return temp;
            }
        }

        private string removeXmlns(string src)
        {
            src = Regex.Replace(src, "xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", "", RegexOptions.IgnoreCase);
            src = Regex.Replace(src, "xmlns=\"http://www.iala-to-be-confirmed.org/XMLSchema/IVEF/0.2.5\"", "", RegexOptions.IgnoreCase);
            return src;
            //src = src.Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", "");
            //return src.Replace("xmlns=\"http://www.iala-to-be-confirmed.org/XMLSchema/IVEF/0.2.5\"", "");
        }

        private void CpSer_UnknownNode(object sender, XmlNodeEventArgs e)
        {
#if DEBUG
            Console.WriteLine("unknownnode");
#endif
        }

        private void CpSer_UnknownElement(object sender, XmlElementEventArgs e)
        {
#if DEBUG
            Console.WriteLine("unknownElement");
#endif
        }

        private void CpSer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
#if DEBUG
            Console.WriteLine("unknownAttribute");
#endif
        }
    }
}

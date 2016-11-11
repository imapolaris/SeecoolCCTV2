using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace GBTModels.Util
{
    public class SerializeHelper
    {
        public static SerializeHelper Instance { get; private set; }
        static SerializeHelper()
        {
            Instance = new SerializeHelper();
        }

        private XmlSerializerNamespaces _namespace;

        private SerializeHelper()
        {
            //用于去除默认的命名空间。
            _namespace = new XmlSerializerNamespaces();
            _namespace.Add("", "");
        }

        public string Serialize<T>(T obj)
        {
            //using (MemoryStream ms = new MemoryStream())
            //{
            //    using (XmlTextWriter tw = new XmlTextWriter(ms, Encoding.UTF8))
            //    {
            //        XmlSerializer temp = new XmlSerializer(typeof(T));
            //        temp.Serialize(tw, obj, _namespace);

            //        byte[] bbb = ms.ToArray();
            //        return Encoding.UTF8.GetString(bbb);
            //    }
            //}

            XmlSerializer seriazlier = new XmlSerializer(typeof(T));
            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            {
                seriazlier.Serialize(sw, obj, _namespace);
            }
            string str = sb.ToString();
            int index = str.IndexOf('>');
            str = str.Substring(index + 1);
            str = "<?xml version=\"1.0\"?>" + str;
            return str;
        }

        public T Deserialize<T>(string xml) where T : class
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StringBuilder sb = new StringBuilder();
            using (StringReader sr = new StringReader(xml))
            {
                object obj = serializer.Deserialize(sr);
                return obj as T;
            }
        }
    }
}

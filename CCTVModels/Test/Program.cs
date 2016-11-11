using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using GBTModels.Global;
using GBTModels.Util;
using IVEFModels;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //string xml = "<?xml version=\"1.0\"?>"
            //            + "<Control>"
            //            + "<CmdType>Keepalive</CmdType>"
            //            + "<SN>2720</SN>"
            //            + "<DeviceID>162211411701073990</DeviceID>"
            //            + "<Status>OK</Status>"
            //            + "</Control>";
            string xml = "对于某一级节点数就多达几千的情况 延迟加载无效，这种情况建议考虑分页异步加载";
            byte[] bytes = Encoding.UTF8.GetBytes(xml);
            int half = bytes.Length / 2;
            string first = Encoding.UTF8.GetString(bytes, 0, half);
            string second = Encoding.UTF8.GetString(bytes, half, bytes.Length - half);
            Console.WriteLine(first);
            Console.WriteLine(second);
            List<byte> bList = new List<byte>();
            bList.AddRange(Encoding.UTF8.GetBytes(first.Substring(5)));
            bList.AddRange(Encoding.UTF8.GetBytes(second));
            string third = Encoding.UTF8.GetString(bList.ToArray());
            Console.WriteLine(third);
            Console.ReadLine();
            //CommandInfo ci = CmdIdentifier.GetCommandInfo(xml);
            //Console.ReadLine();
            //using (FileStream fs = new FileStream("XMLFile1.xml", FileMode.Open, FileAccess.Read))
            //{
            //    using (StreamReader sr = new StreamReader(fs))
            //    {
            //        string xml = sr.ReadToEnd();
            //        IVEFParser parse = new IVEFParser();
            //        MSG_IVEF[] msg = parse.Parse(xml);
            //        Console.WriteLine("sdfsfsf");
            //    }
            //}
            //Console.ReadLine();
        }
    }
}

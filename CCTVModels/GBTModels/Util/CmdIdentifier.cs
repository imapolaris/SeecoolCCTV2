using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using GBTModels.Global;

namespace GBTModels.Util
{
    /// <summary>
    /// 命令识别器。
    /// </summary>
    public class CmdIdentifier
    {
        /// <summary>
        /// 识别命令类型及命令名称。
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static CommandInfo GetCommandInfo(string xml)
        {
            using (StringReader sr = new StringReader(xml))
            {
                XmlReader reader = XmlReader.Create(sr);
                XmlDocument doc = new XmlDocument();
                doc.Load(reader);

                CommandInfo ci = new CommandInfo();
                ci.CommandTypeStr = doc.DocumentElement.Name;
                ci.CommandType = parseCmdType(ci.CommandTypeStr);

                XmlNodeList nList = doc.GetElementsByTagName("CmdType");
                if (nList.Count > 0)
                {
                    ci.CommandNameStr = nList[0].InnerText.Trim();
                    ci.CommandName = parseCmdName(ci.CommandNameStr);
                }
                reader.Close();
                return ci;
            }
        }

        private static CommandType parseCmdType(string cmdType)
        {
            foreach (string str in Enum.GetNames(typeof(CommandType)))
            {
                if (str.Equals(cmdType, StringComparison.OrdinalIgnoreCase))
                    return (CommandType)Enum.Parse(typeof(CommandType), str);
            }
            return CommandType.Unknown;
        }

        private static CommandName parseCmdName(string cmdName)
        {
            foreach (string str in Enum.GetNames(typeof(CommandName)))
            {
                if (str.Equals(cmdName, StringComparison.OrdinalIgnoreCase))
                    return (CommandName)Enum.Parse(typeof(CommandName), str);
            }
            return CommandName.Unknown;
        }
    }
}

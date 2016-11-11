using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GBTModels.Global
{
    public class CommandInfo
    {
        public CommandInfo()
        {
            CommandType = CommandType.Unknown;
            CommandName = CommandName.Unknown;
        }

        public CommandInfo(CommandType cmdType, CommandName cmdName, string cmdTypeStr, string cmdNameStr)
        {
            CommandType = cmdType;
            CommandName = cmdName;
            CommandTypeStr = cmdTypeStr;
            CommandNameStr = cmdNameStr;
        }

        public string CommandNameStr { get; set; }
        public string CommandTypeStr { get; set; }
        public CommandType CommandType { get; set; }
        public CommandName CommandName { get; set; }
    }
}

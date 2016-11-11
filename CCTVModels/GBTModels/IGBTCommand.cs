using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GBTModels
{
    public interface IGBTCommand
    {
        string CmdType { get; }
        int SN { get; }
    }
}

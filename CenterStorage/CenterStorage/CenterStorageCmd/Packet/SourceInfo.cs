using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageCmd
{
    public class SourceInfo : ISourceInfo
    {
        public string SourceIp { get; private set; }

        public int SourcePort { get; private set; }
        public SourceInfo(string sourceIp, int sourcePort)
        {
            SourceIp = sourceIp;
            SourcePort = sourcePort;
        }
    }
}

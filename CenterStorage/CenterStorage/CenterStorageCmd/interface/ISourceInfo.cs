using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageCmd
{
    public interface ISourceInfo
    {
        string SourceIp { get; }
        int SourcePort { get; }
    }
}

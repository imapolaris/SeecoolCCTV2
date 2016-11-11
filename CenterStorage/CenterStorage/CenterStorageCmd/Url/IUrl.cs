using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageCmd.Url
{
    public interface IUrl
    {
        string LocalPath { get; }
        IVideoInfo[] VideoInfos { get; }
    }
}

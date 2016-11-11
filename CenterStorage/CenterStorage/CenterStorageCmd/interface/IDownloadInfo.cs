using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageCmd
{
    public interface IDownloadInfo: IVideoBaseInfom, IVideoInfo, ITimePeriod, ISourceInfo
    {
        string DownloadPath { get; }
        void UpdatePath(string path);
    }
}

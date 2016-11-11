using CenterStorageCmd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVDownloadService
{
    public interface IOnlinePlayback
    {
        Guid GuidCode { get; }
        void SetProbeTime(DateTime time);
        string DownloadToLocal(string path);
        void SetPriority(bool priority);
        bool IsEndOfDownload();
        IDownloadInfo DownloadInfo { get; }
        bool IsLocalDownload { get; }
    }
}

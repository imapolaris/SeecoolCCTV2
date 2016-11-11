using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageCmd
{
    public interface IDownloadInfoExpand
    {
        IDownloadInfo DownloadInfo { get; }
        Guid GuidCode { get; }
        string Name { get; }
        string Quality { get; }
        long Size { get; }
        bool IsLocalDownload { get; }
        DownloadStatus DownloadStatus { get; }
        TimePeriodPacket[] TimePeriodsAll { get; }
        TimePeriodPacket[] TimePeriodsCompleted { get; }
        string ErrorInfo { get; }
        DateTime UpdatedLastestTime { get; }
        long Speed { get; }
    }
}

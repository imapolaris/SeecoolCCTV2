using CenterStorageCmd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVDownloadService
{
    public interface IDownloadManager
    {
        VideoStreamsPacket GetVideoStreamsPacket(DateTime time);
        VideoTimePeriodsPacket GetVideoTimePeriods();
        VideoBasePacket GetVideoBasePacket();
        VideoTimePeriodsPacket GetCompletedTimePeriods();
    }
}

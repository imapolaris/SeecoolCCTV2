using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageCmd
{
    public enum ParamCode
    {
        TimePeriods = 10001,
        VideoPacket = 10002,
        VideoBaseInfo = 10003,
        VideoInfosTimePeriods = 10004,

        DownloadBase =10010,
        DownloadBegin = 10011,
        DownloadToLocal = 10012,
        DownloadProgress = 10013,

        ProbeTime = 10021,

        DownloadInfosAll = 10031,
        DownloadInfosAdd = 10032,
        DownloadInfoPart = 10033,
        DownloadControl = 10034,

        LocalDownloadPath = 10040,
        LocalDownSource = 10041,
        LocalDownloadBegin = 10042,

        Message = 20001,

        StorageFlagAll = 30001,
        StorageFlag = 30002,

        EnsureConnect = 90001,
    }
}

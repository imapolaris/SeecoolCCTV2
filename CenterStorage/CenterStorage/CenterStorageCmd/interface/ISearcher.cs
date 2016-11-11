using CenterStorageCmd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageService
{
    public interface ISearcher
    {
        VideoTimePeriodsPacket Search(DateTime start, DateTime end, IVideoInfo videoInfo);
        VideoTimePeriodsPacket[] Search(DateTime start, DateTime end, IVideoInfo[] videoInfos);
    }
}

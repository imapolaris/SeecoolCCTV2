using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVReplay
{
    public interface ITimeProcess
    {
        Action JumpEvent { get; set; }
        DateTime GetPlayingTime();
        double PlayRate { get; }
        int FastTimes { get; }
        Action FastTimesEvent { get; set; }

        void AddCache(Guid guid);
        void UpdateCache(Guid guid, DateTime dateTime);
        void RemoveCache(Guid guid);
    }
}

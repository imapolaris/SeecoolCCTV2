using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageCmd
{
    public static class TimeProbeManager
    {
        public static DateTime GetProbeTime(TimePeriodPacket[] times, DateTime begin)
        {
            DateTime probe = DateTime.MaxValue;
            if (times == null || times.Length == 0)
                return probe;
            if (begin >= times.Last().EndTime)
                return probe;
            TimePeriodPacket packet = times.FirstOrDefault(_ => _.IsInRange(begin));
            if (packet != null)
                return begin;
            packet = times.FirstOrDefault(_ => _.BeginTime >= begin);
            return packet.BeginTime;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVDownloadService
{
    public class DownloadSpeedMonitor
    {
        List<DownloadCount> _list = new List<DownloadCount>();
        long _length;
        public long Speed(DateTime curTime)
        {
            removeInvalid(curTime);
            if (_list.Count < 2)
                return 0;
            var first = _list.First();
            var last = _list.Last();
            return (long)((last.Count - first.Count) * 1e+7 / (last.Time - first.Time).Ticks);
        }

        public void Add(DateTime time, long len)
        {
            _length += len;
            _list.Add(new DownloadCount(time, _length));
            removeInvalid(time);
        }

        private void removeInvalid(DateTime curTime)
        {
            while (_list.Count > 1)
            {
                if (curTime > _list.First().Time.AddSeconds(10))
                    _list.RemoveAt(0);
                else
                    break;
            }
        }

        struct DownloadCount
        {
            public DateTime Time { get; private set; }
            public long Count { get; private set; }
            public DownloadCount(DateTime time,long count)
            {
                Time = time;
                Count = count;
            }
        }
    }
}

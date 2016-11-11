using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCTVStreamCmd
{
    public class StreamKeyDetector
    {
        static TimeSpan Span = TimeSpan.FromSeconds(60);
        Queue<StreamLengthInfo> _queue = new Queue<StreamLengthInfo>();
        int max = 0;
        public bool Update(DateTime time, int len)
        {
            if (len < max / 10)
                return false;
            _queue.Enqueue(new StreamLengthInfo(time, len));
            removeInvalidInfo();
            max = _queue.Max(_ => _.Length);
            if (max < 1000)
                return false;
            return (len > max * 2 / 5);
        }

        private void removeInvalidInfo()
        {
            while (_queue.Last().Time - _queue.First().Time > Span)
                _queue.Dequeue();
        }

        class StreamLengthInfo
        {
            public DateTime Time { get; private set; }
            public int Length { get; private set; }
            public StreamLengthInfo(DateTime time, int length)
            {
                Time = time;
                Length = length;
            }
        }
    }
}

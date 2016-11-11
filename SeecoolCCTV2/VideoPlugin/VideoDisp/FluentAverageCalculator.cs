using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoNS.VideoDisp
{
    public class FluentAverageCalculator
    {
        private DateTime _lastTime = DateTime.Now;
        private TimeSpan _maxInterval = TimeSpan.FromSeconds(10);
        private TimeSpan _refreshInterval = TimeSpan.FromSeconds(1);
        private Queue<TimeSpan> _frameIntervals = new Queue<TimeSpan>();
        private TimeSpan _totalInterval = TimeSpan.Zero;
        private double _average = 0;

        public FluentAverageCalculator()
        {
        }

        public FluentAverageCalculator(TimeSpan interval) : this()
        {
            _maxInterval = interval;
        }

        public double Calculate()
        {
            DateTime time = DateTime.Now;
            TimeSpan interval = time - _lastTime;
            _lastTime = time;
            _frameIntervals.Enqueue(interval);
            _totalInterval += interval;

            if (_totalInterval >= _maxInterval)
            {
                if (_frameIntervals.Count > 0 && _totalInterval > TimeSpan.Zero)
                {
                    double avg = _totalInterval.TotalMilliseconds / _frameIntervals.Count;
                    double sum = _frameIntervals.Average(x =>
                        {
                            double delta = x.TotalMilliseconds - avg;
                            return delta * delta;
                        });
                    _average = Math.Sqrt(sum);
                }

                TimeSpan ts = TimeSpan.Zero;
                if (_totalInterval > _refreshInterval)
                    ts = _totalInterval - _refreshInterval;
                while (_totalInterval > ts)
                    _totalInterval -= _frameIntervals.Dequeue();
            }

            return _average;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoNS.VideoDisp
{
    public class XpsCalculator
    {
        private DateTime _startTime = DateTime.Now;
        private TimeSpan _interval = TimeSpan.FromSeconds(5);
        private double _count = 0;
        private double _xps = 0;
        private bool _first = true;

        public XpsCalculator()
        {
        }

        public XpsCalculator(TimeSpan interval) : this()
        {
            _interval = interval;
        }

        public double Calculate(double value)
        {
            _count += value;
            DateTime now = DateTime.Now;
            TimeSpan elapsed = now - _startTime;
            if (elapsed >= _interval)
            {
                _xps = _count / elapsed.TotalSeconds;

                _count = 0;
                _startTime = now;
                _first = false;
            }
            else if (_first && elapsed > TimeSpan.FromMilliseconds(1))
                _xps = _count / elapsed.TotalSeconds;

            return _xps;
        }
    }
}

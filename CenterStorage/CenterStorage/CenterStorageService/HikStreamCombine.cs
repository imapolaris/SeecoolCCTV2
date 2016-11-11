using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CenterStorageService
{
    public class HikStreamCombine
    {
        static TimeSpan _timeoutSpan = TimeSpan.FromMilliseconds(70);//30ms合并时间
        List<HikStreamInfo> _streams = new List<HikStreamInfo>();
        const int _maxStatic = 5120;//>5120 的情况下不考虑合并的问题
        int _max = _maxStatic;
        public List<HikStreamInfo[]> Update(DateTime time, byte[] data)
        {
            List<HikStreamInfo[]> list = new List<HikStreamInfo[]>();
            if (data.Length > _max)
            {
                _max = data.Length;
                if (_streams.Count > 0)
                    list.Add(dequeue());
                Console.WriteLine("非Packets组成的Frame" + data.Length);
            }
            if (_max != _maxStatic)
            {
                list.Add(new HikStreamInfo[] { new HikStreamInfo(time, data) });
            }
            else if (isTimeout(time))
            {
                Console.WriteLine("-----------------Time Out----------------------");
                list.Add(dequeue());
                _streams.Add(new HikStreamInfo(time, data));
            }
            else
            {
                _streams.Add(new HikStreamInfo(time, data));
                if (data.Length < 5120 && data.Length > 50)
                    list.Add(dequeue());
            }
            return list;
        }

        HikStreamInfo[] dequeue()
        {
            var streams = _streams.ToArray();
            _streams = new List<HikStreamInfo>();
            return streams;
        }

        private bool isTimeout(DateTime time)
        {
            return _streams.Count > 0 && (_streams.First().Time.Add(_timeoutSpan) < time);
        }

        public class HikStreamInfo
        {
            public DateTime Time { get; private set; }
            public byte[] Buffer { get; private set; }
            public HikStreamInfo(DateTime time, byte[] buffer)
            {
                Time = time;
                Buffer = buffer;
            }
        }
    }
}
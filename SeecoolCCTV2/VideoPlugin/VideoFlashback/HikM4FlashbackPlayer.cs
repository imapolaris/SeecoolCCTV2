using CCTVClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VideoNS.VideoDistribute;
using VideoStreamClient.Entity;

namespace VideoNS.VideoFlashback
{
    internal class HikM4FlashbackPlayer : IFlashbackPlayer
    {
        public DateTime StartTime { get; private set; } = DateTime.Now;
        public DateTime EndTime { get; private set; } = DateTime.Now;

        public DateTime CurrentTime
        {
            get { return _currentTime; }
            set
            {
                DateTime time = value;
                if (time < StartTime)
                    time = StartTime;
                if (time > EndTime)
                    time = EndTime;

                lock (_playTimeLockObj)
                {
                    _startPlayTime = time;
                    _startRealTime = DateTime.Now;
                    _reposed = true;
                }
            }
        }
        DateTime _currentTime = DateTime.Now;

        public bool Playing
        {
            get { return _playing; }
            set
            {
                if (value)
                {
                    initThread();

                    lock (_playTimeLockObj)
                    {
                        _startPlayTime = CurrentTime;
                        _startRealTime = DateTime.Now;
                        _playing = true;
                    }
                }
                else
                    _playing = false;
            }
        }
        bool _playing = false;

        public double Speed
        {
            get{ return _speed; }
            set
            {
                lock (_playTimeLockObj)
                {
                    _startPlayTime = CurrentTime;
                    _startRealTime = DateTime.Now;
                    _speed = value;
                    _expFastTimes = Math.Max(-4, Math.Min(4, (int)Math.Round(Math.Log(_speed, 2))));
                }
            }
        }
        double _speed = 1;

        public event Action<VideoFrame> VideoFrameEvent;
        private void fireVideoFrameEvent(VideoFrame frame)
        {
            var callback = VideoFrameEvent;
            if (callback != null)
                callback(frame);
        }

        Record<HikM4Package>[] _records;

        public HikM4FlashbackPlayer(Record<HikM4Package>[] records)
        {
            _records = records;
            if (_records.Length > 0)
            {
                StartTime = _records[0].Time;
                EndTime = _records[_records.Length - 1].Time + TimeSpan.FromMilliseconds(40);
                _currentTime = StartTime;
            }
        }

        public void Dispose()
        {
            _disposeEvent.Set();
            _thread = null;
        }

        ~HikM4FlashbackPlayer()
        {
            Dispose();
        }

        ManualResetEvent _disposeEvent = new ManualResetEvent(false);
        Thread _thread;

        private void initThread()
        {
            if (_thread == null)
            {
                _thread = new Thread(run);
                _thread.IsBackground = true;
                _thread.Start();
            }
        }

        DateTime _startRealTime = DateTime.Now;
        DateTime _startPlayTime = DateTime.Now;
        bool _reposed = false;
        object _playTimeLockObj = new object();

        void run()
        {
            HikM4Decoder decoder = new HikM4Decoder();
            decoder.VideoFrameEvent += Hikm4Decoder_VideoFrameEvent;

            byte[] header = null;
            int index = 0;

            int wait = 0;
            while (!_disposeEvent.WaitOne(wait))
            {
                wait = 1;
                if (_records.Length > 0)
                {
                    bool reposed = false;
                    DateTime startRealTime;
                    DateTime startPlayTime;
                    lock (_playTimeLockObj)
                    {
                        startRealTime = _startRealTime;
                        startPlayTime = _startPlayTime;
                        reposed = _reposed;
                        _reposed = false;
                    }

                    if (_playing || reposed)
                    {
                        DateTime curPlayTime = startPlayTime + new TimeSpan((long)Math.Round((DateTime.Now - startRealTime).Ticks * _speed));
                        if (curPlayTime > EndTime)
                            curPlayTime = EndTime;

                        Record<HikM4Package> record = null;
                        if (reposed)
                        {
                            for (index = 0; index < _records.Length; index++)
                                if (_records[index].Time >= curPlayTime)
                                    break;
                            if (index < _records.Length)
                                record = _records[index];
                        }
                        else
                        {
                            int next = index + 1;
                            if (next < _records.Length && _records[next].Time < curPlayTime)
                            {
                                index = next;
                                record = _records[index];
                            }
                        }

                        if (record != null)
                        {
                            _currentTime = record.Time;
                            if (!HikM4Decoder.ByteArrayEqual(header, record.Header))
                            {
                                header = record.Header;
                                decoder.InputData(HikM4Decoder.HeaderType, header);
                            }
                            updatePlaySpeed(decoder);
                            decoder.InputData(record.Package.Type, record.Package.Data);
                            wait = 0;
                        }
                    }
                }
            }
        }
        int _expFastTimes = 0;
        int _fastTimes = 0;
        private void updatePlaySpeed(HikM4Decoder decoder)
        {
            if (_fastTimes == _expFastTimes)
                return;
            while (_fastTimes < _expFastTimes)
            {
                decoder.Fast();
                _fastTimes++;
            }
            while (_fastTimes > _expFastTimes)
            {
                decoder.Slow();
                _fastTimes--;
            }
            Console.WriteLine("FastTimes: " + _fastTimes);
        }

        private void Hikm4Decoder_VideoFrameEvent(HikM4Decoder.VideoFrame frame)
        {
            var videoFrame = new VideoFrame()
            {
                Width = frame.Width,
                Height = frame.Height,
                Timestamp = frame.Timestamp,
                Data = frame.Data,
            };
            fireVideoFrameEvent(videoFrame);
        }

    }
}

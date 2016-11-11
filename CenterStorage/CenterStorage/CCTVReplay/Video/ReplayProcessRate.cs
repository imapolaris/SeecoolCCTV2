using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVReplay.Video
{
    public class ReplayProcessRate : ITimeProcess
    {
        //播放结束时间点
        DateTime _playEndTime;
        //播放开始时对应的回放时间
        DateTime _playBeginTime;
        //播放开始时对应的系统时间（与机器时间对应）
        DateTime _sysBeginTime = DateTime.MaxValue;
        //缓冲时间
        DateTime _cachingTime;
        //播放速度
        double _playRate = 1;

        public ReplayProcessRate(DateTime begin)
        {
            _playEndTime = begin;
            JumpTo(begin);
        }

        #region 播放进度跳转

        public void JumpTo(DateTime time)
        {
            _cachingTime = time;
            resetBeginTime(_cachingTime);
            onJump();
        }

        public Action JumpEvent { get; set; }

        private void onJump()
        {
            var handler = JumpEvent;
            if (handler != null)
                handler();
        }

        #endregion 播放进度跳转

        public bool Playing
        {
            get { return _sysBeginTime != DateTime.MaxValue; }
            set
            {
                resetBeginTimeFromNow();
                _sysBeginTime = value ? DateTime.Now : DateTime.MaxValue;
            }
        }

        public double PlayRate
        {
            get
            {
                return _playRate;
            }
            set
            {
                resetBeginTimeFromNow();
                _playRate = value;
            }
        }

        int _fastTimes = 0;
        public int FastTimes { get { return _fastTimes; }
            set {
                _fastTimes = value;
                //Console.WriteLine("To FastTimes: " + FastTimes);
                onFastTimes();
            } }

        public Action FastTimesEvent { get; set; }
        private void onFastTimes()
        {
            var handler = FastTimesEvent;
            if (handler != null)
                handler();
        }

        private void resetBeginTimeFromNow()
        {
            resetBeginTime(GetPlayingTime());
        }

        //更新开始时间
        private void resetBeginTime(DateTime playTime)
        {
            _playBeginTime = playTime;
            if(_sysBeginTime != DateTime.MaxValue)
                _sysBeginTime = DateTime.Now;
        }

        public DateTime GetPlayingTime()
        {
            DateTime playingTime = getPlayingTime();
            if (playingTime > _cachingTime)
            {
                playingTime = _cachingTime;
                resetBeginTime(_cachingTime);
            }
            return playingTime;
        }

        private DateTime getPlayingTime()
        {
            long sysPeriodTicks = DateTime.Now.Ticks - _sysBeginTime.Ticks;
            long playingTicks = (long)Math.Max(0, sysPeriodTicks * PlayRate);
            DateTime playingTime = _playBeginTime.AddTicks(playingTicks);
            return playingTime;
        }

        #region 各绑定视频缓冲状态
        
        Dictionary<Guid, DateTime> _dictVideoCache = new Dictionary<Guid, DateTime>();
        public void RemoveCache(Guid guid)
        {
            lock (_dictVideoCache)
            {
                _dictVideoCache.Remove(guid);
                updateCache();
            }
        }

        public void AddCache(Guid guid)
        {
            lock (_dictVideoCache)
            {
                if (!_dictVideoCache.ContainsKey(guid))
                    _dictVideoCache.Add(guid, GetPlayingTime());
                updateCache();
            }
        }

        public void UpdateCache(Guid guid, DateTime time)
        {
            lock (_dictVideoCache)
            {
                if (_dictVideoCache.ContainsKey(guid))
                    _dictVideoCache[guid] = time;
                updateCache();
            }
        }

        private void updateCache()
        {
            DateTime time = _playBeginTime;
            if (_dictVideoCache.Count > 0)
            {
                DateTime min = _dictVideoCache.Values.Min();
                if (min > time)
                    time = min;
            }
            _cachingTime = time;
        }

        #endregion 各绑定视频缓冲状态
    }
}
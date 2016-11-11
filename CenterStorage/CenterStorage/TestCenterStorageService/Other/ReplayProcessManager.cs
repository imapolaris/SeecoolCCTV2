using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCenterStorageService.Other
{
    public class ReplayProcessManager
    {
        DateTime _playStartTime;
        DateTime _sysStartTime;

        public ReplayProcessManager(DateTime startTime)
        {
            JumpTo(startTime);
        }

        #region 播放暂停控制
        bool _playing = false;
        public bool Playing
        {
            get { return _playing; }
            set
            {
                if (Playing != value)
                {
                    _playing = value;
                    if (Playing)
                        play();
                    else
                        pause();
                }
            }
        }

        private void play()
        {
            _sysStartTime = DateTime.Now;
        }

        private void pause()
        {
            _playStartTime = getPlayingTime();
        }
        #endregion 播放暂停控制

        #region 获取当前播放时间
        public DateTime GetPlayingTime()
        {
            if (Playing)
                return getPlayingTime();
            else
                return
                    _playStartTime;
        }

        private DateTime getPlayingTime()
        {
            TimeSpan sysSpan = DateTime.Now - _sysStartTime;
            TimeSpan playSpan = TimeSpan.FromTicks((long)(sysSpan.Ticks * PlaySpeed));
            return _playStartTime.Add(playSpan);
        }
        #endregion 获取当前播放时间

        #region 播放速度控制
        double _playSpeed = 1;
        public double PlaySpeed
        {
            get { return _playSpeed; }
            set
            {
                _playSpeed = value;
                resetSpeed();
            }
        }

        private void resetSpeed()
        {
            _playStartTime = GetPlayingTime();
            _sysStartTime = DateTime.Now;
        }
        #endregion 播放速度控制

        public void JumpTo(DateTime time)
        {
            _playStartTime = time;
            _sysStartTime = DateTime.Now;
        }
    }
}
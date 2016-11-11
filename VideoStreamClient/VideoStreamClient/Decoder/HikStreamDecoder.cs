using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoStreamClient.Entity;
using CCTVClient;
using CenterStorageCmd;

namespace VideoStreamClient
{
    public class HikStreamDecoder : IStreamDecoder
    {
        public Action<VideoFrame> VideoFrameEvent { get; set; }
        HikM4Decoder _decoder;
        public HikStreamDecoder(byte[] data)
        {
            _decoder = new HikM4Decoder();
            _decoder.InputData(HikM4Decoder.HeaderType, data);
            _decoder.VideoFrameEvent += onVideoFrame;
        }

        public bool Update(StreamPacket packet)
        {
            if(_decoder != null)
                return _decoder.InputData(1, packet.Buffer);
            return false;
        }

        #region 回放速度控制
        int _expFastTimes = 0;
        int _fastTimes = 0;
        public void PlayingSpeed(int fastTimes)
        {
            _expFastTimes = Math.Max(-4, Math.Min(4, fastTimes));
            updatePlayingSpeed();
        }

        private void updatePlayingSpeed()
        {
            if (_fastTimes == _expFastTimes)
                return;
            if (_decoder == null)
                return;
            Console.WriteLine("FastTimes:  {0} -> {1}", _fastTimes, _expFastTimes);
            while (_fastTimes < _expFastTimes)
                fast();
            while (_fastTimes > _expFastTimes)
                slow();
            Console.WriteLine("FastTimes: {0}", _fastTimes);
        }

        private void slow()
        {
            bool result = _decoder.Slow();
            Console.WriteLine(result);
            _fastTimes--;
        }

        private void fast()
        {
            bool result = _decoder.Fast();
            Console.WriteLine(result);
            _fastTimes++;
        }
        #endregion 回放速度控制

        private void onVideoFrame(HikM4Decoder.VideoFrame obj)
        {
            onDisplay(new VideoFrame(obj.Width, obj.Height, obj.Timestamp, obj.Data));
        }

        private void onDisplay(VideoFrame frame)
        {
            var handler = VideoFrameEvent;
            if (handler != null)
                handler(frame);
        }

        public void Dispose()
        {
            if (_decoder != null)
            {
                _decoder.VideoFrameEvent -= onVideoFrame;
                _decoder.Dispose();
            }
            _decoder = null;
        }
    }
}

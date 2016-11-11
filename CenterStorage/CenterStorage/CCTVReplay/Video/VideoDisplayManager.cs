using CenterStorageCmd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using VideoRender;
using VideoStreamClient.Decoder;
using VideoStreamClient.Entity;

namespace CCTVReplay.Video
{
    public class VideoDisplayManager: IDisposable
    {
        VideoCacheManager _cache;
        VideoStreamDecoder _decoder;
        VideoStreamsPacket _videoPacket;
        Object _obj = new object();
        int _packetIndex = 0;
        AutoResetEvent _disposeEvent = new AutoResetEvent(false);
        Guid _guid = Guid.NewGuid();
        public int Width { get; private set; }
        public int Height { get; private set; }
        IRenderSource _render;
        ITimeProcess _replayProcess;    //播放进度控制

        public VideoFrame LastFrame { get; private set; }
        public Action<ImageSource> VideoFrameImageEvent { get; set; }
        public double Speed
        {
            get { return _replayProcess.PlayRate; }
        }

        public VideoDisplayManager(VideoCacheManager cache, ITimeProcess playProcess)
        {
            _replayProcess = playProcess;
            _replayProcess.JumpEvent += onJump;
            _replayProcess.FastTimesEvent += onFastTimes;
            _replayProcess.AddCache(_guid);
            _render = new D3DImageSource();
            _render.ImageSourceChanged += render_ImageSourceChanged;
            _decoder = new VideoStreamDecoder();
            _decoder.VideoFrameEvent += onVideoFrame;
            _cache = cache;
            _cache.PropertyChanged += _cache_PropertyChanged;
            _disposeEvent.Reset();
            new Thread(run) { IsBackground = true }.Start();
            onJump();
            onFastTimes();
        }

        private void onJump()
        {
            _videoPacket = null;
            onCache(true);
            _cache.Start(_replayProcess.GetPlayingTime());
            _replayProcess.UpdateCache(_guid, _replayProcess.GetPlayingTime());
        }

        private void onFastTimes()
        {
            _decoder?.PlayingSpeed(_replayProcess.FastTimes);
        }

        private void run()
        {
            while (!_disposeEvent.WaitOne(1))
            {
                _replayProcess.UpdateCache(_guid, _cache.NextTime);
                playByTime(_replayProcess.GetPlayingTime());
            }
        }

        private void playByTime(DateTime time)
        {
            try
            {
                onCache(noAnyValidFrameStream(time));
                updateHaveDataFromTimePeriods(time);
                while (!_disposeEvent.WaitOne(0))
                {
                    while (needToNextPacket(time) && _cache.Count() > 0)
                    {
                        _videoPacket = _cache.DequeuePacket();
                        //Console.WriteLine("{0} \t {1}\t{2}", _guid.ToString(), _videoPacket.TimePeriod.BeginTime.TimeOfDay, _videoPacket.TimePeriod.EndTime.TimeOfDay);
                        _packetIndex = 0;
                    }
                    if (_videoPacket == null)
                        return;
                    int index = _packetIndex;
                    if (_packetIndex < _videoPacket.VideoStreams.Length)
                    {
                        var stream = _videoPacket.VideoStreams.ElementAt(_packetIndex);
                        if (stream.Time <= time)
                        {
                            lock (_obj)
                            {
                                if (_decoder != null)
                                {
                                    _decoder.Update(stream);
                                    _packetIndex++;
                                    //if (_decoder.Update(stream))
                                    //    _packetIndex++;
                                    //else
                                    //    Console.WriteLine("缓冲区已满，需要重复送入数据!");
                                }
                            }
                            //Console.WriteLine("Stream: {0} \t{1}\t{2}", stream.Time.TimeOfDay, stream.Type, stream.Buffer.Length);
                        }
                        else
                            return;
                    }
                    if (index == _packetIndex)
                        Thread.Sleep(1);
                }
            }
            catch(Exception ex) {
                Console.WriteLine("playByTime error! \n" + ex.ToString());
            }
        }

        private void updateHaveDataFromTimePeriods(DateTime time)
        {
            if(_cache?.TimePeriods != null)
            {
                bool haveData = _cache.TimePeriods.Any(_ => _.IsInRange(time));
                onHavingVideoData(haveData);
            }
        }

        bool noAnyValidFrameStream(DateTime time)
        {
            if (_cache?.TimePeriods == null)
                return true;
            if (_cache.TimePeriods.Length == 0 || _cache.TimePeriods.Last().EndTime <= time)
                return false;
            return (_cache.Count() == 0 && (_videoPacket == null || _videoPacket.TimePeriod.EndTime <= time));
        }

        bool needToNextPacket(DateTime time)
        {
            if (_videoPacket == null)
                return true;
            if (_videoPacket.TimePeriod.EndTime < time)
                return true;
            if (_videoPacket.VideoStreams.Length == 0)
                return true;
            if (_packetIndex >= _videoPacket.VideoStreams.Length)
                return true;
            return false;
        }

        private void render_ImageSourceChanged()
        {
            onVideoImage(_render.ImageSource);
        }

        private void _cache_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(_cache.BaseInfo):
                    {
                        try
                        {
                            lock (_obj)
                            {
                                _decoder.InitHeader(_cache.BaseInfo.Header);
                                onFastTimes();
                            }
                            Common.Log.Logger.Default.Debug("解析视频头完毕！ ");
                        }
                        catch (Exception ex)
                        {
                            Common.Log.Logger.Default.Error(ex);
                            _cache.Reload();
                        }
                    }
                    break;
            }
        }

        private void onVideoFrame(VideoFrame frame)
        {
            //Console.WriteLine("Out:\t{0}\t{1}\t{2}\t{3}\t{4}", frame.Timestamp, frame.Width, frame.Height, frame.Data.Length, DateTime.Now.TimeOfDay);
            if (frame.Width != Width || frame.Height != Height)
            {
                Width = frame.Width;
                Height = frame.Height;
                _render.SetupSurface(frame.Width, frame.Height);
            }
            LastFrame = frame;
            _render.Render(frame.Data);
        }

        private void onVideoImage(ImageSource source)
        {
            var handle = VideoFrameImageEvent;
            if (handle != null)
                handle(source);
        }

        public void Dispose()
        {
            _replayProcess.RemoveCache(_guid);
            _disposeEvent.Set();
            lock(_obj)
            {
                if (_decoder != null)
                {
                    _decoder.VideoFrameEvent -= onVideoFrame;
                    _decoder.Dispose();
                }
                _decoder = null;
            }
        }
        #region 事件通知

        public Action<bool> OnCaching;
        private void onCache(bool caching)
        {
            var handle = OnCaching;
            if (handle != null)
                handle(caching);
        }

        public Action<bool> OnHaveData;
        private void onHavingVideoData(bool haveData)
        {
            var handle = OnHaveData;
            if (handle != null)
                handle(haveData);
        }
        # endregion 事件通知
    }
}

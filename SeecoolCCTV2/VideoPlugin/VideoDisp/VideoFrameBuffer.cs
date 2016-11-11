using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VideoNS.VideoDisp
{
    public class VideoFrameBuffer
    {
        public delegate void OnVideoFrame(int width, int height, byte[] frameData, int timeStamp);
        public event OnVideoFrame VideoFrameEvent;
        private void fireOnVideoFrame(int width, int height, byte[] frameData, int timeStamp)
        {
            OnVideoFrame callback = VideoFrameEvent;
            if (callback != null)
                callback(width, height, frameData, timeStamp);
        }

        long _maxBufferSize = 0;

        public VideoFrameBuffer(long maxBufferSize = -1)
        {
            _maxBufferSize = maxBufferSize;
        }

        public int Count { get { return _frames.Count; } }

        private class VideoFrame
        {
            public int Width;
            public int Height;
            public byte[] Data;
            public int TimeStamp;
            public int FrameInterval;
        }

        private Queue<VideoFrame> _frames = new Queue<VideoFrame>();
        private long _bufferSize = 0;

        private ManualResetEvent _stopEvent = new ManualResetEvent(false);
        private Thread _thread;

        public void Start()
        {
            Stop();

            _lastTimeStamp = 0;
            _stopEvent.Reset();
            _thread = new Thread(new ThreadStart(runThread));
            _thread.IsBackground = true;
            _thread.Start();
        }

        public void Stop()
        {
            _stopEvent.Set();
            Thread thread = _thread;
            if (thread != null)
                thread.Join();
            _thread = null;

            Clear();
        }

        public void Clear()
        {
            lock (_frames)
            {
                _frames.Clear();
                _bufferElasped = 0;
            }
        }

        private int _lastTimeStamp = 0;
        private int _bufferElasped = 0;

        public void InputVideoFrame(int width, int height, byte[] frameData, int timeStamp)
        {
            VideoFrame frame = new VideoFrame();
            frame.Width = width;
            frame.Height = height;
            frame.Data = frameData;
            frame.TimeStamp = timeStamp;

            if (timeStamp > _lastTimeStamp && (timeStamp - _lastTimeStamp) < 3000)
                frame.FrameInterval = timeStamp - _lastTimeStamp;
            else
                frame.FrameInterval = 40;
            _lastTimeStamp = timeStamp;

            lock (_frames)
                enqueueFrame(frame);
        }

        private void runThread()
        {
            bool first = true;
            bool firstFrame = true;
            DateTime startTime = DateTime.Now;
            int wait = 0;
            while (!_stopEvent.WaitOne(wait))
            {
                VideoFrame frame = null;
                lock (_frames)
                {
                    while (_maxBufferSize > 0 && _bufferSize > _maxBufferSize)
                        dequeueFrame();

                    if (_frames.Count > 0)
                    {
                        wait = 1;
                        const int minBuffTime = 200;
                        DateTime now = DateTime.Now;
                        VideoFrame temp = _frames.Peek();

                        if (first && _bufferElasped >= minBuffTime)
                        {
                            first = false;
                            firstFrame = false;
                            startTime = DateTime.Now;
                        }

                        if (first)
                        {
                            if (firstFrame)
                            {
                                firstFrame = false;
                                frame = temp;
                            }
                        }
                        else
                        {
                            int dec = (_bufferElasped - minBuffTime) * 2 / minBuffTime;
                            if (_bufferElasped > minBuffTime * 3)
                                dec = temp.FrameInterval;
                            startTime += TimeSpan.FromMilliseconds(Math.Max(0, temp.FrameInterval - dec));
                            frame = dequeueFrame();
                            wait = Math.Max(1, (int)Math.Round((startTime - DateTime.Now).TotalMilliseconds));
                        }
                    }
                    else
                    {
                        wait = 10;
                        first = true;
                        firstFrame = true;
                    }
                }

                if (frame != null)
                    fireOnVideoFrame(frame.Width, frame.Height, frame.Data, frame.TimeStamp);
            }
        }

        private void enqueueFrame(VideoFrame frame)
        {
            _frames.Enqueue(frame);
            _bufferElasped += frame.FrameInterval;
            if (frame.Data != null)
                _bufferSize += frame.Data.Length;
        }

        private VideoFrame dequeueFrame()
        {
            VideoFrame frame = _frames.Dequeue();
            _bufferElasped -= frame.FrameInterval;
            if (frame.Data != null)
                _bufferSize -= frame.Data.Length;
            return frame;
        }
    }
}

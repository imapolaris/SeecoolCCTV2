using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCTVClient
{
    public class UniviewDecoder : IDisposable
    {
        public class VideoFrame
        {
            public int Width;
            public int Height;
            public long Timestamp;
            public byte[] Data;
        }

        public event Action<VideoFrame> VideoFrameEvent;
        private void fireVideoFrameEvent(VideoFrame frame)
        {
            var callback = VideoFrameEvent;
            if (callback != null)
                callback(frame);
        }

        UniviewPlayer _player = null;

        public void ReleasePlayer()
        {
            if (_player != null)
            {
                _player.DecFrameEvent -= _player_DecFrameEvent;
                _player.Dispose();
            }
            _player = null;
        }

        public void Dispose()
        {
            ReleasePlayer();
        }

        public void InputData(string decodeTag, byte[] data)
        {
            if (_player == null || _player.DecodeTag != decodeTag)
            {
                ReleasePlayer();
                _player = new UniviewPlayer(decodeTag);
                _player.DecFrameEvent += _player_DecFrameEvent;
            }

            _player.InputData(data);
        }

        private void _player_DecFrameEvent(int width, int height, byte[] frame, long stamp)
        {
            VideoFrame videoFrame = new VideoFrame()
            {
                Width = width,
                Height = height,
                Data = frame,
                Timestamp = stamp,
            };
            fireVideoFrameEvent(videoFrame);
        }
    }
}

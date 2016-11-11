using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCTVClient
{
    public class HikM4Decoder : IDisposable
    {
        public class VideoFrame
        {
            public int Width;
            public int Height;
            public int Timestamp;
            public byte[] Data;
        }

        public event Action<VideoFrame> VideoFrameEvent;
        private void fireVideoFrameEvent(VideoFrame frame)
        {
            var callback = VideoFrameEvent;
            if (callback != null)
                callback(frame);
        }

        private HikPlayer _player;
        private double _playTimeStamp;
        private byte[] _playHead;

        public static int HeaderType { get; private set; } = 0x80;

        public bool InputData(int type, byte[] data)
        {
            if (type == HeaderType)
            {
                if (!ByteArrayEqual(_playHead, data))
                {
                    if (_playHead != null)
                        ReleasePlayer();

                    setupPlayer();
                    _player.OpenStream(data, 0, data.Length, 1024 * 1024 * 8);
                    _playHead = data;
                    _player.Play(IntPtr.Zero);
                }
            }
            else if (_player != null)
                return _player.InputData(data, 0, data.Length);
            return true;
        }

        public bool Slow()
        {
            if (_player != null)
                return _player.Slow();
            return false;
        }

        public bool Fast()
        {
            if (_player != null)
                return _player.Fast();
            return false;
        }

        private void setupPlayer()
        {
            if (_player == null)
            {
                _player = new HikPlayer();
                _player.SetStreamMode(HikPlayer.StreamMode.Realtime);
                _playTimeStamp = 0;
                _player.DecFrameEvent += _player_DecFrameEvent;
            }
        }

        public static bool ByteArrayEqual(byte[] array1, byte[] array2)
        {
            if (array1 == null)
                return array2 == null;
            else if (array2 != null && array1.Length == array2.Length)
            {
                for (int i = 0; i < array1.Length; i++)
                    if (array1[i] != array2[i])
                        return false;
                return true;
            }
            return false;
        }

        DateTime _startTime = DateTime.Now;

        void _player_DecFrameEvent(byte[] frame, int width, int height, int stamp, HikPlayer.FrameType type, int frameRate)
        {
            if (type == HikPlayer.FrameType.YV12)
            {
                fireVideoFrameEvent(new VideoFrame()
                {
                    Width = width,
                    Height = height,
                    //Timestamp = (int)Math.Round(_playTimeStamp),
                    Timestamp = stamp,
                    Data = frame,
                });
            }
            _playTimeStamp += (1000.0 / frameRate);
        }

        public void ReleasePlayer()
        {
            if (_player != null)
            {
                _player.Stop();
                _player.CloseStream();
                _player.Dispose();
            }
            _player = null;
            _playHead = null;
        }

        public void Dispose()
        {
            ReleasePlayer();
        }
    }
}

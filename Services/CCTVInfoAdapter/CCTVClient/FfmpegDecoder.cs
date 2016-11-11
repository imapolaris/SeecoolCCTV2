using FFmpeg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCTVClient
{
    public class FfmpegDecoder : IDisposable
    {
        public class VideoFrame
        {
            public int Width;
            public int Height;
            public ulong Pts;
            public byte[] Data;
        }

        private VideoDecoder _decoder;
        Constants.AVCodecID _codecId = Constants.AVCodecID.AV_CODEC_ID_NONE;
        int _width = 0;
        int _Height = 0;

        public void Init(Constants.AVCodecID codecID, int width, int height)
        {
            if (_decoder == null || _codecId != codecID || _width != width || _Height != height)
            {
                Release();

                _decoder = new VideoDecoder();
                _decoder.Create(codecID);
                _codecId = codecID;
                _width = width;
                _Height = height;
            }
        }

        public void Release()
        {
            if (_decoder != null)
                _decoder.Dispose();
            _decoder = null;
        }

        public VideoFrame Decode(int type, ulong pts, byte[] data)
        {
            if (_decoder != null)
            {
                int width;
                int height;
                byte[] frame = _decoder.Decode(data, out width, out height);
                if (frame != null)
                {
                    return new VideoFrame()
                    {
                        Width = width,
                        Height = height,
                        Pts = pts,
                        Data = frame,
                    };
                }
            }

            return null;
        }

        public void Dispose()
        {
            Release();
        }
    }
}

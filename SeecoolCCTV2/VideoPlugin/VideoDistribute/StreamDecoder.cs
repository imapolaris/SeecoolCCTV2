using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCTVClient;
using FFmpeg;
using VideoStreamClient.Entity;
using VideoStreamClient.Events;

namespace VideoNS.VideoDistribute
{
    public class StreamDecoder
    {
        private bool _hikHeaderInit = false;
        private bool _ffmpegHeaderInit = false;
        private HikM4Decoder _hikDecoder;
        private FfmpegDecoder _ffmpegDecoder;

        public StreamDecoder()
        {
            _hikDecoder = new HikM4Decoder();
            _ffmpegDecoder = new FfmpegDecoder();

            _hikDecoder.VideoFrameEvent += HikDecoder_VideoFrameEvent;
        }

        private void HikDecoder_VideoFrameEvent(HikM4Decoder.VideoFrame obj)
        {
            VideoFrame vf = new VideoFrame()
            {
                Width = obj.Width,
                Height = obj.Height,
                Timestamp = obj.Timestamp,
                Data = obj.Data
            };
            onDecoded(vf);
        }

        public void InitHeader(HikM4Header header)
        {
            _hikHeaderInit = true;
            _hikDecoder.InputData(HikM4Decoder.HeaderType, header.Data);
        }

        public void InitHikm4Header(int type, byte[] data)
        {
            InitHeader(new HikM4Header()
            {
                Type = type,
                Data = data
            });
        }

        public void InitHeader(FfmpegHeader header)
        {
            _ffmpegHeaderInit = true;
            _ffmpegDecoder.Init(header.CodecID, header.Width, header.Height);
        }

        public void InitFfmpegHeader(int codeId, int width, int height)
        {
            InitHeader(new FfmpegHeader()
            {
                CodecID = (Constants.AVCodecID)codeId,
                Width = width,
                Height = height
            });
        }

        public void StartDecoder(HikM4Package package)
        {
            if (!_hikHeaderInit)
                throw new InvalidOperationException("没有初始化海康视频流包头。");
            _hikDecoder.InputData(package.Type, package.Data);
        }

        public void StartDecodegHikm4(int type, byte[] data)
        {
            if (!_hikHeaderInit)
                throw new InvalidOperationException("没有初始化海康视频流包头。");
            _hikDecoder.InputData(type, data);
        }

        public void StartDecoder(FfmpegPackage package)
        {
            if (!_ffmpegHeaderInit)
                throw new InvalidOperationException("没有初始化FFMPEG视频流包头。");
            var frame = _ffmpegDecoder.Decode(package.Type, package.Pts, package.Data);
            completeFfmpegDecode(frame);
        }

        public void StartDecoderFfmpeg(int type, ulong pts, byte[] data)
        {
            if (!_ffmpegHeaderInit)
                throw new InvalidOperationException("没有初始化FFMPEG视频流包头。");
            var frame = _ffmpegDecoder.Decode(type, pts, data);
            completeFfmpegDecode(frame);
        }

        private void completeFfmpegDecode(FfmpegDecoder.VideoFrame frame)
        {
            if (frame != null)
            {
                VideoFrame vf = new VideoFrame()
                {
                    Width = frame.Width,
                    Height = frame.Height,
                    Timestamp = (int)frame.Pts,
                    Data = frame.Data
                };
                onDecoded(vf);
            }
        }

        #region 【事件定义】
        public event EventHandler<VideoFrameEventArgs> Decoded;

        private void onDecoded(VideoFrame frame)
        {
            var handler = Decoded;
            if (handler != null)
                handler(this, new VideoFrameEventArgs(frame));
        }
        #endregion 【事件定义】
    }
}

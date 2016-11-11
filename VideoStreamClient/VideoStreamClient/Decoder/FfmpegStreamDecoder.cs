using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoStreamClient.Entity;
using FFmpeg;
using CCTVClient;
using CenterStorageCmd;

namespace VideoStreamClient
{
    public class FfmpegStreamDecoder: IStreamDecoder
    {
        public Action<VideoFrame> VideoFrameEvent { get; set; }
        FfmpegDecoder _decoder;
        public FfmpegStreamDecoder(Constants.AVCodecID codecId, int width, int height)
        {
            _decoder = new FfmpegDecoder();
            _decoder.Init(codecId, width, height);
        }

        public bool Update(StreamPacket packet)
        {
            FfmpegDecoder.VideoFrame frame = _decoder?.Decode(1, (ulong)packet.Time.Ticks / 10000, packet.Buffer);
            if (frame != null)
                onDisplay(new VideoFrame(frame.Width, frame.Height, packet.Time.Ticks, frame.Data));
            return true;
        }

        private void onDisplay(VideoFrame frame)
        {
            var handler = VideoFrameEvent;
            if (handler != null)
                handler(frame);
        }

        public void Dispose()
        {
            _decoder?.Dispose();
            _decoder = null;
        }

        public void PlayingSpeed(int fastTimes)
        {
        }
    }
}

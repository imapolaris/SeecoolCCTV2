using CCTVClient;
using CenterStorageCmd;
using FFmpeg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoStreamClient.Entity;
using VideoStreamClient.Events;

namespace VideoStreamClient.Decoder
{
    public class VideoStreamDecoder: IStreamDecoder
    {
        IStreamDecoder _decoder;
        public Action<VideoFrame> VideoFrameEvent { get; set; }

        public VideoStreamDecoder()
        {
        }

        public void InitHeader(byte[] buffer)
        {
            disposeDecoder();
            _decoder = getDecoder(buffer);
            if(_decoder != null)
                _decoder.VideoFrameEvent += onDisplay;
        }

        public bool Update(StreamPacket packet)
        {
            if(_decoder != null)
                return _decoder.Update(packet);
            Console.WriteLine("_decoder == NULL!");
            return false;
        }

        public void PlayingSpeed(int fastTimes)
        {
            _decoder?.PlayingSpeed(fastTimes);
        }

        IStreamDecoder getDecoder(byte[] buffer)
        {
            FfmpegHeaderPacket fhp = FfmpegHeaderPacket.Decode(buffer);
            if (fhp != null)
            {
                Console.WriteLine($"{(Constants.AVCodecID)fhp.CodecID}, {fhp.Width}, {fhp.Height}");
                return new FfmpegStreamDecoder((Constants.AVCodecID)fhp.CodecID, fhp.Width, fhp.Height);
            }
            HikHeaderPacket hhp = HikHeaderPacket.Decode(buffer);
            if (hhp != null)
                return new HikStreamDecoder(hhp.Header);
            return null;
        }

        private void disposeDecoder()
        {
            if (_decoder != null)
        {
                _decoder.VideoFrameEvent -= onDisplay;
                _decoder.Dispose();
        }
            _decoder = null;
        }

        private void onDisplay(VideoFrame frame)
            {
            var handler = VideoFrameEvent;
            if (handler != null)
                handler(frame);
        }

        public void Dispose()
        {
            disposeDecoder();
        }
    }
}

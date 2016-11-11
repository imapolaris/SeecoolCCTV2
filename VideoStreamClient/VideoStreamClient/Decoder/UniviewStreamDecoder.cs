using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCTVClient;
using CenterStorageCmd;
using CenterStorageCmd.Packet;
using VideoStreamClient.Entity;

namespace VideoStreamClient.Decoder
{
    public class UniviewStreamDecoder : IStreamDecoder
    {
        public Action<VideoFrame> VideoFrameEvent { get; set; }
        private UniviewDecoder _decoder;

        public UniviewStreamDecoder()
        {
            _decoder = new UniviewDecoder();
            _decoder.VideoFrameEvent += onVideoFrame;
        }

        public bool Update(StreamPacket packet)
        {
            UniviewStreamPacket usp = (UniviewStreamPacket)packet;
            _decoder.InputData(usp.DecodeTag, usp.Buffer);
            return true;
        }

        private void onVideoFrame(UniviewDecoder.VideoFrame obj)
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
            _decoder?.Dispose();
            _decoder = null;
        }

        public void PlayingSpeed(int fastTimes)
        {
            throw new NotImplementedException();
        }
    }
}

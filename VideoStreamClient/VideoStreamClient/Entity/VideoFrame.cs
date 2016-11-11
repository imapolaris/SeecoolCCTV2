using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoStreamClient.Entity
{
    public class VideoFrame
    {
        public int Width;
        public int Height;
        public long Timestamp;
        public byte[] Data;
        public VideoFrame()
        {

        }

        public VideoFrame(int width, int height, long stamp, byte[] buffer)
        {
            Width = width;
            Height = height;
            Timestamp = stamp;
            Data = buffer;
        }
    }
}

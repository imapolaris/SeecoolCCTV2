using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFmpeg
{
    public class VideoFile : IDisposable
    {
        IntPtr _file = IntPtr.Zero;

        public bool IsOpen { get { return _file != IntPtr.Zero; } }

        public bool Create(string fileName, int width, int height, Constants.AVCodecID videoCodecID, int bitRate)
        {
            _file = NativeMethods.FileBuilder_Create(fileName, width, height, videoCodecID, bitRate);
            return IsOpen;
        }

        public void WriteVideoFrame(bool key, byte[] frame, ulong pts)
        {
            if (IsOpen)
                NativeMethods.FileBuilder_WriteVideoFrame(_file, key, frame, pts);
        }

        public void Close()
        {
            if (IsOpen)
                NativeMethods.FileBuilder_Close(_file);
            _file = IntPtr.Zero;
        }

        public void Dispose()
        {
            Close();
        }
    }
}

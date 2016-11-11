using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFmpeg
{
    public class StreamReader : IDisposable
    {
        IntPtr _reader = IntPtr.Zero;

        public bool IsOpen { get { return _reader != IntPtr.Zero; } }

        public bool Create(string url, int timeout = 3000)
        {
            _reader = NativeMethods.StreamReader_Create(url, timeout);
            return IsOpen;
        }

        public int GetVideoStreamIndex()
        {
            if (IsOpen)
                return NativeMethods.StreamReader_GetVideoStreamIndex(_reader);
            else
                return -1;
        }

        public long GetStreamStartTime(int streamIndex)
        {
            if (IsOpen)
                return NativeMethods.StreamReader_GetStreamStartTime(_reader, streamIndex);
            else
                return -1;
        }

        public Constants.AVRational GetStreamTimeBase(int streamIndex)
        {
            if (IsOpen)
                return NativeMethods.StreamReader_GetStreamTimeBase(_reader, streamIndex);
            else
                return Constants.AVRational.Invalid;
        }

        public VideoDecoder GetVideoDecoder()
        {
            if (IsOpen)
                return new VideoDecoder(NativeMethods.StreamReader_GetVideoDecoder(_reader));
            else
                return null;
        }

        public byte[] ReadPacketData(out int streamIndex, out long timeStamp)
        {
            if (IsOpen)
                return NativeMethods.StreamReader_ReadPacketData(_reader, out streamIndex, out timeStamp);
            else
            {
                streamIndex = -1;
                timeStamp = Constants.AV_NOPTS_VALUE;
                return null;
            }
        }

        public IntPtr ReadPacket(out int streamIndex, out long timeStamp)
        {
            if (IsOpen)
                return NativeMethods.StreamReader_ReadPacket(_reader, out streamIndex, out timeStamp);
            else
            {
                streamIndex = -1;
                timeStamp = Constants.AV_NOPTS_VALUE;
                return IntPtr.Zero;
            }
        }

        public static void FreePacket(IntPtr packet)
        {
            NativeMethods.StreamReader_FreePacket(packet);
        }

        public void Close()
        {
            if (IsOpen)
                NativeMethods.StreamReader_Close(_reader);
            _reader = IntPtr.Zero;
        }

        public void Dispose()
        {
            Close();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFmpeg
{
    public class VideoDecoder : IDisposable
    {
        IntPtr _decoder = IntPtr.Zero;
        bool _attachedDecoder = false;

        public VideoDecoder()
        {
        }

        public VideoDecoder(IntPtr decoder)
        {
            _decoder = decoder;
            _attachedDecoder = true;
        }

        public bool IsOpen { get { return _decoder != IntPtr.Zero; } }

        public bool Create(Constants.AVCodecID videoCodecID)
        {
            _decoder = NativeMethods.VideoDecoder_Create(videoCodecID);
            return IsOpen;
        }

        public byte[] Decode(byte[] data, out int width, out int height)
        {
            if (IsOpen)
                return NativeMethods.VideoDecoder_Decode(_decoder, data, out width, out height);
            else
            {
                width = height = 0;
                return null;
            }
        }

        public byte[] DecodePacket(IntPtr packet, out int width, out int height)
        {
            if (IsOpen)
                return NativeMethods.VideoDecoder_DecodePacket(_decoder, packet, out width, out height);
            else
            {
                width = height = 0;
                return null;
            }
        }

        public void Close()
        {
            if (IsOpen && !_attachedDecoder)
                NativeMethods.VideoDecoder_Close(_decoder);
            _decoder = IntPtr.Zero;
        }

        public void Dispose()
        {
            Close();
        }
    }
}

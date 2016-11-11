using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFmpeg
{
    public class ImageScaler : IDisposable
    {
        IntPtr _scaler = IntPtr.Zero;

        public bool IsOpen { get { return _scaler != IntPtr.Zero; } }

        public bool Init(int srcWidth, int srcHeight, Constants.SwsPixelFormat srcFormat, int dstWidth, int dstHeight, Constants.SwsPixelFormat dstFormat, Constants.ConvertionFlags flags)
        {
            _scaler = NativeMethods.ImageScaler_Create(srcWidth, srcHeight, srcFormat, dstWidth, dstHeight, dstFormat, flags);
            return IsOpen;
        }

        public void Dispose()
        {
            if (IsOpen)
                NativeMethods.ImageScaler_Close(_scaler);
            _scaler = IntPtr.Zero;
        }

        public int Scale(byte[][] srcSlices, int[] srcStrides, byte[][] dstSlices, int[] dstStrides)
        {
            if (IsOpen)
                return NativeMethods.ImageScaler_Scale(_scaler, srcSlices, srcStrides, dstSlices, dstStrides);
            else
                return 0;
        }
    }
}

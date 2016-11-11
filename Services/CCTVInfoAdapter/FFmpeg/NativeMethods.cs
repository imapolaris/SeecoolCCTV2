using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace FFmpeg
{
    internal static class NativeMethods
    {
        private static bool _isX64 = IntPtr.Size == 8;

        static NativeMethods()
        {
            if (_isX64)
                FFmpeg64.FFmpeg_Init();
            else
                FFmpeg32.FFmpeg_Init();
        }

        static void FFmpeg_FreeMemory(IntPtr ptr)
        {
            if (_isX64)
                FFmpeg64.FFmpeg_FreeMemory(ptr);
            else
                FFmpeg32.FFmpeg_FreeMemory(ptr);
        }

        #region ffmpeg file writer
        public static IntPtr FileBuilder_Create(string fileName, int width, int height, Constants.AVCodecID videoCodecID, int bitRate)
        {
            if (_isX64)
                return FFmpeg64.FileBuilder_Create(fileName, width, height, videoCodecID, bitRate);
            else
                return FFmpeg32.FileBuilder_Create(fileName, width, height, videoCodecID, bitRate);
        }

        public static void FileBuilder_Close(IntPtr file)
        {
            if (_isX64)
                FFmpeg64.FileBuilder_Close(file);
            else
                FFmpeg32.FileBuilder_Close(file);
        }

        public static void FileBuilder_WriteVideoFrame(IntPtr file, bool key, byte[] frame, ulong pts)
        {
            IntPtr buffer = Marshal.AllocHGlobal(frame.Length);
            Marshal.Copy(frame, 0, buffer, frame.Length);

            if (_isX64)
                FFmpeg64.FileBuilder_WriteVideoFrame(file, key, buffer, frame.Length, pts);
            else
                FFmpeg32.FileBuilder_WriteVideoFrame(file, key, buffer, frame.Length, pts);

            Marshal.FreeHGlobal(buffer);
        }
        #endregion

        #region ffmpeg video decoder
        public static IntPtr VideoDecoder_Create(Constants.AVCodecID videoCodecID)
        {
            if (_isX64)
                return FFmpeg64.VideoDecoder_Create(videoCodecID);
            else
                return FFmpeg32.VideoDecoder_Create(videoCodecID);
        }

        public static void VideoDecoder_Close(IntPtr decoder)
        {
            if (_isX64)
                FFmpeg64.VideoDecoder_Close(decoder);
            else
                FFmpeg32.VideoDecoder_Close(decoder);
        }

        public static byte[] VideoDecoder_Decode(IntPtr decoder, byte[] data, out int width, out int height)
        {
            IntPtr dataPtr = Marshal.AllocHGlobal(data.Length);
            Marshal.Copy(data, 0, dataPtr, data.Length);
            IntPtr framePtr = IntPtr.Zero;
            int frameSize = 0;

            bool ret = false;
            if (_isX64)
                ret = FFmpeg64.VideoDecoder_Decode(decoder, dataPtr, data.Length, out framePtr, out frameSize, out width, out height);
            else
                ret = FFmpeg32.VideoDecoder_Decode(decoder, dataPtr, data.Length, out framePtr, out frameSize, out width, out height);

            Marshal.FreeHGlobal(dataPtr);

            if (ret)
            {
                byte[] frame = new byte[frameSize];
                Marshal.Copy(framePtr, frame, 0, frameSize);
                FFmpeg_FreeMemory(framePtr);
                return frame;
            }
            else
                return null;
        }

        public static byte[] VideoDecoder_DecodePacket(IntPtr decoder, IntPtr packet, out int width, out int height)
        {
            IntPtr framePtr = IntPtr.Zero;
            int frameSize = 0;

            bool ret = false;
            if (_isX64)
                ret = FFmpeg64.VideoDecoder_DecodePacket(decoder, packet, out framePtr, out frameSize, out width, out height);
            else
                ret = FFmpeg32.VideoDecoder_DecodePacket(decoder, packet, out framePtr, out frameSize, out width, out height);

            if (ret)
            {
                byte[] frame = new byte[frameSize];
                Marshal.Copy(framePtr, frame, 0, frameSize);
                FFmpeg_FreeMemory(framePtr);
                return frame;
            }
            else
                return null;
        }
        #endregion

        #region ffmpeg stream reader
        public static IntPtr StreamReader_Create(string url, int timeout = 3000)
        {
            if (_isX64)
                return FFmpeg64.StreamReader_Create(url, timeout);
            else
                return FFmpeg32.StreamReader_Create(url, timeout);
        }

        public static void StreamReader_Close(IntPtr reader)
        {
            if (_isX64)
                FFmpeg64.StreamReader_Close(reader);
            else
                FFmpeg32.StreamReader_Close(reader);
        }

        public static IntPtr StreamReader_GetVideoDecoder(IntPtr reader)
        {
            if (_isX64)
                return FFmpeg64.StreamReader_GetVideoDecoder(reader);
            else
                return FFmpeg32.StreamReader_GetVideoDecoder(reader);
        }

        public static int StreamReader_GetVideoStreamIndex(IntPtr reader)
        {
            if (_isX64)
                return FFmpeg64.StreamReader_GetVideoStreamIndex(reader);
            else
                return FFmpeg32.StreamReader_GetVideoStreamIndex(reader);
        }

        public static long StreamReader_GetStreamStartTime(IntPtr reader, int streamIndex)
        {
            if (_isX64)
                return FFmpeg64.StreamReader_GetStreamStartTime(reader, streamIndex);
            else
                return FFmpeg32.StreamReader_GetStreamStartTime(reader, streamIndex);
        }

        public static Constants.AVRational StreamReader_GetStreamTimeBase(IntPtr reader, int streamIndex)
        {
            Constants.AVRational rational = new Constants.AVRational();
            if (_isX64)
                FFmpeg64.StreamReader_GetStreamTimeBase(reader, streamIndex, out rational.Numerator, out rational.Denominator);
            else
                FFmpeg32.StreamReader_GetStreamTimeBase(reader, streamIndex, out rational.Numerator, out rational.Denominator);
            return rational;
        }

        public static byte[] StreamReader_ReadPacketData(IntPtr reader, out int streamIndex, out long timeStamp)
        {
            IntPtr packetPtr = IntPtr.Zero;
            int packetSize = 0;

            bool ret = false;
            if (_isX64)
                ret = FFmpeg64.StreamReader_ReadPacketData(reader, out packetPtr, out packetSize, out streamIndex, out timeStamp);
            else
                ret = FFmpeg32.StreamReader_ReadPacketData(reader, out packetPtr, out packetSize, out streamIndex, out timeStamp);

            if (ret)
            {
                byte[] packet = new byte[packetSize];
                Marshal.Copy(packetPtr, packet, 0, packetSize);
                FFmpeg_FreeMemory(packetPtr);
                return packet;
            }
            else
                return null;
        }

        public static IntPtr StreamReader_ReadPacket(IntPtr reader, out int streamIndex, out long timeStamp)
        {
            IntPtr packet = IntPtr.Zero;
            timeStamp = Constants.AV_NOPTS_VALUE;
            bool ret = false;
            if (_isX64)
                ret = FFmpeg64.StreamReader_ReadPacket(reader, out packet, out streamIndex, out timeStamp);
            else
                ret = FFmpeg32.StreamReader_ReadPacket(reader, out packet, out streamIndex, out timeStamp);

            if (ret)
                return packet;
            else
                return IntPtr.Zero;
        }

        public static void StreamReader_FreePacket(IntPtr packet)
        {
            if (_isX64)
                FFmpeg64.StreamReader_FreePacket(packet);
            else
                FFmpeg32.StreamReader_FreePacket(packet);
        }
        #endregion

        #region ffmpeg image scaler
        public static IntPtr ImageScaler_Create(int srcWidth, int srcHeight, Constants.SwsPixelFormat srcFormat, int dstWidth, int dstHeight, Constants.SwsPixelFormat dstFormat, Constants.ConvertionFlags flags)
        {
            if (_isX64)
                return FFmpeg64.ImageScaler_Create(srcWidth, srcHeight, srcFormat, dstWidth, dstHeight, dstFormat, flags);
            else
                return FFmpeg32.ImageScaler_Create(srcWidth, srcHeight, srcFormat, dstWidth, dstHeight, dstFormat, flags);
        }

        public static void ImageScaler_Close(IntPtr scaler)
        {
            if (_isX64)
                FFmpeg64.ImageScaler_Close(scaler);
            else
                FFmpeg32.ImageScaler_Close(scaler);
        }

        static IntPtr[] bytesArrayToPtrArray(byte[][] bytesArray)
        {
            int length = bytesArray.Length;
            IntPtr[] ptrArray = new IntPtr[length];
            for (int i = 0; i<length;i++)
            {
                byte[] bytes = bytesArray[i];
                ptrArray[i] = Marshal.AllocHGlobal(bytes.Length);
                Marshal.Copy(bytes, 0, ptrArray[i], bytes.Length);
            }
            return ptrArray;
        }

        static void freePtrArray(IntPtr[] ptrArray)
        {
            foreach (IntPtr ptr in ptrArray)
                Marshal.FreeHGlobal(ptr);
        }

        static void ptrArrayToBytesArray(IntPtr[] ptrArray, byte[][] bytesArray)
        {
            for (int i = 0; i < bytesArray.Length; i++)
                Marshal.Copy(ptrArray[i], bytesArray[i], 0, bytesArray[i].Length);
        }

        public static int ImageScaler_Scale(IntPtr scaler, byte[][] srcSlices, int[] srcStrides, byte[][] dstSlices, int[] dstStrides)
        {
            IntPtr[] srcPtrs = bytesArrayToPtrArray(srcSlices);
            IntPtr[] dstPtrs = bytesArrayToPtrArray(dstSlices);
            int ret = 0;
            if (_isX64)
                ret = FFmpeg64.ImageScaler_Scale(scaler, srcPtrs, srcStrides, dstPtrs, dstStrides);
            else
                ret = FFmpeg32.ImageScaler_Scale(scaler, srcPtrs, srcStrides, dstPtrs, dstStrides);
            ptrArrayToBytesArray(dstPtrs, dstSlices);
            freePtrArray(srcPtrs);
            freePtrArray(dstPtrs);
            return ret;
        }
        #endregion

        private static class FFmpeg32
        {
            const string _dllPath = @"x86\FFmpeg32.dll";

            [DllImport(_dllPath)]
            public static extern void FFmpeg_Init();

            [DllImport(_dllPath)]
            public static extern void FFmpeg_FreeMemory(IntPtr data);

            #region ffmpeg file writer
            [DllImport(_dllPath)]
            public static extern IntPtr FileBuilder_Create([MarshalAs(UnmanagedType.LPStr)]string fileName, int width, int height, [MarshalAs(UnmanagedType.I4)]Constants.AVCodecID videoCodecID, int bitRate);

            [DllImport(_dllPath)]
            public static extern void FileBuilder_Close(IntPtr file);

            [DllImport(_dllPath)]
            public static extern void FileBuilder_WriteVideoFrame(IntPtr file, bool key, IntPtr frame, int frameSize, ulong pts);
            #endregion

            #region ffmpeg video decoder
            [DllImport(_dllPath)]
            public static extern IntPtr VideoDecoder_Create([MarshalAs(UnmanagedType.I4)]Constants.AVCodecID videoCodecID);

            [DllImport(_dllPath)]
            public static extern void VideoDecoder_Close(IntPtr decoder);

            [DllImport(_dllPath)]
            public static extern bool VideoDecoder_Decode(IntPtr decoder, IntPtr data, int dataSize, out IntPtr frameData, out int frameSize, out int width, out int height);

            [DllImport(_dllPath)]
            public static extern bool VideoDecoder_DecodePacket(IntPtr decoder, IntPtr packet, out IntPtr frameData, out int frameSize, out int width, out int height);
            #endregion

            #region ffmpeg stream reader
            [DllImport(_dllPath)]
            public static extern IntPtr StreamReader_Create(string url, int timeout);

            [DllImport(_dllPath)]
            public static extern void StreamReader_Close(IntPtr reader);

            [DllImport(_dllPath)]
            public static extern IntPtr StreamReader_GetVideoDecoder(IntPtr reader);

            [DllImport(_dllPath)]
            public static extern int StreamReader_GetVideoStreamIndex(IntPtr reader);

            [DllImport(_dllPath)]
            public static extern long StreamReader_GetStreamStartTime(IntPtr reader, int streamIndex);

            [DllImport(_dllPath)]
            public static extern void StreamReader_GetStreamTimeBase(IntPtr reader, int streamIndex, out int numerator, out int denominator);

            [DllImport(_dllPath)]
            public static extern bool StreamReader_ReadPacketData(IntPtr reader, out IntPtr packetData, out int packetSize, out int streamIndex, out long timeStamp);

            [DllImport(_dllPath)]
            public static extern bool StreamReader_ReadPacket(IntPtr reader, out IntPtr packet, out int streamIndex, out long timeStamp);

            [DllImport(_dllPath)]
            public static extern bool StreamReader_FreePacket(IntPtr packet);
            #endregion

            #region ffmpeg image scaler
            [DllImport(_dllPath)]
            public static extern IntPtr ImageScaler_Create(int srcWidth, int srcHeight, Constants.SwsPixelFormat srcFormat, int dstWidth, int dstHeight, Constants.SwsPixelFormat dstFormat, Constants.ConvertionFlags flags);

            [DllImport(_dllPath)]
            public static extern void ImageScaler_Close(IntPtr scaler);

            [DllImport(_dllPath)]
            public static extern int ImageScaler_Scale(IntPtr scaler, IntPtr[] srcSlices, int[] srcStrides, IntPtr[] dstSlices, int[] dstStrides);
            #endregion
        }

        private static class FFmpeg64
        {
            const string _dllPath = @"x64\FFmpeg64.dll";

            [DllImport(_dllPath)]
            public static extern void FFmpeg_Init();

            [DllImport(_dllPath)]
            public static extern void FFmpeg_FreeMemory(IntPtr data);

            #region ffmpeg file writer
            [DllImport(_dllPath)]
            public static extern IntPtr FileBuilder_Create([MarshalAs(UnmanagedType.LPStr)]string fileName, int width, int height, [MarshalAs(UnmanagedType.I4)]Constants.AVCodecID videoCodecID, int bitRate);

            [DllImport(_dllPath)]
            public static extern void FileBuilder_Close(IntPtr file);

            [DllImport(_dllPath)]
            public static extern void FileBuilder_WriteVideoFrame(IntPtr file, bool key, IntPtr frame, int frameSize, ulong pts);
            #endregion

            #region ffmpeg video decoder
            [DllImport(_dllPath)]
            public static extern IntPtr VideoDecoder_Create([MarshalAs(UnmanagedType.I4)]Constants.AVCodecID videoCodecID);

            [DllImport(_dllPath)]
            public static extern void VideoDecoder_Close(IntPtr decoder);

            [DllImport(_dllPath)]
            public static extern bool VideoDecoder_Decode(IntPtr decoder, IntPtr data, int dataSize, out IntPtr frameData, out int frameSize, out int width, out int height);

            [DllImport(_dllPath)]
            public static extern bool VideoDecoder_DecodePacket(IntPtr decoder, IntPtr packet, out IntPtr frameData, out int frameSize, out int width, out int height);
            #endregion

            #region ffmpeg stream reader
            [DllImport(_dllPath)]
            public static extern IntPtr StreamReader_Create(string url, int timeout);

            [DllImport(_dllPath)]
            public static extern void StreamReader_Close(IntPtr reader);

            [DllImport(_dllPath)]
            public static extern IntPtr StreamReader_GetVideoDecoder(IntPtr reader);

            [DllImport(_dllPath)]
            public static extern int StreamReader_GetVideoStreamIndex(IntPtr reader);

            [DllImport(_dllPath)]
            public static extern long StreamReader_GetStreamStartTime(IntPtr reader, int streamIndex);

            [DllImport(_dllPath)]
            public static extern void StreamReader_GetStreamTimeBase(IntPtr reader, int streamIndex, out int numerator, out int denominator);

            [DllImport(_dllPath)]
            public static extern bool StreamReader_ReadPacketData(IntPtr reader, out IntPtr packetData, out int packetSize, out int streamIndex, out long timeStamp);

            [DllImport(_dllPath)]
            public static extern bool StreamReader_ReadPacket(IntPtr reader, out IntPtr packet, out int streamIndex, out long timeStamp);

            [DllImport(_dllPath)]
            public static extern bool StreamReader_FreePacket(IntPtr packet);
            #endregion

            #region ffmpeg image scaler
            [DllImport(_dllPath)]
            public static extern IntPtr ImageScaler_Create(int srcWidth, int srcHeight, Constants.SwsPixelFormat srcFormat, int dstWidth, int dstHeight, Constants.SwsPixelFormat dstFormat, Constants.ConvertionFlags flags);

            [DllImport(_dllPath)]
            public static extern void ImageScaler_Close(IntPtr scaler);

            [DllImport(_dllPath)]
            public static extern int ImageScaler_Scale(IntPtr scaler, IntPtr[] srcSlices, int[] srcStrides, IntPtr[] dstSlices, int[] dstStrides);
            #endregion
        }
    }
}

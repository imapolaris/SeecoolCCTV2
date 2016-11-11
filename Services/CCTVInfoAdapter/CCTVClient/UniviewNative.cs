using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CCTVClient
{
    internal static class UniviewNative
    {
        static bool _isX64 = (IntPtr.Size == 8);

        public static int IMOS_XP_Init()
        {
            if (_isX64)
                return Native64.IMOS_XP_Init();
            else
                return Native32.IMOS_XP_Init();
        }

        public static void IMOS_XP_Cleanup()
        {
            if (_isX64)
                Native64.IMOS_XP_Cleanup();
            else
                Native32.IMOS_XP_Cleanup();
        }

        public static int IMOS_XP_OpenInputStream(int port, byte[] header, uint bufPoolSize = 2 * 1024 * 1024)
        {
            int size = 0;
            IntPtr ptr = IntPtr.Zero;
            if (header != null)
            {
                size = header.Length;
                ptr = Marshal.AllocHGlobal(size);
                Marshal.Copy(header, 0, ptr, size);
            }

            int ret = 0;
            if (_isX64)
                ret = Native64.IMOS_XP_OpenInputStream((uint)port, ptr, (uint)size, bufPoolSize);
            else
                ret = Native32.IMOS_XP_OpenInputStream((uint)port, ptr, (uint)size, bufPoolSize);

            if (ptr != IntPtr.Zero)
                Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static int IMOS_XP_CloseInputStream(int port)
        {
            if (_isX64)
                return Native64.IMOS_XP_CloseInputStream((uint)port);
            else
                return Native32.IMOS_XP_CloseInputStream((uint)port);
        }

        public static int IMOS_XP_SetDecoderTag(int port, string decoderTag)
        {
            if (_isX64)
                return Native64.IMOS_XP_SetDecoderTag((uint)port, decoderTag);
            else
                return Native32.IMOS_XP_SetDecoderTag((uint)port, decoderTag);
        }

        public static int IMOS_XP_InputMediaData(int port, byte[] data)
        {
            int size = 0;
            IntPtr ptr = IntPtr.Zero;
            if (data != null)
            {
                size = data.Length;
                ptr = Marshal.AllocHGlobal(size);
                Marshal.Copy(data, 0, ptr, size);
            }

            int ret = 0;
            if (_isX64)
                ret = Native64.IMOS_XP_InputMediaData((uint)port, ptr, (uint)size);
            else
                ret = Native32.IMOS_XP_InputMediaData((uint)port, ptr, (uint)size);

            if (ptr != IntPtr.Zero)
                Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static int IMOS_XP_SetPlayWnd(int port, IntPtr handle)
        {
            if (_isX64)
                return Native64.IMOS_XP_SetPlayWnd((uint)port, handle);
            else
                return Native32.IMOS_XP_SetPlayWnd((uint)port, handle);
        }

        public static int IMOS_XP_StartPlay(int port)
        {
            if (_isX64)
                return Native64.IMOS_XP_StartPlay((uint)port);
            else
                return Native32.IMOS_XP_StartPlay((uint)port);
        }

        public static int IMOS_XP_StopPlay(int port)
        {
            if (_isX64)
                return Native64.IMOS_XP_StopPlay((uint)port);
            else
                return Native32.IMOS_XP_StopPlay((uint)port);
        }

        private static Native32.XP_PLAYER_DECODE_VIDEO_DATA_CALLBACK_PF _onDecVideoData = onDecVideoData;
        private static ConcurrentDictionary<int, DecodeVideoDataCallback> _decVideoCallbacks = new ConcurrentDictionary<int, DecodeVideoDataCallback>();

        public delegate void DecodeVideoDataCallback(int port, int width, int height, byte[] frame, int timestampType, long timestamp);
        public static int IMOS_XP_SetDecodeVideoMediaDataCB(int port, DecodeVideoDataCallback onDecodeVideoData, bool bContinue)
        {
            _decVideoCallbacks[port] = onDecodeVideoData;
            if (onDecodeVideoData != null)
            {
                IntPtr userParam = Marshal.GetFunctionPointerForDelegate(onDecodeVideoData);
                if (_isX64)
                    return Native64.IMOS_XP_SetDecodeVideoMediaDataCB((uint)port, _onDecVideoData, bContinue, userParam);
                else
                    return Native32.IMOS_XP_SetDecodeVideoMediaDataCB((uint)port, _onDecVideoData, bContinue, userParam);
            }
            else
            {
                if (_isX64)
                    return Native64.IMOS_XP_SetDecodeVideoMediaDataCB((uint)port, null, bContinue, IntPtr.Zero);
                else
                    return Native32.IMOS_XP_SetDecodeVideoMediaDataCB((uint)port, null, bContinue, IntPtr.Zero);
            }
        }

        private static void onDecVideoData(uint ulPort, ref Native32.XP_PICTURE_DATA_S pPictureData, IntPtr lUserParam, IntPtr lReserved)
        {
            int width = (int)pPictureData.ulPicWidth;
            int height = (int)pPictureData.ulPicHeight;
            byte[] frame = new byte[width * height * 3 / 2];
            int halfWidth = width / 2;
            int halfHeight = height / 2;

            int destOffset = 0;
            IntPtr srcPtr = (IntPtr)pPictureData.pucData[0];
            for (int i = 0; i < height; i++)
            {
                Marshal.Copy(srcPtr, frame, destOffset, width);
                destOffset += width;
                srcPtr += (int)pPictureData.ulLineSize[0];
            }
            srcPtr = (IntPtr)pPictureData.pucData[2];
            for (int i = 0; i < halfHeight; i++)
            {
                Marshal.Copy(srcPtr, frame, destOffset, halfWidth);
                destOffset += halfWidth;
                srcPtr += (int)pPictureData.ulLineSize[2];
            }
            srcPtr = (IntPtr)pPictureData.pucData[1];
            for (int i = 0; i < halfHeight; i++)
            {
                Marshal.Copy(srcPtr, frame, destOffset, halfWidth);
                destOffset += halfWidth;
                srcPtr += (int)pPictureData.ulLineSize[1];
            }

            DecodeVideoDataCallback onDecodeVideoData = (DecodeVideoDataCallback)Marshal.GetDelegateForFunctionPointer(lUserParam, typeof(DecodeVideoDataCallback));
            onDecodeVideoData((int)ulPort, width, height, frame, (int)pPictureData.ulRenderTimeType, pPictureData.dlRenderTime);
        }

        static class Native32
        {
            const string _dllPath = @"Uniview\x86\xp_player.dll";

            [DllImport(_dllPath)]
            public static extern int IMOS_XP_Init();

            [DllImport(_dllPath)]
            public static extern void IMOS_XP_Cleanup();

            [DllImport(_dllPath)]
            public static extern int IMOS_XP_OpenInputStream(uint ulPort, IntPtr pucFileHeadBuf, uint ulSize, uint ulBufPoolSize);

            [DllImport(_dllPath)]
            public static extern int IMOS_XP_SetPlayWnd(uint ulPort, IntPtr hWnd);

            [DllImport(_dllPath)]
            public static extern int IMOS_XP_StartPlay(uint ulPort);

            [DllImport(_dllPath)]
            public static extern int IMOS_XP_StopPlay(uint ulPort);

            [DllImport(_dllPath)]
            public static extern int IMOS_XP_CloseInputStream(uint ulPort);

            [DllImport(_dllPath)]
            public static extern int IMOS_XP_InputMediaData(uint ulPort, IntPtr pucDataBuf, uint ulDataLen);

            [DllImport(_dllPath)]
            public static extern int IMOS_XP_SetDecoderTag(uint ulPort, string pcDecoderTag);

            [StructLayout(LayoutKind.Sequential)]
            public struct XP_PICTURE_DATA_S
            {
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
                public IntPtr[] pucData;                          /**< pucData[0]:Y 平面指针,pucData[1]:U 平面指针,pucData[2]:V 平面指针 */
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
                public uint[] ulLineSize;                        /**< ulLineSize[0]:Y平面每行跨距, ulLineSize[1]:U平面每行跨距, ulLineSize[2]:V平面每行跨距 */
                public uint ulPicHeight;                          /**< 图片高度 */
                public uint ulPicWidth;                           /**< 图片宽度 */
                public uint ulRenderTimeType;                     /**< 用于渲染的时间数据类型，对应tagRenderTimeType枚举中的值 */
                public long dlRenderTime;                         /**< 用于渲染的时间数据 */
            };
            public delegate void XP_PLAYER_DECODE_VIDEO_DATA_CALLBACK_PF(uint ulPort, ref XP_PICTURE_DATA_S pPictureData, IntPtr lUserParam, IntPtr lReserved);
            [DllImport(_dllPath)]
            public static extern int IMOS_XP_SetDecodeVideoMediaDataCB(uint ulPort, XP_PLAYER_DECODE_VIDEO_DATA_CALLBACK_PF pfnDecodeVideoDataCBFun, bool bContinue, IntPtr lUserParam);
        }

        static class Native64
        {
            const string _dllPath = @"Uniview\x64\xp_player.dll";

            [DllImport(_dllPath)]
            public static extern int IMOS_XP_Init();

            [DllImport(_dllPath)]
            public static extern void IMOS_XP_Cleanup();

            [DllImport(_dllPath)]
            public static extern int IMOS_XP_OpenInputStream(uint ulPort, IntPtr pucFileHeadBuf, uint ulSize, uint ulBufPoolSize);

            [DllImport(_dllPath)]
            public static extern int IMOS_XP_SetPlayWnd(uint ulPort, IntPtr hWnd);

            [DllImport(_dllPath)]
            public static extern int IMOS_XP_StartPlay(uint ulPort);

            [DllImport(_dllPath)]
            public static extern int IMOS_XP_StopPlay(uint ulPort);

            [DllImport(_dllPath)]
            public static extern int IMOS_XP_CloseInputStream(uint ulPort);

            [DllImport(_dllPath)]
            public static extern int IMOS_XP_InputMediaData(uint ulPort, IntPtr pucDataBuf, uint ulDataLen);

            [DllImport(_dllPath)]
            public static extern int IMOS_XP_SetDecoderTag(uint ulPort, string pcDecoderTag);

            [DllImport(_dllPath)]
            public static extern int IMOS_XP_SetDecodeVideoMediaDataCB(uint ulPort, Native32.XP_PLAYER_DECODE_VIDEO_DATA_CALLBACK_PF pfnDecodeVideoDataCBFun, bool bContinue, IntPtr lUserParam);
        }
    }
}

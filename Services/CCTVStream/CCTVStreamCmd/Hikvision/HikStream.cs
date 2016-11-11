using CCTVStreamCmd.Hikvision.HikFrameProcess;
using Seecool.VideoStreamBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Media.Rtp;

namespace CCTVStreamCmd.Hikvision
{
    public class HikStream : StreamBase, IStream, IDisposable
    {
        NetDvr.Serial _serial;
        int _playHandle = -1;
        StreamKeyDetector _keyDetector = new StreamKeyDetector();
        HikFrameProcessor _hikStream = new HikFrameProcessor();
        //StreamHikToStandard _hikToStand;
        public HikStream(string ip, ushort port, string user, string pass, int channel, bool isSubStream, IntPtr handle)
        {
            //_hikToStand = new StreamHikToStandard();
            //_hikToStand.HeaderEvent += onHeader;
            //_hikToStand.StreamEvent += onStandardStream;
            _serial = new NetDvr.Serial(ip, port, user, pass);
            realPlay(channel, isSubStream, handle);
        }

        private void onStandardStream(IStreamPacket obj)
        {
            onStream(obj.Buffer, obj.FrameType, obj.Time);
            if (obj.FrameType == CCTVFrameType.StreamFrame || obj.FrameType == CCTVFrameType.StreamKeyFrame)
                onFrame(obj.Buffer, obj.FrameType);
        }

        private void realPlay(int channel, bool isSubStream, IntPtr handle)
        {
            NET_DVR_CLIENTINFO ci = new NET_DVR_CLIENTINFO();
            ci.lChannel = channel;
            if (isSubStream)
                ci.lLinkMode = 0x80000000;
            ci.hPlayWnd = handle;
            _realDataCallBack = onRealData;
            _playHandle = _serial.RealPlay_V30(ref ci, _realDataCallBack, IntPtr.Zero, 0);
            Console.WriteLine("Play Handle: " + _playHandle);
            _serial.GetError();
            if (_playHandle < 0)
            {
                throw new InvalidOperationException("视频播放失败！");
            }
        }

        RealDataCallBack_V30 _realDataCallBack;
        /// <summary>
        /// 实时预览 Callback Function
        /// </summary>
        /// <param name="lRealHandle">当前的预览句柄 </param>
        /// <param name="dwDataType">数据类型 1 系统头数据 ，2 流数据（包括复合流或音视频分开的视频流数据） 3 音频数据 </param>
        /// <param name="pBuffer">存放数据的缓冲区指针</param>
        /// <param name="dwBufSize">缓冲区大小 </param>
        /// <param name="pUser">用户数据</param>
        private void onRealData(int lRealHandle, uint dwDataType, IntPtr pBuffer, uint dwBufSize, IntPtr pUser)
        {
            byte[] buffer = DataConverter.ToByteArray(pBuffer, (int)dwBufSize);
            if (dwDataType == NetDvrDll32.NET_DVR_SYSHEAD)
                onHeader(new HikHeaderPacket(buffer));
            else if (dwDataType == NetDvrDll32.NET_DVR_STREAMDATA)
            {
                //_hikToStand.UpdateStandardStream(buffer);
                var infos = _hikStream.Update(DateTime.Now, buffer);
                foreach (var packets in infos)
                {
                    if (packets != null && packets.Length > 0)
                    {
                        int length = packets.Sum(_ => _.Buffer.Length);
                        bool isKey = _keyDetector.Update(packets.First().Time, length);

                        CCTVFrameType type = CCTVFrameType.StreamFrame;
                        if (_keyDetector.Update(packets.First().Time, length))
                            type = CCTVFrameType.StreamKeyFrame;

                        onStream(packets.First().Buffer, type, packets.First().Time);//合并视频流的第一个packet
                        for (int i = 1; i < packets.Length; i++)//合并视频流的其他packet
                            onStream(packets[i].Buffer, CCTVFrameType.StreamFrame, packets[i].Time);
                    }
                }
            }
            else if (dwDataType == NetDvrDll32.NET_DVR_AUDIOSTREAMDATA)
                onStream(buffer, CCTVFrameType.AudioFrame, DateTime.Now);
        }

        public override void Dispose()
        {
            if (_playHandle >= 0)
            {
                _serial.StopRealPlay(_playHandle);
                Console.WriteLine("Stop Play:" + _playHandle);
            }
            _playHandle = -1;

            if (_serial != null)
                _serial.Dispose();
            _serial = null;
        }
    }
}

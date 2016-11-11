using CenterStorageCmd;
using SocketHelper.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StorageDataProxy
{
    public class DownloadProxy : ProxyBase, IStreamDownload
    {
        public DownloadProxy(string ip, int port) : base(ip, port)
        {
            Client.ReceiveCompleted += Client_ReceiveCompleted;
        }

        private void Client_ReceiveCompleted(object sender, ReceiveEventArgs args)
        {
            try
            {
                if (args.ByteLength > 0)
                {
                    onBytesLengthReceived(args.ByteLength);
                    using (MemoryStream ms = new MemoryStream(args.ReceivedBytes))
                    {
                        int code = PacketBase.ReadInt(ms);
                        switch ((ParamCode)code)
                        {
                            case ParamCode.TimePeriods:
                                onVideoStoragePacket(VideoTimePeriodsPacket.Decode(ms));
                                break;
                            case ParamCode.VideoPacket:
                                onVideoPacket(VideoStreamsPacket.Decode(ms));
                                break;
                            case ParamCode.VideoBaseInfo:
                                onVideoBasePacket(VideoBasePacket.Decode(ms));
                                break;
                            case ParamCode.DownloadInfosAll:
                                onDownloadInfosAll(DownloadInfoExpandPacket.DecodeArray(ms));
                                break;
                            case ParamCode.DownloadInfosAdd:
                                onDownloadInfosAdd(DownloadInfoExpandPacket.Decode(ms));
                                break;
                            case ParamCode.DownloadInfoPart:
                                onDownloadInfoPart(DownloadInfoPartConverter.Decode(ms));
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                onException(ex);
            }
        }

        public void DownloadControl(DownloadControlCode code, byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                PacketBase.WriteBytes(ms, (int)code);
                PacketBase.WriteBytes(ms, buffer);
                Send((int)ParamCode.DownloadControl, ms.ToArray());
            }
        }

        private void onDownloadInfoPart(DownloadExpandPart part)
        {
            if (part != null)
            {
                var handle = DownloadExpandPartReceived;
                if (handle != null)
                    handle(part);
            }
        }

        private void onDownloadInfosAdd(DownloadInfoExpandPacket expand)
        {
            var handle = DownloadInfoExpandAddReceived;
            if (handle != null)
                handle(expand);
        }

        private void onDownloadInfosAll(DownloadInfoExpandPacket[] expands)
        {
            var handle = DownloadInfoExpandAllReceived;
            if (handle != null)
                handle(expands);
        }

        public void GetVideoData(VideoDataParam param)
        {
            Send((int)ParamCode.VideoPacket, VideoDataParam.Encode(param));
        }

        public void GetVideoBaseInfo(IVideoBaseInfom param)
        {
            Send((int)ParamCode.VideoBaseInfo, VideoBaseInfomParam.Encode(param));
        }

        public void GetTimePeriods(IVideoBaseInfom param)
        {
            Send((int)ParamCode.TimePeriods, VideoBaseInfomParam.Encode(param));
        }

        public void GetAllDownloadInfos()
        {
            Send((int)ParamCode.DownloadInfosAll, new byte[0]);
        }
        
        #region 【事件定义】
        public Action<VideoTimePeriodsPacket> VideoInfoReceived;
        public Action<VideoStreamsPacket> VideoDataReceived;
        public Action<VideoBasePacket> VideoBaseReceived;

        public Action<DownloadInfoExpandPacket[]> DownloadInfoExpandAllReceived;
        public Action<DownloadInfoExpandPacket> DownloadInfoExpandAddReceived;
        public Action<DownloadExpandPart> DownloadExpandPartReceived;
        public Action<Exception> ExceptionReceived;
        public Action<int> BytesLengthReceived;

        private void onBytesLengthReceived(int length)
        {
            var handler = BytesLengthReceived;
            if (handler != null)
                handler(length);
        }

        private void onVideoPacket(VideoStreamsPacket packet)
        {
            var handler = VideoDataReceived;
            if (handler != null)
                handler(packet);
        }

        private void onVideoBasePacket(VideoBasePacket packet)
        {
            var handler = VideoBaseReceived;
            if (handler != null)
                handler(packet);
        }

        private void onException(Exception ex)
        {
            var handler = ExceptionReceived;
            if (handler != null)
                handler(ex);
        }

        private void onVideoStoragePacket(VideoTimePeriodsPacket packet)
        {
            var handler = VideoInfoReceived;
            if (handler != null)
                handler(packet);
        }
        #endregion 【事件定义】
    }
}
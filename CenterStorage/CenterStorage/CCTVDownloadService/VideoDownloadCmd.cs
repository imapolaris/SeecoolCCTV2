using CenterStorageCmd;
using StorageDataProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CCTVDownloadService
{
    public class VideoDownloadCmd: IDisposable
    {
        DownloadProxy _downProxy;
        IDownloadInfo _info;
        public Action<int> BytesLengthEvent;
        public Action<string> ErrorEvent;
        public Action<VideoDownloadCmd, VideoTimePeriodsPacket> VideoTimePeriodsEvent;
        public Action<VideoStreamsPacket> VideoStreamEvent;
        public Action<VideoBasePacket> VideoBaseEvent;

        public VideoDownloadCmd(IDownloadInfo info)
        {
            _info = info;
            loadProxy();
        }

        /// <summary>获取视频分布区间</summary>
        public void GetTimePeriods()
        {
            _downProxy.GetTimePeriods(_info);
        }

        /// <summary>获取视频基本信息</summary>
        public void GetVideoBaseInfo()
        {
            _downProxy.GetVideoBaseInfo(_info);
        }

        /// <summary>获取指定时间点视频流数据包</summary>
        public void GetVideoStreamsPacket(DateTime time)
        {
            VideoDataParam param = new VideoDataParam(_info as VideoInfo, time);
            _downProxy.GetVideoData(param);
        }

        private void loadProxy()
        {
            _downProxy = new DownloadProxy(_info.SourceIp, _info.SourcePort);
            _downProxy.BytesLengthReceived += onBytes;
            _downProxy.ErrorOccured += onErrorEvent;
            _downProxy.ExceptionReceived += onErrorReceived;
            _downProxy.VideoDataReceived += onVideoPacketReceived;
            _downProxy.VideoBaseReceived += onVideoBasePacketReceived;
            _downProxy.VideoInfoReceived += onVideoTimePeriodsReceived;
        }

        private void onVideoBasePacketReceived(VideoBasePacket obj)
        {
            var handle = VideoBaseEvent;
            if (handle != null && obj != null)
                handle(obj);
        }

        private void disposeProxy()
        {
            if (_downProxy != null)
            {
                _downProxy.BytesLengthReceived -= onBytes;
                _downProxy.ErrorOccured -= onErrorEvent;
                _downProxy.ExceptionReceived -= onErrorReceived;
                _downProxy.VideoDataReceived -= onVideoPacketReceived;
                _downProxy.VideoBaseReceived -= onVideoBasePacketReceived;
                _downProxy.VideoInfoReceived -= onVideoTimePeriodsReceived;
                _downProxy.Close();
            }
            _downProxy = null;
        }

        private void onBytes(int bytesLength)
        {
            var handle = BytesLengthEvent;
            if (handle != null)
                handle(bytesLength);
        }

        private void onVideoPacketReceived(VideoStreamsPacket packet)
        {
            var handle = VideoStreamEvent;
            if (handle != null)
                handle(packet);
        }

        private void onVideoTimePeriodsReceived(VideoTimePeriodsPacket packet)
        {
            if (packet != null)
            {
                var handle = VideoTimePeriodsEvent;
                if (handle != null)
                    handle(this, packet);
                if (packet.TimePeriods.Length == 0)
                    onErrorStatus("未找到相关视频。");
            }
        }

        private void onErrorEvent(Exception ex)
        {
            if(ex is SocketException)
                onErrorStatus("网络连接错误！" + _info.SourceIp + ":"+_info.SourcePort);
            else
                onErrorStatus(ex.Message);
        }

        private void onErrorReceived(Exception ex)
        {
            onErrorStatus(ex.Message);
        }

        private void onErrorStatus(string promptInfom)
        {
            var handle = ErrorEvent;
            if (handle != null)
                handle(promptInfom);
        }

        public void Dispose()
        {
            disposeProxy();
        }
    }
}
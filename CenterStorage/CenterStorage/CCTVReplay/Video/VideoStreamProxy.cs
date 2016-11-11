using CCTVReplay.Proxy;
using CenterStorageCmd;
using SocketHelper.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCTVReplay.Util;
using Common.Log;

namespace CCTVReplay
{
    public class VideoStreamProxy: VideoProxyBase, IDisposable
    {
        public VideoStreamProxy()
        {
            Client.ReceiveCompleted += onReceived;
            try
            {
                EnsureStart();
            }
            catch(Exception ex)
            {
                Logger.Default.Error("尝试连接下载服务失败!", ex);
                string msg = "尝试连接下载服务失败!\n" + ex.Message;
                DialogUtil.ShowError(msg);
            }
        }

        public void Load(DownloadInfoParam param)
        {
            sendData((int)ParamCode.DownloadBegin, DownloadInfoParam.Encode(param));
        }

        public void LoadLocal(LocalDownloadInfoPacket param)
        {
            sendData((int)ParamCode.LocalDownloadBegin, LocalDownloadInfoPacket.Encode(param));
        }

        public void DownloadToPath(string downloadPath)
        {
            sendData((int)ParamCode.DownloadToLocal, PacketBase.GetBytes(downloadPath));
        }

        public void ProbeTime(DateTime time)
        {
            sendData((int)ParamCode.ProbeTime, PacketBase.GetBytes(time));
        }

        public void GetVideoStream(VideoDataParam param)
        {
            sendData((int)ParamCode.VideoPacket, VideoDataParam.Encode(param));
        }

        private void sendData(int code,byte[] data)
        {
            try
            {
                Client.Send(code, data);
            }
            catch (Exception ex)
            {
                Logger.Default.Error("发送数据失败!", ex);
                string msg = "发送数据失败!\n" + ex.Message;
                DialogUtil.ShowError(msg);
            }
        }

        private void onReceived(object sender, ReceiveEventArgs args)
        {
            if (args.ByteLength > 0)
            {
                using (MemoryStream ms = new MemoryStream(args.ReceivedBytes))
                {
                    int code = PacketBase.ReadInt(ms);
                    //Console.WriteLine("{0} - {1}", (ParamCode)code, args.ByteLength);
                    switch ((ParamCode)code)
                    {
                        case ParamCode.TimePeriods:
                            //接收所有有效视频分布
                            onTimePeriodsAll(VideoTimePeriodsPacket.Decode(ms).TimePeriods);
                            break;
                        case ParamCode.VideoBaseInfo:
                            //设置视频基本信息
                            onVideoBase(VideoBasePacket.Decode(ms));
                            break;
                        case ParamCode.DownloadProgress:
                            //接收推送的下载进度。
                            onTimePeriodsDownloaded(VideoTimePeriodsPacket.Decode(ms).TimePeriods);
                            break;
                        case ParamCode.VideoPacket:
                            //接收视频包数据流
                            onVideoStreams(VideoStreamsPacket.Decode(ms));
                            break;
                        case ParamCode.LocalDownloadPath:
                            onDownloadPathReceived(PacketBase.ReadString(ms));
                            break;
                        case ParamCode.Message:
                            onVideoMessageFeedback(MessagePacket.Decode(ms));
                            break;
                    }
                }
            }
        }

        public Action<TimePeriodPacket[]> TimePeriodsAllReceived;
        public Action<TimePeriodPacket[]> TimePeriodsDownloadedReceived;
        public Action<VideoBasePacket> VideoBaseReceived;
        public Action<VideoStreamsPacket> VideoStreamsReceived;
        public Action<MessagePacket> MessageReceived;
        public event Action<string> DownloadPathReceived;

        private void onTimePeriodsAll(TimePeriodPacket[] packets)
        {
            var handle = TimePeriodsAllReceived;
            if (handle != null)
                handle(packets);
        }

        private async void onTimePeriodsDownloaded(TimePeriodPacket[] packets)
        {
            await Task.Yield();
            var handle = TimePeriodsDownloadedReceived;
            if (handle != null)
                handle(packets);
        }

        private void onVideoBase(VideoBasePacket packet)
        {
            var handle = VideoBaseReceived;
            if (handle != null)
                handle(packet);
        }

        private async void onVideoStreams(VideoStreamsPacket packet)
        {
            await Task.Yield();
            var handle = VideoStreamsReceived;
            if (handle != null)
                handle(packet);
        }

        private void onVideoMessageFeedback(MessagePacket packet)
        {
            var handle = MessageReceived;
            if (handle != null)
                handle(packet);
        }

        private void onDownloadPathReceived(string path)
        {
            var handle = DownloadPathReceived;
            if (handle != null)
                handle(path);
        }
        public void Dispose()
        {
            Close();
        }
    }
}

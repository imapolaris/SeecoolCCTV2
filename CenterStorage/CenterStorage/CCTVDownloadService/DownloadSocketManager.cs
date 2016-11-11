using CenterStorageCmd;
using Common.Log;
using SocketHelper;
using SocketHelper.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CCTVDownloadService
{
    public class DownloadSocketManager: IDisposable
    {
        ManualResetEvent _disposeEvent = new ManualResetEvent(false);
        public SocketAdapter Adapter { get; private set; }

        IDownloadManager _download;
        public DownloadSocketManager(SocketAdapter adapter)
        {
            _disposeEvent.Reset();
            Adapter = adapter;
            Adapter.ErrorOccured += onErrorOccured;
            Adapter.Closed += onClosed;
            Adapter.ReceiveCompleted += onReceiveCompleted;
            send(ParamCode.EnsureConnect, new byte[0]);
        }

        private void onClosed(object sender, EventArgs e)
        {
            SocketAdapter adapter = sender as SocketAdapter;
            Logger.Default.Debug("Adapter has closed!_" + adapter.RemoteEndPoint);
            Dispose();
        }

        private void onErrorOccured(object sender, SocketHelper.Events.ErrorEventArgs args)
        {
            SocketAdapter adapter = sender as SocketAdapter;
            Logger.Default.Debug("Adapter ErrorOccured:" + args.ErrorMessage + "__" + adapter.RemoteEndPoint);
            adapter.Close();
        }
        
        private void onReceiveCompleted(object sender, ReceiveEventArgs args)
        {
            try
            {
                if (args.ByteLength > 0)
                {
                    using (MemoryStream ms = new MemoryStream(args.ReceivedBytes))
                    {
                        ParamCode code = (ParamCode)PacketBase.ReadInt(ms);
                        byte[] buffer = null;
                        switch (code)
                        {
                            case ParamCode.VideoInfosTimePeriods:
                                updateVideoInfosTimePeriods(VideoDataInfoParam.Decode(ms));
                                break;
                            case ParamCode.DownloadBegin:
                                beginDownload(DownloadInfoParam.Decode(ms));
                                break;
                            case ParamCode.DownloadToLocal:
                                downloadToLocal(PacketBase.ReadString(ms));
                                break;
                            case ParamCode.VideoPacket:
                                getVideoStreamsPacket(VideoDataParam.Decode(ms));
                                break;
                            case ParamCode.ProbeTime:
                                setProbeTime(PacketBase.ReadTime(ms));
                                break;
                            case ParamCode.DownloadInfosAll:
                                feedbackDownloadInfosToClient();
                                break;
                            case ParamCode.DownloadControl:
                                var controlCode = (DownloadControlCode)PacketBase.ReadInt(ms);
                                onDownloadControl(controlCode, ms);
                                break;
                            case ParamCode.LocalDownloadPath:
                                onLocalDownloadPath(PacketBase.ReadString(ms));
                                break;
                            case ParamCode.LocalDownloadBegin:
                                onLocalDownloadStart(LocalDownloadInfoPacket.Decode(ms));
                                break;
                        }
                        if (buffer != null)
                            send((ParamCode)code, buffer);
                    }
                }
            }
            catch(IOException ex)
            {
                sendMessage(MessageType.Warn, ex.Message, null);
            }
            catch(Exception ex)
            {
                sendMessage(MessageType.Error, ex.Message, null);
                Console.WriteLine(ex.ToString());
            }
        }

        private void onLocalDownloadPath(string path)
        {
            Logger.Default.Trace("获取本地视频信息，Path：{0}", path);
            LocalVideosInfoPacket packet = FolderManager.GetLocalVideoInfoPacket(path);
            send(ParamCode.LocalDownSource, LocalVideosInfoPacket.Encode(packet));
        }

        private void onDownloadControl(DownloadControlCode controlCode, MemoryStream ms)
        {
            switch (controlCode)
            {
                case DownloadControlCode.Add:
                    IDownloadInfo[] param = DownloadInfoParam.DecodeArray(ms);
                    OnlineDownloadsManager.Instance.AddRange(false, true, param);
                    break;
                case DownloadControlCode.Start:
                case DownloadControlCode.Pause:
                case DownloadControlCode.Delete:
                case DownloadControlCode.GoTop:
                    Guid guid = PacketBase.ReadGuid(ms);
                    OnlineDownloadsManager.Instance.DownloadControl(controlCode, guid);
                    break;
            }
        }

        private void feedbackDownloadInfosToClient()
        {
            Logger.Default.Trace("反馈所有下载信息及状态，用于下载客户端所有数据刷新！");
            IDownloadInfoExpand[] dps = OnlineDownloadsManager.Instance.GetDownloadPackets();
            send(ParamCode.DownloadInfosAll, DownloadInfoExpandPacket.EncodeArray(dps));
            OnlineDownloadsManager.Instance.DownloadInfoPartChanged -= onDownloadInfoPartChanged;
            OnlineDownloadsManager.Instance.DownloadInfoPartChanged += onDownloadInfoPartChanged;
            OnlineDownloadsManager.Instance.DownloadAdded -= onDownloadAdded;
            OnlineDownloadsManager.Instance.DownloadAdded += onDownloadAdded;
        }

        private void onDownloadAdded(byte[] obj)
        {
            send(ParamCode.DownloadInfosAdd, obj);
        }

        private void onDownloadInfoPartChanged(byte[] obj)
        {
            send(ParamCode.DownloadInfoPart, obj);
        }

        //拖拽进度条功能
        private void setProbeTime(DateTime time)
        {
            var down = _download as IOnlinePlayback;
            if (down != null)
            {
                Logger.Default.Trace(down.GuidCode + " 进度条时间变更： " + time);
                down?.SetProbeTime(time);
            }
        }

        private void getVideoStreamsPacket(VideoDataParam videoDataParam)
        {
            VideoStreamsPacket packet = _download?.GetVideoStreamsPacket(videoDataParam.Time);
            if (packet != null)
                send(ParamCode.VideoPacket, VideoStreamsPacket.Encode(packet));
            else
                send(ParamCode.VideoPacket, new byte[0]);
        }

        private void downloadToLocal(string path)
        {
            if (_download != null && _download is IOnlinePlayback)
            {
                var online = _download as IOnlinePlayback;
                string error = online.DownloadToLocal(path);
                if (error != null)
                {
                    Logger.Default.Error(error + "  " + path);
                    sendMessage(MessageType.Error, error, null);
                }
                Logger.Default.Trace("下载路径变更：{0}", online?.DownloadInfo.DownloadPath);
                sendDownloadPath(online);
            }
        }

        private void sendDownloadPath(IOnlinePlayback onlineDownload)
        {
            if (onlineDownload == null)
                return;
            if (onlineDownload.IsLocalDownload)
                send(ParamCode.LocalDownloadPath, PacketBase.GetBytes(onlineDownload.DownloadInfo.DownloadPath));
            else
                send(ParamCode.LocalDownloadPath, PacketBase.GetBytes(""));
        }

        private void beginDownload(DownloadInfoParam info)
        {
            OnlineDownloadsManager.Instance.AddRange(true, false, info);
            var download = OnlineDownloadsManager.Instance.GetDownloadManagerIgnorePath(info);
            _download = download;
            download.ExpandChanged += onDownloadExpandChanged;
            sendDownloadPath(_download as IOnlinePlayback);
            feedbackBase();
        }

        private void onDownloadExpandChanged(OnlineDownloadManager download, string obj)
        {
            if(download == _download)
            {
                if(obj == nameof(download.IsLocalDownload) || obj == nameof(download.DownloadInfo))
                    sendDownloadPath(download);
            }
        }

        private void onLocalDownloadStart(LocalDownloadInfoPacket packet)
        {
            Logger.Default.Trace("播放本地视频，名称：{0} VideoId：{1} Stream：{2} Path：{3}", packet.Info.VideoName, packet.Info.VideoId, packet.Info.StreamId, packet.Path);
            _download = new OfflinePlayManager(packet);
            feedbackBase();
        }

        private void feedbackBase()
        {
            new Thread(run) { IsBackground = true }.Start();
        }

        private void run()
        {
            try
            {
                //时间段分布
                while (!_disposeEvent.WaitOne(1))
                {
                    VideoTimePeriodsPacket  valid = _download.GetVideoTimePeriods();
                    if (valid != null)
                    {
                        send(ParamCode.TimePeriods, VideoTimePeriodsPacket.Encode(valid));
                        break;
                    }
                }

                //视频包头
                while (!_disposeEvent.WaitOne(1))
                {
                    VideoBasePacket vbp = _download.GetVideoBasePacket();
                    if (vbp != null)
                    {
                        send(ParamCode.VideoBaseInfo, VideoBasePacket.Encode(vbp));
                        break;
                    }
                }
                //实时下载进度
                while (!_disposeEvent.WaitOne(1))
                {
                    Thread.Sleep(1000);
                    bool canStopFeedback = canStopFeedbackProcess();
                    VideoTimePeriodsPacket down = _download.GetCompletedTimePeriods();
                    if (down != null)
                        send(ParamCode.DownloadProgress, VideoTimePeriodsPacket.Encode(down));
                    if (canStopFeedback)
                    {
                        Console.WriteLine("Stop Feedback RealTime Process! ");
                        if (_download is OnlineDownloadManager)
                        {
                            var downStatus = (_download as OnlineDownloadManager).DownloadStatus;
                            if(downStatus != DownloadStatus.Completed)
                            {
                                var name = _download.GetVideoTimePeriods().VideoName;
                                string status = DownloadStatusManager.ToHanZi(downStatus);
                                string message = name + status + "!";
                                if(downStatus == DownloadStatus.Error)
                                    message += "\n" + (_download as OnlineDownloadManager).ErrorInfo;
                                if(downStatus != DownloadStatus.Deleted)
                                    sendMessage(MessageType.Warn, message, "停止更新下载进度！");
                            }
                        }
                        break;
                    }
                }
            }
            catch(Exception ex)
            {
                Exception e = ex.InnerException  == null? ex : ex.InnerException;
                sendMessage(MessageType.Error, e.Message,"停止更新下载进度！");
                Console.WriteLine(ex);
            }
        }
        
        private bool canStopFeedbackProcess()
        {
            if (_download is IOfflinePlayback)
                return true;
            if (_download is IOnlinePlayback)
                return (_download as IOnlinePlayback).IsEndOfDownload();
            return false;
        }

        private void updateVideoInfosTimePeriods(VideoDataInfoParam infosParam)
        {//TODO:获取视频列表在指定时间段内的视频分布信息
            Logger.Default.Trace("获取 {0} - {1} 时间内 {2} 路视频资源分布。", infosParam.BeginTime, infosParam.EndTime, infosParam.VideoInfos.Length);
            new Thread(()=>runVideoInfosTimePeriods(infosParam)) { IsBackground = true }.Start();
        }

        private void runVideoInfosTimePeriods(VideoDataInfoParam infosParam)
        {
            Parallel.ForEach(infosParam.VideoInfos, videoInfo =>
            {
                readVideoTimePeriodsPacket(infosParam, infosParam, videoInfo);
            });
        }

        private void readVideoTimePeriodsPacket(ISourceInfo source, ITimePeriod tp, IVideoInfo vi)
        {
            try
            {
                VideoDownloadCmd cmd = new VideoDownloadCmd(new DownloadInfoParam(source, tp, vi, null));
                cmd.VideoTimePeriodsEvent += onVideoTimePeriodsEvent;
                cmd.GetTimePeriods();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Read Video Time Periods Packet Error! {0}({1}) - {2} : {3}", vi.VideoName, vi.VideoId, vi.StreamId, ex.ToString());
            }
        }

        private void onVideoTimePeriodsEvent(VideoDownloadCmd vd, VideoTimePeriodsPacket packet)
        {
            send(ParamCode.TimePeriods, VideoTimePeriodsPacket.Encode(packet));
            vd.Dispose();
        }

        private void send(ParamCode code, byte[] buffer)
        {
            if(Adapter != null && Adapter.IsConnected)
                Adapter.Send((int)code, buffer);
        }

        private void sendMessage(MessageType type, string message, string operate)
        {
            Logger.Default.Info("Send Message : {0} - {1} - {2}",type, message, operate);
            MessagePacket packet = new MessagePacket(type, message, operate);
            send(ParamCode.Message, MessagePacket.Encode(packet));
        }

        public void Dispose()
        {
            _disposeEvent.Set();
            (_download as IOnlinePlayback)?.SetPriority(false);
            if (Adapter != null)
            {
                Adapter.ErrorOccured -= onErrorOccured;
                Adapter.Closed -= onClosed;
                Adapter.ReceiveCompleted -= onReceiveCompleted;
            }
            Adapter = null;
            OnlineDownloadsManager.Instance.DownloadInfoPartChanged -= onDownloadInfoPartChanged;
        }
    }
}
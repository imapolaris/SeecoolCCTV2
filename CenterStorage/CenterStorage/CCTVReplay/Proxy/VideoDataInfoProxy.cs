using CCTVReplay.Util;
using CenterStorageCmd;
using SocketHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CCTVReplay.Source;

namespace CCTVReplay.Proxy
{
    public class VideoDataInfoProxy : VideoProxyBase
    {
        private VideoDataSource _src;
        private LocalVideosInfoPacket _localVideos;
        public bool SourceValid { get; private set; }

        public VideoDataInfoProxy()
        {
            Client.ReceiveCompleted += Client_ReceiveCompleted;
        }

        public void UpdateSource(VideoDataSource src)
        {
            if (src == null)
                throw new ErrorMessageException("视频数据源参数不能为空值。");
            Close();
            _src = src;
            _localVideos = null;
            SourceValid = true;
        }


        public void GetVideoDataInfoAsync(VideoInfo[] vis, DateTime begin, DateTime end)
        {
            if (!SourceValid)
                throw new ErrorMessageException("尚未设置数据源地址。");
            if (_src.SrcType == SourceType.Local)
            {
                if (_localVideos != null && _localVideos.ValidTimePeriods != null)
                {
                    //启动线程模拟异步事件。
                    new Thread(() =>
                    {
                        foreach (VideoInfo vi in vis)
                        {
                            VideoTimePeriodsPacket rst = _localVideos.ValidTimePeriods.FirstOrDefault(vtpp => vtpp.VideoId == vi.VideoId);
                            if (rst != null)
                                OnVideoDataInfoReceived(rst);
                        }
                    })
                    {
                        IsBackground = true
                    }.Start();
                }
            }
            else {
                EnsureStart();
                if (_src.Storage != null)
                {
                    VideoDataInfoParam param = new VideoDataInfoParam(_src.Storage.Ip, _src.Storage.Port, vis, begin, end);
                    Client.Send((int)ParamCode.VideoInfosTimePeriods, VideoDataInfoParam.Encode(param));
                }
                else
                {
                    throw new ErrorMessageException("未配置集中存储相关信息。");
                }
            }
        }

        public void GetVideoTreeNodesAsync()
        {
            if (!SourceValid)
                throw new ErrorMessageException("尚未设置数据源地址。");
            if (_src.SrcType == SourceType.Remote)
                throw new ErrorMessageException("此方法不能获取远程数据源视频录像信息。");
            EnsureStart();
            Client.Send((int)ParamCode.LocalDownloadPath, PacketBase.GetBytes(_src.LocalSourcePath));
        }


        private void Client_ReceiveCompleted(object sender, SocketHelper.Events.ReceiveEventArgs args)
        {
            if (args.ByteLength > 0)
            {
                using (MemoryStream ms = new MemoryStream(args.ReceivedBytes))
                {
                    using (BinaryReader br = new BinaryReader(ms))
                    {
                        int code = br.ReadInt32();
                        switch ((ParamCode)code)
                        {
                            case ParamCode.TimePeriods:
                                {
                                    OnVideoDataInfoReceived(VideoTimePeriodsPacket.Decode(ms));
                                }
                                break;
                            case ParamCode.LocalDownSource:
                                {
                                    OnTreeNodesReceived(LocalVideosInfoPacket.Decode(ms));
                                }
                                break;
                            case ParamCode.Message:
                                onMessage(MessagePacket.Decode(ms));
                                break;
                        }
                    }
                }
            }
        }

        #region 【事件定义】
        public event EventHandler<VideoDataInfoEventArgs> VideoDataInfoReceived;
        public event Action<LocalVideosInfoPacket> TreeNodesReceived;
        public Action<MessagePacket> MessageReceived;

        private void OnVideoDataInfoReceived(VideoTimePeriodsPacket vtpp)
        {
            EventHandler<VideoDataInfoEventArgs> handler = VideoDataInfoReceived;
            if (handler != null)
                handler(this, new VideoDataInfoEventArgs(vtpp.VideoId, vtpp.StreamId, vtpp.TimePeriods));
        }

        private void OnTreeNodesReceived(LocalVideosInfoPacket lvip)
        {
            _localVideos = lvip;
            Action<LocalVideosInfoPacket> handler = TreeNodesReceived;
            if (handler != null)
                handler(lvip);
            foreach (VideoTimePeriodsPacket vtpp in lvip.ValidTimePeriods)
                OnVideoDataInfoReceived(vtpp);
        }

        private void onMessage(MessagePacket packet)
        {
            var handle = MessageReceived;
            if (handle != null)
                handle(packet);
        }
        #endregion 【事件定义】
    }
}

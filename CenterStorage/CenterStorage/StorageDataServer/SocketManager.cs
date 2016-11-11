using CenterStorageCmd;
using CenterStorageService;
using SocketHelper;
using SocketHelper.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace StorageDataServer
{
    public class SocketManager
    {
        private SocketServer _server;
        public static SocketManager Instance { get; private set; }
        bool _isDownload = false;
        static SocketManager()
        {
            Instance = new SocketManager();
        }

        private SocketManager()
        {
            startListen();
        }

        private void startListen()
        {
            stopListen();
            _server = new SocketServer();
            int port = int.Parse(System.Configuration.ConfigurationManager.AppSettings["StorageHostPort"]);
            _server.Listen(port);
            _server.ClientAccepted += Server_ClientAccepted;
            _server.ErrorOccured += Server_ErrorOccured;
        }

        private void stopListen()
        {
            if (_server != null)
            {
                _server.ClientAccepted -= Server_ClientAccepted;
                _server.ErrorOccured -= Server_ErrorOccured;
                _server.StopListen();
                _server.Close();
            }
            _server = null;
        }

        private void Server_ErrorOccured(object sender, SocketHelper.Events.ErrorEventArgs args)
        {
            if (args.ErrorType == ErrorTypes.SocketAccept)
            {
                System.Threading.Thread.Sleep(5000);
                startListen();
            }
        }
        
        private void Server_ClientAccepted(object sender, SocketHelper.Events.ClientAcceptedEventArgs args)
        {
            string endpoint = args.Adapter.RemoteEndPoint.ToString();
            SocketAdapter adapter = args.Adapter;
            addEvent(adapter);
            adapter.Send((int)ParamCode.EnsureConnect);
        }

        private void addEvent(SocketAdapter adapter)
        {
            adapter.ErrorOccured += Adapter_ErrorOccured;
            adapter.Closed += Adapter_Closed;
            adapter.ReceiveCompleted += Adapter_ReceiveCompleted;
        }

        private void removeEvent(SocketAdapter adapter)
        {
            adapter.ErrorOccured -= Adapter_ErrorOccured;
            adapter.Closed -= Adapter_Closed;
            adapter.ReceiveCompleted -= Adapter_ReceiveCompleted;
        }

        private void Adapter_ReceiveCompleted(object sender, ReceiveEventArgs args)
        {
            if (args.ByteLength > 0)
            {
                SocketAdapter adapter = sender as SocketAdapter;
                using (MemoryStream ms = new MemoryStream(args.ReceivedBytes))
                {
                    int code = PacketBase.ReadInt(ms);
                    byte[] buffer = null;
                    switch ((ParamCode)code)
                    {
                        case ParamCode.TimePeriods:
                                buffer = getVideoTimePeriodsPacketBuffer(VideoBaseInfomParam.Decode(ms));
                            break;
                        case ParamCode.VideoPacket:
                                buffer = getVideoPacketBuffer(VideoDataParam.Decode(ms));
                            break;
                        case ParamCode.VideoBaseInfo:
                            VideoBaseInfomParam param = VideoBaseInfomParam.Decode(ms);
                            Common.Log.Logger.Default.Trace($"下载视频：{param.BeginTime} - {param.EndTime} - {param.VideoName} - {param.VideoId} - {param.StreamId} -- {adapter.RemoteEndPoint}");
                            _isDownload = true;
                            buffer = getVideoBasePacketBuffer(param);
                            break;
                        case ParamCode.StorageFlag:
                            onStorageFlag(StorageFlagParam.Decode(ms));
                            break;
                        case ParamCode.StorageFlagAll:
                            buffer = getStorageFlagAll();
                            Console.WriteLine("StorageFlagAll: {0}", buffer.Length);
                            break;
                    }
                    if(buffer != null)
                    {
                        adapter.Send(code, buffer);
                    }
                }
            }
        }

        private byte[] getStorageFlagAll()
        {
            return VideoInfo.EncodeArray(StorageFlagsManager.Instance.StorageFlags);
        }

        private void onStorageFlag(StorageFlagParam storageFlagParam)
        {
            StorageFlagsManager.Instance.Add(storageFlagParam);
        }

        private void Adapter_Closed(object sender, EventArgs e)
        {
            SocketAdapter adapter = sender as SocketAdapter;
            if(_isDownload)
                Common.Log.Logger.Default.Trace("结束下载!" + adapter.RemoteEndPoint);
            removeEvent(adapter);
        }

        private void Adapter_ErrorOccured(object sender, SocketHelper.Events.ErrorEventArgs args)
        {
            SocketAdapter adapter = sender as SocketAdapter;
            Console.WriteLine("Adapter ErrorOccured:"+args.ErrorMessage+"__" + adapter.RemoteEndPoint);
            adapter.Close();
        }

        private byte[] getVideoTimePeriodsPacketBuffer(VideoBaseInfomParam param)
        {
            VideoTimePeriodsPacket packet = StorageSearcher.Instance.Search(param.BeginTime, param.EndTime, param as VideoInfo);
            if (packet != null)
                return VideoTimePeriodsPacket.Encode(packet);
            else
                return new byte[0];
        }

        private byte[] getVideoPacketBuffer(VideoDataParam param)
        {
            VideoStreamsPacket packet = StorageDownloader.Instance.GetVideoPacket(param.VideoId, param.StreamId, param.Time);
            if (packet != null)
                return VideoStreamsPacket.Encode(packet);
            else
                return new byte[0];
        }

        private byte[] getVideoBasePacketBuffer(VideoBaseInfomParam param)
        {
            var packet = StorageDownloader.Instance.GetVideoBaseInfom(param.VideoId, param.StreamId, param.BeginTime, param.EndTime);
            if (packet != null)
                return VideoBasePacket.Encode(packet);
            else
                return new byte[0];
        }
    }
}

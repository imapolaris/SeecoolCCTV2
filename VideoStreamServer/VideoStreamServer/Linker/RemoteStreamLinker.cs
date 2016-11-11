using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Log;
using SocketHelper;
using SocketHelper.Events;
using VideoStreamModels;
using VideoStreamModels.Model;

namespace VideoStreamServer.Linker
{
    internal class RemoteStreamLinker : AbstractStreamLinker
    {
        private string _serverIp;
        private int _serverPort;
        private SocketClient _client;
        public RemoteStreamLinker(Uri uri, string serverIp, int serverPort)
        {
            StreamUri = uri;
            _serverIp = serverIp;
            _serverPort = serverPort;
        }

        protected override void Start()
        {
            if (_client != null)
                Stop();
            _client = new SocketClient();
            _client.ConnectCompleted += _client_ConnectCompleted;
            _client.ErrorOccured += _client_ErrorOccured;
            _client.ReceiveCompleted += _client_ReceiveCompleted;
            _client.BeginConnect(_serverIp, _serverPort);
        }

        protected override void Stop()
        {
            if (_client != null)
            {
                _client.ConnectCompleted -= _client_ConnectCompleted;
                _client.ErrorOccured -= _client_ErrorOccured;
                _client.ReceiveCompleted -= _client_ReceiveCompleted;
                _client.Disconnect();
            }
            CurrentHeader = null;
            _client = null;
        }

        private void _client_ConnectCompleted(object sender, EventArgs e)
        {
            try
            {
                if (_client.IsConnected)
                    _client.Send(new StreamAddress(StreamUri.AbsoluteUri, _serverIp, _serverPort).Serialize());
            }
            catch (Exception ex)
            {
                string err = $"向远程流媒体服务发送数据失败:Server:{_serverIp};URL:{StreamUri.AbsoluteUri};Error:{ex.Message}";
                Logger.Default.Error(err);
                OnErrorOccurred(new SocketHelper.Events.ErrorEventArgs(err, SocketHelper.Events.ErrorTypes.Other, -1, ex));
            }
        }

        private void _client_ReceiveCompleted(object sender, ReceiveEventArgs args)
        {
            try
            {
                if (args.ByteLenght >= 4)
                {
                    int code = BitConverter.ToInt32(args.ReceivedBytes, 0);
                    switch (code)
                    {
                        case StreamEntityCode.FfmpegHeader:
                            {
                                CurrentHeader = FfmpegStreamHeader.DeserializeTo(args.ReceivedBytes);
                                OnStreamHeaderRecieved(CurrentHeader);
                            }
                            break;
                        case StreamEntityCode.HikvHeader:
                            {
                                CurrentHeader = HikvStreamHeader.DeserializeTo(args.ReceivedBytes);
                                OnStreamHeaderRecieved(CurrentHeader);
                            }
                            break;
                        case StreamEntityCode.StreamData:
                            {
                                OnStreamDataReceived(StreamData.DeserializeTo(args.ReceivedBytes));
                            }
                            break;
                        case StreamEntityCode.RemoteError:
                            {
                                RemoteError re = RemoteError.DeserializeTo(args.ReceivedBytes);
                                OnErrorOccurred(new ErrorEventArgs(re.ErrorMessage, ErrorTypes.Receive));
                            }
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                OnErrorOccurred(new ErrorEventArgs($"从远程流媒体服务器获取到异常数据:{e.Message}", ErrorTypes.Receive));
            }
        }

        private void _client_ErrorOccured(object sender, SocketHelper.Events.ErrorEventArgs args)
        {
            OnErrorOccurred(args);
        }

        protected override void Dispose(bool disposing)
        {
            Stop();
            base.Dispose(disposing);
        }
    }
}

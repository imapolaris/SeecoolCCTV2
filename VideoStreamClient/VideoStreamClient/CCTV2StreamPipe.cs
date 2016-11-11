using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketHelper;
using SocketHelper.Events;
using VideoStreamModels;
using VideoStreamModels.Model;

namespace VideoStreamClient
{
    internal class CCTV2StreamPipe : IDisposable
    {
        public Uri StreamUri { get; protected set; }
        public IStreamHeader CurrentHeader { get; protected set; }

        private string _serverIp;
        private int _serverPort;
        private string _streamServerIp;
        private int _streamServerPort;
        private SocketClient _client;

        public CCTV2StreamPipe(Uri uri, string serverIp, int serverPort, string streamIp, int streamPort)
        {
            StreamUri = uri;
            _serverIp = serverIp;
            _serverPort = serverPort;
            _streamServerIp = streamIp;
            _streamServerPort = streamPort;
        }

        private bool _started = false;
        public void Start()
        {
            if (!_started)
            {
                if (_client != null)
                    Stop();
                _client = new SocketClient();
                _client.ConnectCompleted += _client_ConnectCompleted;
                _client.ErrorOccured += _client_ErrorOccured;
                _client.ReceiveCompleted += _client_ReceiveCompleted;
                _client.BeginConnect(_serverIp, _serverPort);
                _started = true;
            }
        }

        public void Stop()
        {
            if (_started)
            {
                if (_client != null)
                {
                    _client.ConnectCompleted -= _client_ConnectCompleted;
                    _client.ErrorOccured -= _client_ErrorOccured;
                    _client.ReceiveCompleted -= _client_ReceiveCompleted;
                    _client.Disconnect();
                }
                _client = null;
                _started = false;
            }
        }

        private void _client_ConnectCompleted(object sender, EventArgs e)
        {
            try
            {
                if (_client.IsConnected)
                    _client.Send(new StreamAddress(StreamUri.AbsoluteUri, _streamServerIp, _streamServerPort).Serialize());
            }
            catch (Exception ex)
            {
                string err = $"向远程流媒体服务发送数据失败:Server:{_serverIp};URL:{StreamUri.AbsoluteUri};Error:{ex.Message}";
                OnErrorOccurred(new ErrorEventArgs(err, ErrorTypes.Other, -1, ex));
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

        private void _client_ErrorOccured(object sender, ErrorEventArgs args)
        {
            OnErrorOccurred(args);
        }

        #region 【事件定义】
        public event EventHandler<StreamData> StreamDataReceived;
        public event EventHandler<IStreamHeader> StreamHeaderReceived;
        public event EventHandler<ErrorEventArgs> ErrorOccurred;
        protected virtual void OnStreamDataReceived(StreamData data)
        {
            var handler = StreamDataReceived;
            if (handler != null)
                handler(this, data);
        }

        protected virtual void OnStreamHeaderRecieved(IStreamHeader header)
        {
            var handler = StreamHeaderReceived;
            if (handler != null)
                handler(this, header);
        }

        protected virtual void OnErrorOccurred(ErrorEventArgs args)
        {
            var handler = ErrorOccurred;
            if (handler != null)
                handler(this, args);
        }
        #endregion 【事件定义】

        #region 【实现IDisposable接口】
        protected bool IsDisposed { get; private set; } = false;
        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                Stop();
                IsDisposed = true;
            }
        }

        ~CCTV2StreamPipe()
        {
            Dispose(false);
        }
        #endregion 【实现IDisposable接口】
    }
}

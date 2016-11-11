using CCTVReplay.Util;
using CenterStorageCmd;
using SocketHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CCTVReplay.Proxy
{
    public abstract class VideoProxyBase
    {
        private SocketClient _client;
        private AutoResetEvent _waitHandle;

        protected SocketClient Client { get { return _client; } }
        protected VideoProxyBase()
        {
            _client = new SocketClient();
            _client.ErrorOccured += Client_ErrorOccured;
        }

        public bool IsConnected { get { return Client.IsConnected; } }

        public virtual void Close()
        {
            _client.Disconnect();
        }

        protected void EnsureStart()
        {
            if (!DownloadServerProxy.IsDownloadServerAlive())
            {
                DownloadServerProxy.StartDownloadServer();
            }

            if (!_client.IsConnected)
            {
                _waitHandle = new AutoResetEvent(false);
                _client.ReceiveCompleted += Client_ReceiveCompleted;
                if (!_client.Connect(ConfigReader.Instance.DownloadHost, ConfigReader.Instance.DownloadPort))
                {
                    _client.ReceiveCompleted -= Client_ReceiveCompleted;
                    throw new UnConnectedException("未能连接到下载服务：" + ConfigReader.Instance.DownloadHost + "_" + ConfigReader.Instance.DownloadPort);
                }
                else
                {
                    _waitHandle.WaitOne(5000);
                    _client.ReceiveCompleted -= Client_ReceiveCompleted;
                }
            }
        }

        private void Client_ReceiveCompleted(object sender, SocketHelper.Events.ReceiveEventArgs args)
        {
            if (args.ByteLength == 4)
            {
                if (BitConverter.ToInt32(args.ReceivedBytes, 0) == (int)ParamCode.EnsureConnect)
                {
                    if (_waitHandle != null)
                        _waitHandle.Set();
                }
            }
        }

        private void Client_ErrorOccured(object sender, SocketHelper.Events.ErrorEventArgs args)
        {
            Console.WriteLine(args.ErrorMessage);
            ProcError();
        }

        protected virtual void ProcError()
        {
            if (_waitHandle != null)
            {
                _waitHandle.Set();
                Thread.Sleep(10);
            }
            Close();
        }
    }
}

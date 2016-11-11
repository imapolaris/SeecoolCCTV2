using CenterStorageCmd;
using SocketHelper;
using StorageDataProxy.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StorageDataProxy
{
    public class ProxyBase : IDisposable
    {
        private string _remoteIP;
        private int _port;
        private SocketClient _client;
        protected SocketClient Client { get { return _client; } }

        private AutoResetEvent _waitHandle;
        public ProxyBase(string ip, int port)
        {

            _remoteIP = ip;
            _port = port;
            _client = new SocketClient();
            _client.ErrorOccured += Client_ErrorOccured;
            _client.ReceiveCompleted += Client_ReceiveCompleted;
        }

        protected void EnsureStart()
        {
            if (!_client.IsConnected)
            {
                _waitHandle = new AutoResetEvent(false);
                if (!_client.Connect(_remoteIP, _port))
                {
                    throw new ConnnectErrorException($"网络连接失败！{_remoteIP}:{_port}");
                }
                else
                    _waitHandle.WaitOne();
            }
        }

        protected void Send(int code, byte[] buffer)
        {
            EnsureStart();
            Client.Send(code, buffer);
        }

        private void Client_ReceiveCompleted(object sender, SocketHelper.Events.ReceiveEventArgs args)
        {
            if (args.ByteLength == 4)
            {
                if (BitConverter.ToInt32(args.ReceivedBytes, 0) == (int)ParamCode.EnsureConnect)
                {
                    if (_waitHandle != null)
                        _waitHandle.Set();
                    _client.ReceiveCompleted -= Client_ReceiveCompleted;
                }
            }
        }

        private void Client_ErrorOccured(object sender, SocketHelper.Events.ErrorEventArgs args)
        {
            if (_waitHandle != null)
            {
                _waitHandle.Set();
                Thread.Sleep(10);
            }
            OnErrorOccured(args.InnerException);
        }

        public void Close()
        {
            if (_client != null)
            {
                _client.ErrorOccured -= Client_ErrorOccured;
                _client.ReceiveCompleted -= Client_ReceiveCompleted;
                _client.Disconnect();
            }
            _client = null;
        }

        public Action<Exception> ErrorOccured;
        protected void OnErrorOccured(Exception ex)
        {
            var handler = ErrorOccured;
            if (handler != null)
            {
                handler(ex);
            }
        }

        #region 【实现IDisposable接口】
        private bool _disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                Close();
                _client = null;
                _disposed = true;
            }
        }

        ~ProxyBase()
        {
            Dispose(false);
        }
        #endregion 【实现IDisposable接口】
    }
}

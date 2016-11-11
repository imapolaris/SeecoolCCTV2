using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CenterStorageCmd;
using SocketHelper;

namespace CenterStorageDeploy.Proxy
{
    public abstract class ProxyBase
    {
        private SocketClient _client;
        private AutoResetEvent _waitHandle;

        protected SocketClient Client { get { return _client; } }
        private string _serverIp;
        private int _port;
        protected ProxyBase(string ip, int port)
        {
            _serverIp = ip;
            _port = port;
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
            if (!_client.IsConnected)
            {
                _waitHandle = new AutoResetEvent(false);
                _client.ReceiveCompleted += Client_ReceiveCompleted;
                if (!_client.Connect(_serverIp, _port))
                {
                    _client.ReceiveCompleted -= Client_ReceiveCompleted;
                    throw new UnConnectedException("无法连接到数据服务器！");
                }
                else
                {
                    _waitHandle.WaitOne();
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

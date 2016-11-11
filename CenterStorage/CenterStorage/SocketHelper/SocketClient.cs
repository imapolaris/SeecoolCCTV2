using SocketHelper.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketHelper
{
    public class SocketClient
    {
        private SocketAdapter _adapter;
        public SocketClient()
        {

        }

        public bool IsConnected
        {
            get { return _adapter != null && _adapter.IsConnected; }
        }

        public EndPoint LocalEndPoint
        {
            get { return _adapter?.LocalEndPoint; }
        }

        public EndPoint RemoteEndPoint
        {
            get { return _adapter?.RemoteEndPoint; }
        }

        public bool Connect(string remoteIp, int port)
        {
            Disconnect();
            IPAddress ip = IPAddress.Parse(remoteIp);
            IPEndPoint ep = new IPEndPoint(ip, port);
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try {
                client.Connect(ep);
                buildAdapter(client);
            }
            catch(SocketException se)
            {
                OnErrorOccured(new ErrorEventArgs(se.Message, ErrorTypes.SocketConnect, se.ErrorCode, se));
                return false;
            }
            return true;
        }

        public void BeginConnect(string remoteIp, int port)
        {
            Disconnect();
            IPAddress ip = IPAddress.Parse(remoteIp);
            IPEndPoint ep = new IPEndPoint(ip, port);
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.BeginConnect(ep, connectCallback, client);
        }

        private void connectCallback(IAsyncResult ar)
        {
            Socket s = ar.AsyncState as Socket;
            try
            {
                s.EndConnect(ar);
                buildAdapter(s);
                OnConnectCompleted();
            }
            catch (SocketException se)
            {
                OnErrorOccured(new ErrorEventArgs(se.Message, ErrorTypes.SocketConnect, se.ErrorCode, se));
            }
        }

        public void Disconnect()
        {
            destoryAdapter();
        }

        private void buildAdapter(Socket soc)
        {
            _adapter = new SocketAdapter(soc);
            _adapter.SendCompleted += Adapter_SendCompleted;
            _adapter.ReceiveCompleted += Adapter_ReceiveCompleted;
            _adapter.ErrorOccured += Adapter_ErrorOccured;
            _adapter.Receive();
        }

        private void destoryAdapter()
        {
            if (_adapter != null)
            {
                _adapter.SendCompleted -= Adapter_SendCompleted;
                _adapter.ReceiveCompleted -= Adapter_ReceiveCompleted;
                _adapter.ErrorOccured -= Adapter_ErrorOccured;
                _adapter.Close();
                _adapter = null;
            }
        }

        private void Adapter_ErrorOccured(object sender, ErrorEventArgs args)
        {
            OnErrorOccured(args);
        }

        private void Adapter_ReceiveCompleted(object sender, ReceiveEventArgs args)
        {
            OnReceiveCompleted(args);
        }

        private void Adapter_SendCompleted(object sender, SendEventArgs args)
        {
            OnSendCompleted(args);
        }

        public int Send(int code, byte[] bytes)
        {
            if (_adapter == null)
                throw new InvalidOperationException("尚未连接到远程服务器,或连接已关闭。");
            return _adapter.Send(code, bytes);
        }

        #region 【事件定义】
        public event ReceiveCompletedHandler ReceiveCompleted;
        public event SendCompletedHandler SendCompleted;
        public event ErrorOccuredHandler ErrorOccured;
        public event EventHandler ConnectCompleted;

        private void OnReceiveCompleted(ReceiveEventArgs args)
        {
            ReceiveCompletedHandler handler = ReceiveCompleted;
            if (handler != null)
                handler(this, args);
        }

        private void OnSendCompleted(SendEventArgs args)
        {
            SendCompletedHandler handler = SendCompleted;
            if (handler != null)
                handler(this, args);
        }

        private void OnErrorOccured(ErrorEventArgs args)
        {
            ErrorOccuredHandler handler = ErrorOccured;
            if (handler != null)
                handler(this, args);
        }

        private void OnConnectCompleted()
        {
            EventHandler handler = ConnectCompleted;
            if (handler != null)
                handler(this, new EventArgs());
        }
        #endregion 【事件定义】
    }
}

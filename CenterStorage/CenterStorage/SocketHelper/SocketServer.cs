using SocketHelper.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketHelper
{
    public class SocketServer
    {
        private Socket _listener;
        private int _maxConn;
        private bool _closing = false;
        private bool _closed = false;

        public SocketServer() : this(16)
        {

        }

        /// <summary>
        /// 同时工作的最大连接数，默认为16.
        /// </summary>
        /// <param name="connections"></param>
        public SocketServer(int connections)
        {
            if (connections < 0)
                connections = 0;
            _maxConn = connections;
        }

        public void Listen(int port)
        {
            StopListen();
            IPAddress ip = IPAddress.Any;
            IPEndPoint ep = new IPEndPoint(ip, port);
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            s.Bind(ep);
            s.Listen(_maxConn);
            _listener = s;
            _listener.BeginAccept(acceptCallback, _listener);
        }

        private object _closeObj = new object();
        public void Close()
        {
            new Thread(StopListen)
            {
                IsBackground = true
            }.Start();
            _closed = true;
        }

        public void StopListen()
        {
            _closing = true;
            lock (_closeObj)
            {
                if (_listener != null)
                    _listener.Close();
                _listener = null;
            }
            _closing = false;
        }

        private void acceptCallback(IAsyncResult ar)
        {
            try
            {
                if (_closed || _closing)
                    return;
                Socket s = ar.AsyncState as Socket;
                Socket wSocket = s.EndAccept(ar);
                //引发事件
                SocketAdapter adapter = new SocketAdapter(wSocket);
                adapter.Receive();
                OnClientAccepted(new ClientAcceptedEventArgs(adapter));

                s.BeginAccept(acceptCallback, s);
            }
            catch (SocketException se)
            {
                if (_closed || _closing)
                    return;
                OnErrorOccured(new ErrorEventArgs(se.Message, ErrorTypes.SocketAccept, se.ErrorCode, se));
            }
        }

        #region 【事件定义】
        public event ErrorOccuredHandler ErrorOccured;
        public event ClientAcceptedHandler ClientAccepted;

        private void OnErrorOccured(ErrorEventArgs args)
        {
            ErrorOccuredHandler handler = ErrorOccured;
            if (handler != null)
                handler(this, args);
        }

        private void OnClientAccepted(ClientAcceptedEventArgs args)
        {
            ClientAcceptedHandler handler = ClientAccepted;
            if (handler != null)
                handler(this, args);
        }
        #endregion 【事件定义】
    }
}

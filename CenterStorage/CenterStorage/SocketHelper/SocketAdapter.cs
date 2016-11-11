using SocketHelper.Events;
using SocketHelper.State;
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
    public class SocketAdapter
    {
        private string _guid;
        private int _seed = 1;
        private Socket _socket;
        private int _emptyTimes = 0;
        private bool _closed = false;

        internal SocketAdapter(Socket soc)
        {
            _guid = Guid.NewGuid().ToString(); ;
            _socket = soc;
            _remoteEndPoint = _socket.RemoteEndPoint;
        }

        internal Socket WorkSocket
        {
            get { return _socket; }
        }

        public EndPoint LocalEndPoint
        {
            get
            {
                try
                {
                    return _socket != null ? _socket.LocalEndPoint : null;
                }
                catch (ObjectDisposedException) { return null; }
            }
        }
        EndPoint _remoteEndPoint = null;
        public EndPoint RemoteEndPoint
        {
            get
            {
                return _remoteEndPoint;
            }
        }

        public string GUID
        {
            get { return _guid; }
        }

        public bool IsConnected
        {
            get { return _socket.Connected && !_closed; }
        }

        private object _closeObj = new object();
        public void Close()
        {
            new Thread(() =>
            {
                lock (_closeObj)
                {
                    if (_socket != null)
                    {
                        _socket.Close();
                    }
                }
                OnClosed();
            })
            {
                IsBackground = true
            }.Start();
            _closed = true;
        }

        internal void Receive()
        {
            ReceiveState state = new ReceiveState()
            {
                WorkSocket = _socket
            };
            try
            {
                _socket.BeginReceive(state.Buffer, 0, ReceiveState.BufferSize, SocketFlags.None, receiveCallBack, state);
            }
            catch (SocketException se)
            {
                handleSocketException(se, ErrorTypes.Receive);
            }
        }

        private void receiveCallBack(IAsyncResult ar)
        {
            ReceiveState state = ar.AsyncState as ReceiveState;
            Socket s = state.WorkSocket;
            try
            {
                int byteLen = s.EndReceive(ar);
                if (byteLen == 0)
                {
                    _emptyTimes++;
                    if (_emptyTimes == 100)
                    {//连续接收到无效的空白信息，可能由于远端连接已异常关闭！
                        OnErrorOccured(new ErrorEventArgs("接收数据异常！", ErrorTypes.SocketAccept));
                        Close();
                        return;
                    }
                }
                else
                {
                    _emptyTimes = 0;
                }
                byte[] buffer = state.Buffer;
                //判断是否有上次接收但未处理的字节。
                if (state.UnhandledBytes != null)
                {
                    buffer = BytesHelper.Combine(state.UnhandledBytes, buffer);
                    byteLen += state.UnhandledBytes.Length;
                    state.UnhandledBytes = null;
                }
                //根据前缀字节长度对接收数据分组。
                Queue<ReceiveState> sQueue = new Queue<ReceiveState>();
                processReceived(sQueue, state, buffer, 0, byteLen);

                ReceiveState workingRS = null;
                while (sQueue.Count > 0)
                {
                    ReceiveState rs = sQueue.Dequeue();
                    if (rs.Completed)
                    {
                        OnReceiveCompleted(new ReceiveEventArgs(rs.Received));
                    }
                    else
                    {
                        workingRS = rs;
                        break;
                    }
                }
                if (sQueue.Count > 0)
                {
                    Close();
                    OnErrorOccured(new ErrorEventArgs("接收数据错误!", ErrorTypes.Receive));//由于数据包丢失，导致接收数据不完整，连接已关闭！
                    return;
                }

                if (workingRS != null)
                    s.BeginReceive(workingRS.Buffer, 0, ReceiveState.BufferSize, SocketFlags.None, receiveCallBack, workingRS);
                else
                    Receive();
            }
            catch (SocketException se)
            {
                handleSocketException(se, ErrorTypes.Receive);
            }
            catch (ObjectDisposedException)
            {
                //不做处理
            }
        }

        private void processReceived(Queue<ReceiveState> stateQueue, ReceiveState state, byte[] buffer, int start, int byteLen)
        {
            if (state.IsNew)
            {
                if (byteLen >= 4)
                {
                    //解析本次待接收的字节长度。
                    int len = BitConverter.ToInt32(buffer, start);
                    state.TotalBytes = len;
                    start += 4;
                    byteLen -= 4;
                }
                else
                {
                    //当发送端多次发送的字节流拥堵时，接收端会接收到连续的字节数据。
                    //所以单次接收的数据有可能不完整。
                    byte[] sub = BytesHelper.SubArray(buffer, start, byteLen);
                    state.UnhandledBytes = sub;
                    stateQueue.Enqueue(state);
                    return;
                }
            }
            int missing = state.TotalBytes - state.ReceivedBytes;
            if (missing >= byteLen)
            {
                state.AppendBytes(buffer, start, byteLen);
                stateQueue.Enqueue(state);
            }
            else
            {
                state.AppendBytes(buffer, start, missing);
                //上一接收事件入列。
                stateQueue.Enqueue(state);

                start += missing;
                byteLen -= missing;
                //生成新的接收事件。
                ReceiveState newState = new ReceiveState()
                {
                    WorkSocket = state.WorkSocket,
                };
                processReceived(stateQueue, newState, buffer, start, byteLen);
            }
        }

        public int Send(int code, byte[] bytes = null)
        {
            if (!_socket.Connected)
                throw new UnConnectedException("尚未连接到远程服务器,或连接已被关闭。");
            byte[] buffer = addPrefixLength(code, bytes);
            if (_seed == int.MaxValue)
                _seed = 0;
            int handle = _seed++;
            sendImmediately(buffer, handle);
            return handle;
        }

        private byte[] addPrefixLength(int code, byte[] bytes)
        {
            bytes = bytes ?? new byte[0];
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                ms.Write(BitConverter.GetBytes(bytes.Length + 4), 0, 4);
                ms.Write(BitConverter.GetBytes(code), 0, 4);
                ms.Write(bytes, 0, bytes.Length);
                return ms.ToArray();
            }
        }

        private IAsyncResult sendImmediately(byte[] bytes, int handle)
        {
            try
            {
                SendState state = new SendState()
                {
                    Handle = handle,
                    WorkSocket = _socket
                };
                return _socket.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, sendCallback, state);
            }
            catch (SocketException se)
            {
                handleSocketException(se, ErrorTypes.Send);
            }
            catch (ObjectDisposedException ex)
            {
                OnErrorOccured(new ErrorEventArgs(ex.Message, ErrorTypes.Send, 0, ex));
            }
            return null;
        }

        private void sendCallback(IAsyncResult ar)
        {
            SendState state = ar.AsyncState as SendState;
            try
            {
                int len = state.WorkSocket.EndSend(ar);
                if (state.Handle > 0)
                {
                    OnSendCompleted(new SendEventArgs(state.Handle));
                }
            }
            catch (SocketException se)
            {
                handleSocketException(se, ErrorTypes.Send);
            }
            catch (ObjectDisposedException)
            {
                //不做处理
            }
        }

        private void handleSocketException(SocketException se, ErrorTypes eType)
        {
            OnErrorOccured(new Events.ErrorEventArgs(se.Message, eType, se.ErrorCode, se));
        }

        #region 【事件定义】
        public event ReceiveCompletedHandler ReceiveCompleted;
        public event SendCompletedHandler SendCompleted;
        public event ErrorOccuredHandler ErrorOccured;
        public event EventHandler Closed;

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

        private void OnErrorOccured(Events.ErrorEventArgs args)
        {
            ErrorOccuredHandler handler = ErrorOccured;
            if (handler != null)
                handler(this, args);
        }

        private void OnClosed()
        {
            EventHandler handler = Closed;
            if (handler != null)
                handler(this, new EventArgs());
        }
        #endregion 【事件定义】

        class SendQueueParam
        {
            public int Handle { get; set; }
            public Socket WorkSocket { get; set; }
            public byte[] SendBytes { get; set; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CCTVClient
{
    public class CCTVConnection
    {
        private string _host;
        private int _port;

        public CCTVConnection(string host, int port)
        {
            _host = host;
            _port = port;
            Connected = false;
        }

        public delegate void OnMessage(byte[] message);
        public OnMessage MessageEvent;
        private void fireOnMessage(byte[] message)
        {
            OnMessage callback = MessageEvent;
            if (callback != null)
                callback(message);
        }

        public Action ConnectEvent;
        private void fireOnConnect()
        {
            Action callback = ConnectEvent;
            if (callback != null)
                callback();
        }

        public Action DisconnectEvent;
        private void fireOnDisconnect()
        {
            Action callback = DisconnectEvent;
            if (callback != null)
                callback();
        }

        public bool Connected { get; private set; }

        private ManualResetEvent _event = new ManualResetEvent(false);
        private Thread _socketThread;
        private TcpClient _client;

        public void Start()
        {
            _event.Reset();
            _socketThread = new Thread(socketThread);
            _socketThread.IsBackground = true;
            _socketThread.Start();
        }

        private void socketThread()
        {
            do
            {
                if (connect())
                    socketRun();
            }
            while (!_event.WaitOne(5000));
        }

        private bool connect()
        {
            _client = new TcpClient();
            try
            {
                _client.Connect(_host, _port);
                return true;
            }
            catch
            {
            }

            return false;
        }

        void socketRun()
        {
			int recvBuffer = 8192;
			try
			{
				recvBuffer = _client.ReceiveBufferSize;
			}
			catch
			{
				closeSocket();
				_client = null;
				Connected = false;
				return;
			}

			_lastRecv = _lastSend = DateTime.Now;
            Connected = true;
            fireOnConnect();

            ManualResetEvent exitEvent = new ManualResetEvent(false);
            Thread sendThread = new Thread(sendThreadFunc);
            sendThread.IsBackground = true;
            sendThread.Start(exitEvent);

			byte[] buffer = new byte[recvBuffer];
            MemoryStream ms = new MemoryStream();
            while (!_event.WaitOne(1))
            {
                int received = socketReceive(buffer);

                if (received > 0)
                {
                    ms.Write(buffer, 0, received);
                    int parsed = parse(ms.GetBuffer(), (int)ms.Length);
                    if (parsed > 0)
                    {
                        MemoryStream newMs = new MemoryStream();
                        newMs.Write(ms.GetBuffer(), parsed, (int)ms.Length - parsed);
                        ms = newMs;
                    }

                    _lastRecv = DateTime.Now;
                }
                else
                    break;
            }

            exitEvent.Set();
            sendThread.Join();

            _client = null;
            Connected = false;
            fireOnDisconnect();
        }

        void sendThreadFunc(object obj)
        {
            ManualResetEvent exitEvent = (ManualResetEvent)obj;
            while (!exitEvent.WaitOne(1))
                socketIdle();

            lock (_messages)
                _messages.Clear();
        }

        private int parse(byte[] buffer, int length)
        {
            int parsed = 0;
            while (true)
            {
                if (length - parsed >= 4)
                {
                    int messageLen = BitConverter.ToInt32(buffer, parsed);
                    if (length - parsed >= messageLen)
                    {
                        byte[] message = new byte[messageLen];
                        Array.Copy(buffer, parsed, message, 0, messageLen);
                        fireOnMessage(message);
                        parsed += messageLen;
                    }
                    else
                        break;
                }
                else
                    break;
            }

            return parsed;
        }

        DateTime _lastSend = DateTime.MinValue;
        DateTime _lastRecv = DateTime.MinValue;
        byte[] _heartbeatMessage = new byte[] { 0x08, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF };

        private void socketIdle()
        {
            DateTime now = DateTime.Now;
            if (now - _lastRecv > TimeSpan.FromSeconds(30))
                closeSocket();
            else
            {
                lock (_messages)
                {
                    while (_messages.Count > 0)
                    {
                        if (!send(_messages.Dequeue()))
                            break;
                    }
                }

                if (now - _lastSend > TimeSpan.FromSeconds(3))
                    send(_heartbeatMessage);
            }
        }

        private bool send(byte[] message)
        {
            _lastSend = DateTime.Now;
            try
            {
                _client.Client.Send(message);
            }
            catch
            {
                closeSocket();
                return false;
            }

            return true;
        }

        private void closeSocket()
        {
            try
            {
                _client.Close();
            }
            catch
            {
            }
        }

        int socketReceive(byte[] buffer)
        {
            try
            {
                int received = _client.Client.Receive(buffer);
                if (received > 0)
                    return received;
            }
            catch
            {
                return -1;
            }

            return 0;
        }

        public void Stop()
        {
            _event.Set();
            closeSocket();
            _socketThread.Join();
            Connected = false;
        }

        Queue<byte[]> _messages = new Queue<byte[]>();
        public void Send(byte[] message)
        {
            lock (_messages)
                _messages.Enqueue(message);
        }

        public void Reconnect()
        {
            closeSocket();
        }
    }
}

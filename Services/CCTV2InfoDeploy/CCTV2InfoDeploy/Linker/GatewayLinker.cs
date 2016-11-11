using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CCTV2InfoDeploy.Util;
using GatewayModels;
using GatewayModels.Param;
using SocketHelper;

namespace CCTV2InfoDeploy.Linker
{
    public class GatewayLinker
    {
        public static GatewayLinker Instance { get; private set; }
        static GatewayLinker()
        {
            Instance = new GatewayLinker();
        }

        private Uri _netUri;
        private SocketClient _client;
        private AutoResetEvent _ensureConn;
        private AutoResetEvent _wait;
        private object _receiveObj;
        private object locker = new object();

        public bool IsConnected { get { return _client.IsConnected; } }

        private GatewayLinker()
        {
            string gAddr = ConfigurationManager.AppSettings["GatewayAddress"];
            _netUri = new Uri(gAddr);
            _client = new SocketClient();
            _client.ErrorOccured += _client_ErrorOccured;
            _client.ReceiveCompleted += _client_ReceiveCompleted;
        }

        private bool ensureConnect()
        {
            if (!_client.IsConnected)
            {
                try
                {
                    if (_ensureConn != null)
                        _ensureConn.Set();
                    _ensureConn = new AutoResetEvent(false);
                    bool conn = _client.Connect(_netUri.Host, _netUri.Port);
                    if (conn)
                    {
                        _ensureConn.WaitOne(Constants.WaitingTime);
                        _ensureConn.Dispose();
                        _ensureConn = null;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            return _client.IsConnected;
        }

        public T GetValue<T>(Packet p)
        {
            lock (locker)
            {
                if (ensureConnect())
                {
                    _receiveObj = null;
                    _wait = new AutoResetEvent(false);
                    try
                    {
                        _client.Send(p.Serialize());
                    }
                    catch (Exception e) { Trace.WriteLine(e.Message); }
                    _wait.WaitOne(Constants.WaitingTime);
                    _wait.Dispose();
                    _wait = null;
                    return (T)_receiveObj;
                }
                return default(T);
            }
        }

        public void SendCommand(Packet p)
        {
            lock (locker)
            {
                if (ensureConnect())
                {
                    try
                    {
                        _client.Send(p.Serialize());
                    }
                    catch (Exception e) { Trace.WriteLine(e.Message); }
                }
            }
        }

        private void _client_ReceiveCompleted(object sender, SocketHelper.Events.ReceiveEventArgs args)
        {
            if (args.ByteLenght >= 4)
            {
                int code = BitConverter.ToInt32(args.ReceivedBytes, 0);
                switch (code)
                {
                    case MessageCode.EnsureConnect:
                        if (_ensureConn != null)
                            _ensureConn.Set();
                        break;
                    case MessageCode.StartServer:
                        break;
                    case MessageCode.StopServer:
                        break;
                    case MessageCode.StartRegister:
                        break;
                    case MessageCode.StopRegister:
                        break;
                    case MessageCode.IsServerStarted:
                    case MessageCode.IsSuperiorOnline:
                    case MessageCode.IsLowerOnline:
                        _receiveObj = BoolPacket.DeserializeObject(args.ReceivedBytes);
                        if (_wait != null)
                            _wait.Set();
                        break;
                }
            }
        }

        private void _client_ErrorOccured(object sender, SocketHelper.Events.ErrorEventArgs args)
        {
            Trace.WriteLine(args.ErrorMessage);
            if (_wait != null)
                _wait.Set();
        }
    }
}

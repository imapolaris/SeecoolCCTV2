using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GatewayModels;
using GatewayModels.Param;
using GatewayNet;
using SocketHelper;

namespace GatewayNetService
{
    public class NetPipe : IDisposable
    {
        private string _id;
        public string Id { get { return _id; } }

        private SocketAdapter _adapter;
        public NetPipe(SocketAdapter adapter)
        {
            _id = Guid.NewGuid().ToString();
            _adapter = adapter;
            installEvent();
        }

        public void EnsureConnect()
        {
            _adapter.Send(new CodePacket(MessageCode.EnsureConnect).Serialize());
        }

        private void _adapter_Closed(object sender, EventArgs e)
        {
            uninstallEvent();
            onClosed();
        }

        private void _adapter_ErrorOccured(object sender, SocketHelper.Events.ErrorEventArgs args)
        {
            _adapter.Close();
        }

        private void _adapter_ReceiveCompleted(object sender, SocketHelper.Events.ReceiveEventArgs args)
        {
            if (args.ByteLenght >= 4)
            {
                int code = BitConverter.ToInt32(args.ReceivedBytes, 0);
                switch (code)
                {
                    case MessageCode.StartServer:
                        {
                            GatewayServer.StartServer();
                        }
                        break;
                    case MessageCode.StopServer:
                        {
                            GatewayServer.StopServer();
                        }
                        break;
                    case MessageCode.StartRegister:
                        {
                            StringPacket sp = StringPacket.DeserializeObject(args.ReceivedBytes);
                            GatewayServer.StartRegister(sp.Content);
                        }
                        break;
                    case MessageCode.StopRegister:
                        {
                            StringPacket sp = StringPacket.DeserializeObject(args.ReceivedBytes);
                            GatewayServer.StopRegister(sp.Content);
                        }
                        break;
                    case MessageCode.IsServerStarted:
                        {
                            send(new BoolPacket(code, GatewayServer.IsServerStarted()));
                        }
                        break;
                    case MessageCode.IsSuperiorOnline:
                        {
                            StringPacket sp = StringPacket.DeserializeObject(args.ReceivedBytes);
                            send(new BoolPacket(code, GatewayServer.IsSuperOnline(sp.Content)));
                        }
                        break;
                    case MessageCode.ShareDevice:
                        {
                            StringPacket sp = StringPacket.DeserializeObject(args.ReceivedBytes);
                            GatewayServer.ShareToPlatform(sp.Content);
                        }
                        break;
                }
            }
        }

        private void send(Packet p)
        {
            _adapter.Send(p.Serialize());
        }

        private void installEvent()
        {
            _adapter.Closed += _adapter_Closed;
            _adapter.ErrorOccured += _adapter_ErrorOccured;
            _adapter.ReceiveCompleted += _adapter_ReceiveCompleted;
        }

        private void uninstallEvent()
        {
            _adapter.Closed -= _adapter_Closed;
            _adapter.ErrorOccured -= _adapter_ErrorOccured;
            _adapter.ReceiveCompleted -= _adapter_ReceiveCompleted;
            _adapter = null;
        }

        #region 【事件】
        public event EventHandler Closed;
        private void onClosed()
        {
            Closed?.Invoke(this, EventArgs.Empty);
        }
        #endregion 【事件】

        #region 【实现IDisposable接口】
        bool _disposed = false;
        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
                if (disposing)
                {
                    if (_adapter != null)
                    {
                        uninstallEvent();
                    }
                }
                Closed = null;
            }
        }

        ~NetPipe()
        {
            Dispose(false);
        }
        #endregion 【实现IDisposable接口】
    }
}

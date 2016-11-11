using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Log;
using SocketHelper;

namespace GatewayNetService
{
    public class NetServer
    {
        SocketServer _server;
        private int _port;
        private Dictionary<string, NetPipe> _dictPipe;
        public NetServer(int port)
        {
            _dictPipe = new Dictionary<string, NetPipe>();
            _server = new SocketServer();
            _port = port;

            _server.ClientAccepted += _server_ClientAccepted;
            _server.ErrorOccured += _server_ErrorOccured;
        }

        public void Start()
        {
            _server.Listen(_port);
        }

        private void _server_ErrorOccured(object sender, SocketHelper.Events.ErrorEventArgs args)
        {
            Console.WriteLine(args.ErrorMessage);
            Logger.Default.Error(args.ErrorMessage, args.InnerException);
        }

        private void _server_ClientAccepted(object sender, SocketHelper.Events.ClientAcceptedEventArgs args)
        {
            NetPipe np = new NetPipe(args.Adapter);
            np.Closed += Np_Closed;
            _dictPipe[np.Id] = np;
            np.EnsureConnect();
        }

        private void Np_Closed(object sender, EventArgs e)
        {
            NetPipe np = sender as NetPipe;
            np.Closed -= Np_Closed;
            _dictPipe.Remove(np.Id);
            np.Dispose();
        }
    }
}

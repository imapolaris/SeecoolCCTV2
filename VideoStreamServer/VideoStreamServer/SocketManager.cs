using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Log;
using SocketHelper;

namespace VideoStreamServer
{
    internal class SocketManager
    {
        public static SocketManager Instance { get; private set; }
        static SocketManager()
        {
            Instance = new SocketManager();
        }

        public void Init()
        {
            //占位方法。
        }

        private SocketServer _server;
        private Dictionary<string, StreamPipe> _dictPipes = new Dictionary<string, StreamPipe>();
        private SocketManager()
        {
            var portStr = ConfigurationManager.AppSettings["ServerPort"];
            var port = int.Parse(portStr);
            _server = new SocketServer();
            _server.ClientAccepted += _server_ClientAccepted;
            _server.ErrorOccured += _server_ErrorOccured;
            _server.Listen(port);
        }

        private void _server_ErrorOccured(object sender, SocketHelper.Events.ErrorEventArgs args)
        {
            Logger.Default.Trace(args.ErrorMessage, args.InnerException);
        }

        private void _server_ClientAccepted(object sender, SocketHelper.Events.ClientAcceptedEventArgs args)
        {
            Console.WriteLine("客户端接入:" + args.Adapter.RemoteEndPoint);
            StreamPipe sp = new StreamPipe(args.Adapter);
            _dictPipes[sp.Id] = sp;
            sp.Abandoned += StreamPipe_Abandoned;
        }

        private void StreamPipe_Abandoned(object sender, EventArgs e)
        {
            StreamPipe sp = sender as StreamPipe;
            sp.Abandoned -= StreamPipe_Abandoned;
            _dictPipes.Remove(sp.Id);
        }
    }
}

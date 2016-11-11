using SocketHelper;
using SocketHelper.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVDownloadService
{
    public class DownloadSocketsManager
    {
        private SocketServer _server;
        public static DownloadSocketsManager Instance { get; private set; }

        static DownloadSocketsManager()
        {
            Instance = new DownloadSocketsManager();
        }
        List<DownloadSocketManager> _sockets = new List<DownloadSocketManager>();

        private DownloadSocketsManager()
        {
            startListen();
        }

        private void startListen()
        {
            stopListen();
            _server = new SocketServer();
            int port = -1;
            int.TryParse(System.Configuration.ConfigurationManager.AppSettings["DownloadPort"], out port);
            _server.Listen(port);
            _server.ClientAccepted += Server_ClientAccepted;
            _server.ErrorOccured += Server_ErrorOccured;
        }

        private void stopListen()
        {
            if (_server != null)
            {
                _server.ClientAccepted -= Server_ClientAccepted;
                _server.ErrorOccured -= Server_ErrorOccured;
                _server.StopListen();
                _server.Close();
            }
            _server = null;
        }
        
        private void Server_ErrorOccured(object sender, SocketHelper.Events.ErrorEventArgs args)
        {
            if (args.ErrorType == ErrorTypes.SocketAccept)
            {
                System.Threading.Thread.Sleep(5000);
                startListen();
            }
        }

        private void Server_ClientAccepted(object sender, SocketHelper.Events.ClientAcceptedEventArgs args)
        {
            string endpoint = args.Adapter.RemoteEndPoint.ToString();
            Console.WriteLine("ClientAccepted:" + endpoint);
            SocketAdapter adapter = args.Adapter;
            _sockets.RemoveAll(_=> _.Adapter == null);
            _sockets.Add(new DownloadSocketManager(adapter));
        }
    }
}

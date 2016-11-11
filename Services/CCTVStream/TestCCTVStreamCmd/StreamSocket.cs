using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace TestCCTVStreamCmd
{
    public class StreamSocket: IDisposable
    {
        Socket _socket;
        List<Socket> _connList = new List<Socket>();
        byte[] Header;
        bool _isRunning = true;
        public StreamSocket()
        {
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 8010);
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Bind(endpoint);
            _socket.Listen(16);
            _socket.BeginAccept(acceptCallback, _socket);
            //监听本地端口
            System.Console.WriteLine("开始监听本地端口:8010");
        }

        private void acceptCallback(IAsyncResult ar)
        {
            try
            {
                if (!_isRunning)
                    return;
                Socket s = ar.AsyncState as Socket;
                Socket wSocket = s.EndAccept(ar);
                _connList.Add(wSocket);
                if(Header != null)
                    wSocket.Send(Header);
            }
            catch (SocketException se)
            {
                Console.WriteLine(se);
            }
        }

        public void UpdateHeader(byte[] header)
        {
            Header = header;
            sendAll(header);
            Console.WriteLine("Header: " + BitConverter.ToString(header??new byte[0]));
        }

        public void UpdateStream(byte[] buffer)
        {
            //if (Header != null)
            {
                sendAll(buffer);
            }
        }

        void sendAll(byte[] buffer)
        {
            if (buffer == null)
                return;
            List<Socket> unconns = new List<Socket>();
            foreach (var s in _connList)
            {
                try
                {
                    s.Send(buffer);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    s.Dispose();
                    unconns.Add(s);
                }
            }

            foreach (var del in unconns)
            {
                _connList.Remove(del);
            }
        }

        public void Dispose()
        {
            _isRunning = false;
            if (_socket != null)
                _socket.Dispose();
            _socket = null;
            foreach (var sock in _connList)
            {
                sock.Close();
                sock.Dispose();
            }
        }
    }
}

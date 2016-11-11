using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SocketHelper;
using VideoStreamModels;
using VideoStreamModels.Model;

namespace TestServer
{
    class Program
    {
        static void Main(string[] args)
        {
            SocketClient sc = new SocketClient();
            sc.ConnectCompleted += Sc_ConnectCompleted;
            sc.ReceiveCompleted += Sc_ReceiveCompleted;
            sc.SendCompleted += Sc_SendCompleted;
            sc.ErrorOccured += Sc_ErrorOccured;
            Thread.Sleep(1000);
            sc.BeginConnect("127.0.0.1", 37010);
            Console.ReadLine();
        }

        private static void Sc_ErrorOccured(object sender, SocketHelper.Events.ErrorEventArgs args)
        {
            Console.WriteLine("error:{0}", args.ErrorMessage);
        }

        private static void Sc_SendCompleted(object sender, SocketHelper.Events.SendEventArgs args)
        {
        }

        private static void Sc_ReceiveCompleted(object sender, SocketHelper.Events.ReceiveEventArgs args)
        {
            if (args.ByteLenght >= 4)
            {
                int code = BitConverter.ToInt32(args.ReceivedBytes, 0);
                switch (code)
                {
                    case StreamEntityCode.FfmpegHeader:
                        {
                            Console.WriteLine("FFmpeg包头Received:" + (args.ByteLenght - 4));
                        }
                        break;
                    case StreamEntityCode.HikvHeader:
                        {
                            Console.WriteLine("Hikv包头Received:" + (args.ByteLenght - 4));
                        }
                        break;
                    case StreamEntityCode.StreamData:
                        {
                            Console.WriteLine("码流数据Received:" + (args.ByteLenght - 4));
                        }
                        break;
                }
            }
        }

        private static void Sc_ConnectCompleted(object sender, EventArgs e)
        {
            Console.WriteLine("连接成功!");
            //_hik = new CCTVStreamCmd.Hikvision.HikStream("192.168.9.155", 8000, "admin", "admin12345", 1, false, hwnd);
            string HikvUrl = "hikv://{0}:{1}@{2}:{3}/stream?channel={4}&profile={5}";
            string url = string.Format(HikvUrl, new object[] { "admin", "admin12345", "192.168.9.155", 8000, 1, "main" });
            StreamAddress sa = new StreamAddress(url, "127.0.0.1", 37010);
            SocketClient client = sender as SocketClient;
            client.Send(sa.Serialize());
        }
    }
}

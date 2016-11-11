using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Log;
using GatewayNet;
using GatewayNet.Lower;
using GatewayNet.Tools;

namespace GatewayNetService
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            ConsoleMonitor.Closed += ConsoleMonitor_Closed;

            string portStr = ConfigurationManager.AppSettings["GatewayPort"];
            int port = int.Parse(portStr);
            //NetServer server = new NetServer(port);
            //server.Start();
            GatewayServer.StartServer();
            Console.WriteLine("网关服务启动!");
            Logger.Default.Trace("网关服务启动!");
            while (true)
            {
                string line = Console.ReadLine();
                if (line.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    break;
                else if (line.Equals("clear", StringComparison.OrdinalIgnoreCase))
                    Console.Clear();
                else if (line.Equals("share", StringComparison.OrdinalIgnoreCase))
                    ResourceSharer.Instance.NotifyToPlatform("7aad6dba-819a-42f8-b712-3415d4aa228e");
            }
        }

        private static void ConsoleMonitor_Closed()
        {
            Logger.Default.Trace("网关服务关闭...!");
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Default.Error(e.ExceptionObject as Exception);
        }
    }
}

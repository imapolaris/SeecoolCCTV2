using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCTVModels;
using GatewayModels;
using GatewayNet.Lower;
using GatewayNet.Server;
using GatewayNet.Tools;

namespace GatewayNet
{
    public static class GatewayServer
    {
        public static void StartServer()
        {
            SipProxyWrapper.Instance.Start(); //启动服务。
            RegisterManager.Instance.Start();
        }

        public static void StopServer()
        {
            Console.WriteLine($"Stop Server:{DateTime.Now.ToString("s")} -----------");
            RegisterManager.Instance.Stop();
            SipProxyWrapper.Instance.Stop(); //停止服务。
        }

        public static bool IsServerStarted()
        {
            return SipProxyWrapper.Instance.HasStarted;
        }

        public static void StartRegister(string platformId)
        {
            Console.WriteLine($"Start Register:{platformId} -----------");
            RegisterManager.Instance.StartRegister(platformId);
        }

        public static void StopRegister(string platformId)
        {
            Console.WriteLine($"Stop Register:{platformId} -----------");
            RegisterManager.Instance.StopRegister(platformId);
        }

        public static bool IsSuperOnline(string platformId)
        {
            Register reg = RegisterManager.Instance.GetRegister(platformId);
            return reg != null && reg.IsAlive;
        }

        public static void ShareToPlatform(string platformId)
        {
            Console.WriteLine($"Share To Platform:{platformId} -----------");
            ResourceSharer.Instance.NotifyToPlatform(platformId);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GatewayNet.Util
{
    internal class IPAddressHelper
    {
        public static string CorrectIp(string ip)
        {
            if (ip.Equals("127.0.0.1") || ip.ToLower().Equals("localhost"))
            {
                return GetLocalIp();
            }
            return ip;
        }

        public static string GetLocalIp()
        {
            IPHostEntry he =Dns.GetHostEntry(Dns.GetHostName());
            foreach(IPAddress ip in he.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    return ip.ToString();
            }
            return "0.0.0.0";
        }
    }
}

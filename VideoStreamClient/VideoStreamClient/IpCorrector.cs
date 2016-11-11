using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace VideoStreamClient
{
    internal class IpCorrector
    {
        public static string CorrectIp(string ip)
        {
            if (ip.Equals("127.0.0.1") || ip.ToLower().Equals("localhost"))
            {
                return GetLocalIpV4();
            }
            return ip;
        }

        public static string GetLocalIpV4()
        {
            IPAddress[] host = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress ip in host)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return null;
        }
    }
}

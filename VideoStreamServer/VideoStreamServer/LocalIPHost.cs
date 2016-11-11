using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace VideoStreamServer
{
    internal class LocalIPHost
    {
        public static LocalIPHost Instance { get; private set; }
        static LocalIPHost()
        {
            Instance = new LocalIPHost();
        }

        private Dictionary<string, string> _dictIpIp;
        private LocalIPHost()
        {
            loadLocalIp();
        }

        private void loadLocalIp()
        {
            _dictIpIp = new Dictionary<string, string>();
            IPAddress[] host = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress ipa in host)
            {
                string ip = ipa.ToString();
                _dictIpIp[ip] = ip;
            }
        }

        public bool IsLocalIp(string ip)
        {
            if (ip.Equals("127.0.0.1"))
                return true;
            else if (ip.Equals("localhost", StringComparison.OrdinalIgnoreCase))
                return true;
            return ip != null && _dictIpIp.ContainsKey(ip);
        }
    }
}

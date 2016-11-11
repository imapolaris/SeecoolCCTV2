using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace GatewayNet.Util
{
    public static class PortUtils
    {
        public static int GetFreeRTPPort()
        {
            List<int> ports = getUsedPorts();
            int start = 5000;
            for (; start < 65535; start++)
            {
                if (!ports.Contains(start) && !ports.Contains(start + 1))
                    return start;
            }
            throw new IndexOutOfRangeException("无法找到一个可用的RTP端口");
        }

        private static List<int> getUsedPorts()
        {
            IPGlobalProperties gp = IPGlobalProperties.GetIPGlobalProperties();

            IPEndPoint[] tcps = gp.GetActiveTcpListeners();
            IPEndPoint[] udps = gp.GetActiveUdpListeners();
            TcpConnectionInformation[] tcpcs = gp.GetActiveTcpConnections();

            List<int> ports = new List<int>();
            foreach (IPEndPoint ep in tcps)
                ports.Add(ep.Port);
            foreach (IPEndPoint ep in udps)
                ports.Add(ep.Port);
            foreach (TcpConnectionInformation tci in tcpcs)
                ports.Add(tci.LocalEndPoint.Port);
            return ports;
        }
    }
}

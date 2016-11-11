using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GatewayNet.Util;

namespace GatewayNet.Lower
{
    public class RTPServerManager
    {
        private Dictionary<string, RTPServer> _rtpServers;

        internal RTPServerManager()
        {
            _rtpServers = new Dictionary<string, RTPServer>();
        }

        public RTPServer GetOrAddServer(string videoId)
        {
            if (_rtpServers.ContainsKey(videoId))
                return _rtpServers[videoId];
            else
            {
                int port = PortUtils.GetFreeRTPPort();
                RTPServer rs = new RTPServer(port, videoId);
                rs.TargetsCleared += Rs_TargetsCleared;
                _rtpServers[videoId] = rs;
                return rs;
            }
        }

        public void RemoveServer(string videoId)
        {
            if (_rtpServers.ContainsKey(videoId))
            {
                RTPServer rs = _rtpServers[videoId];
                rs.TargetsCleared -= Rs_TargetsCleared;
                rs.Dispose();
                _rtpServers.Remove(rs.VideoId);
            }
        }

        private void Rs_TargetsCleared(object sender, EventArgs e)
        {
            RTPServer rs = sender as RTPServer;
            rs.TargetsCleared -= Rs_TargetsCleared;
            rs.Dispose();
            _rtpServers.Remove(rs.VideoId);
        }

        public bool AddTarget(string videoId, string ip, int port)
        {
            if (_rtpServers.ContainsKey(videoId))
            {
                RTPServer rs = _rtpServers[videoId];
                rs.AddTarget(ip, port);
                if (!rs.Started)
                    rs.Start();
                return true;
            }
            return false;
        }

        public bool RemoveTarget(string videoId, string ip, int port)
        {
            if (_rtpServers.ContainsKey(videoId))
            {
                RTPServer rs = _rtpServers[videoId];
                rs.RemoveTarget(ip, port);
                return true;
            }
            return false;
        }

        public void RemoveTargets(string ip)
        {
            foreach (RTPServer rs in _rtpServers.Values.ToArray())
            {
                rs.RemoveTargets(ip);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GatewayNet.Server;
using LumiSoft.Net.SIP.Stack;

namespace GatewayNet.Session
{
    public class InviteHolderManager
    {
        private Dictionary<string, InviteHolder> _holders;
        private Dictionary<string, InviteSource> _targets;
        private SipProxyWrapper _sipServer;

        internal InviteHolderManager(SipProxyWrapper server)
        {
            _sipServer = server;
            _holders = new Dictionary<string, InviteHolder>();
            _targets = new Dictionary<string, InviteSource>();
        }

        public void PutInvite(SIP_RequestReceivedEventArgs e)
        {
            if (e.Request.RequestLine.Method.ToUpper().Equals(SIP_Methods.INVITE))
            {
                InviteHolder holder = new InviteHolder(_sipServer, e);
                holder.CommandTimeout += Holder_CommandTimeout;
                holder.Completed += Holder_Completed;
                _holders[holder.GUID] = holder;
                holder.Start();
            }
        }

        public void ByeInvite(SIP_RequestReceivedEventArgs e)
        {
            if (e.Request.RequestLine.Method.ToUpper().Equals(SIP_Methods.BYE))
            {
                if (_targets.ContainsKey(e.Request.CallID))
                {
                    InviteSource src = _targets[e.Request.CallID];
                    //停止发送。
                    _sipServer.RTPManager.RemoveTarget(src.VideoId, src.Ip, src.Port);
                    _targets.Remove(e.Request.CallID);
                }
            }
        }

        private void Holder_Completed(object sender, EventArgs e)
        {
            InviteHolder holder = sender as InviteHolder;
            remoteHolder(holder);
            //记录本次呼叫对应的接收端地址。
            _targets[holder.CallID] = new InviteSource(holder.VideoId, holder.RemoteIP, holder.RemotePort);
        }

        private void Holder_CommandTimeout(object sender, EventArgs e)
        {
            remoteHolder(sender as InviteHolder);
        }

        private void remoteHolder(InviteHolder holder)
        {
            holder.CommandTimeout -= Holder_CommandTimeout;
            holder.Completed -= Holder_Completed;
            _holders.Remove(holder.GUID);
        }

        class InviteSource
        {
            public InviteSource(string vId, string ip, int port)
            {
                VideoId = vId;
                Ip = ip;
                Port = port;
            }
            public string VideoId { get; set; }
            public string Ip { get; set; }
            public int Port { get; set; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GatewayNet.Lower;
using GatewayNet.Server;
using GatewayNet.Util;
using LumiSoft.Net;
using LumiSoft.Net.SDP;
using LumiSoft.Net.SIP.Message;
using LumiSoft.Net.SIP.Stack;

namespace GatewayNet.Session
{
    public class InviteHolder
    {
        private SipProxyWrapper _sipServer;
        private SDP_Message _sdp;
        private string _videoId;
        private Timer _timer;
        private SIP_RequestReceivedEventArgs _requestArgs;

        public string VideoId { get { return _videoId; } }
        public string CallID { get; private set; }
        public string RemoteIP { get; private set; } = "0.0.0.0";
        public int RemotePort { get; private set; } = 0;

        internal InviteHolder(SipProxyWrapper server, SIP_RequestReceivedEventArgs args)
        {
            GUID = Guid.NewGuid().ToString();
            _requestArgs = args;
            CallID = args.Request.CallID;
            _sipServer = server;
            server.Stack.RequestReceived += Stack_RequestReceived;
        }

        public void Start()
        {
            toResponse(_requestArgs);
            _timer = new Timer(timer_Callback, null, 20000, Timeout.Infinite);
        }

        private void timer_Callback(object state)
        {
            terminate();
            //从RTP服务中移除目标。
            if (_videoId != null)
            {
                _sipServer.RTPManager.RemoveTarget(_videoId, RemoteIP, RemotePort);
            }
            OnCommandTimeout();
        }

        private void terminate()
        {
            _sipServer.Stack.RequestReceived -= Stack_RequestReceived;
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void toResponse(SIP_RequestReceivedEventArgs e)
        {
            SIP_Uri uri = e.Request.RequestLine.Uri as SIP_Uri;
            try
            {
                _sdp = SDP_Message.Parse(MyEncoder.Encoder.GetString(e.Request.Data));
                RemoteIP = _sdp.Origin.UnicastAddress;
                if (_sdp.MediaDescriptions.Count == 0)
                {
                    e.ServerTransaction.SendResponse(_sipServer.Stack.CreateResponse(SIP_ResponseCodes.x400_Bad_Request, e.Request));
                    return;
                }
                else
                {
                    RemotePort = _sdp.MediaDescriptions[0].Port;
                }
            }
            catch (Exception)
            {
                //解析SDP失败。
                e.ServerTransaction.SendResponse(_sipServer.Stack.CreateResponse(SIP_ResponseCodes.x400_Bad_Request, e.Request));
                return;
            }

            //send 100 Trying;
            e.ServerTransaction.SendResponse(_sipServer.Stack.CreateResponse(SIP_ResponseCodes.x100_Trying, e.Request));

            _videoId = _sipServer.DeviceManager.GetVideoId(uri.User);
            if (_videoId != null)
            {
                RTPServer rtp = _sipServer.RTPManager.GetOrAddServer(_videoId);
                SDP_Message respSDP = new SDP_Message();
                respSDP.Version = "0";
                respSDP.Origin = new SDP_Origin(uri.User, 0, 0, "IN", "IPV4", rtp.LocalIP);
                respSDP.SessionName = "Play";
                respSDP.Connection = new SDP_Connection("IN", "IPV4", rtp.LocalIP);
                respSDP.SSRC = SDP_Utils.SSRC2String(SDP_Utils.GenSSRC(uri.User, true)); //根据国标补充协议标准生成SSRC。
                respSDP.Times.Add(new SDP_Time(0, 0));
                respSDP.MediaDescriptions.Add(new SDP_MediaDescription("video", rtp.Port, 2, "RTP/AVP", new string[] { "96", "97", "98" }));
                respSDP.Attributes.Add(new SDP_Attribute("sendonly", ""));
                respSDP.Attributes.Add(new SDP_Attribute("rtpmap", "96 PS/90000"));
                respSDP.Attributes.Add(new SDP_Attribute("rtpmap", "97 MPEG4/90000"));
                respSDP.Attributes.Add(new SDP_Attribute("rtpmap", "98 H264/90000"));

                SIP_Response resp = _sipServer.Stack.CreateResponse(SIP_ResponseCodes.x200_Ok, e.Request);
                resp.Data = respSDP.ToByte();
                e.ServerTransaction.SendResponse(resp);
            }
            else
            {
                //没有找到视频源。
                e.ServerTransaction.SendResponse(_sipServer.Stack.CreateResponse(SIP_ResponseCodes.x404_Not_Found, e.Request));
            }
        }

        private void Stack_RequestReceived(object sender, SIP_RequestReceivedEventArgs e)
        {
            if (CallID == e.Request.CallID)
            {
                if (e.Request.RequestLine.Method.ToUpper().Equals(SIP_Methods.ACK))
                {
                    _sipServer.RTPManager.AddTarget(_videoId, RemoteIP, RemotePort);
                    terminate();
                    OnCompleted();
                }
            }
        }

        public string GUID { get; private set; }

        #region 【事件定义】
        public event EventHandler CommandTimeout;
        private void OnCommandTimeout()
        {
            CommandTimeout?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler Completed;
        private void OnCompleted()
        {
            Completed?.Invoke(this, EventArgs.Empty);
        }
        #endregion 【事件定义】
    }
}

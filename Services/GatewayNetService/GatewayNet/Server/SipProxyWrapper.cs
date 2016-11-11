using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GatewayModels;
using GatewayNet.Lower;
using GatewayNet.Session;
using GatewayNet.Tools;
using GatewayNet.Util;
using GBTModels;
using GBTModels.Global;
using GBTModels.Query;
using GBTModels.Util;
using LumiSoft.Net;
using LumiSoft.Net.Log;
using LumiSoft.Net.SIP.Message;
using LumiSoft.Net.SIP.Proxy;
using LumiSoft.Net.SIP.Stack;

namespace GatewayNet.Server
{
    internal class SipProxyWrapper
    {
        public static SipProxyWrapper Instance { get; private set; }
        static SipProxyWrapper()
        {
            Instance = new SipProxyWrapper();
        }

        private const int Expires = 7200; //单位S
        private bool _started = false;
        //private SIP_Stack m_stack = null;
        private SIP_Proxy _sipProxy;
        private string _localAOR;
        private string _localIP;
        private CommandProcessor _processor;
        private VirtualDeviceManager _deviceMgr;
        private RTPServerManager _rtpManager;
        private InviteHolderManager _inviteMgr;
        private Timer _regTimer;

        public bool HasStarted { get { return _started; } }
        public SIP_Stack Stack { get { return _sipProxy.Stack; } }
        public SIP_Proxy Proxy { get { return _sipProxy; } }
        public VirtualDeviceManager DeviceManager { get { return _deviceMgr; } }
        public RTPServerManager RTPManager { get { return _rtpManager; } }
        private SipProxyWrapper()
        {
            _processor = new CommandProcessor();
            _deviceMgr = new VirtualDeviceManager(this);
            _rtpManager = new RTPServerManager();
            _inviteMgr = new InviteHolderManager(this);

            _sipProxy = new SIP_Proxy(new SIP_Stack());
            _sipProxy.Stack.Realm = "seecool"; //本地域，当需要验证远程注册用户的时候，发送此域。
            _sipProxy.Stack.UserAgent = "NetGateway www.seecool.cc"; //必须设置。
            _sipProxy.Stack.Error += M_stack_Error;
            _sipProxy.Stack.RequestReceived += M_stack_RequestReceived;
            _sipProxy.Stack.Logger.WriteLog += Logger_WriteLog;
            _sipProxy.ProxyMode = SIP_ProxyMode.Registrar | SIP_ProxyMode.Statefull | SIP_ProxyMode.B2BUA;
            _sipProxy.AddressExists += SipProxy_AddressExists; //验证地址。
            _sipProxy.Authenticate += SipProxy_Authenticate; //验证授权
            _sipProxy.Registrar.CanRegister += Registrar_CanRegister;
            _sipProxy.IsLocalUri += SipProxy_IsLocalUri;
            _sipProxy.Stack.ValidateRequest += Stack_ValidateRequest;
        }

        private void Stack_ValidateRequest(object sender, SIP_ValidateRequestEventArgs e)
        {

        }

        private bool SipProxy_IsLocalUri(string uri)
        {
            if (uri == _localIP)
                return true;
            Register[] regs = RegisterManager.Instance.AllRegister;
            foreach (Register r in regs)
            {
                if (r.Platform.Ip == uri)
                    return true;
            }
            return false;
        }

        private bool Registrar_CanRegister(string userName, string address)
        {
            return false;
        }

        private void SipProxy_Authenticate(SIP_AuthenticateEventArgs e)
        {
            return;
        }

        private bool SipProxy_AddressExists(string address)
        {
            address = address.ToLower();
            //如果是当前server，无需从已注册平台中判断。
            if (address == _localAOR)
                return true;
            Platform plat;
            return RegisterManager.Instance.IsAliveUpperPlatform(address, out plat);
        }

        public void Start()
        {
            if (!_started)
            {
                _localIP = IPAddressHelper.GetLocalIp();
                Gateway gw = InfoService.Instance.CurrentGateway; //当前网关配置。
                _localAOR = $"{gw.SipNumber}@{_localIP}";

                //m_stack = new SIP_Stack();
                //m_stack.Realm = "seecool";　//本地域，当需要验证远程注册用户的时候，发送此域。
                //m_stack.UserAgent = "NetGateway www.seecool.cc";//必须设置。
                //m_stack.Error += M_stack_Error;
                //m_stack.RequestReceived += M_stack_RequestReceived;
                //m_stack.ResponseReceived += M_stack_ResponseReceived;
                //m_stack.Logger.WriteLog += Logger_WriteLog;

                //用户注册授权凭证，向远程服务器注册本机时需要。
                //m_stack.Credentials.Add(new NetworkCredential("admin", "admin", "seecool"));

                //添加本地端口绑定
                IPBindInfo[] bindInfo = new IPBindInfo[]{
                    new IPBindInfo(_localIP,BindInfoProtocol.UDP,IPAddress.Parse(_localIP),gw.Port),
                    new IPBindInfo(_localIP,BindInfoProtocol.TCP,IPAddress.Parse(_localIP),gw.Port)
                };

                Stack.BindInfo = bindInfo;
                Stack.Start();
                addVirtualServer();
                _deviceMgr.Start();
                _started = true;
            }
        }

        public void Stop()
        {
            if (_started)
            {
                _deviceMgr.Stop();
                Stack.Stop();
                _started = false;
            }
        }

        private void addVirtualServer()
        {
            Gateway gw = InfoService.Instance.CurrentGateway; //当前网关配置。
            SIP_t_ContactParam[] contacts = new SIP_t_ContactParam[1];

            SIP_t_ContactParam c = new SIP_t_ContactParam();
            StringReader sr = new StringReader($"<sip:{gw.SipNumber}@{_localIP}:{gw.Port}>;expires={Expires};qvalue=1.00");
            c.Parse(sr);
            contacts[0] = c;

            string aor = $"{gw.SipNumber}@{_localIP}";
            _sipProxy.Registrar.SetRegistration(aor, contacts);

            if (_regTimer == null)
                _regTimer = new System.Threading.Timer(timer_Callback, null, (int)(Expires * 0.9) * 1000, Timeout.Infinite);
            else
                _regTimer.Change((int)(Expires * 0.9) * 1000, Timeout.Infinite);
        }

        private void timer_Callback(object state)
        {
            addVirtualServer();
        }

        private void Logger_WriteLog(object sender, WriteLogEventArgs e)
        {
            if (e.LogEntry.Data != null)
            {
                try
                {
                    Console.WriteLine("");
                    Console.WriteLine($"====================={DateTime.Now.ToString("s")}");
                    string text = MyEncoder.Encoder.GetString(e.LogEntry.Data);
                    Console.WriteLine(text);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void M_stack_RequestReceived(object sender, SIP_RequestReceivedEventArgs e)
        {
            //=====================================//
            //一般用于处理【上】级域收到的请求，但并不局限。
            //=====================================//
            SIP_Uri uri = e.Request.RequestLine.Uri as SIP_Uri;
            //只在此处处理非转发请求，转发请求由协议栈自动处理。
            if (uri != null && uri.Host == _localIP)
            {
                SIP_Uri from = e.Request.From.Address.Uri as SIP_Uri;
                //如果是针对当前服务本身的Request
                if (uri.Address == _localAOR)
                {
                    if (e.Request.RequestLine.Method.Equals(SIP_Methods.MESSAGE, StringComparison.OrdinalIgnoreCase))
                    {

                        //判断请求源设备是否是当前平台的有效上级。
                        Platform plat;
                        if (RegisterManager.Instance.IsAliveUpperPlatform(from.Address, out plat))
                        {
                            e.ServerTransaction.SendResponse(Stack.CreateResponse(SIP_ResponseCodes.x200_Ok, e.Request));
                            if (e.Request.Data != null)
                            {
                                string xml = MyEncoder.Encoder.GetString(e.Request.Data);
                                _processor.Process(from.Address, xml);
                            }
                        }
                        else
                        {
                            e.ServerTransaction.SendResponse(Stack.CreateResponse(SIP_ResponseCodes.x403_Forbidden, e.Request));
                        }
                    }
                }
                else if (e.Request.RequestLine.Method.ToUpper().Equals(SIP_Methods.INVITE))
                {
                    //处理一次Invite请求。
                    _inviteMgr.PutInvite(e);
                }
                else if (e.Request.RequestLine.Method.ToUpper().Equals(SIP_Methods.BYE))
                {
                    _inviteMgr.ByeInvite(e);
                }
            }
        }

        private void M_stack_Error(object sender, ExceptionEventArgs e)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GatewayModels;
using GatewayNet.Server;
using GatewayNet.Util;
using GBTModels.Notify;
using GBTModels.Util;
using LumiSoft.Net;
using LumiSoft.Net.SIP.Message;
using LumiSoft.Net.SIP.Stack;

namespace GatewayNet.Lower
{
    public class Register : IDisposable
    {
        private string _localIp;
        private Gateway _gateway;
        private Platform _platform;
        private SIP_UA_Registration _regi;
        private bool _registered = false;
        private bool _alive = false;
        //private Thread _tdAlive;
        //private Thread _tdRegister;
        private Timer _timerRegi;
        private Timer _timerAlive;

        internal Register(Gateway gw, Platform pf, string localIp)
        {
            _gateway = gw;
            _platform = pf;
            _localIp = localIp;
        }

        public Gateway Gateway { get { return _gateway; } }
        public Platform Platform { get { return _platform; } }
        public bool HasRegistered { get { return _registered; } }
        public bool IsAlive { get { return _alive; } }

        public void BeginRegister()
        {
            if (_disposed)
                throw new ObjectDisposedException(this.GetType().Name, "不能执行注册操作。");
            if (_regi != null)
            {
                _regi.Dispose();
            }

            string localSn = _gateway.SipNumber;
            int localPort = _gateway.Port;

            string remoteSn = _platform.SipNumber;
            int remotePort = _platform.Port;
            string remoteIP = _platform.Ip;


            string user = _platform.UserName;
            string pwd = _platform.Password;
            string realm = _platform.Realm;

            SIP_Stack stack = SipProxyWrapper.Instance.Stack;
            SIP_t_NameAddress from = new SIP_t_NameAddress($"sip:{localSn}@{_localIp}:{localPort}");
            SIP_t_NameAddress to = new SIP_t_NameAddress($"sip:{localSn}@{_localIp}:{localPort}");

            SIP_Uri server = new SIP_Uri()
            {
                Host = remoteIP,
                Port = remotePort,
                User = remoteSn
            };
            string aor = $"{localSn}@{_localIp}:{localPort}";
            AbsoluteUri contact = AbsoluteUri.Parse($"sip:{localSn}@" + stack.BindInfo[0].EndPoint.ToString());
            SIP_UA_Registration regi = stack.CreateRegistration(server, aor, contact, 7200);
            _regi = regi; //记录

            regi.Credential = new NetworkCredential(user, pwd, realm);
            regi.Registered += Regi_Registered;
            regi.StateChanged += Regi_StateChanged;

            if (_timerRegi == null)
                _timerRegi = new Timer(timerRegi_Callback, null, 21000, Timeout.Infinite);
            else
                _timerRegi.Change(21000, Timeout.Infinite);
            regi.BeginRegister(true);
        }

        private void timerRegi_Callback(object state)
        {
            try
            {
                BeginRegister();
            }
            catch (ObjectDisposedException) { }
        }

        private void Regi_StateChanged(object sender, EventArgs e)
        {
            SIP_UA_Registration reg = sender as SIP_UA_Registration;
            if (reg != null && (reg.State == SIP_UA_RegistrationState.Registered || reg.State == SIP_UA_RegistrationState.Unregistered))
            {
                if (_timerRegi != null)
                    _timerRegi.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        public void BeginUnregister()
        {
            if (_regi != null)
            {
                _regi.BeginUnregister(true);
                _regi.Registered -= Regi_Registered;
                _regi.StateChanged -= Regi_StateChanged;
                _regi.Dispose();
                _regi = null;

                if (_timerAlive != null)
                    _timerAlive.Change(Timeout.Infinite, Timeout.Infinite); //关闭心跳定时器。
                _registered = false;
                _alive = false;
            }
        }

        private void Regi_Registered(object sender, EventArgs e)
        {
            if (_disposed)
                return;
            _registered = true;
            _alive = true;
            _aliveCount = 0;
            //启动线程。
            if (_timerAlive == null)
                _timerAlive = new Timer(timerAlive_Callback, null, 20000, 20000);
            else
                _timerAlive.Change(20000, 20000);
        }

        private int _aliveCount = 0;
        private SIP_RequestSender _sender;
        private void timerAlive_Callback(object state)
        {
            //资源已释放，停止心跳。
            if (_disposed || !_registered)
                return;
            if (_aliveCount >= 3)
            {
                //重新注册。
                _registered = false;
                _alive = false;
                _timerAlive.Change(Timeout.Infinite, Timeout.Infinite); //关闭心跳定时器。
                SipProxyWrapper.Instance.RTPManager.RemoveTargets(Platform.Ip);
                closeSender();
                BeginRegister();
                return;
            }

            try
            {
                Gateway gw = _gateway;
                Platform pf = _platform;
                SIP_Stack stack = SipProxyWrapper.Instance.Stack;

                KeepAlive ka = new KeepAlive()
                {
                    DeviceID = gw.SipNumber
                };
                string body = SerializeHelper.Instance.Serialize(ka);

                SIP_t_NameAddress from = new SIP_t_NameAddress($"sip:{gw.SipNumber}@{_localIp}:{gw.Port}");
                SIP_t_NameAddress to = new SIP_t_NameAddress($"sip:{pf.SipNumber}@{pf.Ip}:{pf.Port}");

                SIP_Request message = stack.CreateRequest(SIP_Methods.MESSAGE, to, from);
                message.ContentType = "Application/MANSCDP+xml";
                message.Data = MyEncoder.Encoder.GetBytes(body);

                closeSender();
                _sender = stack.CreateRequestSender(message);
                _sender.ResponseReceived += Send_ResponseReceived;
                _sender.Start();
            }
            catch (Exception e)
            {
                Common.Log.Logger.Default.Error($"心跳指令发送失败,平台:{Platform.Name}:{Platform.SipNumber}", e);
            }
            _aliveCount++;
        }

        private void closeSender()
        {
            if (_sender != null)
            {
                _sender.ResponseReceived -= Send_ResponseReceived;
                _sender.Dispose();
                _sender = null;
            }
        }

        private void Send_ResponseReceived(object sender, SIP_ResponseReceivedEventArgs e)
        {
            SIP_RequestSender send = sender as SIP_RequestSender;
            if (e.Response.StatusCodeType == SIP_StatusCodeType.Success)
            {
                _aliveCount--;
                send.ResponseReceived -= Send_ResponseReceived;
            }
        }

        #region 【实现IDisposable接口】
        private bool _disposed = false;
        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;

                if (_timerAlive != null)
                {
                    _timerAlive.Change(Timeout.Infinite, Timeout.Infinite);
                    _timerAlive.Dispose();
                    _timerAlive = null;
                }
                if (_timerRegi != null)
                {
                    _timerRegi.Change(Timeout.Infinite, Timeout.Infinite);
                    _timerRegi.Dispose();
                    _timerRegi = null;
                }

                if (disposing)
                {
                    if (_registered)
                    {
                        BeginUnregister();
                    }
                }
                if (_regi != null)
                {
                    _regi.Dispose();
                    _regi = null;
                }
            }
        }

        ~Register()
        {
            Dispose(false);
        }
        #endregion 【实现IDisposable接口】
    }
}

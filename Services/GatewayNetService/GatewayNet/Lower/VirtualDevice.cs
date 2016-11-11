using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CCTVModels;
using GatewayModels;
using GatewayNet.Util;
using LumiSoft.Net;
using LumiSoft.Net.SIP.Message;
using LumiSoft.Net.SIP.Proxy;

namespace GatewayNet.Lower
{
    public class VirtualDevice : IDisposable
    {
        private const int Expires = 7200;//单位s
        private CCTVStaticInfo _realInfo;
        private string _sipNum;
        private Gateway _gateway;
        private SIP_Registrar _registrar;
        private Timer _regTimer;
        private string _localAOR;
        private bool _started = false;

        public bool Started { get { return _started; } }
        public string LocalAOR { get { return _localAOR; } }
        public string VideoId { get { return _realInfo.VideoId; } }
        public string DeviceId { get { return _sipNum; } }

        internal VirtualDevice(CCTVStaticInfo real, Gateway gw, string sipNum, SIP_Registrar _reg)
        {
            _realInfo = real;
            _sipNum = sipNum;
            _gateway = gw;
            _registrar = _reg;
            string localIP = IPAddressHelper.GetLocalIp();
            _localAOR = $"{sipNum}@{localIP}";
        }

        internal void Start()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(this.GetType().Name);
            if (!_started)
            {
                register();
                _started = true;
            }
        }

        internal void Stop()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(this.GetType().Name);
            if (_started)
            {
                _regTimer.Change(Timeout.Infinite, Timeout.Infinite);
                _registrar.DeleteRegistration(_localAOR);
                _started = false;
            }
        }

        private void register()
        {
            SIP_t_ContactParam[] contacts = new SIP_t_ContactParam[1];

            SIP_t_ContactParam c = new SIP_t_ContactParam();
            StringReader sr = new StringReader($"<sip:{_localAOR}:{_gateway.Port}>;expires={Expires};qvalue=1.00");
            c.Parse(sr);
            contacts[0] = c;

            _registrar.SetRegistration(_localAOR, contacts);

            if (_regTimer == null)
                _regTimer = new Timer(timer_Callback, null, (int)(Expires * 0.9) * 1000, Timeout.Infinite);
            else
                _regTimer.Change((int)(Expires * 0.9) * 1000, Timeout.Infinite);
        }

        private void timer_Callback(object state)
        {
            register();
        }

        #region 【实现IDsiposalbe接口】
        public bool IsDisposed { get; private set; } = false;
        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                Stop();
                IsDisposed = true;
            }
        }
        ~VirtualDevice()
        {
            Dispose(false);
        }
        #endregion 【实现IDsiposalbe接口】
    }
}

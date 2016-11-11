using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GatewayModels;
using GatewayNet.Server;
using GatewayNet.Tools;
using GatewayNet.Util;

namespace GatewayNet.Lower
{
    internal class RegisterManager
    {
        public static RegisterManager Instance { get; private set; }
        static RegisterManager()
        {
            Instance = new RegisterManager();
        }

        private string _localIp;
        private Dictionary<string, Register> _dictRegister;
        public Register[] AllRegister { get { return _dictRegister.Values.ToArray(); } }
        private RegisterManager()
        {
            _dictRegister = new Dictionary<string, Register>();
            _localIp = IPAddressHelper.GetLocalIp();
        }

        public void Start()
        {
            foreach (Platform plat in InfoService.Instance.GetAllPlatformSuper())
            {
                startRegister(plat);
            }
        }

        public void Stop()
        {
            ClearRegisters();
        }

        public Register GetRegister(string platformId)
        {
            if (_dictRegister.ContainsKey(platformId))
                return _dictRegister[platformId];
            return null;
        }

        public void StartRegister(string platformId)
        {
            Platform pf = InfoService.Instance.GetPlatform(platformId);
            startRegister(pf);
        }

        private void startRegister(Platform pf)
        {
            Gateway gw = InfoService.Instance.CurrentGateway;
            if (pf != null)
            {
                //避免修改了内存实例。
                pf = new Platform(pf);
                gw = new Gateway(gw);

                Register reg = GetRegister(pf.Id);
                if (reg == null)
                {
                    createRegister(pf.Id, gw, pf);
                }
                else
                {
                    if (!(reg.Gateway.Equals(gw) && reg.Platform.Equals(pf)))
                    {
                        reg.Dispose();
                        _dictRegister.Remove(pf.Id);
                        createRegister(pf.Id, gw, pf);
                    }
                }
            }
        }

        private void createRegister(string platformId, Gateway gw, Platform pf)
        {
            Register reg = new Register(gw, pf, _localIp);
            _dictRegister[platformId] = reg;
            reg.BeginRegister();
        }

        public void StopRegister(string platformId)
        {
            Register reg = GetRegister(platformId);
            if (reg != null)
            {
                reg.Dispose();
                _dictRegister.Remove(platformId);
                SipProxyWrapper.Instance.RTPManager.RemoveTargets(reg.Platform.Ip);
            }
        }

        private void ClearRegisters()
        {
            foreach (string key in _dictRegister.Keys.ToArray())
            {
                Register reg = _dictRegister[key];
                reg.Dispose();
            }
            _dictRegister.Clear();
        }

        /// <summary>
        /// 判断指定的平台地址是否是当前平台的已联通上级域。
        /// </summary>
        /// <param name="aor">平台地址</param>
        /// <returns></returns>
        public bool IsAliveUpperPlatform(string aor, out Platform plat)
        {
            plat = null;
            foreach (Register reg in _dictRegister.Values)
            {
                if (reg.IsAlive)
                {
                    string pAor = $"{reg.Platform.SipNumber}@{reg.Platform.Ip}";
                    if (aor == pAor)
                    {
                        plat = reg.Platform;
                        return true;
                    }
                }
            }
            return false;
        }

        public Platform GetPlatformByAOR(string aor)
        {
            foreach (Register reg in _dictRegister.Values)
            {
                if (reg.IsAlive)
                {
                    string pAor = $"{reg.Platform.SipNumber}@{reg.Platform.Ip}";
                    if (aor == pAor)
                    {
                        return reg.Platform;
                    }
                }
            }
            return null;
        }
    }
}

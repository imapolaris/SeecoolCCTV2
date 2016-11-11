using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CCTVModels;
using GatewayModels;
using GatewayModels.Util;
using GatewayNet.Server;
using GatewayNet.Tools;
using LumiSoft.Net.SIP.Proxy;

namespace GatewayNet.Lower
{
    /// <summary>
    /// 虚拟设备注册管理。
    /// </summary>
    public class VirtualDeviceManager
    {
        private const int Interval = 25000;
        private Dictionary<string, VirtualDevice> _allDevices;
        private List<string> _allAOR;
        private bool _started;
        private Timer _refreshTimer;
        private object _locker = new object();
        private SipProxyWrapper _sipServer;

        internal VirtualDeviceManager(SipProxyWrapper server)
        {
            _sipServer = server;
            _allDevices = new Dictionary<string, VirtualDevice>();
            _allAOR = new List<string>();
        }

        internal void Start()
        {
            lock (_locker)
            {
                if (!_started)
                {
                    IEnumerable<CCTVStaticInfo> infos = InfoService.Instance.GetAllStaticInfo();
                    addDevices(infos);

                    if (_refreshTimer == null)
                        _refreshTimer = new Timer(refresh_Callback, null, Interval, Timeout.Infinite);
                    else
                        _refreshTimer.Change(Interval, Timeout.Infinite);

                    _started = true;
                }
            }
        }

        internal void Stop()
        {
            lock (_locker)
            {
                if (_started)
                {
                    if (_refreshTimer != null)
                    {
                        _refreshTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    }

                    _allAOR.Clear();
                    foreach (string key in _allDevices.Keys)
                    {
                        _allDevices[key].Stop();
                    }
                    _allDevices.Clear();
                    _started = false;
                }
            }
        }

        internal bool IsVirtualAOR(string aor)
        {
            return _allAOR.Contains(aor);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceId">虚拟设备的域标识。</param>
        /// <returns></returns>
        internal string GetVideoId(string deviceId)
        {
            foreach (VirtualDevice vd in _allDevices.Values.ToArray())
            {
                if (vd.DeviceId == deviceId)
                    return vd.VideoId;
            }
            return null;
        }

        private void addDevices(IEnumerable<CCTVStaticInfo> infos)
        {
            Gateway gw = InfoService.Instance.CurrentGateway;
            if (infos != null && infos.Count() > 0)
            {
                Dictionary<string, string> dictIdPairs = new Dictionary<string, string>();
                IEnumerable<SipIdMap> dids = InfoService.Instance.GetAllSipIdMap();
                if (dids != null)
                {
                    foreach (SipIdMap sp in dids)
                    {
                        dictIdPairs[sp.StaticId] = sp.SipNumber;
                    }
                }
                foreach (CCTVStaticInfo si in infos)
                {
                    string sip = null;
                    if (dictIdPairs.ContainsKey(si.VideoId))
                        sip = dictIdPairs[si.VideoId];
                    else
                    {
                        sip = SipIdGenner.GenDeviceID();
                        InfoService.Instance.PutSipIdMap(si.VideoId, new SipIdMap(si.VideoId, sip), false);
                    }

                    VirtualDevice vd = new VirtualDevice(si, gw, sip, _sipServer.Proxy.Registrar);
                    _allDevices[si.VideoId] = vd;
                    _allAOR.Add(vd.LocalAOR);
                    vd.Start();
                }
            }
        }

        private void refresh_Callback(object state)
        {
            lock (_locker)
            {
                IEnumerable<CCTVStaticInfo> infos = InfoService.Instance.GetAllStaticInfo();
                List<string> delKeys = new List<string>();
                if (infos != null)
                {
                    //首先添加新的设备注册。
                    List<CCTVStaticInfo> newSI = new List<CCTVStaticInfo>();
                    foreach (CCTVStaticInfo si in infos)
                    {
                        if (!_allDevices.ContainsKey(si.VideoId))
                            newSI.Add(si);
                    }
                    addDevices(newSI);
                    //再删除旧设备。
                    List<string> ids = infos.Select(si => si.VideoId).ToList();
                    foreach (string key in _allDevices.Keys)
                        if (!ids.Contains(key))
                            delKeys.Add(key);
                }
                else
                {
                    delKeys.AddRange(_allDevices.Keys);
                }
                //删除旧的虚拟设备注册。
                foreach (string key in delKeys)
                {
                    _allAOR.Remove(_allDevices[key].LocalAOR);
                    _allDevices[key].Dispose();
                    _allDevices.Remove(key);
                    //移除链接到该设备的流传输通道。
                    _sipServer.RTPManager.RemoveServer(key);
                }
                _refreshTimer.Change(Interval, Timeout.Infinite);
            }
        }
    }
}

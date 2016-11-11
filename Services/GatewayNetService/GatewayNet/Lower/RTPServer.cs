using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CCTVStreamCmd;
using GatewayNet.H264;
using GatewayNet.Tools;
using GatewayNet.Util;
using LumiSoft.Net.RTP;
using Seecool.VideoStreamBase;
using VideoStreamClient;

namespace GatewayNet.Lower
{
    public class RTPServer : IDisposable
    {
        private const int VideoRate = 90000;
        private string _localIP;
        private int _localPort;
        private string _videoId;
        private VideoSource _vSoure;
        private UnpackPS _ups;
        private RTPPackHelper _rtpHelper;
        private RTP_MultimediaSession _multiSession;
        private RTP_SendStream _sendStream;
        private RTPHeaderTrigger _hTrigger;
        private bool _hasNewTarget = false;
        private Dictionary<string, TargetItem> _targets;

        public string LocalIP { get { return _localIP; } }
        public int Port { get { return _localPort; } }
        public bool Started { get; private set; } = false;
        public string VideoId { get { return _videoId; } }

        internal RTPServer(int localPort, string videoId)
        {
            _rtpHelper = new RTPPackHelper();
            _hTrigger = new RTPHeaderTrigger();
            _ups = new UnpackPS();
            _ups.Unpacked += _ups_Unpacked;

            _localIP = IPAddressHelper.GetLocalIp();
            _localPort = localPort;
            _videoId = videoId;
            _targets = new Dictionary<string, TargetItem>();
        }

        private void _ups_Unpacked(object arg1, PSFragment psf)
        {
            List<Nalu> nList = _rtpHelper.ToRTPPayload(psf);
            uint timestamp = _multiSession.Sessions[0].RtpClock.RtpTimestamp;
            //判断是否有新的接收客户端加入。
            if (_hasNewTarget)
            {
                _hTrigger.SendSPS(_sendStream, timestamp);
                _hTrigger.SendPPS(_sendStream, timestamp);
                _hasNewTarget = true;
            }
            for (int i = 0; i < nList.Count; i++)
            {
                _hTrigger.Update(nList[i]);
                RTP_Packet packet = new RTP_Packet();
                packet.Timestamp = timestamp;
                packet.Data = nList[i].NaluBytes();
                if (psf.IsFrameEnd && i == nList.Count - 1)
                    packet.IsMarker = true;
                else
                    packet.IsMarker = false;
                _sendStream.Send(packet);
            }
            //判断是否需要冗余发送SPS和PPS。
            if (_hTrigger.IsSPSTimeout)
                _hTrigger.SendSPS(_sendStream, timestamp);
            if (_hTrigger.IsPPSTimeout)
                _hTrigger.SendPPS(_sendStream, timestamp);
        }

        public void Start()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(this.GetType().Name);
            if (!Started)
            {
                _hTrigger.Init();
                _multiSession = new RTP_MultimediaSession(RTP_Utils.GenerateCNAME());
                RTP_Session session = _multiSession.CreateSession(new RTP_Address(IPAddress.Parse(_localIP), _localPort, _localPort + 1), new RTP_Clock(0, VideoRate));
                session.Payload = RTP_PayloadTypes.H264;
                session.Start();
                _sendStream = session.CreateSendStream();

                _vSoure = VideoSourceCreator.Instance.GetVideoSource(_videoId);
                if (_targets.Count > 0)
                {
                    foreach (string key in _targets.Keys)
                    {
                        TargetItem ti = _targets[key];
                        session.AddTarget(new RTP_Address(ti.IP, ti.Port, ti.Port + 1));
                    }
                    startPlay();
                }
                Started = true;
            }
        }

        public void Stop()
        {
            if (Started)
            {
                stopPlay();
                _sendStream.Close();
                _multiSession.Close("User Close");
                _multiSession = null;
                Started = false;
            }
        }

        public void AddTarget(string ip, int port)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(this.GetType().Name);
            TargetItem ti = new TargetItem(ip, port);
            if (!_targets.ContainsKey(ti.Key))
            {
                _targets[ti.Key] = ti;
                if (Started)
                {
                    _multiSession.Sessions[0].AddTarget(new RTP_Address(ti.IP, ti.Port, ti.Port + 1));
                    _hasNewTarget = true;
                    startPlay();
                }
            }
        }

        public void RemoveTarget(string ip, int port)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(this.GetType().Name);
            TargetItem ti = new TargetItem(ip, port);
            _targets.Remove(ti.Key);
            if (Started)
            {
                _multiSession.Sessions[0].RemoveTarget(new RTP_Address(ti.IP, ti.Port, ti.Port + 1));
            }
            if (_targets.Count == 0)
            {
                stopPlay();
                OnTargetsCleared();
            }
        }

        public void RemoveTargets(string ip)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(this.GetType().Name);
            foreach (string key in _targets.Keys.ToArray())
            {
                if (key.StartsWith(ip))
                {
                    TargetItem ti = _targets[key];
                    _targets.Remove(key);
                    if (Started)
                    {
                        _multiSession.Sessions[0].RemoveTarget(new RTP_Address(ti.IP, ti.Port, ti.Port + 1));
                    }
                }
            }
            if (_targets.Count == 0)
            {
                stopPlay();
                OnTargetsCleared();
            }
        }

        public void ClearTargets()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(this.GetType().Name);
            if (Started)
            {
                foreach (string key in _targets.Keys)
                {
                    TargetItem ti = _targets[key];
                    _multiSession.Sessions[0].RemoveTarget(new RTP_Address(ti.IP, ti.Port, ti.Port + 1));
                }
            }
            _targets.Clear();
            stopPlay();
            OnTargetsCleared();
        }

        private bool _playAlive = false;
        private void startPlay()
        {
            if (!_playAlive)
            {
                if (_vSoure != null)
                    _vSoure.Hikm4StreamReceived += _vSoure_Hikm4StreamReceived;
                _playAlive = true;
            }
        }

        private object _locker = new object();
        private void _vSoure_Hikm4StreamReceived(object sender, VideoStreamClient.Events.HikM4StreamEventArgs e)
        {
            lock (_locker)
            {
                _ups.UpdateStandardStream(e.Package.Data);
            }
        }

        private void stopPlay()
        {
            if (_playAlive)
            {
                if (_vSoure != null)
                    _vSoure.Hikm4StreamReceived -= _vSoure_Hikm4StreamReceived;
                _playAlive = false;
            }
        }
        #region 【事件定义】
        public event EventHandler TargetsCleared;
        private void OnTargetsCleared()
        {
            TargetsCleared?.Invoke(this, EventArgs.Empty);
        }
        #endregion 【事件定义】

        #region 【实现IDisposable接口】
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
                _targets.Clear();
                TargetsCleared = null;
                IsDisposed = true;
            }
        }

        ~RTPServer()
        {
            Dispose(false);
        }
        #endregion 【实现IDisposable接口】

        class TargetItem
        {
            public TargetItem(string ip, int port)
            {
                Port = port;
                IP = IPAddress.Parse(ip);
            }
            public string Key { get { return $"{IP.ToString()}:{Port}"; } }
            public IPAddress IP { get; set; }
            public int Port { get; set; }
        }
    }
}

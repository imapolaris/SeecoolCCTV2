using CCTVClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CCTVInfoHub;
using CCTVInfoHub.Entity;
using CCTVModels;

namespace VideoStreamClient
{
    public class VideoSourceManager
    {

        public static VideoSourceManager CreateInstance(string staticInfoBaseAddress)
        {
            return new VideoSourceManager(staticInfoBaseAddress);
        }

        /// <summary>
        /// 创建一个视频数据统一管理对象实例，由外界传入同步服务实例。
        /// </summary>
        /// <param name="clientHub"></param>
        /// <returns></returns>
        public static VideoSourceManager CreateInstance(CCTVDefaultInfoSync clientHub)
        {
            return new VideoSourceManager(clientHub);
        }

        private CCTVDefaultInfoSync _clientHub;
        private CCTVInfo _cctvInfo;
        private ConcurrentDictionary<string, VideoSource> _videoSourceDict;
        private bool _autoGenHub = false;
        private CCTVServerInfo _currentServer;

        private VideoSourceManager()
        {
            _videoSourceDict = new ConcurrentDictionary<string, VideoSource>();
        }

        private VideoSourceManager(string staticInfoBaseAddress)
            : this()
        {
            _clientHub = new CCTVDefaultInfoSync(staticInfoBaseAddress);
            _clientHub.RegisterDefaultWithoutUpdate(CCTVInfoType.GlobalInfo);
            _clientHub.RegisterDefaultWithoutUpdate(CCTVInfoType.StaticInfo);
            _clientHub.RegisterDefaultWithoutUpdate(CCTVInfoType.ServerInfo);
            _clientHub.RegisterDefaultWithoutUpdate(CCTVInfoType.DeviceInfo);
            _autoGenHub = true;
            findCurrentServer();
        }

        /// <summary>
        /// 生成视频数据统一管理对象，由外界传入同步服务实例。
        /// </summary>
        /// <param name="clientHub"></param>
        private VideoSourceManager(CCTVDefaultInfoSync clientHub)
            : this()
        {
            if (clientHub == null)
                throw new ArgumentNullException("参数 " + nameof(clientHub) + " 不能为空值null");
            _clientHub = clientHub;
            if (!clientHub.HasRegisteredDefault(CCTVInfoType.GlobalInfo))
                _clientHub.RegisterDefaultWithoutUpdate(CCTVInfoType.GlobalInfo);
            if (!clientHub.HasRegisteredDefault(CCTVInfoType.StaticInfo))
                _clientHub.RegisterDefaultWithoutUpdate(CCTVInfoType.StaticInfo);
            if (!clientHub.HasRegisteredDefault(CCTVInfoType.ServerInfo))
                _clientHub.RegisterDefaultWithoutUpdate(CCTVInfoType.ServerInfo);
            if (!clientHub.HasRegisteredDefault(CCTVInfoType.DeviceInfo))
                _clientHub.RegisterDefaultWithoutUpdate(CCTVInfoType.DeviceInfo);
            findCurrentServer();
        }

        private void findCurrentServer()
        {
            //使用Infoservice的IP和端口来
            Uri uri = new Uri(_clientHub.BaseAddress);
            string ip = IpCorrector.CorrectIp(uri.Host);
            int port = uri.Port;

            CCTVServerInfo[] sis = _clientHub.GetAllServerInfo();
            foreach (CCTVServerInfo si in sis)
            {
                if (ip.Equals(si.InfoServiceIp) && port == si.InfoServicePort)
                {
                    _currentServer = new CCTVServerInfo()
                    {
                        ServerId = si.ServerId,
                        Name = si.Name,
                        InfoServiceIp = si.InfoServiceIp,
                        InfoServicePort = si.InfoServicePort,
                        StreamServerIp = si.StreamServerIp,
                        StreamServerPort = si.StreamServerPort,
                        ControlServerIp = si.ControlServerIp,
                        ControlServerPort = si.ControlServerPort
                    };
                    break;
                }
            }
            if (_currentServer == null)
            {
                _currentServer = new CCTVServerInfo()
                {
                    ServerId = Guid.NewGuid().ToString(),
                    Name = "独立信息服务",
                    InfoServiceIp = ip,
                    InfoServicePort = port,
                    StreamServerIp = ip,
                    StreamServerPort = 37010,
                    ControlServerIp = ip,
                    ControlServerPort = 47010
                };
            }
        }

        private CCTVInfo getCCTVInfo()
        {
            if (_autoGenHub)
                _clientHub.UpdateDefault(CCTVInfoType.GlobalInfo);
            CCTVGlobalInfo gInfo = _clientHub.GetGlobalInfo();
            if (gInfo != null)
            {
                if (_cctvInfo == null || _cctvInfo.ServerHost != gInfo.CCTV1Host)
                {
                    if (_cctvInfo != null)
                        _cctvInfo.Stop();
                    _cctvInfo = new CCTVInfo(gInfo.CCTV1Host);
                    _cctvInfo.Start();
                }
            }
            return _cctvInfo;
        }

        /// <summary>
        /// 获取一个视频流数据源。
        /// </summary>
        /// <param name="videoId">视频ID,如:CCTV1_50BAD15900010302</param>
        /// <param name="url">
        /// 视频URL，如:cctv1://192.168.9.222/5817192048884777730?stream=1
        /// </param>
        /// <returns></returns>
        public VideoSource GetVideoSource(string videoId, string url)
        {
            Uri uri = new Uri(url);
            if (uri.Scheme.ToLower() == "cctv1")
            {
                if (_videoSourceDict.ContainsKey(url))
                {
                    VideoSource vs = _videoSourceDict[url];
                    if (vs.IsDisposed)
                    {
                        vs = new CCTV1VideoSource(_clientHub, getCCTVInfo(), videoId, url);
                        _videoSourceDict[url] = vs;
                    }
                    return vs;
                }
                else
                {
                    VideoSource vs = new CCTV1VideoSource(_clientHub, getCCTVInfo(), videoId, url);
                    _videoSourceDict[url] = vs;
                    return vs;
                }
            }
            else
            {
                CCTVServerInfo preferServer = getPreferServer(videoId);
                if (_videoSourceDict.ContainsKey(url))
                {
                    VideoSource vs = _videoSourceDict[url];
                    if (vs.IsDisposed)
                    {
                        vs = new CCTV2VideoSource(_currentServer, preferServer, videoId, url);
                        _videoSourceDict[url] = vs;
                    }
                    return vs;
                }
                else
                {
                    VideoSource vs = new CCTV2VideoSource(_currentServer, preferServer, videoId, url);
                    _videoSourceDict[url] = vs;
                    return vs;
                }
            }
        }

        public VideoSource GetVideoSource(string videoId, int streamId)
        {
            _clientHub.UpdateDefault(CCTVInfoType.StaticInfo);
            CCTVStaticInfo info = _clientHub.GetStaticInfo(videoId);
            var streamInfo = info.Streams.FirstOrDefault(_ => _.Index == streamId);
            if (streamInfo != null)
                return GetVideoSource(videoId, streamInfo.Url);
            return null;
        }

        public VideoSource GetVideoSource(string videoId)
        {
            _clientHub.UpdateDefault(CCTVInfoType.StaticInfo);
            CCTVStaticInfo info = _clientHub.GetStaticInfo(videoId);
            var streamInfo = info.Streams.OrderBy(x => x.Index).First();
            if (streamInfo != null)
                return GetVideoSource(videoId, streamInfo.Url);
            return null;
        }

        private CCTVServerInfo getPreferServer(string videoId)
        {
            CCTVServerInfo preferServer = _currentServer;
            CCTVDeviceInfo di = _clientHub.GetDeviceInfo(videoId);
            if (di != null && !string.IsNullOrWhiteSpace(di.PreferredServerId))
            {
                CCTVServerInfo temp = _clientHub.GetServerInfo(di.PreferredServerId);
                if (temp != null && !string.IsNullOrWhiteSpace(temp.StreamServerIp))
                {
                    preferServer = temp;
                }
            }
            return preferServer;
        }
    }
}

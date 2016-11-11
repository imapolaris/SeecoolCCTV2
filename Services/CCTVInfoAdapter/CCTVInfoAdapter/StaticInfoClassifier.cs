using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCTVClient;
using CCTVModels;
using StaticInfoClient;

namespace CCTVInfoAdapter
{
    public class StaticInfoClassifier
    {
        private Dictionary<string, CCTVServerInfo> _servers = new Dictionary<string, CCTVServerInfo>();
        private Dictionary<string, CCTVStaticInfo> _statics = new Dictionary<string, CCTVStaticInfo>();
        private Dictionary<string, CCTVCameraLimits> _cameras = new Dictionary<string, CCTVCameraLimits>();
        private Dictionary<string, CCTVDeviceInfo> _devices = new Dictionary<string, CCTVDeviceInfo>();
        private Dictionary<string, CCTVControlConfig> _controls = new Dictionary<string, CCTVControlConfig>();
        private Dictionary<string, CCTVVideoTrack> _videoTracks = new Dictionary<string, CCTVVideoTrack>();
        private Dictionary<string, CCTVOnlineStatus> _onlineStatus = new Dictionary<string, CCTVOnlineStatus>();

        public Dictionary<string, CCTVServerInfo> Servers
        {
            get { return _servers; }
        }

        public Dictionary<string, CCTVStaticInfo> StaticInfos
        {
            get { return _statics; }
        }

        public Dictionary<string, CCTVCameraLimits> Cameras
        {
            get { return _cameras; }
        }

        public Dictionary<string, CCTVDeviceInfo> Devices
        {
            get { return _devices; }
        }

        public Dictionary<string, CCTVControlConfig> Controls
        {
            get { return _controls; }
        }

        public Dictionary<string, CCTVVideoTrack> VideoTracks
        {
            get { return _videoTracks; }
        }

        public Dictionary<string, CCTVOnlineStatus> OnlineStatus
        {
            get { return _onlineStatus; }
        }

        private CCTVGlobalInfo _global;
        private StaticInfoSynchronizer<VideoPosition> _vpSync;
        public StaticInfoClassifier(CCTVGlobalInfo global, StaticInfoSynchronizer<VideoPosition> vpSync)
        {
            _global = global;
            _vpSync = vpSync;
        }

        public void Classify(VideoParser.Node node)
        {
            classifyInfo(node, null);
        }

        private void classifyInfo(VideoParser.Node node, string serverId)
        {
            //在线标识。
            var nodeId = getNodeId(node.Id);
            _onlineStatus[nodeId] = new CCTVOnlineStatus() { VideoId = nodeId, Online = node.Online };

            VideoParser.Server server = node as VideoParser.Server;
            if (server != null)
            {
                CCTVServerInfo si = new CCTVServerInfo()
                {
                    ServerId = getNodeId(node.Id),
                    Name = node.Name,
                    InfoServiceIp = node.Host,
                    InfoServicePort = 27010,
                    StreamServerIp = node.Host,
                    StreamServerPort = 37010,
                    ControlServerIp = node.Host,
                    ControlServerPort = 47010
                };
                _servers[si.ServerId] = si;
                if (server.Childs != null)
                    foreach (VideoParser.Node child in server.Childs)
                        classifyInfo(child, si.ServerId);
            }
            else
            {
                VideoParser.Front front = node as VideoParser.Front;
                if (front != null)
                {
                    if (front.Childs != null)
                    {
                        int channel = 1;
                        foreach (VideoParser.Video child in front.Childs)
                        {
                            classifyInfo(front, child, channel, serverId);
                            if (front.Type == 1 || front.Type == 11)
                                channel += 4;
                            else
                                channel++;
                        }
                    }
                }
            }
        }

        private void classifyInfo(VideoParser.Front front, VideoParser.Video video, int channel, string serverId)
        {
            //在线标识。
            var videoId = getNodeId(video.Id);
            if (video.DvrChannel > 0)
                channel = video.DvrChannel;

            _onlineStatus[videoId] = new CCTVOnlineStatus() { VideoId = videoId, Online = video.Online };

            CCTVStaticInfo info = new CCTVStaticInfo()
            {
                VideoId = videoId,
                Name = video.Name,
                ImageType = video.HighDef ? ImageType.HighDef : ImageType.Unknow,
                Platform = CCTVPlatformType.CCTV1,
                Streams = getStreamInfos(video, channel),
            };
            if (video.PanTiltUnit != null && video.PanTiltUnit.Trackable)
            {
                var ptz = video.PanTiltUnit;
                info.Latitude = ptz.Latitude;
                info.Longitude = ptz.Longitude;
                info.Altitude = ptz.Altitude;
                //生成摄像机象限信息
                CCTVCameraLimits camera = new CCTVCameraLimits()
                {
                    VideoId = info.VideoId,
                    UpLimit = ptz.UpLimit,
                    DownLimit = ptz.DownLimit,
                    LeftLimit = ptz.LeftLimit,
                    RightLimit = ptz.RightLimit,
                    MaxViewPort = ptz.WideView,
                    MinViewPort = ptz.TeleView
                };
                _cameras[camera.VideoId] = camera;
            }
            else
            {
                VideoPosition videoPosition;
                if (_vpSync.TryGetValue(info.VideoId, out videoPosition))
                {
                    info.Latitude = videoPosition.Latitude;
                    info.Longitude = videoPosition.Longitude;
                    info.Altitude = videoPosition.Altitude;
                    info.Heading = videoPosition.Heading;
                    info.ViewPort = videoPosition.ViewPort;
                }
            }
            _statics[info.VideoId] = info;

            //视频跟踪
            var vTrackHost = ImageTrackInfoProvider.Instance.GetImageTrackHost(video.Id);
            if (!string.IsNullOrWhiteSpace(vTrackHost))
            {
                var vTrack = new CCTVVideoTrack()
                {
                    Ip = vTrackHost,
                    RpcPort = 8068,
                    SubPort = 8061,
                    VideoId = info.VideoId
                };
                _videoTracks[vTrack.VideoId] = vTrack;
            }


            //生成设备信息。
            CCTVDeviceInfo di = new CCTVDeviceInfo()
            {
                VideoId = info.VideoId,
                DeviceType = DeviceType.HikIP,
                PreferredServerId = serverId,
                User = front.User,
                Password = front.Pass,
                Port = front.Port
            };
            //大于33的认为是IP直连。
            if (channel >= 33)
                di.Ip = video.Host;
            else
                di.Ip = front.Host;
            _devices[di.VideoId] = di;

            //TODO:生成控制信息。徐测试以此方法生成是否可行。
            CCTVControlConfig cc = new CCTVControlConfig()
            {
                VideoId = info.VideoId,
                Type = getControlType(video.PanTiltUnit),
                Ip = di.Ip,
                Port = di.Port,
                Channel = channel,
                AuxSwitch = getAuxSwitch(video.PanTiltUnit)
            };
            _controls[cc.VideoId] = cc;
        }

        private CCTVControlType getControlType(VideoParser.Video._tagPanTiltUnit ptz)
        {
            if (ptz == null)
                return CCTVControlType.UnControl;
            bool ctrl = ptz.EightDirections | ptz.FourDirections | ptz.Focus
                | ptz.Zoom | ptz.Preset | ptz.Trackable;
            return ctrl ? CCTVControlType.DVR : CCTVControlType.UnControl;
        }

        static string getNodeId(ulong id)
        {
            return $"CCTV1_{id:X}";
        }

        private SwitchInfo[] getAuxSwitch(VideoParser.Video._tagPanTiltUnit panTiltUnit)
        {
            if (panTiltUnit == null)
                return CCTVControlConfig.EmptySwitches;
            else
                return panTiltUnit.AuxSwitchs.Select(x => new SwitchInfo() { Index = x.Index, Name = x.Name }).ToArray();
        }

        private StreamInfo[] getStreamInfos(VideoParser.Video video, int channel)
        {
            List<StreamInfo> streamList = new List<StreamInfo>();
            if (video != null)
            {
                int index = 0;
                var flags = video.Flags;
                if (video.D1Avail | video.HighDef)
                {
                    string name = video.HighDef ? "高清" : "标清";
                    streamList.Add(createStreamInfo(index++, name, 1, video.Id, channel));
                }

                if (flags.HasFlag(VideoParser.Video.ParamFlags.H264))
                    streamList.Add(createStreamInfo(index++, "普清", 2, video.Id, channel));

                if (flags.HasFlag(VideoParser.Video.ParamFlags.Mpeg4))
                    streamList.Add(createStreamInfo(index++, video.HighDef ? "标清" : "流畅", 3, video.Id, channel));
            }
            return streamList.ToArray();
        }

        private StreamInfo createStreamInfo(int index, string name, int stream, ulong videoId, int channel)
        {
            return new StreamInfo()
            {
                Index = index,
                Name = name,
                Channel = channel,
                Url = $"cctv1://{_global.CCTV1Host}/{videoId}?stream={stream}",
                StreamType = stream == 1 ? StreamType.Main : StreamType.Sub
            };
        }
    }
}

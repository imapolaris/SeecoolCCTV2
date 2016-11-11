using CCTVClient;
using StaticInfoClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace CCTVInfoAdapterOld
{
    [Obsolete]
    public class CCTV1Adapter : IDisposable
    {
        Common.Logging.ILog _log { get { return Common.Logging.LogManager.GetLogger(GetType()); } }

        CCTVInfo _info;
        string _baseUri;
        CCTVGlobalInfo _globalInfo;

        ThumbnailProducer _thumbnailProducer;

        StaticInfoSynchronizer<CCTVGlobalInfo> _globalSync;
        StaticInfoSynchronizer<CCTVStaticInfo> _staticSync;
        StaticInfoSynchronizer<CCTVDynamicInfo> _dynamicSync;
        StaticInfoSynchronizer<HierarchyInfo> _hierarchySync;
        StaticInfoSynchronizer<OnlineStatus> _onlineSync;
        StaticInfoSynchronizer<VideoPosition> _videoPosSync;
        StaticInfoSynchronizer<VideoBuffering> _videoBufSync;

        public CCTV1Adapter(string host, string infoServiceBaseUri)
        {
            _baseUri = infoServiceBaseUri;
            _globalSync = new StaticInfoSynchronizer<CCTVGlobalInfo>(_baseUri, "CCTVGlobal", TimeSpan.FromSeconds(5));
            _staticSync = new StaticInfoSynchronizer<CCTVStaticInfo>(_baseUri, "CCTVStatic", TimeSpan.FromSeconds(5));
            _dynamicSync = new StaticInfoSynchronizer<CCTVDynamicInfo>(_baseUri, "CCTVDynamic", TimeSpan.FromSeconds(5));
            _hierarchySync = new StaticInfoSynchronizer<HierarchyInfo>(_baseUri, "CCTVHierarchy.Default", TimeSpan.FromSeconds(5));
            _onlineSync = new StaticInfoSynchronizer<OnlineStatus>(_baseUri, "CCTVOnlineStatus", TimeSpan.FromSeconds(5));
            _videoPosSync = new StaticInfoSynchronizer<VideoPosition>(_baseUri, "VideoPosition", TimeSpan.FromSeconds(5));
            _videoBufSync = new StaticInfoSynchronizer<VideoBuffering>(_baseUri, "VideoBuffering", TimeSpan.FromHours(1));

            _globalInfo = new CCTVGlobalInfo()
            {
                CCTV1Host = host,
            };
            _globalSync.ReplaceAll((new CCTVGlobalInfo[] { _globalInfo }).ToDictionary(x => "Default"));

            _info = new CCTVInfo(host);
            _thumbnailProducer = new ThumbnailProducer(_info);
            _thumbnailProducer.ThumbnailEvent += _thumbnailProducer_ThumbnailEvent;

            _info.ConnectedEvent += _info_ConnectedEvent;
            _info.DisconnectedEvent += _info_DisconnectedEvent;
            _info.AuthenticationEvent += _info_AuthenticationEvent;
            _info.NodeTreeEvent += _info_NodeTreeEvent;
            _info.RealtimeInfoEvent += _info_RealtimeInfoEvent;
            _info.Start();

            var bufferInfo = VideoBufferingInfoProvider.Instance.GetVideoBufferingInfo();
            _videoBufSync.ReplaceAll(bufferInfo.ToDictionary(x => getKey(x)));
        }

        private static string getKey(VideoBuffering buffering)
        {
            return $"{buffering.Id}_{buffering.StreamIndex}";
        }

        private static byte[] getImageBytes(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Jpeg);
                ms.Flush();
                return ms.ToArray();
            }
        }

        private void _thumbnailProducer_ThumbnailEvent(ulong videoId, Image thumbnail)
        {
            ThumbnailInfo ti = new ThumbnailInfo()
            {
                VideoId = getNodeId(videoId),
                Time = DateTime.Now,
                ImageBytes = getImageBytes(thumbnail)
            };

            InfoItem item = new InfoItem()
            {
                Key = ti.VideoId,
                IsDeleted = false,
                Info = StaticInfoSynchronizer<ThumbnailInfo>.SerializeItem(ti)
            };

            string section = $"Thumbnail/{getSection(ti.VideoId)}";
            using (var client = new StaticInfoClient.StaticInfoClient(_baseUri, section))
                client.PutUpdate(new InfoItem[] { item });
        }

        private static string getSection(string videoId)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(videoId);
            string base64 = Convert.ToBase64String(bytes);
            return base64.Replace('/', '-');
        }

        private void _info_ConnectedEvent()
        {
            _log.Info($"Info socket to {_globalInfo.CCTV1Host} connected.");
        }

        private void _info_DisconnectedEvent()
        {
            _log.Info($"Info socket to {_globalInfo.CCTV1Host} disconnected.");
        }

        private void _info_AuthenticationEvent(bool success)
        {
            _log.Info($"Info socket authentication result: {success}.");
        }

        private void _info_NodeTreeEvent(VideoParser.Node tree, string xml)
        {
            updateHierarchy(tree);
            updateStatic(tree);
            updateOnline(tree);
        }

        private void updateOnline(VideoParser.Node tree)
        {
            if (tree != null)
            {
                Dictionary<string, OnlineStatus> onlineDict = new Dictionary<string, OnlineStatus>();
                getOnlineStatus(onlineDict, tree);
                _onlineSync.ReplaceAll(onlineDict);
            }
        }

        private void getOnlineStatus(Dictionary<string, OnlineStatus> onlineDict, VideoParser.Node node)
        {
            string nodeId = getNodeId(node.Id);
            onlineDict[nodeId] = new OnlineStatus() { NodeId = nodeId, Online = node.Online };

            VideoParser.Server server = node as VideoParser.Server;
            if (server != null)
            {
                if (server.Childs != null)
                    foreach (VideoParser.Node child in server.Childs)
                        getOnlineStatus(onlineDict, child);
            }
            else
            {
                VideoParser.Front front = node as VideoParser.Front;
                if (front != null)
                {
                    if (front.Childs != null)
                        foreach (VideoParser.Video child in front.Childs)
                            getOnlineStatus(onlineDict, child);
                }
            }
        }

        private void getOnlineStatus(Dictionary<string, OnlineStatus> onlineDict, VideoParser.Video video)
        {
            string videoId = getNodeId(video.Id);
            onlineDict[videoId] = new OnlineStatus() { NodeId = videoId, Online = video.Online };
        }

        private void updateStatic(VideoParser.Node tree)
        {
            if (tree != null)
            {
                Dictionary<string, CCTVStaticInfo> staticDict = new Dictionary<string, CCTVStaticInfo>();
                getStaticInfo(staticDict, tree);
                _staticSync.ReplaceAll(staticDict);
            }
        }

        private void getStaticInfo(Dictionary<string, CCTVStaticInfo> staticDict, VideoParser.Node node)
        {
            VideoParser.Server server = node as VideoParser.Server;
            if (server != null)
            {
                if (server.Childs != null)
                    foreach (VideoParser.Node child in server.Childs)
                        getStaticInfo(staticDict, child);
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
                            getStaticInfo(staticDict, front, child, channel);
                            if (front.Type == 1 || front.Type == 11)
                                channel += 4;
                            else
                                channel++;
                        }
                    }
                }
            }
        }

        private void getStaticInfo(Dictionary<string, CCTVStaticInfo> staticDict, VideoParser.Front front, VideoParser.Video video, int channel)
        {
            CCTVStaticInfo info = new CCTVStaticInfo()
            {
                VideoId = getNodeId(video.Id),
                Name = video.Name,
                IP = front.Host,
                HighDef = video.HighDef,
                PanTilt = video.PanTiltUnit != null && (video.PanTiltUnit.EightDirections || video.PanTiltUnit.FourDirections),
                Zoom = video.PanTiltUnit != null && video.PanTiltUnit.Zoom,
                AuxSwitch = getAuxSwitch(video.PanTiltUnit),
                VideoAnalyze = video.VideoAnalyze,
                TargetTrack = video.PanTiltUnit != null && video.PanTiltUnit.Trackable,
                Streams = getStreamInfos(video),
            };
            info.ImageTrackHost = ImageTrackInfoProvider.Instance.GetImageTrackHost(video.Id);
            info.ImageTrack = !string.IsNullOrEmpty(info.ImageTrackHost);
            if (video.DvrChannel > 0)
                channel = video.DvrChannel;
            info.DvrChannelInfo = getDvrChannelInfo(front, channel);

            if (info.TargetTrack && video.PanTiltUnit != null)
            {
                var ptz = video.PanTiltUnit;
                info.Latitude = ptz.Latitude;
                info.Longitude = ptz.Longitude;
                info.Altitude = ptz.Altitude;
                info.TrackInfo = new TrackVideoInfo()
                {
                    UpLimit = ptz.UpLimit,
                    DownLimit = ptz.DownLimit,
                    LeftLimit = ptz.LeftLimit,
                    RightLimit = ptz.RightLimit,
                    MaxViewPort = ptz.WideView,
                    MinViewPort = ptz.TeleView,
                };
            }
            else
            {
                VideoPosition videoPosition;
                if (_videoPosSync.TryGetValue(info.VideoId, out videoPosition))
                {
                    info.Latitude = videoPosition.Latitude;
                    info.Longitude = videoPosition.Longitude;
                    info.Altitude = videoPosition.Altitude;
                    info.Heading = videoPosition.Heading;
                    info.ViewPort = videoPosition.ViewPort;
                }
            }

            staticDict[info.VideoId] = info;
        }

        private DVRChannelInfo getDvrChannelInfo(VideoParser.Front front, int channel)
        {
            var type = getDvrType(front.Type);
            if (type == DVRChannelInfo.DVRType.Unknown)
                return null;
            else
                return new DVRChannelInfo()
                {
                    Type = type,
                    Host = front.Host,
                    Port = front.Port,
                    User = front.User,
                    Pass = front.Pass,
                    Channel = channel,
                };
        }

        static DVRChannelInfo.DVRType getDvrType(int type)
        {
            switch (type)
            {
                case 1:
                case 4:
                case 6:
                case 7:
                    return DVRChannelInfo.DVRType.HikVision;
                case 8:
                case 9:
                case 10:
                case 11:
                    return DVRChannelInfo.DVRType.USNT;
                default:
                    return DVRChannelInfo.DVRType.Unknown;
            }
        }

        private SwitchInfo[] getAuxSwitch(VideoParser.Video._tagPanTiltUnit panTiltUnit)
        {
            if (panTiltUnit == null)
                return CCTVStaticInfo.EmptySwitches;
            else
                return panTiltUnit.AuxSwitchs.Select(x => new SwitchInfo() { Index = x.Index, Name = x.Name }).ToArray();
        }

        private StreamInfo[] getStreamInfos(VideoParser.Video video)
        {
            List<StreamInfo> streamList = new List<StreamInfo>();
            if (video != null)
            {
                int index = 0;
                var flags = video.Flags;
                if (video.D1Avail | video.HighDef)
                {
                    string name = video.HighDef ? "高清" : "标清";
                    streamList.Add(createStreamInfo(index++, name, 1, video.Id));
                }

                if (flags.HasFlag(VideoParser.Video.ParamFlags.H264))
                    streamList.Add(createStreamInfo(index++, "普清", 2, video.Id));

                if (flags.HasFlag(VideoParser.Video.ParamFlags.Mpeg4))
                    streamList.Add(createStreamInfo(index++, video.HighDef ? "标清" : "流畅", 3, video.Id));
            }
            return streamList.ToArray();
        }

        private StreamInfo createStreamInfo(int index, string name, int stream, ulong videoId)
        {
            return new StreamInfo()
            {
                Index = index,
                Name = name,
                Url = $"cctv1://{_globalInfo.CCTV1Host}/{videoId}?stream={stream}",
            };
        }

        private void updateHierarchy(VideoParser.Node tree)
        {
            if (tree != null)
            {
                Dictionary<string, HierarchyInfo> hierarchyDict = new Dictionary<string, HierarchyInfo>();
                getHierarchyInfo(hierarchyDict, null, tree);
                _hierarchySync.ReplaceAll(hierarchyDict);
            }
        }

        static string getNodeId(ulong id)
        {
            return $"CCTV1_{id:X}";
        }

        static string getNodeType(VideoParser.Node node)
        {
            if (node is VideoParser.Server)
                return "Server";
            else if (node is VideoParser.Front)
                return "Front";
            else
                return "Node";
        }

        private void getHierarchyInfo(Dictionary<string, HierarchyInfo> hierarchyDict, string parentId, VideoParser.Node node)
        {
            string nodeId = getNodeId(node.Id);
            hierarchyDict[nodeId] = new HierarchyInfo()
            {
                NodeId = nodeId,
                Name = node.Name,
                Type = getNodeType(node),
                ParentId = parentId,
            };

            VideoParser.Server server = node as VideoParser.Server;
            if (server != null)
            {
                if (server.Childs != null)
                    foreach (VideoParser.Node child in server.Childs)
                        getHierarchyInfo(hierarchyDict, nodeId, child);
            }
            else
            {
                VideoParser.Front front = node as VideoParser.Front;
                if (front != null)
                {
                    if (front.Childs != null)
                        foreach (VideoParser.Video child in front.Childs)
                            getHierarchyInfo(hierarchyDict, nodeId, child);
                }
            }
        }

        private void getHierarchyInfo(Dictionary<string, HierarchyInfo> hierarchyDict, string parentId, VideoParser.Video video)
        {
            string videoId = getNodeId(video.Id);
            hierarchyDict[videoId] = new HierarchyInfo()
            {
                NodeId = videoId,
                Name = video.Name,
                Type = "Video",
                ParentId = parentId,
            };
        }

        ConcurrentDictionary<string, CCTVDynamicInfo> _dynamicInfoDict = new ConcurrentDictionary<string, CCTVDynamicInfo>();

        CCTVDynamicInfo getDynamicInfo(string id)
        {
            return _dynamicInfoDict.GetOrAdd(id, x => new CCTVDynamicInfo() { VideoId = x });
        }

        DateTime _lastUpdateTime = DateTime.Now;
        List<ObjectItem<CCTVDynamicInfo>> _dynamicItemList = new List<ObjectItem<CCTVDynamicInfo>>();

        private void _info_RealtimeInfoEvent(VideoParser.Camera camera, VideoParser.GPS gps)
        {
            CCTVDynamicInfo dynamicInfo = getCameraInfo(camera);
            if (dynamicInfo == null)
                dynamicInfo = getGpsInfo(gps);
            if (dynamicInfo != null)
            {
                ObjectItem<CCTVDynamicInfo> item = new ObjectItem<CCTVDynamicInfo>()
                {
                    Key = dynamicInfo.VideoId,
                    IsDeleted = false,
                    Item = dynamicInfo,
                };
                _dynamicItemList.Add(item);
                if (DateTime.Now - _lastUpdateTime > TimeSpan.FromSeconds(0.5))
                {
                    _lastUpdateTime = DateTime.Now;
                    _dynamicSync.PutUpdate(_dynamicItemList);
                    _dynamicItemList.Clear();
                }
            }
        }

        CCTVDynamicInfo getCameraInfo(VideoParser.Camera camera)
        {
            if (camera != null)
            {
                CCTVDynamicInfo dynamicInfo = getDynamicInfo(getNodeId(camera.Id));
                dynamicInfo.Heading = camera.Pointing.Pan;
                CCTVStaticInfo staticInfo;
                if (_staticSync.TryGetValue(dynamicInfo.VideoId, out staticInfo) && staticInfo.TrackInfo != null)
                {
                    double zoom = Math.Min(1, Math.Max(0, camera.Zoom));
                    dynamicInfo.ViewPort = (1 - zoom) * (staticInfo.TrackInfo.MaxViewPort - staticInfo.TrackInfo.MinViewPort) + staticInfo.TrackInfo.MinViewPort;
                }
                return dynamicInfo;
            }

            return null;
        }

        CCTVDynamicInfo getGpsInfo(VideoParser.GPS gps)
        {
            if (gps != null)
            {
                CCTVDynamicInfo dynamicInfo = getDynamicInfo(getNodeId(gps.Id));
                dynamicInfo.Latitude = gps.Latitude;
                dynamicInfo.Longitude = gps.Longitude;
                dynamicInfo.SOG = gps.SOG;
                dynamicInfo.COG = gps.COG;
                return dynamicInfo;
            }

            return null;
        }

        public void Dispose()
        {
            _info.Stop();
            _thumbnailProducer.ThumbnailEvent -= _thumbnailProducer_ThumbnailEvent;
            _thumbnailProducer.Dispose();

            _globalSync.Dispose();
            _staticSync.Dispose();
            _dynamicSync.Dispose();
            _hierarchySync.Dispose();
            _onlineSync.Dispose();
            _videoPosSync.Dispose();
            _videoBufSync.Dispose();
        }
    }
}

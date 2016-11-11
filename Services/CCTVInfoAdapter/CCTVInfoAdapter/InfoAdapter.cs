using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CCTVClient;
using CCTVModels;
using StaticInfoClient;

namespace CCTVInfoAdapter
{
    public class InfoAdapter : IDisposable
    {
        Common.Logging.ILog _log { get { return Common.Logging.LogManager.GetLogger(GetType()); } }

        CCTVInfo _info;
        string _baseUri;
        CCTVGlobalInfo _globalInfo;

        ThumbnailProducer _thumbnailProducer;

        StaticInfoSynchronizer<CCTVGlobalInfo> _globalSync;
        StaticInfoSynchronizer<CCTVServerInfo> _serverSync;
        StaticInfoSynchronizer<CCTVStaticInfo> _staticSync;
        StaticInfoSynchronizer<CCTVControlConfig> _controlSync;
        StaticInfoSynchronizer<CCTVCameraLimits> _cameraSync;
        StaticInfoSynchronizer<CCTVDeviceInfo> _deviceSync;
        StaticInfoSynchronizer<CCTVDynamicInfo> _dynamicSync;
        StaticInfoSynchronizer<CCTVHierarchyInfo> _hierarchySync;
        StaticInfoSynchronizer<CCTVOnlineStatus> _onlineSync;
        StaticInfoSynchronizer<CCTVVideoTrack> _videoTrackSync;
        StaticInfoSynchronizer<VideoPosition> _videoPosSync;
        StaticInfoSynchronizer<VideoBuffering> _videoBufSync;

        public InfoAdapter(string host, string infoServiceBaseUri)
        {
            _baseUri = infoServiceBaseUri;
            _globalSync = new StaticInfoSynchronizer<CCTVGlobalInfo>(_baseUri, "CCTVGlobal", TimeSpan.FromSeconds(5));
            _serverSync = new StaticInfoSynchronizer<CCTVServerInfo>(_baseUri, "CCTVServer", TimeSpan.Zero);
            _staticSync = new StaticInfoSynchronizer<CCTVStaticInfo>(_baseUri, "CCTVStatic", TimeSpan.FromSeconds(5));
            _controlSync = new StaticInfoSynchronizer<CCTVControlConfig>(_baseUri, "CCTVControl", TimeSpan.Zero);
            _cameraSync = new StaticInfoSynchronizer<CCTVCameraLimits>(_baseUri, "CCTVCameraLimits", TimeSpan.Zero);
            _deviceSync = new StaticInfoSynchronizer<CCTVDeviceInfo>(_baseUri, "CCTVDeviceInfo", TimeSpan.Zero);
            _dynamicSync = new StaticInfoSynchronizer<CCTVDynamicInfo>(_baseUri, "CCTVDynamic", TimeSpan.FromSeconds(5));
            _hierarchySync = new StaticInfoSynchronizer<CCTVHierarchyInfo>(_baseUri, "CCTVHierarchy.Default", TimeSpan.FromSeconds(5));
            _onlineSync = new StaticInfoSynchronizer<CCTVOnlineStatus>(_baseUri, "CCTVOnlineStatus", TimeSpan.FromSeconds(5));
            _videoTrackSync = new StaticInfoSynchronizer<CCTVVideoTrack>(_baseUri, "CCTVVideoTrack", TimeSpan.FromSeconds(10));
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
            Console.WriteLine("Update once!");
            var allKeys = getAllCCTV1Id();
            updateHierarchy(tree, allKeys);
            updateStatic(tree, allKeys);
        }

        private List<string> getAllCCTV1Id()
        {
            _staticSync.GetUpdate();
            _deviceSync.GetUpdate();
            List<string> allKeys = new List<string>();
            foreach (CCTVStaticInfo si in _staticSync.Values)
            {
                if (si.Platform == CCTVPlatformType.CCTV1)
                    allKeys.Add(si.VideoId);
            }
            return allKeys;
        }

        private void updateStatic(VideoParser.Node tree, List<string> allKeys)
        {
            if (tree != null)
            {
                StaticInfoClassifier classifier = new StaticInfoClassifier(_globalInfo, _videoPosSync);
                classifier.Classify(tree);
                //_staticSync.ReplaceAll(staticDict);
                deleteAndUpdate(_staticSync, allKeys, classifier.StaticInfos, x => x.VideoId);
                deleteAndUpdate(_onlineSync, allKeys, classifier.OnlineStatus, x => x.VideoId);
                deleteAndUpdate(_serverSync, allKeys, classifier.Servers, x => x.ServerId);
                deleteAndUpdate(_cameraSync, allKeys, classifier.Cameras, x => x.VideoId);
                deleteAndUpdate(_controlSync, allKeys, classifier.Controls, x => x.VideoId);
                deleteAndUpdate(_deviceSync, allKeys, classifier.Devices, x => x.VideoId);
                deleteAndUpdate(_videoTrackSync, allKeys, classifier.VideoTracks, x => x.VideoId);
            }
        }

        private void deleteAndUpdate<T>(StaticInfoSynchronizer<T> sync, IEnumerable<string> allKeys, Dictionary<string, T> newValues, Func<T, string> keyInEle)
        {
            var delKeyDict = allKeys.Where(x => !newValues.ContainsKey(x)).ToDictionary(x => x);
            var delValues = sync.Values.Where(x => delKeyDict.ContainsKey(keyInEle(x)));
            var delDict = delValues.ToDictionary(x => keyInEle(x));
            sync.PutUpdate(toObjectItems(delDict, true));
            sync.PutUpdate(toObjectItems(newValues, false));
        }

        private IEnumerable<ObjectItem<T>> toObjectItems<T>(Dictionary<string, T> infos, bool isDelete)
        {
            return infos.Select(x => new ObjectItem<T>()
            {
                Key = x.Key,
                IsDeleted = isDelete,
                Item = x.Value
            });
        }

        private void updateHierarchy(VideoParser.Node tree, List<string> allKeys)
        {
            if (tree != null)
            {
                HierarchyClassifier classifier = new HierarchyClassifier();
                classifier.Classify(tree);
                deleteAndUpdate(_hierarchySync, allKeys, classifier.Hierarchies, x => x.Id);
            }
        }

        static string getNodeId(ulong id)
        {
            return $"CCTV1_{id:X}";
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
                dynamicInfo.Tilt = camera.Pointing.Tilt;
                CCTVStaticInfo staticInfo;
                CCTVCameraLimits cLimits;
                if (_staticSync.TryGetValue(dynamicInfo.VideoId, out staticInfo) && _cameraSync.TryGetValue(dynamicInfo.VideoId, out cLimits))
                {
                    double zoom = Math.Min(1, Math.Max(0, camera.Zoom));
                    dynamicInfo.ViewPort = (1 - zoom) * (cLimits.MaxViewPort - cLimits.MinViewPort) + cLimits.MinViewPort;
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

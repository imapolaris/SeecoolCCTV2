using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCTVReplay.Properties;
using CCTVReplay.Proxy;
using CCTVReplay.Source;
using CCTVReplay.Util;
using CenterStorageCmd;
using CenterStorageCmd.Url;
using Common.Log;
using CCTVModels;

namespace CCTVReplay.StaticInfo
{
    public class LocalInfoProxy : IVideoInfoManager
    {
        private static byte[] imageToBytes(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                return ms.ToArray();
            }
        }

        private byte[] _imgBytes;
        private VideoDataSource _src;
        private VideoDataInfoProxy _proxy;
        private CCTVHierarchyNode _root;

        public LocalInfoProxy(string localPath)
        {
            _src = new VideoDataSource()
            {
                SrcType = SourceType.Local,
                LocalSourcePath = localPath
            };
            _imgBytes = imageToBytes(Resources.DefaultThumbnail);
            _proxy = new VideoDataInfoProxy();
            _proxy.TreeNodesReceived += proxy_TreeNodesReceived;
            _proxy.MessageReceived += proxy_MessageReceived;
            _proxy.UpdateSource(_src);
            try
            {
                _proxy.GetVideoTreeNodesAsync();
            }
            catch (Exception e)
            {
                Logger.Default.Error("加载本地视频信息失败!", e);
                string msg = "加载本地视频信息失败!\n" + e.Message;
                DialogUtil.ShowError(msg);
            }
        }

        private void proxy_MessageReceived(MessagePacket obj)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => {
                Util.DialogUtil.ShowWarning(obj.Message);
            }));
        }

        private void proxy_TreeNodesReceived(CenterStorageCmd.LocalVideosInfoPacket obj)
        {
            _root = new CCTVHierarchyNode()
            {
                Id = "ServerID",
                Name = "本地视频",
                Type = NodeType.Server,
                Children = obj.ValidTimePeriods.Select(x => new CCTVHierarchyNode() { Id = x.VideoId, Name = x.VideoName, Type = NodeType.Video}).ToArray()
            };
            onNodeUpdated(obj);
        }

        public CCTVDynamicInfo GetDynamicInfo(string videoId)
        {
            return null;
        }

        public CCTVHierarchyNode GetHierarchyRoot()
        {
            return _root;
        }

        public CCTVOnlineStatus GetOnlineStatus(string videoId)
        {
            return new CCTVOnlineStatus() { VideoId = videoId, Online = true };
        }

        public CCTVStaticInfo GetStaticInfo(string videoId)
        {
            return null;
        }

        public VideoDataSource GetStorageSource()
        {
            return new VideoDataSource()
            {
                SrcType = SourceType.Local,
                LocalSourcePath = _src.LocalSourcePath,
            };
        }

        public void AddUpdateHandler(string videoId, Action<ThumbnailInfo> handler)
        {
            if (handler != null)
            {
                ThumbnailInfo ti = new ThumbnailInfo()
                {
                    VideoId = videoId,
                    ImageBytes = _imgBytes,
                    IsDefault = true,
                    Time = DateTime.Now
                };
                handler(ti);
            }
        }

        public void RemoveUpdateHandler(string videoId, Action<ThumbnailInfo> handler)
        {
            return;
        }

        #region 【事件】
        public event Action<LocalVideosInfoPacket> LocalSourceInfoReceived;

        private void onNodeUpdated(LocalVideosInfoPacket node)
        {
            Action<LocalVideosInfoPacket> handler = LocalSourceInfoReceived;
            if (handler != null)
                handler(node);
        }
        #endregion 【事件】

        public void Dispose()
        {
            _imgBytes = null;
            if (_proxy != null)
                _proxy.Close();
        }
    }
}

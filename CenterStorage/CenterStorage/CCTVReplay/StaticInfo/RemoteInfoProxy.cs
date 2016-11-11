using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CCTVReplay.Source;
using CCTVReplay.Util;
using CenterStorageCmd;
using CCTVModels;
using CCTVInfoHub.Thumbnail;
using CCTVInfoHub;
using CCTVInfoHub.Entity;

namespace CCTVReplay.StaticInfo
{
    public class RemoteInfoProxy : IVideoInfoManager
    {
        //不实现的事件。
        public event Action<LocalVideosInfoPacket> LocalSourceInfoReceived;

        private CCTVDefaultInfoSync ClientHub { get; set; }

        private ThumbnailManager ThumbnailMgr { get; set; }

        public RemoteInfoProxy(string ip)
        {
            UpdateWebApiUrl(ip);
        }

        private void UpdateWebApiUrl(string ip)
        {
            string url = $"http://{ip}:{ConfigReader.Instance.WebApiPort}";
            if (ClientHub != null)
                ClientHub.Dispose();
            if (ThumbnailMgr != null)
                ThumbnailMgr.Dispose();

            ClientHub = new CCTVDefaultInfoSync(url);
            ThumbnailMgr = new ThumbnailManager(url, ConstSettings.ThumbnailPath);
            //集中存储配置信息获取。
            SyncParams<StorageSource> param = new SyncParams<StorageSource>("CenterStorage", Timeout.InfiniteTimeSpan);
            ClientHub.RegisterDefault(CCTVInfoType.HierarchyInfo, TimeSpan.Zero);
            ClientHub.RegisterDefault(CCTVInfoType.StaticInfo, TimeSpan.Zero);
            ClientHub.RegisterDefault(CCTVInfoType.OnlineStatus, TimeSpan.FromSeconds(5));
            ClientHub.RegisterDefault(CCTVInfoType.DynamicInfo, TimeSpan.FromSeconds(5)); 
            ClientHub.RegisterSynchronizer(param);

        }

        public CCTVHierarchyNode GetHierarchyRoot()
        {
            return ClientHub?.GetAllHierarchyRoots()?.First();
        }

        public CCTVStaticInfo GetStaticInfo(string videoId)
        {
            return ClientHub?.GetStaticInfo(videoId);
        }

        public CCTVDynamicInfo GetDynamicInfo(string videoId)
        {
            return ClientHub?.GetDynamicInfo(videoId);
        }

        public CCTVOnlineStatus GetOnlineStatus(string videoId)
        {
            return ClientHub?.GetOnlineStatus(videoId);
        }

        public VideoDataSource GetStorageSource()
        {
            ClientHub?.UpdateRegistered<StorageSource>();
            StorageSource ss = ClientHub?.GetRegisteredInfo<StorageSource>("Default");
            return new VideoDataSource()
            {
                SrcType = SourceType.Remote,
                Storage = ss,
            };
        }

        public void AddUpdateHandler(string videoId, Action<ThumbnailInfo> handler)
        {
            ThumbnailMgr?.AddUpdateHandler(videoId, handler);
        }

        public void RemoveUpdateHandler(string videoId, Action<ThumbnailInfo> handler)
        {
            ThumbnailMgr?.RemoveUpdateHandler(videoId, handler);
        }

        #region 【实现IDisposable接口】
        private bool _disposed = false;
        public void Dispose()
        {
            dispose(true);
            GC.SuppressFinalize(this);
        }

        private void dispose(bool disposing)
        {
            if (_disposed)
            {
                if (disposing)
                {
                    ClientHub.Dispose();
                    ThumbnailMgr.Dispose();
                }
                ClientHub = null;
                ThumbnailMgr = null;
                _disposed = true;
            }
        }

        ~RemoteInfoProxy()
        {
            dispose(false);
        }
        #endregion 【实现IDisposable接口】
    }
}

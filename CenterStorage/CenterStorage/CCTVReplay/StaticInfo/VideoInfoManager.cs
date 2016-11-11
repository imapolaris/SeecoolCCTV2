using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using CCTVReplay.Source;
using CCTVReplay.Util;
using CenterStorageCmd;
using CenterStorageCmd.Url;
using CCTVModels;

namespace CCTVReplay.StaticInfo
{
    public class VideoInfoManager : IVideoInfoManager
    {
        public static VideoInfoManager Instance { get; private set; }
        static VideoInfoManager()
        {
            Instance = new VideoInfoManager();
        }

        public IVideoInfoManager InfoManager { get; private set; }
        public DataSource GlobalDataSource { get; private set; }
        private VideoInfoManager()
        {

        }

        private void DestoryProxy()
        {
            if (InfoManager != null)
            {
                InfoManager.LocalSourceInfoReceived -= InfoManager_NodeUpdated;
                InfoManager.Dispose();
                InfoManager = null;
            }
        }

        public int SourceIndex { get; private set; }

        public void UpdateSource(DataSource si)
        {
            GlobalDataSource = si;
            SourceIndex++;
            DestoryProxy();
            if (si.SourceType == SourceType.Remote)
                InfoManager = new RemoteInfoProxy(si.RemoteSourceIp);
            else
                InfoManager = new LocalInfoProxy(si.LocalSourcePath);

            InfoManager.LocalSourceInfoReceived += InfoManager_NodeUpdated;
            onDataSourceChanged();
        }

        public void UpdateSource(IUrl ui)
        {
            GlobalDataSource = null;
            SourceIndex++;
            DestoryProxy();
            if (ui is ILocalUrl)
                InfoManager = new LocalInfoProxy(ui.LocalPath);
            else if (ui is IRemoteUrl)
                InfoManager = new ImportInfoProxy(ui as IRemoteUrl);
            else {
                Common.Log.Logger.Default.Trace("未找到正确的URL配置");
               DialogUtil.ShowError("未找到正确的URL配置");
                return;
            }
            InfoManager.LocalSourceInfoReceived += InfoManager_NodeUpdated;
            onDataSourceChanged();
        }

        private void InfoManager_NodeUpdated(LocalVideosInfoPacket obj)
        {
            onNodeUpdated(obj);
        }

        public void CheckServerValid(DataSource si)
        {
            if (si.SourceType == SourceType.Remote)
            {
                if (string.IsNullOrWhiteSpace(si.RemoteSourceIp))
                    throw new ErrorMessageException("未配置Ip地址！");
                IPEndPoint ep = new IPEndPoint(IPAddress.Parse(si.RemoteSourceIp), ConfigReader.Instance.WebApiPort);
                TcpClient tcp = new TcpClient();
                tcp.Connect(ep);
                tcp.Close();
            }
            else
            {
                if (!Directory.Exists(si.LocalSourcePath))
                    throw new ErrorMessageException("未找到指定的本地数据源路径。");
            }
        }

        public CCTVHierarchyNode GetHierarchyRoot()
        {
            return InfoManager?.GetHierarchyRoot();
        }

        public CCTVStaticInfo GetStaticInfo(string videoId)
        {
            return InfoManager?.GetStaticInfo(videoId);
        }

        public CCTVDynamicInfo GetDynamicInfo(string videoId)
        {
            return InfoManager?.GetDynamicInfo(videoId);
        }

        public CCTVOnlineStatus GetOnlineStatus(string videoId)
        {
            return InfoManager?.GetOnlineStatus(videoId);
        }

        public VideoDataSource GetStorageSource()
        {
            return InfoManager?.GetStorageSource();
        }

        public void AddUpdateHandler(string videoId, Action<ThumbnailInfo> handler)
        {
            InfoManager?.AddUpdateHandler(videoId, handler);
        }

        public void RemoveUpdateHandler(string videoId, Action<ThumbnailInfo> handler)
        {
            InfoManager?.RemoveUpdateHandler(videoId, handler);
        }

        public void Dispose()
        {
            if (InfoManager != null)
                InfoManager.Dispose();
        }
        #region【事件】
        public event EventHandler DataSourceChanged;
        public event Action<LocalVideosInfoPacket> LocalSourceInfoReceived;

        private void onDataSourceChanged()
        {
            EventHandler handler = DataSourceChanged;
            if (handler != null)
                handler(null, EventArgs.Empty);
        }

        private void onNodeUpdated(LocalVideosInfoPacket node)
        {
            Action<LocalVideosInfoPacket> handler = LocalSourceInfoReceived;
            if (handler != null)
                handler(node);
        }
        #endregion【事件】

    }
}

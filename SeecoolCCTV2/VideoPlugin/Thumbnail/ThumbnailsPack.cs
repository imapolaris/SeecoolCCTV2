using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CCTVInfoHub.Thumbnail;
using CCTVModels;
using Common.Configuration;
using VideoNS.VideoInfo;

namespace VideoNS.Thumbnail
{
    public class ThumbnailsPack
    {
        #region 【单例】
        static ThumbnailsPack()
        {
            try
            {
                string webApiBaseUri = ConfigHandler.GetValue<VideoInfoPlugin>("WebApiBaseUri");
                bool fullThumbnail = Utils.Config.StringToBool(ConfigHandler.GetValue<VideoInfoPlugin>("FullThumbnail"));
                Reload(webApiBaseUri, fullThumbnail);
            }
            catch (Exception ex)
            {
                Common.Log.Logger.Default.Error(ex);
            }
        }
        public static ThumbnailsPack Instance { get; private set; }

        internal void Init()
        {

        }

        public static void Reload(string webApiBaseUri, bool fullThumbnail)
        {
            ThumbnailManager.UpdateDefaultThumbnail(Resource.DefaultThumbnail); //修改默认的缩略图图像。
            Instance = new ThumbnailsPack(webApiBaseUri, fullThumbnail);
        }
        #endregion 【单例】


        string getStaticInfoPath(string baseUrl)
        {
            string dataPath = System.Windows.Forms.Application.LocalUserAppDataPath;
            dataPath = Directory.GetParent(dataPath).FullName;
            Uri uri = new Uri(baseUrl);
            string path = Path.Combine(dataPath, "StaticInfo", uri.Host + "-" + uri.Port);
            return path;
        }

        private ThumbnailManager _mgr;
        private ThumbnailsPack(string baseUrl, bool fullThumbnail)
        {
            _mgr = new ThumbnailManager(baseUrl, getStaticInfoPath(baseUrl),!fullThumbnail);
            if (fullThumbnail)
                _videoInfoTimer = new Timer(_videoInfoTimer_Tick, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5));
        }

        Timer _videoInfoTimer;
        private void _videoInfoTimer_Tick(object state)
        {
            var staticInfoArray = CCTVInfoManager.Instance.GetAllStaticInfo();
            foreach (CCTVStaticInfo staticInfo in staticInfoArray)
                AddUpdateHandler(staticInfo.VideoId, null);
        }

        public void AddUpdateHandler(string videoId, Action<ThumbnailInfo> handler)
        {
            _mgr.AddUpdateHandler(videoId, handler);
        }

        public void RemoveUpdateHandler(string videoId, Action<ThumbnailInfo> handler)
        {
            _mgr.RemoveUpdateHandler(videoId, handler);
        }

        public ThumbnailInfo GetThumbnail(string videoId)
        {
            return _mgr.GetThumbnail(videoId);
        }
    }
}

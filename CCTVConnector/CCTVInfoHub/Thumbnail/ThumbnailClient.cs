using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCTVInfoHub.Properties;
using CCTVModels;
using StaticInfoClient;

namespace CCTVInfoHub.Thumbnail
{
    internal class ThumbnailClient : IDisposable
    {
        public string VideoId { get; private set; }

        static byte[] _defaultThumbnailBytes = imageToBytes(Resource.DefaultThumbnail);

        public static void UpdateDefaultThumbnail(Image img)
        {
            if (img != null)
                _defaultThumbnailBytes = imageToBytes(img);
        }

        StaticInfoSynchronizer<ThumbnailInfo> _synchronizer;
        ThumbnailInfo _info = null;
        string _baseUrl;
        string _savePath;
        bool _stopWhileNoHandler = true;

        public ThumbnailClient(string baseUrl, string videoId, string savePath = null, bool stopWhileNoHandler = true)
        {
            VideoId = videoId;
            _baseUrl = baseUrl;
            _savePath = savePath;
            _stopWhileNoHandler = stopWhileNoHandler;

            _info = new ThumbnailInfo()
            {
                VideoId = videoId,
                Time = DateTime.Now,
                ImageBytes = _defaultThumbnailBytes,
                IsDefault = true,
            };

            start();
        }

        private static byte[] imageToBytes(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                return ms.ToArray();
            }
        }

        private static string getSection(string videoId)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(videoId);
            string base64 = Convert.ToBase64String(bytes);
            return base64.Replace('/', '-');
        }

        public void Update()
        {
            if (_synchronizer != null)
                _synchronizer.GetUpdate();
        }

        private void onUpdate(StaticInfoSynchronizer<ThumbnailInfo> synchronizer, IEnumerable<string> keysUpdated)
        {
            string key = keysUpdated.FirstOrDefault();
            ThumbnailInfo info;
            if (key != null && synchronizer.TryGetValue(key, out info))
            {
                _info = info;
                fireUpdateEvent(info);
            }
        }

        private void fireUpdateEvent(ThumbnailInfo info)
        {
            var callback = _updateEvent;
            if (callback != null)
                callback(info);
        }

        public event Action<ThumbnailInfo> UpdateEvent
        {
            add
            {
                lock (_updateEventLocker)
                {
                    _updateEvent += value;
                    var info = _info;
                    if (info != null && value != null)
                        value(info);
                    start();
                }
            }
            remove
            {
                lock (_updateEventLocker)
                {
                    _updateEvent -= value;

                    if (_stopWhileNoHandler && _updateEvent == null)
                        stop();
                }
            }
        }
        private event Action<ThumbnailInfo> _updateEvent;
        object _updateEventLocker = new object();

        void start()
        {
            if (!_disposed)
            {
                if (_synchronizer == null)
                {
                    string section = $"Thumbnail/{getSection(VideoId)}";
                    _synchronizer = new StaticInfoSynchronizer<ThumbnailInfo>(_baseUrl, section, TimeSpan.Zero, _savePath, onUpdate);
                }
            }
        }

        void stop()
        {
            if (_synchronizer != null)
            {
                var synchronizer = _synchronizer;
                _synchronizer = null;
                synchronizer.Dispose();
            }
        }

        #region 【实现IDisposable】
        private bool _disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_synchronizer != null)
                        _synchronizer.Dispose();
                }
                _synchronizer = null;
                _disposed = true;
            }
        }

        ~ThumbnailClient()
        {
            Dispose(false);
        }
        #endregion 【实现IDisposable】
    }
}

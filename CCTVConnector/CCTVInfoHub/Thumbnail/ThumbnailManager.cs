using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CCTVModels;

namespace CCTVInfoHub.Thumbnail
{
    public class ThumbnailManager : IDisposable
    {
        public static void UpdateDefaultThumbnail(Image img)
        {
            ThumbnailClient.UpdateDefaultThumbnail(img);
        }

        string _baseUrl;
        string _savePath;
        private bool _stopWhileNoHandler = true;

        public void Init()
        {
        }

        public ThumbnailManager(string baseUrl, string savePath, bool stopWhileNoHandler = true)
        {
            _baseUrl = baseUrl;
            _savePath = savePath;
            _stopWhileNoHandler = stopWhileNoHandler;

            _updateThread = new Thread(update);
            _updateThread.IsBackground = true;
            _updateThread.Start();
        }

        Thread _updateThread;

        private void update()
        {
            while (true)
            {
                if (_disposed)
                    break;
                ThumbnailClient[] clientArray = _clientDict.Values.ToArray();
                foreach (ThumbnailClient client in clientArray)
                {
                    client.Update();

                    Thread.Sleep(1000);
                }
                Thread.Sleep(1000);
            }
        }

        ConcurrentDictionary<string, ThumbnailClient> _clientDict = new ConcurrentDictionary<string, ThumbnailClient>();
        public void AddUpdateHandler(string videoId, Action<ThumbnailInfo> handler)
        {
            if (_disposed)
                return;
            if (!string.IsNullOrWhiteSpace(videoId))
            {
                ThumbnailClient client = _clientDict.GetOrAdd(videoId, x => new ThumbnailClient(_baseUrl, x, _savePath, _stopWhileNoHandler));
                client.UpdateEvent += handler;
            }
        }

        public void RemoveUpdateHandler(string videoId, Action<ThumbnailInfo> handler)
        {
            if (_disposed)
                return;
            if (!string.IsNullOrWhiteSpace(videoId))
            {
                ThumbnailClient client;
                if (_clientDict.TryGetValue(videoId, out client))
                    client.UpdateEvent -= handler;
            }
        }

        public ThumbnailInfo GetThumbnail(string videoId)
        {
            ThumbnailInfo ti = null;
            Action<ThumbnailInfo> handler = x => { ti = x; };
            AddUpdateHandler(videoId, handler);
            RemoveUpdateHandler(videoId, handler);
            return ti;
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
                _disposed = true;
                if (disposing)
                {
                    ThumbnailClient[] clientArray = _clientDict.Values.ToArray();
                    foreach (ThumbnailClient client in clientArray)
                        client.Dispose();
                }
                _clientDict = null;
            }
        }

        ~ThumbnailManager()
        {
            Dispose(false);
        }
        #endregion 【实现IDisposable】
    }
}

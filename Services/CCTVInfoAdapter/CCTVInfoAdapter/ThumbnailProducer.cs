using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CCTVClient;

namespace CCTVInfoAdapter
{
    internal class ThumbnailProducer : IDisposable
    {
        CCTVInfo _info;
        TimeSpan _timeout = TimeSpan.FromSeconds(10);
        Thread _thread;
        ManualResetEventSlim _disposeEvent = new ManualResetEventSlim(false);
        int _defaultWidth = 352;
        int _defaultHeight = 288;
        ulong[] _videoIdArray = new ulong[0];

        public delegate void OnThumbnail(ulong videoId, Image thumbnail);
        public event OnThumbnail ThumbnailEvent;
        private void fireThumbnailEvent(ulong videoId, Image thumbnail)
        {
            var callback = ThumbnailEvent;
            if (callback != null)
                callback(videoId, thumbnail);
        }

        public ThumbnailProducer(CCTVInfo info)
        {
            _info = info;
            _info.NodeTreeEvent += Info_NodeTreeEvent;

            _disposeEvent.Reset();
            _thread = new Thread(run);
            _thread.Start();
        }

        private void Info_NodeTreeEvent(VideoParser.Node tree, string xml)
        {
            HashSet<ulong> videoIdSet = new HashSet<ulong>();
            collectVideoId(videoIdSet, tree);
            _videoIdArray = videoIdSet.ToArray();
        }

        private static void collectVideoId(HashSet<ulong> videoIdSet, VideoParser.Node node)
        {
            VideoParser.Server server = node as VideoParser.Server;
            if (server != null)
            {
                foreach (VideoParser.Node child in server.Childs)
                    collectVideoId(videoIdSet, child);
            }
            else
            {
                VideoParser.Front front = node as VideoParser.Front;
                if (front != null)
                    foreach (VideoParser.Video video in front.Childs)
                        videoIdSet.Add(video.Id);
            }
        }

        private void run()
        {
            using (ImageGrabber imageGrabber = new ImageGrabber(_info))
            {
                imageGrabber.Bandwidth = 128000;

                ulong videoId = 0;
                int wait = 0;
                while (!_disposeEvent.Wait(wait))
                {
                    videoId = getNextVideoId(videoId);
                    if (videoId != 0)
                    {
                        wait = 1;
                        Image image = imageGrabber.GrabImage(videoId, _timeout);
                        Image thumbnail = createThumbnail(image);
                        if (image != null)
                            fireThumbnailEvent(videoId, thumbnail);
                    }
                    else
                        wait = 1000;
                }
            }
        }

        private Image createThumbnail(Image image)
        {
            if (image == null || image.Width == 0 || image.Height == 0)
                return null;
            else if (image.Width <= _defaultWidth || image.Height <= _defaultHeight)
                return image;
            else
            {
                double ratioX = image.Width * 1.0 / _defaultWidth;
                double ratioY = image.Height * 1.0 / _defaultHeight;
                double ratio = Math.Min(ratioX, ratioY);
                int width = (int)Math.Round(image.Width / ratio);
                int height = (int)Math.Round(image.Height / ratio);
                Bitmap bitmap = new Bitmap(image, width, height);
                return bitmap;
            }
        }

        private ulong getNextVideoId(ulong lastId)
        {
            ulong[] videoIdArray = _videoIdArray;
            if (videoIdArray.Length == 0)
                return 0;
            else
            {
                int index = Array.IndexOf(videoIdArray, lastId);
                if (index >= 0)
                {
                    index++;
                    index %= videoIdArray.Length;
                    return videoIdArray[index];
                }
                else
                    return videoIdArray[0];
            }
        }

        public void Dispose()
        {
            _info.NodeTreeEvent -= Info_NodeTreeEvent;

            _disposeEvent.Set();
        }
    }
}

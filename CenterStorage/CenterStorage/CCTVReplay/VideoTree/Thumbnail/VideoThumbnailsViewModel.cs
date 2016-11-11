using AopUtil.WpfBinding;
using CCTVReplay.Util;
using CCTVReplay.Video;
using Common.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using CCTVReplay.StaticInfo;
using CCTVModels;

namespace CCTVReplay.VideoTree.Thumbnail
{
    public class VideoThumbnailsViewModel : ObservableObject, IDisposable
    {
        internal VideoThumbnailsViewModel()
        {
            PropertyChanged += onPropertyChanged;
            VideoDataInfo = new VideoDataInfoViewModel();
        }

        string _oldId = string.Empty;

        [AutoNotify]
        public string ID { get; set; } = string.Empty;
        [AutoNotify]
        public string Name { get; set; } = string.Empty;
        [AutoNotify]
        public BitmapImage Thumbnail { get; set; } = new System.Windows.Media.Imaging.BitmapImage();
        [AutoNotify]
        public VideoDataInfoViewModel VideoDataInfo { get; set; }
        [AutoNotify]
        public bool IsOnline { get; set; }
        [AutoNotify]
        public bool IsOnPlaying { get; set; } = false;
        public int StreamId { get; set; } = ConstSettings.StreamId;

        private void updateImage(ThumbnailInfo info)
        {
            updateOnlineStatus();
            System.Drawing.Image bmp = info.Thumbnail;

            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);// 格式自处理,这里用 bitmap
                try
                {
                    WindowUtil.BeginInvoke(() =>
                    {
                        var bi = new System.Windows.Media.Imaging.BitmapImage();
                        bi.BeginInit();
                        bi.StreamSource = new MemoryStream(ms.ToArray()); // 不要直接使用 ms
                        bi.EndInit();
                        Thumbnail = bi;
                    });
                }
                catch
                {
                    Dispose();
                }
            }
        }

        private void updateOnlineStatus()
        {
            var onlineStatus = VideoInfoManager.Instance.GetOnlineStatus(ID);
            if (onlineStatus != null && onlineStatus.Online)
                IsOnline = true;
            else
                IsOnline = false;
        }

        private void onPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ID):
                    VideoInfoManager.Instance.RemoveUpdateHandler(_oldId, updateImage);
                    WindowUtil.BeginInvoke(() => { Thumbnail = new System.Windows.Media.Imaging.BitmapImage(); });
                    VideoInfoManager.Instance.AddUpdateHandler(ID, updateImage);
                    _oldId = ID;
                    break;
            }
        }
        public bool IsDisposed { get; private set; }
        public void Dispose()
        {
            if (!IsDisposed)
            {
                VideoInfoManager.Instance.RemoveUpdateHandler(ID, updateImage);
                PropertyChanged -= onPropertyChanged;
                IsDisposed = true;
            }
        }

        ~VideoThumbnailsViewModel()
        {
            Dispose();
        }
    }
}


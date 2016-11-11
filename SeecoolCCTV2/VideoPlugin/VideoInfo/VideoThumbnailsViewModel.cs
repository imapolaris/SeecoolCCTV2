using AopUtil.WpfBinding;
using Common.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using VideoNS.Thumbnail;
using CCTVModels;

namespace VideoNS.VideoInfo
{
    public class VideoThumbnailsViewModel : ObservableObject, IDisposable
    {
        public VideoThumbnailsViewModel()
        {
            PropertyChanged += onPropertyChanged;
        }

        string _oldId = string.Empty;

        [AutoNotify]
        public string ID { get; set; } = string.Empty;
        [AutoNotify]
        public string Name { get; set; } = string.Empty;
        [AutoNotify]
        public BitmapImage Thumbnail { get; set; } = new System.Windows.Media.Imaging.BitmapImage();

        [AutoNotify]
        public bool IsOnline { get; set; }
        [AutoNotify]
        public bool IsOnPlaying { get; set; } = false;

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
                catch {
                    Dispose();
                }
            }
        }

        private void updateOnlineStatus()
        {
            var onlineStatus = CCTVInfoManager.Instance.GetOnlineStatus(ID);
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
                    ThumbnailsPack.Instance.RemoveUpdateHandler(_oldId, updateImage);
                    WindowUtil.BeginInvoke(() => { Thumbnail = new System.Windows.Media.Imaging.BitmapImage(); });
                    ThumbnailsPack.Instance.AddUpdateHandler(ID, updateImage);
                    _oldId = ID;
                    break;
            }
        }
        public bool IsDisposed { get; private set; }
        public void Dispose()
        {
            if (!IsDisposed)
            {
                ThumbnailsPack.Instance.RemoveUpdateHandler(ID, updateImage);
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

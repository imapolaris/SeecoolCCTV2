using CenterStorageCmd;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace CCTVDownload
{
    public class DownloadStatusToPngConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DownloadStatus status = (DownloadStatus)value;
            BitmapImage img =null;
            switch (status)
            {
                case DownloadStatus.Ready:
                    img = getImage(@"../Images/downloadstatus/准备下载.png");
                    break;
                case DownloadStatus.Waiting:
                    img = getImage(@"../Images/downloadstatus/等待下载.png");
                    break;
                case DownloadStatus.Paused:
                    img = getImage(@"../Images/downloadstatus/暂停下载.png");
                    break;
                case DownloadStatus.Downloading:
                    img = getImage(@"../Images/downloadstatus/正在下载.png");
                    break;
                case DownloadStatus.Completed:
                    img = getImage(@"../Images/downloadstatus/下载完成.png");
                    break;
                case DownloadStatus.Deleted:
                    img = getImage(@"../Images/downloadstatus/已删除.png");
                    break;
                case DownloadStatus.Error:
                    img = getImage(@"../Images/downloadstatus/下载错误.png");
                    break;
            }
            return img;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static BitmapImage getImage(string uri)
        {
            return new BitmapImage(new Uri(uri, UriKind.Relative));
        }
    }
}

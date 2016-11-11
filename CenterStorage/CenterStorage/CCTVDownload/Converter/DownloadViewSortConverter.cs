using CenterStorageCmd;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace CCTVDownload
{
    public class DownloadViewSortConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (DownloadStatus)value;
            var param = (DownloadViewSort)Enum.Parse(typeof(DownloadViewSort), parameter.ToString());
            if (param == DownloadViewSort.已完成)
            {
                if (status == DownloadStatus.Completed)
                    return Visibility.Visible;
            }
            else if (param == DownloadViewSort.正在下载)
            {
                if (status != DownloadStatus.Completed)
                    return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

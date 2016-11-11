using CenterStorageCmd;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace CCTVDownload.Converter
{
    public class SelectStatusToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool selected = (bool)values[0];
            DownloadStatus status = (DownloadStatus)values[1];
            var param = (DownloadViewSort)Enum.Parse(typeof(DownloadViewSort), parameter.ToString());
            if(selected)
            {
                if(param == DownloadViewSort.已完成 && status == DownloadStatus.Completed)
                    return Visibility.Visible;
                if (param == DownloadViewSort.正在下载 && status != DownloadStatus.Completed)
                    return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

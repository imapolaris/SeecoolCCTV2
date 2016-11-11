using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using VideoNS.Thumbnail;

namespace VideoNS.VideoInfo
{
    public class VideoIdToSnapshotConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            string id = value as string;
            if (string.IsNullOrWhiteSpace(id))
                return null;
            var fi = new FileInfo(string.Format(@"./config/snapshot/{0}.jpg", id));
            if (!fi.Exists)
                return null;
            return new Uri(fi.FullName, UriKind.RelativeOrAbsolute);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VideoNS
{
    public class RatioValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string[] strs = parameter.ToString().Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
            double ratio = System.Convert.ToDouble(strs[0]);
            var len = (double)value * ratio;
            if (strs.Length > 1)
                len = len + System.Convert.ToDouble(strs[1]);
            return len;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string[] strs = parameter.ToString().Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
            double ratio = System.Convert.ToDouble(strs[0]);
            double len = (double)value;
            if (strs.Length > 1)
                len = len - System.Convert.ToDouble(strs[1]);
            return ratio == 0 ? len : len / ratio;
        }
    }
}

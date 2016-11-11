using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace VideoNS.BaseXaml
{
    public class MultiBoolAndToVisibleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string para = parameter as string;
                for (int i = 0; i < values.Length; i++)
                {
                    bool exp = (length(para) <= i || para[i] == '1');
                    if ((bool)values[i] != exp)
                        return Visibility.Collapsed;
                }
            }
            catch { }
            return Visibility.Visible;
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        int length(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return 0;
            return str.Length;
        }
    }
}

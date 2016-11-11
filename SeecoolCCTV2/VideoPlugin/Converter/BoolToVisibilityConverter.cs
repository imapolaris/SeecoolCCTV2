using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace VideoNS.Converter
{
    internal class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = System.Convert.ToBoolean(value);
            bool isAnd = System.Convert.ToBoolean(parameter);
            if (isAnd)
            {
                if (flag)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
            else
            {
                if (flag)
                    return Visibility.Collapsed;
                else
                    return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visi = (Visibility)value;
            bool isAnd = System.Convert.ToBoolean(parameter);
            if (isAnd)
            {
                switch (visi)
                {
                    case Visibility.Visible:
                    default:
                        return true;
                    case Visibility.Hidden:
                    case Visibility.Collapsed:
                        return false;
                }
            }
            else
            {
                switch (visi)
                {
                    case Visibility.Visible:
                    default:
                        return false;
                    case Visibility.Hidden:
                    case Visibility.Collapsed:
                        return true;
                }
            }
        }
    }
}

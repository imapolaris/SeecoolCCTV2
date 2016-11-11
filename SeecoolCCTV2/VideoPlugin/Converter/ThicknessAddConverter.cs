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
    public class ThicknessAddConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Thickness tk = (Thickness)value;
            double add = 0;
            double.TryParse(parameter.ToString(), out add);
            return new Thickness(
                ToAdd(tk.Left,add),
                ToAdd(tk.Top,add),
                ToAdd(tk.Right,add),
                ToAdd(tk.Bottom,add));
        }

        private double ToAdd(double origin,double add)
        {
            return origin > 0 ? origin + add : 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

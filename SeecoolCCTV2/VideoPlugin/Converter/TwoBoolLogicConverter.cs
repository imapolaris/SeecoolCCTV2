using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VideoNS.Converter
{
    public class TwoBoolLogicConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag1 = (bool)values[0];
            bool flag2 = (bool)values[1];
            int logic = System.Convert.ToInt32(parameter);
            switch (logic)
            {
                default:
                case 1: //and 
                    return flag1 && flag2;
                case 2: //or
                    return flag1 || flag2;
                case 3: //xor
                    return flag1 ^ flag2;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

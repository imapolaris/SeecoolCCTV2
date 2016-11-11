using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VideoNS.SubControls
{
    /// <summary>
    /// SpeedTipBar.xaml 的交互逻辑
    /// </summary>
    public partial class SpeedTipBar : UserControl
    {
        public static readonly DependencyProperty SpeedProperty =
            DependencyProperty.Register("Speed", typeof(double), typeof(SpeedTipBar));

        public SpeedTipBar()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 速度值，介于0-1.0之间。
        /// </summary>
        public double Speed
        {
            get { return (double)GetValue(SpeedProperty); }
            set { SetValue(SpeedProperty, value);}
        }
    }

    public class ThreeOfFourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0.75d * (double)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value * 4d / 3d;
        }
    }

    public class SpeedToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double baseD = System.Convert.ToDouble(parameter);
            double speed = (double)value;
            double opacity = (speed - baseD) / 0.5d;
            if (opacity > 1)
                return 1;
            else if (opacity < 0)
                return 0;
            return opacity;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

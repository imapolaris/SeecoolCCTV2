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

namespace VideoNS.TimeSwitch
{
    /// <summary>
    /// PlusPanel.xaml 的交互逻辑
    /// </summary>
    public partial class PlusPanel : UserControl
    {
        public PlusPanel()
        {
            InitializeComponent();
        }
    }

    public class PlusIconSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double size = (double)value / 3;
            return Math.Min(Math.Max(size, 10) + 5,160);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

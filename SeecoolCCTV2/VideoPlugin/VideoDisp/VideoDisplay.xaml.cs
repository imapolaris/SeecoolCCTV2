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
using VideoNS.VideoInfo;

namespace VideoNS.VideoDisp
{
    /// <summary>
    /// VideoDisplay.xaml 的交互逻辑
    /// </summary>
    public partial class VideoDisplay : UserControl
    {
        public VideoDisplay()
        {
            InitializeComponent();
        }

        public VideoDisplayViewModel ViewModel { get { return (VideoDisplayViewModel)DataContext; } }
    }

    public class LatencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TimeSpan latency = (TimeSpan)value;
            const string prefix = "网络延迟：";
            if (latency < TimeSpan.FromSeconds(1))
                return prefix + "低";
            else if (latency < TimeSpan.FromSeconds(5))
                return prefix + "中";
            else
                return prefix + "高";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StatusInfoConverter : IMultiValueConverter
    {
        static bool _showFluent = Utils.Config.StringToBool(Common.Configuration.ConfigHandler.GetValue<VideoInfoPlugin>("ShowFluent"));

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int width ,height;
            double bitsPerSec,framePerSec;
            int.TryParse(values[0].ToString(), out width);
            int.TryParse(values[1].ToString(), out height);
            double.TryParse(values[2].ToString(), out bitsPerSec);
            double.TryParse(values[3].ToString(), out framePerSec);
            List<string> strs = new List<string>
                {
                    $"分辨率：{width} X {height}",
                    $"流速：{bpsString(bitsPerSec)} 位/秒",
                    $"帧速：{framePerSec:F1} 帧/秒",
                };
            if (_showFluent)
            {
                double fluentAverage;
                int bufferFrameCount;
                double.TryParse(values[4].ToString(), out fluentAverage);
                int.TryParse(values[5].ToString(), out bufferFrameCount);
                strs.AddRange(new string[]
                    {
                       $"流畅指数：{fluentAverage:F0}",
                       $"帧缓冲：{bufferFrameCount}",
                    });
            }
            return string.Join("\n", strs);
        }

        private static string bpsString(double bps)
        {
            if (bps < 1000)
                return $"{bps:F3}";
            else
            {
                bps /= 1000;
                if (bps < 1000)
                    return $"{bps:F3}K";
                else
                {
                    bps /= 1000;
                    return $"{bps:F3}M";
                }
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

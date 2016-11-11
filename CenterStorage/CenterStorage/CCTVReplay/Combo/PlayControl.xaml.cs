using CCTVReplay.Video;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Globalization;
using Common.Util;

namespace CCTVReplay.Combo
{
    /// <summary>
    /// PlayControl.xaml 的交互逻辑
    /// </summary>
    public partial class PlayControl : UserControl
    {
        public PlayControlViewModel ViewModel { get { return DataContext as PlayControlViewModel; } }

        public PlayControl()
        {
            InitializeComponent();
            this.Loaded += PlayControl_Loaded;
        }

        private void PlayControl_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.VideoAdded += videoAdded;
            ViewModel.VideoRemoved += videoRemoved;
            ViewModel.Cleared += videosCleared;
        }

        private void videosCleared(object sender, EventArgs e)
        {
            WindowUtil.Invoke(() =>
            {
                gridVideo.Children.Clear();
            });
        }

        private void videoRemoved(VideoControlViewModel model)
        {
            List<VideoControl> vcs = new List<VideoControl>();
            foreach (VideoControl vc in gridVideo.Children)
            {
                vcs.Add(vc);
            }
            gridVideo.Children.Clear();
            int index = vcs.FindIndex(vc => vc.ViewModel == model);
            if (index >= 0)
                vcs.RemoveAt(index);
            replaceVideo(vcs);
        }

        private void videoAdded(VideoControlViewModel model)
        {
            List<VideoControl> vcs = new List<VideoControl>();
            foreach (VideoControl vc in gridVideo.Children)
            {
                vcs.Add(vc);
            }
            gridVideo.Children.Clear();
            vcs.Add(new VideoControl() { DataContext = model });
            replaceVideo(vcs);
        }

        private void replaceVideo(List<VideoControl> videos)
        {
            int split = (int)Math.Ceiling(Math.Sqrt(videos.Count));
            gridVideo.RowDefinitions.Clear();
            gridVideo.ColumnDefinitions.Clear();
            for (int i = 0; i < split; i++)
            {
                gridVideo.RowDefinitions.Add(new RowDefinition());
                gridVideo.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int i = 0; i < videos.Count; i++)
            {
                int row = i / split;
                int col = i % split;
                VideoControl vc = videos[i];
                vc.SetValue(Grid.RowProperty, row);
                vc.SetValue(Grid.ColumnProperty, col);
                gridVideo.Children.Add(vc);
            }
        }


        private void TimeLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TimeSelectorWin win = new TimeSelectorWin();
            win.ViewModel.BeginTime = ViewModel.PlaySlider.BeginTime;
            win.ViewModel.EndTime = ViewModel.PlaySlider.EndTime;
            bool rst = (bool)win.ShowDialog();
            if (rst)
            {
                if (win.ViewModel.BeginTime != null && win.ViewModel.EndTime != null)
                {
                    ViewModel.UpdateTimePeriod((DateTime)win.ViewModel.BeginTime, (DateTime)win.ViewModel.EndTime);
                }
            }
        }

        private void Grid_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            ViewModel.ShowCtrlBar = !ViewModel.ShowCtrlBar;
        }
    }

    public class FullScreenToMargin : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                bool isFull;
                if (bool.TryParse(value.ToString(), out isFull))
                {
                    if (isFull)
                        return new Thickness(0);
                }
            }
            return new Thickness(0, 0, 0, 65);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CtrlBarVisiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                return Visibility.Visible;
            bool isFull = (bool)values[0];
            bool other = (bool)values[1];
            if (isFull)
            {
                return other ? Visibility.Visible : Visibility.Collapsed;
            }
            else
                return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BooleanToVisi : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;
            bool b;
            if (Boolean.TryParse(value.ToString(), out b))
            {
                bool c = bool.Parse(parameter.ToString());
                return b ^ c ? Visibility.Collapsed : Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MultiBoolToVisi : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || parameter == null)
                return Visibility.Collapsed;
            List<bool> flags = new List<bool>();
            foreach (object obj in values)
            {
                bool b;
                if (bool.TryParse(obj.ToString(), out b))
                {
                    flags.Add(b);
                }
                else
                    return Visibility.Collapsed;
            }
            string param = parameter.ToString().ToUpper();
            if (param.Equals("OR"))
            {
                foreach (bool b in flags)
                    if (b)
                        return Visibility.Visible;
            }
            else if (param.Equals("XOR"))
            {
                bool b = flags[0];
                for (int i = 1; i < flags.Count; i++)
                {
                    b ^= flags[i];
                }
                if (b)
                    return Visibility.Visible;
            }
            else
            {
                bool flag = true;
                foreach (bool b in flags)
                    flag &= b;
                if (param.Equals("NOT"))
                {
                    if (!flag)
                        return Visibility.Visible;
                }
                else
                {
                    if (flag)
                        return Visibility.Visible;
                }
            }
            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

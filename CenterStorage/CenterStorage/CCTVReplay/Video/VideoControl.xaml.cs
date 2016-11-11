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

namespace CCTVReplay.Video
{
    /// <summary>
    /// VideoControl.xaml 的交互逻辑
    /// </summary>
    public partial class VideoControl : UserControl
    {
        public VideoControlViewModel ViewModel { get { return DataContext as VideoControlViewModel; } }

        public VideoControl()
        {
            InitializeComponent();
            this.DataContextChanged += this_DataContextChanged;
        }

        private void this_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            VideoControlViewModel oldVM = e.OldValue as VideoControlViewModel;
            VideoControlViewModel newVM = e.NewValue as VideoControlViewModel;
            if (oldVM != null)
                oldVM.PropertyChanged -= model_PropertyChanged;
            if (newVM != null)
                newVM.PropertyChanged += model_PropertyChanged;
        }

        private int _initRow = 0;
        private int _initCol = 0;
        private void model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(VideoControlViewModel.IsFullScreen):
                    {
                        if (ViewModel.IsFullScreen)
                        {
                            Panel p = this.Parent as Panel;
                            if(p!=null&&p.Children.Count<=1)
                            {
                                ViewModel.IsFullScreen = false;
                                return;
                            }
                            _initRow = (int)this.GetValue(Grid.RowProperty);
                            _initCol = (int)this.GetValue(Grid.ColumnProperty);
                            this.SetValue(Panel.ZIndexProperty, 100); //设置一个足够大的数，确保其比当前父容器中的子节点要多。
                            this.SetValue(Grid.RowProperty, 0);
                            this.SetValue(Grid.ColumnProperty, 0);
                            this.SetValue(Grid.RowSpanProperty, 10); //跨越足够多的行列。
                            this.SetValue(Grid.ColumnSpanProperty, 10);
                        }
                        else
                        {
                            this.SetValue(Panel.ZIndexProperty, 0); 
                            this.SetValue(Grid.RowProperty, _initRow);
                            this.SetValue(Grid.ColumnProperty, _initCol);
                            this.SetValue(Grid.RowSpanProperty, 1); //跨越足够多的行列。
                            this.SetValue(Grid.ColumnSpanProperty, 1);
                        }
                    }
                    break;
            }
        }
    }

    public class TickTransConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                return 0;
            double width = (double)values[0];
            double ratio = (double)values[1];
            return width * ratio;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DownloadingToVisi : IValueConverter
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
}

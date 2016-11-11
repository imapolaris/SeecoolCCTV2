using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CCTVReplay.Time
{
    /// <summary>
    /// TimePicker.xaml 的交互逻辑
    /// </summary>
    public partial class TimePicker : UserControl
    {
        public TimePicker()
        {
            InitializeComponent();
            init();
        }

        private void init()
        {
            int rows = 6, cols = 4;
            for (int r = 0; r < rows; r++)
            {
                gridTime.RowDefinitions.Add(new RowDefinition());
            }
            for (int c = 0; c < cols; c++)
            {
                gridTime.ColumnDefinitions.Add(new ColumnDefinition());
            }
            int index = 0;
            IValueConverter conv = new SelTimeToChecked();
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    ToggleButton btn = new ToggleButton()
                    {
                        IsChecked = true
                    };
                    btn.SetValue(Grid.RowProperty, r);
                    btn.SetValue(Grid.ColumnProperty, c);
                    btn.Content = new TimeSpan(index, 0, 0);
                    btn.ContentStringFormat = "HH";
                    btn.SetBinding(ToggleButton.IsCheckedProperty, CreateBinding(this, BindingMode.OneWay, "SelectedTime", conv, index++));
                    btn.Checked += Btn_Checked;
                    gridTime.Children.Add(btn);
                }
            }
        }

        private void Btn_Checked(object sender, RoutedEventArgs e)
        {
            ToggleButton btn = sender as ToggleButton;
            TimeSpan ts = (TimeSpan)btn.Content;
            DateTime? dt = this.SelectedTime;
            if (dt == null)
            {
                dt = DateTime.Now;
            }
            DateTime ndt = (DateTime)dt;
            if (ndt.Hour != ts.Hours)
            {
                ndt = ndt.AddHours(-ndt.Hour).AddMinutes(-ndt.Minute).AddHours(ts.Hours);
                this.SelectedTime = ndt;
            }
        }

        private Binding CreateBinding(object source, BindingMode mode, string path, IValueConverter conv, object convParam)
        {
            Binding binding = new Binding();
            binding.Mode = mode;
            binding.Source = source;
            if (path != null)
                binding.Path = new PropertyPath(path);
            binding.Converter = conv;
            binding.ConverterParameter = convParam;
            return binding;
        }

        public DateTime? SelectedTime
        {
            get { return (DateTime?)GetValue(SelectedTimeProperty); }
            set { SetValue(SelectedTimeProperty, value); }
        }

        public static readonly DependencyProperty SelectedTimeProperty =
            DependencyProperty.Register("SelectedTime", typeof(DateTime?), typeof(TimePicker), new PropertyMetadata(DateTime.Now));
    }

    internal class SelTimeToChecked : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value == null)
                return false;
            
            DateTime dt;
            if (DateTime.TryParse(value.ToString(), out dt))
            {
                if (dt.Hour == (int)parameter)
                    return true;
            }
            return false;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

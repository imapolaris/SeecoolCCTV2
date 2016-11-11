using System;
using System.Collections.Generic;
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

namespace VideoNS.SubControls
{
    /// <summary>
    /// PresetPanel.xaml 的交互逻辑
    /// </summary>
    public partial class PresetPanel : UserControl
    {
        public PresetViewModel ViewModel { get { return this.DataContext as PresetViewModel; } }

        public PresetPanel()
        {
            InitializeComponent();
            InitButtons();
            this.MouseDown += (s, e) => e.Handled = true;
        }

        private void InitButtons()
        {
            int index = 1;
            for (int row = 0; row < 4; row++)
            {
                gridMain.RowDefinitions.Add(new RowDefinition());
                for (int col = 0; col < 8; col++)
                {
                    if (row == 0)
                    {
                        gridMain.ColumnDefinitions.Add(new ColumnDefinition());
                    }
                    ToggleButton tBtn = new ToggleButton();
                    tBtn.SetValue(Grid.RowProperty, row);
                    tBtn.SetValue(Grid.ColumnProperty, col);
                    tBtn.Content = index;

                    Binding binding = CreateBinding(this, BindingMode.TwoWay, string.Format($"{nameof(this.DataContext)}.{nameof(ViewModel.SelectedIndex)}"));
                    binding.Converter = new PresetIndexToCheckState();
                    binding.ConverterParameter = index;
                    tBtn.SetBinding(ToggleButton.IsCheckedProperty, binding);

                    gridMain.Children.Add(tBtn);
                    index++;
                }
            }
        }

        private Binding CreateBinding(object source, BindingMode mode, string path)
        {
            Binding binding = new Binding();
            binding.Mode = mode;
            binding.Source = source;
            if (path != null)
                binding.Path = new PropertyPath(path);
            return binding;
        }
    }

    public class PresetIndexToCheckState : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int index = (int)value;
            int compare = System.Convert.ToInt32(parameter);
            if (index != compare)
                return false;
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool checkstate = (bool)value;
            int compare = System.Convert.ToInt32(parameter);
            if (!checkstate)
                return -1;
            return compare;
        }
    }

    public class PresetIndexToEnable : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int index = (int)value;
            if (index > 0)
                return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

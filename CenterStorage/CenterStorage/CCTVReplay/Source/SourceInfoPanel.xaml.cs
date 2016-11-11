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

namespace CCTVReplay.Source
{
    /// <summary>
    /// SourceInfoPanel.xaml 的交互逻辑
    /// </summary>
    public partial class SourceInfoPanel : UserControl
    {
        public SourceInfoViewModel ViewModel { get { return DataContext as SourceInfoViewModel; } }

        public SourceInfoPanel()
        {
            InitializeComponent();
            DataContextChanged += thisDataContextChanged;
        }

        private void thisDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            SourceInfoViewModel oldVM = e.OldValue as SourceInfoViewModel;
            SourceInfoViewModel newVM = e.NewValue as SourceInfoViewModel;
            if (oldVM != null)
                oldVM.PropertyChanged -= Model_PropertyChanged;
            if (newVM != null)
                newVM.PropertyChanged += Model_PropertyChanged;
        }

        private bool _modelUpdating = false;
        private bool _userInput = false;
        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _modelUpdating = true;
            switch (e.PropertyName)
            {
                case nameof(SourceInfoViewModel.Password):
                    if (!_userInput)
                        pwdBox.Password = ViewModel.Password;
                    break;
            }
            _modelUpdating = false;
        }

        private void pwdBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null && !_modelUpdating)
            {
                _userInput = true;
                ViewModel.Password = pwdBox.Password;
                _userInput = false;
            }
        }
    }

    public class SourceTypeToVisiConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SourceTypeView stv = value as SourceTypeView;
            if (stv == null)
                return Visibility.Collapsed;

            SourceType st;
            if (Enum.TryParse(parameter.ToString(), out st))
                return stv.SourceType == st ? Visibility.Visible : Visibility.Collapsed;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToVisiConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;
            bool flag = (bool)value;
            bool compare = bool.Parse(parameter.ToString());
            return (flag ^ compare) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

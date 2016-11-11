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
using CenterStorageCmd;
using CCTVDownload.Download;
using CenterStorageCmd.Url;
using CCTVDownload.Util;

namespace CCTVDownload
{
    /// <summary>
    /// DownloadsControl.xaml 的交互逻辑
    /// </summary>
    public partial class DownloadsControl : UserControl
    {
        public DownloadsViewModel ViewModel { get { return DataContext as DownloadsViewModel; } }
        public DownloadsControl()
        {
            InitializeComponent();
            this.DataContextChanged += DownloadersControl_DataContextChanged;
        }

        private void DownloadersControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var newModel = e.NewValue as DownloadsViewModel;
            if (newModel != null && newModel.CommandModel != null)
            {
                newModel.CommandModel.PropertyChanged += CommandModel_PropertyChanged;
            }
        }

        private void CommandModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ViewModel.CommandModel.MultiSelected):
                    listModel.SelectedValue = null;
                    if (ViewModel.CommandModel.MultiSelected)
                        listModel.SelectionMode = SelectionMode.Multiple;
                    else
                        listModel.SelectionMode = SelectionMode.Single;
                    break;
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var removeList = e.RemovedItems;
            foreach (var rem in removeList)
            {
                var model = rem as DownloadViewModel;
                if (model != null)
                    model.Selected = false;
            }

            var addList = e.AddedItems;
            foreach (var add in addList)
            {
                var model = add as DownloadViewModel;
                if (model != null)
                    model.Selected = true;
            }
        }
    }

    public class DownloadShowTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DownloadViewSort sort = (DownloadViewSort)Enum.Parse(typeof(DownloadViewSort), value.ToString());
            string result = "";
            switch (sort)
            {
                case DownloadViewSort.正在下载:
                    result = @"..\Images\downloadcontrol\download.png";
                    break;
                case DownloadViewSort.已完成:
                    result = @"..\Images\downloadcontrol\finished.png";
                    break;
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

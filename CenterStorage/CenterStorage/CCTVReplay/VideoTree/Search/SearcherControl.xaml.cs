using CCTVReplay.Util;
using CCTVReplay.VideoTree.Model;
using CCTVReplay.VideoTree.Thumbnail;
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
using Telerik.Windows.Controls;

namespace CCTVReplay.VideoTree.Search
{
    /// <summary>
    /// SearcherControl.xaml 的交互逻辑
    /// </summary>
    public partial class SearcherControl : UserControl
    {
        public SearcherViewModel ViewModel
        {
            get { return DataContext as SearcherViewModel; }
        }

        public SearcherControl()
        {
            InitializeComponent();
            resultList.AddHandler(FrameworkElement.MouseWheelEvent, new MouseWheelEventHandler(ResultList_MouseWheel), true);
        }

        private void ResultList_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer viewer = resultList.FindVisualChild<ScrollViewer>();
            if (viewer != null)
            {
                if (e.Delta > 0)
                {
                    viewer.LineUp();
                    viewer.LineUp();
                    viewer.LineUp();
                }
                else {
                    viewer.LineDown();
                    viewer.LineDown();
                    viewer.LineDown();
                }
            }
        }

        private void videoListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            object selectedObj = null;
            var listbox = sender as ListBox;
            if (listbox != null)
            {
                selectedObj = listbox.SelectedValue;
                listbox.SelectedValue = null;
            }
            else
            {
                var radListBox = sender as RadListBox;
                if (radListBox != null)
                {
                    selectedObj = radListBox.SelectedValue;
                    radListBox.SelectedValue = null;
                }
            }
            if (selectedObj != null)
            {
                var selected = selectedObj as VideoThumbnailsViewModel;
                if (selected != null)
                    ViewModel.PlayVideo(selected.ID, selected.StreamId, selected.Name);
            }
            e.Handled = true;
        }

        private void RadListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ViewModel.OpenSelectedNode();
        }
    }

    class BoolToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string para = parameter as string;
            bool exp = (string.IsNullOrWhiteSpace(para) || para.Equals("1"));
            if ((bool)value == exp)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class VideoTypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = (CctvNode.NodeType)value;
            string param = (string)parameter;
            if (param == "Height")
            {
                if (type == CctvNode.NodeType.Unknown)
                    return 0;
                else
                    return 35;
            }
            else if (param == "Node")
            {
                if (type == CctvNode.NodeType.Unknown)
                    return Visibility.Collapsed;
                else
                    return Visibility.Visible;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class VideoIDGroupConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SearchNodeHistoryViewModel model = value as SearchNodeHistoryViewModel;
            List<string> ids = new List<string>();
            FindAllVideo(model.Node, ids);
            return ids;
        }

        private void FindAllVideo(CctvNode node, List<string> ids)
        {
            if (node != null)
            {
                if (node.Type == CctvNode.NodeType.Video)
                {
                    ids.Add(node.ID);
                }
                else
                {
                    if (node.Children != null)
                        foreach (CctvNode child in node.Children)
                            FindAllVideo(child, ids);
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class ThumbnailBackgroundConveter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool status = (bool)value;
            if ("border".Equals(parameter))
            {
                if (status)
                    return new SolidColorBrush(Color.FromRgb(0x47, 0xe1, 0x13));
                else
                    return new SolidColorBrush(Color.FromRgb(0x10, 0x26, 0x42));
            }
            else
            {
                if (status)
                    return new SolidColorBrush(Color.FromRgb(0x47, 0xe1, 0x13));
                else
                    return new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class CctvTypeToPngConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            CctvNode.NodeType type = (CctvNode.NodeType)value;
            if (type == CctvNode.NodeType.Server)
                return @"../../Images/Search/server.png";
            else
                return @"../../Images/Search/DVR.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MultiCctvTypeToPngConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            CctvNode.NodeType type = CctvNode.NodeType.Unknown;
            bool isOnline = true;
            try
            {
                if (!DependencyProperty.UnsetValue.Equals(values[0]))
                {
                    type = (CctvNode.NodeType)values[0];
                    isOnline = (bool)values[1];
                }
            }
            catch (Exception)
            {
                Console.WriteLine("MultiCctvTypeToPngConverter {0} ---- {1}", values[0], values[1]);
            }
            if (type == CctvNode.NodeType.Server)
            {
                if (isOnline)
                    return getImage(@"../../Images/Search/server.png");
                else
                    return getImage(@"../../Images/Search/server_灰色.png");
            }
            else
            {
                if (isOnline)
                    return getImage(@"../../Images/Search/DVR.png");
                else
                    return getImage(@"../../Images/Search/DVR_灰色.png");
            }
        }

        private BitmapImage getImage(string uri)
        {
            return new BitmapImage(new Uri(uri, UriKind.Relative));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

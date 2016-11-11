using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using VideoNS.Helper;
using VideoNS.SplitScreen;

namespace VideoNS.SubWindow
{
    /// <summary>
    /// DialogWin.xaml 的交互逻辑
    /// </summary>
    public partial class DialogWin : Window
    {
        private DialogWin()
        {
            InitializeComponent();
        }

        private DialogWinModel ViewModel { get { return DataContext as DialogWinModel; } }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private static Window FindTopWin(UIElement relative)
        {
            if (relative != null)
                return relative.FindVisualParent<Window>();
            return null;
        }

        public static bool? Show(string message, string title, bool showCancelButton, DialogWinImage image, UIElement relative)
        {
            DialogWin win = new DialogWin();
            Window owner = FindTopWin(relative);
            if (owner != null)
            {
                win.Owner = owner;
                win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
            win.ViewModel.Content = message;
            win.ViewModel.Image = image;
            if (title != null)
                win.ViewModel.Title = title;
            win.ViewModel.ButtonCancelVisible = showCancelButton ? Visibility.Visible : Visibility.Collapsed;
            SplitScreenControl.SetCoverVisible(true);
            bool? flag = win.ShowDialog();
            SplitScreenControl.SetCoverVisible(false);
            return flag;
        }

        public static bool? Show(string message, string title, bool showCancelButton, DialogWinImage image)
        {
            return Show(message, title, showCancelButton, image, null);
        }

        public static bool? Show(string message, string title, DialogWinImage image)
        {
            return Show(message, title, false, image);
        }

        public static bool? Show(string message, string title, DialogWinImage image, UIElement relative)
        {
            return Show(message, title, false, image, relative);
        }

        public static bool? Show(string message, DialogWinImage image, UIElement relative)
        {
            string title = null;
            switch (image)
            {
                case DialogWinImage.Error:
                    title = "错误";
                    break;
                case DialogWinImage.Warning:
                    title = "警告";
                    break;
                case DialogWinImage.Information:
                    title = "提示";
                    break;
                case DialogWinImage.None:
                default:
                    title = "";
                    break;
            }
            return Show(message, title, image, relative);
        }

        public static bool? Show(string message, DialogWinImage image)
        {
            return Show(message, image, null);
        }

        public static bool? Show(string message, UIElement relative)
        {
            return Show(message, DialogWinImage.None, relative);
        }

        public static bool? Show(string message)
        {
            return Show(message, DialogWinImage.None);
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }

    public enum DialogWinImage
    {
        Information,
        Error,
        Warning,
        None
    }
}

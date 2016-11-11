using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace CCTVDownload
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MaxHeight = SystemParameters.WorkArea.Height + 12;
            MaxWidth = SystemParameters.WorkArea.Width + 12;
            DataContext = DownloadManager.Instance.DownloadsViewModel;
        }

        #region 【窗体控制事件】
        private void CloseCmdHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void CanCloseExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void MaximizeCmdHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
        }

        private void CanMaximizeExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.WindowState != WindowState.Maximized;
        }

        private void MinimizeCmdHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CanMinimizeExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void RestoreCmdHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.WindowState = WindowState.Normal;
        }

        private void CanRestoreExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.WindowState != WindowState.Normal;
        }
        #endregion 【窗体控制事件】

        private void onMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                    base.OnMouseLeftButtonDown(e);
                    this.DragMove();
            }
            catch { }
        }

        private void onMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            if (this.WindowState == WindowState.Normal)
                this.WindowState = WindowState.Maximized;
            else
                this.WindowState = WindowState.Normal;
        }
    }
}

using CCTVDownload.AutoSave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using Forms = System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CCTVDownload.Download
{
    /// <summary>
    /// NewTaskWin.xaml 的交互逻辑
    /// </summary>
    public partial class NewTaskWin : Window
    {
        public NewTaskViewModel ViewModel { get { return DataContext as NewTaskViewModel; } }
        public NewTaskWin()
        {
            InitializeComponent();
            MaxHeight = SystemParameters.WorkArea.Height + 12;
            MaxWidth = SystemParameters.WorkArea.Width + 12;
            addOrRemoveEvent(this.DataContext as NewTaskViewModel, true);
            this.DataContextChanged += DownloadCheckWindow_DataContextChanged;
        }

        private void DownloadCheckWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            addOrRemoveEvent(e.OldValue as NewTaskViewModel, false);
            addOrRemoveEvent(e.NewValue as NewTaskViewModel, true);
        }

        private void addOrRemoveEvent(NewTaskViewModel model, bool isInstall)
        {
            if (model != null)
            {
                if (isInstall)
                {
                    model.StartDownload += Model_StartDownload;
                }
                else
                {
                    model.StartDownload -= Model_StartDownload;
                }
            }
        }

        private void Model_StartDownload(object sender, EventArgs args)
        {
            if (string.IsNullOrWhiteSpace(ViewModel.DownloadDirectory))
                return;
            try
            {
                string path = ViewModel.DownloadDirectory;
                if (string.IsNullOrEmpty(ViewModel.URLInfo.LocalPath))
                    path = ViewModel.DownloadDirectory;
                DirectoryInfo di = new DirectoryInfo(path);
                if (!di.Exists)
                {
                    if (!di.Root.Exists)
                        throw new AggregateException("错误的存储路径，不存在当前根目录！");
                    di.Create();
                }
                CustomSettingAutoSave.Instance.Setting.DownloadPath = di.FullName;
                this.DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "异常", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDirSel_Click(object sender, RoutedEventArgs e)
        {
            Forms.FolderBrowserDialog ofd = new Forms.FolderBrowserDialog();
            ofd.SelectedPath = ViewModel.DownloadDirectory;
            if (ofd.ShowDialog() == Forms.DialogResult.OK)
            {
                ViewModel.DownloadDirectory = ofd.SelectedPath;
                CustomSettingAutoSave.Instance.Setting.DownloadPath = ViewModel.DownloadDirectory;
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
            if (e.ClickCount >= 2)
            {
                if (ViewModel.IsMaximize)
                    Restore_Click(null, null);
                else
                    Maximize_Click(null, null);
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Restore_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Normal;
            ViewModel.IsMaximize = false;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
            ViewModel.IsMaximize = true;
        }


        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

    }
}

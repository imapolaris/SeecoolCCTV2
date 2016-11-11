using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Forms = System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CCTVReplay.AutoSave;
using System.IO;

namespace CCTVReplay.Video
{
    /// <summary>
    /// PathSelectWin.xaml 的交互逻辑
    /// </summary>
    public partial class PathSelectWin : Window
    {
        public PathSelectWin()
        {
            InitializeComponent();
            txtPath.Text = CustomSettingAutoSave.Instance.Setting.DownloadPath;
            this.Owner = Application.Current.MainWindow;
        }

        public string DownPath { get { return txtPath.Text; } }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btnSel_Click(object sender, RoutedEventArgs e)
        {
            Forms.FolderBrowserDialog ofd = new Forms.FolderBrowserDialog();
            ofd.SelectedPath = txtPath.Text;
            if (ofd.ShowDialog() == Forms.DialogResult.OK)
            {
                txtPath.Text = ofd.SelectedPath;
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            string path = txtPath.Text;
            path = System.IO.Path.GetDirectoryName(path);
            if (string.IsNullOrWhiteSpace(path))
                throw new ErrorMessageException("无效路径配置。");
            var dir = new DirectoryInfo(txtPath.Text);
            if (!dir.Exists)
                dir.Create();
            CustomSettingAutoSave.Instance.Setting.DownloadPath = dir.FullName;
            this.DialogResult = true;
            this.Close();
        }

        private void headerBtnDownHandler(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}

using CCTVDownloadService;
using Common.Log;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CCTVDownloadServer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        DownloadSocketsManager _sockets;
        OnlineDownloadsManager _downloads;
        public MainWindow()
        {
            InitializeComponent();
            
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            Logger.Default.Trace("-----------------开始下载服务-----------------");
            try
            {
                _sockets = DownloadSocketsManager.Instance;
                _downloads = OnlineDownloadsManager.Instance;
            }
            catch (TypeInitializationException ex)
            {
                Logger.Default.Error(ex);
                MessageBox.Show(ex.InnerException.Message);
                Close();
            }
            catch (Exception ex)
            {
                Logger.Default.Error(ex);
                MessageBox.Show(ex.ToString());
                Close();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Logger.Default.Trace("-----------------停止下载服务-----------------");
        }
    }
}

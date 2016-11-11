using CenterStorageService;
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
using Common.Log;

namespace StorageDataServer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        SocketManager _sm;
        StorageFlagsManager _sfm;
        public MainWindow()
        {
            InitializeComponent();
            Logger.Default.Trace("------------------开始集中存储服务------------------");
            GlobalData.StaticInfoAddress = System.Configuration.ConfigurationManager.AppSettings["StaticInfoAddress"];
            GlobalData.Path = System.Configuration.ConfigurationManager.AppSettings["StorageFilePath"];
            updateHardDiskFreeSpaceInf();   //更新最小磁盘空间剩余量限制
            _sfm = StorageFlagsManager.Instance;
            _sm = SocketManager.Instance;
            Logger.Default.Trace("服务已启动");
            Logger.Default.Trace(GlobalData.Path);
            Logger.Default.Trace(GlobalData.StaticInfoAddress);
        }

        private void updateHardDiskFreeSpaceInf()
        {
            try
            {
                long length = GlobalData.HardDiskFreeSpaceInf;
                string str = System.Configuration.ConfigurationManager.AppSettings["HardDiskFreeSpaceInf"];
                switch (str.Last())
                {
                    case 'k':
                    case 'K':
                        length = (long)(double.Parse(str.Substring(0, str.Length - 1)) * 1024);
                        break;
                    case 'm':
                    case 'M':
                        length = (long)(double.Parse(str.Substring(0, str.Length - 1)) * 1024 * 1024);
                        break;
                    case 'g':
                    case 'G':
                        length = (long)(double.Parse(str.Substring(0, str.Length - 1)) * 1024 * 1024 * 1024);
                        break;
                    default:
                        length = (long)double.Parse(str);
                        break;
                }
                GlobalData.HardDiskFreeSpaceInf = length;
            }
            catch(Exception ex)
            {
                Logger.Default.Info("更新磁盘剩余空间大小限制失败，采用默认3G设置！ " + ex.ToString());
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Logger.Default.Trace("------------------结束集中存储服务------------------");
        }
    }
}

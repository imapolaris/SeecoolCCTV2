using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using CCTVInfoHub;
using CCTVInfoHub.Entity;
using CCTVModels;

namespace CCTVConnectorDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public const string WebApiUrl = "http://192.168.9.222:27010/";
        private CCTVDefaultInfoSync _hub;
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _hub = new CCTVDefaultInfoSync(WebApiUrl);
            _hub.RegisterDefaultWithoutUpdate(CCTVInfoType.GlobalInfo);
            _hub.RegisterDefaultWithoutUpdate(CCTVInfoType.StaticInfo);
            _hub.RegisterDefaultWithoutUpdate(CCTVInfoType.DynamicInfo);
            _hub.RegisterDefaultWithoutUpdate(CCTVInfoType.OnlineStatus);
            _hub.RegisterDefaultWithoutUpdate(CCTVInfoType.HierarchyInfo);
            CCTVHierarchyNode[] root = _hub.GetAllHierarchyRoots();
            Console.WriteLine("");
        }

        private void onHierInfoUpdate(IEnumerable<string> updatedKeys)
        {
            Console.WriteLine("Hierarchy");
        }

        private void onStaticInfoUpdate(IEnumerable<CCTVStaticInfo> values, IEnumerable<string> updatedKeys)
        {
            Console.WriteLine("static");
        }

        private void onGlobalInfoUpdate(IEnumerable<CCTVGlobalInfo> values, IEnumerable<string> updatedKeys)
        {
            _hub.GetGlobalInfo();
            Console.WriteLine("over");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CCTVHierarchyNode[] roots = _hub.GetAllHierarchyRoots();
            CCTVOnlineStatus[] oss = _hub.GetAllOnlineStatus();
        }
    }
}

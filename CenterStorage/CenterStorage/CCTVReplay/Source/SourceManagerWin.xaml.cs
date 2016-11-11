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
using CCTVReplay.StaticInfo;

namespace CCTVReplay.Source
{
    /// <summary>
    /// SourceManagerWin.xaml 的交互逻辑
    /// </summary>
    public partial class SourceManagerWin : Window
    {
        public SourceManagerViewModel ViewModel { get { return DataContext as SourceManagerViewModel; } }

        public SourceManagerWin()
        {
            InitializeComponent();
            ViewModel.Connected += ViewModel_Connected;
            this.Loaded += SourceManagerWin_Loaded;
        }

        private void SourceManagerWin_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.SetCurrent(VideoInfoManager.Instance.GlobalDataSource);
        }

        private void ViewModel_Connected(object sender, EventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void headerBtnDownHandler(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}

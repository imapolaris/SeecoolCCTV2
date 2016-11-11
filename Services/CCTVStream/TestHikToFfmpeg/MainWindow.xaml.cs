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

namespace TestHikToFfmpeg
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        VideoPlayViewModel _videoPlay;
        public MainWindow()
        {
            InitializeComponent();
            _videoPlay = new VideoPlayViewModel();
            this.DataContext = _videoPlay;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _videoPlay.Dispose();
        }
    }
}

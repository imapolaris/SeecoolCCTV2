using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.IO;
using System.ComponentModel;

namespace TestCCTVStreamCmd
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        TestHikPlayer hik = null;
        StreamSocket _socket;
        RtspPlayViewModel _rtsp;
        public MainWindow()
        {
            InitializeComponent();
            //this.Loaded += onLoad;
            this.DataContext = new HikPlayViewModel();
            //this.DataContext = new RtspPlayViewModel();
            //_socket = new StreamSocket();
        }

        private void onLoad(object sender, RoutedEventArgs e)
        {
            //IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            IntPtr hwnd = ((HwndSource)PresentationSource.FromVisual(grid1)).Handle;
            hik = new TestHikPlayer(IntPtr.Zero);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            (DataContext as RtspPlayViewModel)?.Dispose();
            (DataContext as HikPlayViewModel)?.Dispose();
            hik?.Dispose();
            _socket?.Dispose();
        }
    }

}

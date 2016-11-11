using CCTVReplay.Source;
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

namespace CCTVReplay
{
    /// <summary>
    /// WindowControlBox.xaml 的交互逻辑
    /// </summary>
    public partial class WindowControlBox : UserControl
    {
        public WindowControlBox()
        {
            InitializeComponent();
        }

        private void btnSrcSwitch_Click(object sender, RoutedEventArgs e)
        {
            SourceManagerWin win = new SourceManagerWin();
            win.Owner = Application.Current.MainWindow;
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            win.ShowDialog();
        }
    }
}

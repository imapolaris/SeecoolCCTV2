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

namespace CCTVReplay.Combo
{
    /// <summary>
    /// TimeSelectorWin.xaml 的交互逻辑
    /// </summary>
    public partial class TimeSelectorWin : Window
    {

        public TimeSelectorViewModel ViewModel { get { return DataContext as TimeSelectorViewModel; } }

        public TimeSelectorWin()
        {
            InitializeComponent();
        }

        private void headerBtnDownHandler(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}

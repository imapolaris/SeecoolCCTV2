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

namespace ConfSetting
{
    /// <summary>
    /// TipWin.xaml 的交互逻辑
    /// </summary>
    public partial class TipWin : Window
    {
        public TipWin()
        {
            InitializeComponent();
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

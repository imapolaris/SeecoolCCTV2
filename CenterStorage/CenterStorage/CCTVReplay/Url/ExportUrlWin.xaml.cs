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
using CenterStorageCmd.Url;

namespace CCTVReplay.Url
{
    /// <summary>
    /// ExportUrlWin.xaml 的交互逻辑
    /// </summary>
    public partial class ExportUrlWin : Window
    {
        public ExportUrlWin()
        {
            InitializeComponent();
        }

        private void headerBtnDownHandler(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();   
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        
        public void SetExportUrl(IUrl ui)
        {
            txtUrl.Text = ui.ToString();
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(txtUrl.Text);
            txtUrl.Focus();
            txtUrl.SelectAll();
        }
    }
}

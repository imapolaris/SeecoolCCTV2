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
using Telerik.Windows.Controls;

namespace VideoNS.SubWindow
{
    /// <summary>
    /// PopupWin.xaml 的交互逻辑
    /// </summary>
    public partial class PopupWin : RadWindow
    {
        public PopupWin()
        {
            InitializeComponent();
        }

        protected override void OnClosed()
        {
            base.OnClosed();
            if (ViewModel != null)
                ViewModel.ControlModel.Dispose();
        }

        public PopupWinModel ViewModel { get { return DataContext as PopupWinModel; } } 
    }
}

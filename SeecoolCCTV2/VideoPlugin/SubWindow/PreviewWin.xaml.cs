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
using VideoNS.SplitScreen;

namespace VideoNS.SubWindow
{
    /// <summary>
    /// PreviewWin.xaml 的交互逻辑
    /// </summary>
    public partial class PreviewWin : Window
    {
        internal PreviewWinModel ViewModel { get { return DataContext as PreviewWinModel; } }

        private PreviewWin()
        {
            InitializeComponent();
            this.MouseMove += PreviewWin_MouseMove;
            ViewModel.SaveAction += ViewModel_SaveAction;
        }

        private void ViewModel_SaveAction()
        {
            this.DialogResult = true;
        }

        private void PreviewWin_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        public static bool? Show(System.Drawing.Image img,string videoName)
        {
            PreviewWin win = new PreviewWin();
            win.ViewModel.VideoName = videoName;
            win.ViewModel.LoadImage(img);
            SplitScreenControl.SetCoverVisible(true);
            bool? flag = win.ShowDialog();
            SplitScreenControl.SetCoverVisible(false);
            return flag;
        }
    }
}

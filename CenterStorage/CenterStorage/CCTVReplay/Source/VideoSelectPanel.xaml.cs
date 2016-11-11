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

namespace CCTVReplay.Source
{
    /// <summary>
    /// VideoSelectPanel.xaml 的交互逻辑
    /// </summary>
    public partial class VideoSelectPanel : UserControl
    {
        public VideoSelectPanel()
        {
            InitializeComponent();
        }

        //刷新时间间隔。
        public void UpdateTimePeriod(DateTime? begin, DateTime? end)
        {
            if (begin != null && end != null)
                searcher.ViewModel.SearchTimePeriod = new CenterStorageCmd.TimePeriodPacket(begin.GetValueOrDefault(), end.GetValueOrDefault());
        }
    }
}

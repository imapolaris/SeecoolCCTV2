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

namespace VideoNS.Adorners
{
    /// <summary>
    /// VideoPanelDropBlur.xaml 的交互逻辑
    /// </summary>
    public partial class VideoPanelDropBlur : UserControl
    {
        public VideoPanelDropBlur()
        {
            InitializeComponent();
        }
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            this.Width = arrangeBounds.Width;
            this.Height = arrangeBounds.Height;
            return base.ArrangeOverride(arrangeBounds);
        }
    }
}

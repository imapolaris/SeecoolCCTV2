using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace VideoNS.SubWindow
{
    /// <summary>
    /// GridBlock.xaml 的交互逻辑
    /// </summary>
    public partial class GridBlock : UserControl
    {
        #region 【路由事件定义】
        public static readonly RoutedEvent DeleteEvent = EventManager.RegisterRoutedEvent
            ("Delete", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GridBlock));

        public event RoutedEventHandler Delete
        {
            add { this.AddHandler(DeleteEvent, value); }
            remove { this.RemoveHandler(DeleteEvent, value); }
        }
        protected virtual void OnDelete()
        {
            this.RaiseEvent(new RoutedEventArgs(DeleteEvent, this));
        }
        #endregion 【路由事件定义】

        internal GridBlockModel ViewModel { get { return DataContext as GridBlockModel; } }
        public GridBlock()
        {
            InitializeComponent();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            OnDelete();
        }
    }
}

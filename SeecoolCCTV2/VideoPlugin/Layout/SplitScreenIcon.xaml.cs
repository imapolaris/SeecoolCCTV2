using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using VideoNS.Model;

namespace VideoNS.Layout
{
    /// <summary>
    /// SplitScreenIcon.xaml 的交互逻辑
    /// </summary>
    public partial class SplitScreenIcon : UserControl
    {
        public SplitScreenIcon()
        {
            InitializeComponent();
        }

        #region  依赖属性
        public static readonly DependencyProperty IconContextProperty =
            DependencyProperty.Register("IconContext", typeof(SplitScreenLayoutModel), typeof(SplitScreenIcon),
                                        new FrameworkPropertyMetadata(null, new PropertyChangedCallback(onIconContextChanged)));

        [Description("获取或设置显示的内容")]
        public SplitScreenLayoutModel IconContext
        {
            get { return (SplitScreenLayoutModel)GetValue(IconContextProperty); }
            set { SetValue(IconContextProperty, value); }
        }

        private static void onIconContextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs arg)
        {
            if (sender is SplitScreenIcon)
                updateIconContext(sender as SplitScreenIcon);
        }

        private static void updateIconContext(SplitScreenIcon splitScreenIcon)
        {
            if (splitScreenIcon == null)
                return;
            splitScreenIcon.reloadUI();
        }
        #endregion 依赖属性

        /// <summary>
        /// 重新加载界面
        /// </summary>
        private void reloadUI()
        {
            clearUI();
            if (IconContext?.SplitScreenInfom?.Nodes != null && IconContext.SplitScreenInfom.Split >= 1)
            {                
                for (int i = 0; i < IconContext.SplitScreenInfom.Split; i++)
                {
                    grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                }
                addBorder(0, 0, IconContext.SplitScreenInfom.Split, IconContext.SplitScreenInfom.Split);
                foreach (SplitScreenNode node in IconContext.SplitScreenInfom.Nodes)
                    addBorder(node.Row, node.Column, node.RowSpan, node.ColumnSpan);
            }
            else
            {
                SplitScreenSettingIcon zidingyi = new SplitScreenSettingIcon();
                grid.Children.Add(zidingyi);
            }
        }

        private void addBorder(int row, int col, int rowSpan, int colSpan)
        {
            Border border = newBorder();
            border.SetValue(Grid.RowProperty, row);
            border.SetValue(Grid.ColumnProperty, col);
            border.SetValue(Grid.RowSpanProperty, rowSpan);
            border.SetValue(Grid.ColumnSpanProperty, colSpan);
            grid.Children.Add(border);
        }

        Border newBorder()
        {
            Border border = new Border();
            border.BorderBrush = Brushes.Gray;
            border.Opacity = 0.8;
            border.BorderThickness = new Thickness(0.5);
            return border;
        }

        private void clearUI()
        {
            if (grid != null)
            {
                grid.Children.Clear();
                grid.ColumnDefinitions.Clear();
                grid.RowDefinitions.Clear();
            }
        }
    }
}

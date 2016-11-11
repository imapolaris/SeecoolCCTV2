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

namespace VideoNS.TimeSwitch
{
    /// <summary>
    /// LayoutPlanPanel.xaml 的交互逻辑
    /// </summary>
    public partial class LayoutPlanPanel : UserControl
    {
        public LayoutPlanPanel()
        {
            InitializeComponent();
            DataContextChanged += onDataContextChanged;
        }

        private void onDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            reloadUI();
        }
        public LayoutPlanModel ViewModel
        {
            get { return DataContext as LayoutPlanModel; }
        }

        private void reloadUI()
        {
            clearUI();
            if (ViewModel?.LayoutSource != null)
            {
                LayoutPlanItem item = new LayoutPlanItem();
                item.DataContext = ViewModel;
                grid.Children.Add(item);
            }
            else
            {
                PlusPanel panel = new PlusPanel();
                grid.Children.Add(panel);
            }
        }

        private void clearUI()
        {
            if (grid != null)
            {
                foreach (var child in grid.Children)
                {
                    var item = (child as LayoutPlanItem);
                    if (item != null)
                    {
                        item.DataContext = null;
                    }
                }
                grid.Children.Clear();
                grid.ColumnDefinitions.Clear();
                grid.RowDefinitions.Clear();
            }
        }

        ~LayoutPlanPanel()
        {
            try
            {
                Common.Util.WindowUtil.Invoke(() =>
                {
                    clearUI();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("~LayoutPlanPanel {0}", ex.Message);
            }
        }
    }
}

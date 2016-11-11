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
    /// LayoutPlanItemPanel.xaml 的交互逻辑
    /// </summary>
    public partial class LayoutPlanItemPanel : UserControl
    {
        public LayoutPlanItemPanel()
        {
            InitializeComponent();
            DataContextChanged += LayoutPlanItemPanel_DataContextChanged;
        }

        private void LayoutPlanItemPanel_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            reloadUI();
        }

        public SplitScreenInfo ViewModel
        {
            get { return DataContext as SplitScreenInfo; }
        }
        
        private void reloadUI()
        {
            DateTime timeStart = DateTime.Now;
            ClearUI();
            if (ViewModel != null && ViewModel.Nodes != null && ViewModel.Split >= 1)
            {
                for (int i = 0; i < ViewModel.Split; i++)
                {
                    grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                }
                foreach (SplitScreenNode node in ViewModel.Nodes)
                    addSnapshot(node);
            }
        }

        public void ClearUI()
        {
            if (grid != null)
            {
                foreach (var child in grid.Children)
                {
                    var item = child as LayoutIcon;
                    if (item != null)
                        item.Dispose();
                }
                grid.Children.Clear();
                grid.ColumnDefinitions.Clear();
                grid.RowDefinitions.Clear();
                GC.SuppressFinalize(this);
            }
        }

        private void addSnapshot(SplitScreenNode node)
        {
            LayoutIcon panel = new LayoutIcon() { DataContext = node };
            panel.SetBinding(Grid.RowProperty, CreateBinding(node, BindingMode.TwoWay, nameof(node.Row)));
            panel.SetBinding(Grid.ColumnProperty, CreateBinding(node, BindingMode.TwoWay, nameof(node.Column)));
            panel.SetBinding(Grid.RowSpanProperty, CreateBinding(node, BindingMode.TwoWay, nameof(node.RowSpan)));
            panel.SetBinding(Grid.ColumnSpanProperty, CreateBinding(node, BindingMode.TwoWay, nameof(node.ColumnSpan)));
            grid.Children.Add(panel);
        }

        private Binding CreateBinding(object source, BindingMode mode, string path)
        {
            Binding binding = new Binding();
            binding.Mode = mode;
            binding.Source = source;
            if (path != null)
                binding.Path = new PropertyPath(path);
            return binding;
        }
    }
}

using DragDropHelper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using VideoNS.Helper;

namespace VideoNS.TimeSwitch
{
    /// <summary>
    /// TimeSwitchPlanControl.xaml 的交互逻辑
    /// </summary>
    public partial class TimeSwitchPlanControl : UserControl
    {
        public TimeSwitchViewModel ViewModel { get { return DataContext as TimeSwitchViewModel; } }

        public TimeSwitchPlanControl()
        {
            InitializeComponent();
            PlansListbox.AddHandler(FrameworkElement.MouseWheelEvent, new MouseWheelEventHandler(ResultList_MouseWheel), true);
        }
        private void ResultList_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer viewer = PlansListbox.FindVisualChild<ScrollViewer>();
            if (viewer != null)
            {
                if (e.Delta > 0)
                    viewer.LineLeft();
                else
                    viewer.LineRight();
            }
        }

        private void PlansListbox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ViewModel?.ShowSelected();
        }
    }

    public class TimeSwitchListDragHandler : IDragHandler
    {
        int _dragStartIndex = -1;
        public void DragEnd(DragInfo info)
        {
            if (_dragStartIndex < 0)
                return;
            RadListBox rlb = info.Source as RadListBox;
            if (rlb != null)
            {
                TimeSwitchViewModel vm = rlb.DataContext as TimeSwitchViewModel;
                if (vm != null)
                    vm.ToDropPlan(_dragStartIndex);
                _dragStartIndex = -1;
            }
        }

        public void Dragging(DragInfo info)
        {
        }

        public void DragStart(DragInfo info)
        {
            if (info.SourceItem == null || info.EventOriginalSource is TextBox)
            {
                info.Cancelled = true;
                return;
            }
            RadListBox rlb = info.Source as RadListBox;
            if (rlb != null)
            {
                //设置拖拽窗口视觉效果。
                IEnumerable<RadListBoxItem> items = rlb.FindVisualChildren<RadListBoxItem>();
                foreach (RadListBoxItem item in items)
                {
                    if (item.DataContext.Equals(rlb.SelectedItem))
                    {
                        ContentPresenter cp = item.FindVisualChild<ContentPresenter>();
                        if (cp != null)
                            rlb.SetValue(DragDropVisual.VisualProperty, cp);
                        else
                            rlb.SetValue(DragDropVisual.VisualProperty, item);
                        break;
                    }
                }

                TimeSwitchViewModel vm = rlb.DataContext as TimeSwitchViewModel;
                //如果没有选中项，或者选中项不是有效节点，取消拖拽。
                if (!(vm != null && vm.SelectedPlan != null && vm.SelectedPlan != TimeSwitchViewModel.AddPlanStatic))
                {
                    info.Cancelled = true;
                }
                else if (vm != null && vm.SelectedPlan != null)
                {
                    var list = vm.PlansSource.Source as ObservableCollection<LayoutPlanModel>;
                    _dragStartIndex = list.IndexOf(vm.SelectedPlan);
                    vm.ToDragPlan();
                }
            }
        }
    }

    public class TimeSwitchListDropHandler : IDropHandler
    {
        public void DragEnter(DropInfo info)
        {
        }

        public void DragLeave(DropInfo info)
        {
        }

        public void DragOver(DropInfo info)
        {
        }

        public void Drop(DropInfo info)
        {
            RadListBox rlb = info.Source as RadListBox;
            if (rlb != null)
            {
                TimeSwitchViewModel source = rlb.DataContext as TimeSwitchViewModel;
                if (source != null && source.DragPlan != null && source.DragPlan != TimeSwitchViewModel.AddPlanStatic)
                {
                    var dragSource = source.DragPlan;
                    TimeSwitchViewModel target = (info.Target as RadListBox)?.DataContext as TimeSwitchViewModel;
                    if (target != source)
                        return;
                    if (target != null)
                    {
                        var list = target.PlansSource.Source as ObservableCollection<LayoutPlanModel>;
                        source.ToDropPlan(info.InsertIndex);
                        GC.Collect();
                    }
                }
            }
        }
    }
}

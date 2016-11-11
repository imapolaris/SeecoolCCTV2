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
using VideoNS.Helper;

namespace VideoNS.SubControls
{
    /// <summary>
    /// SwitchPanel.xaml 的交互逻辑
    /// </summary>
    public partial class SwitchPanel : UserControl
    {
        public SwitchPanelViewModel ViewModel { get { return DataContext as SwitchPanelViewModel; } }

        public SwitchPanel()
        {
            InitializeComponent();
            this.DataContextChanged += thisDataContextChanged;
        }

        private void thisDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            SwitchPanelViewModel oldM = e.OldValue as SwitchPanelViewModel;
            SwitchPanelViewModel newM = e.NewValue as SwitchPanelViewModel;
            if (oldM != null)
                addOrRemoveEvent(oldM, false);
            if (newM != null)
                addOrRemoveEvent(newM, true);
            refreshSwitchBtns(ViewModel);
        }

        private void addOrRemoveEvent(SwitchPanelViewModel model, bool isAdd)
        {
            if (model != null)
            {
                if (isAdd)
                    model.SwitchesUpdated += Model_SwitchsUpdated;
                else
                    model.SwitchesUpdated -= Model_SwitchsUpdated;
            }
        }

        private void Model_SwitchsUpdated(object sender, EventArgs e)
        {
            refreshSwitchBtns(ViewModel);
        }

        private void refreshSwitchBtns(SwitchPanelViewModel model)
        {
            clearSwitchBtns();
            if (model != null)
                for (int i = 0; i < model.AllSwitchStatus.Count; i++)
                {
                    Button btn = new Button();
                    Binding binding = BindingHelper.CreateBinding(model.AllSwitchStatus[i], BindingMode.OneWay, null);
                    btn.SetBinding(Button.DataContextProperty, binding);
                    wrapSwitches.Children.Add(btn);
                }
        }

        private void clearSwitchBtns()
        {
            wrapSwitches.Children.Clear();
        }
    }
}

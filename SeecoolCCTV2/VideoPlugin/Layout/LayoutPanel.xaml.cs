using Common.Message;
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
using VideoNS.Helper;
using VideoNS.Json;
using VideoNS.Model;
using VideoNS.SplitScreen;
using VideoNS.TimeSwitch;

namespace VideoNS.Layout
{
    /// <summary>
    /// LayoutPanel.xaml 的交互逻辑
    /// </summary>
    //[DesignTimeVisible(false)]
    public partial class LayoutPanel : UserControl
    {
        public LayoutViewModel ViewModel
        {
            get { return (LayoutViewModel)DataContext; }
        }

        public LayoutPanel()
        {
            InitializeComponent();
            DataContextChanged += onDataContextChanged;
            updateAction(ViewModel, true);
        }

        private void onDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            updateAction(e.OldValue as LayoutViewModel, false);
            updateAction(e.NewValue as LayoutViewModel, true);
        }

        void updateAction(LayoutViewModel model, bool isAdd)
        {
            if (model == null)
                return;
            if(isAdd)
            {
                model.SearcherModel.PlayVideoEvent += onPlayVideo;
                model.ClearAllDisplayEvent += onClearAll;
            }
            else
            {
                model.SearcherModel.PlayVideoEvent -= onPlayVideo;
                model.ClearAllDisplayEvent -= onClearAll;
            }
        }

        Panel _parent = null;
        TimeSwitchPlanControl _timeSwitch = null;
        private void TimingSwitch_Click(object sender, RoutedEventArgs e)
        {
            _parent = this.FindVisualParent<Panel>();
            if (_parent != null)
            {
                _parent.Children.Remove(this);
                _timeSwitch = new TimeSwitchPlanControl();
                _timeSwitch.ViewModel.ReturnAction += BackToCurrent;
                _parent.Children.Add(_timeSwitch);
            }
        }

        private void BackToCurrent()
        {
            if(_timeSwitch!=null)
                _timeSwitch.ViewModel.ReturnAction -= BackToCurrent;
            if (_parent != null)
            {
                _parent.Children.Remove(_timeSwitch);
                _parent.Children.Add(this);
                _timeSwitch = null;
            }
        }

        private void onClearAll()
        {
            SplitScreenPanel panel = FindSplitScreen();
            if (panel != null)
                panel.StopAll();
        }

        private void onPlayVideo(string videoId)
        {
            SplitScreenPanel panel = FindSplitScreen();
            if (panel != null)
                panel.PlayVideoOnBlank(videoId);
        }

        private SplitScreenPanel FindSplitScreen()
        {
            return borderSplitPanel.Child as SplitScreenPanel;
        }

        public void SetSplitScreen(SplitScreenPanel panel)
        {
            if (panel != null && !defaultSplitPanel.Disposed)
                defaultSplitPanel.Dispose();
            borderSplitPanel.Child = panel;
            if (panel != null)
                this.ViewModel.SplitScreenModel = panel.ViewModel;
        }

        private void RadListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ViewModel?.SplitLayoutsModel?.UpdateSelectedLayout();
        }
    }
}

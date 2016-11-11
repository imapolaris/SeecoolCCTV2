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
using DragDropHelper;
using VideoNS.DragDropHandler;
using VideoNS.Model;
using System.ComponentModel;
using System.Threading;
using System.Globalization;
using VideoNS.VideoInfo.Search;
using VideoNS.AutoSave;
using VideoNS.SubWindow;
using Common.Command;
using System.IO;
using VideoNS.Json;

namespace VideoNS.SplitScreen
{
    /// <summary>
    /// SplitScreenPanel.xaml 的交互逻辑
    /// </summary>
    public partial class SplitScreenPanel : UserControl, IDisposable
    {
        public SplitScreenModel ViewModel { get { return DataContext as SplitScreenModel; } }

        public SplitScreenPanel()
        {
            InitializeComponent();
            this.DataContextChanged += SplitScreenPanel_DataContextChanged;
            gridMain.AddHandler(VideoControl.FullScreenChangedEvent, new RoutedEventHandler(VideoControl_FullScreen));
            gridMain.AddHandler(VideoControl.ControlPanelVisibleChangedEvent, new RoutedEventHandler(VideoControl_PanelVisible));
            gridMain.AddHandler(VideoPanelItem.SearchPanelVisibleChangedEvent, new RoutedEventHandler(SearchPanel_Visible));
        }

        private async void SplitScreenPanel_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            SplitScreenModel oldModel = e.OldValue as SplitScreenModel;
            if (oldModel != null)
            {
                oldModel.PropertyChanged -= Model_PropertyChanged;
                oldModel.DataPrepare -= Model_DataPrepare;
            }
            SplitScreenModel newModel = e.NewValue as SplitScreenModel;
            if (newModel != null)
            {
                newModel.PropertyChanged += Model_PropertyChanged;
                newModel.DataPrepare += Model_DataPrepare;
            }
            await reloadUI();
        }

        SplitScreenInfo _prepareData;
        VideoPanelItem[] _prepareItems;
        bool _onPreparing = false;
        List<VideoPanelItem> _garbageItems = new List<VideoPanelItem>();
        private async void Model_DataPrepare(object sender, SplitScreenModel.PrepareEventArgs e)
        {
            if (!_onPreparing)
            {
                _onPreparing = true;
                _prepareData = e.PrepareData;
                _prepareItems = await CreateVideoPanels(e.PrepareData);
                _onPreparing = false;
            }
        }

        private async void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SplitScreenModel vm = sender as SplitScreenModel;
            switch (e.PropertyName)
            {
                case nameof(vm.SplitScreenData):
                    reposVideoItem();
                    await reloadUI();
                    break;
                case nameof(vm.IsOnEditting):
                    if (vm.IsOnEditting)
                    {
                        reposVideoItem();
                    }
                    setPanelEditStatus(!vm.IsOnEditting);
                    break;
                case nameof(vm.OnSwitching):
                case nameof(vm.SwitchPaused):
                    reposVideoItem();
                    break;
            }
        }

        public void UpdateSelectedVideos()
        {
            List<string> videoIds = ViewModel?.SplitScreenData?.Nodes?.Where(e => !string.IsNullOrWhiteSpace(e.VideoId)).Select(v => v.VideoId).ToList();
            for (int i = 0; i < gridMain.Children.Count; i++)
            {
                VideoPanelItem item = gridMain.Children[i] as VideoPanelItem;
                item.ViewModel.UpdateDisplayVideos(videoIds);
            }
        }

        private void setPanelEditStatus(bool editting)
        {
            for (int i = 0; i < gridMain.Children.Count; i++)
            {
                VideoPanelItem item = gridMain.Children[i] as VideoPanelItem;
                item.ViewModel.IsInEditStatus = editting;
            }
        }

        private void reposVideoItem()
        {
            topCover.ClearVideoPanel();
        }

        private bool _onLoading = false;
        /// <summary>
        /// 重新加载界面
        /// </summary>
        private async Task reloadUI()
        {
            while (_onLoading)
            {
                await Task.Delay(10);
            }
            _onLoading = true;
            ClearUI();
            if (ViewModel == null)
            {
                _onLoading = false;
                return;
            }
            SplitScreenInfo data = ViewModel.SplitScreenData;
            if (data != null && data.Nodes != null)
            {
                for (int i = 0; i < data.Split; i++)
                {
                    gridMain.RowDefinitions.Add(new RowDefinition());
                    gridMain.ColumnDefinitions.Add(new ColumnDefinition());
                }
                VideoPanelItem[] items = await GetVideoPanels(data);
                bool editting = ViewModel.IsOnEditting;
                foreach (VideoPanelItem panel in items)
                {
                    panel.ViewModel.IsInEditStatus = !editting;
                    gridMain.Children.Add(panel);
                }
            }
            _onLoading = false;
        }

        private async Task<VideoPanelItem[]> GetVideoPanels(SplitScreenInfo data)
        {
            while (_onPreparing)
                await Task.Delay(50);

            if (data == _prepareData)
            {
                VideoPanelItem[] items = _prepareItems;
                _prepareItems = null;
                _prepareData = null;
                return items;
            }
            else
            {
                if (_prepareItems != null)
                    foreach (VideoPanelItem vpi in _prepareItems)
                        destoryVideoPanel(vpi);
                _prepareItems = null;
                _prepareData = null;
                return await CreateVideoPanels(data);
            }
        }

        private async Task<VideoPanelItem[]> CreateVideoPanels(SplitScreenInfo data)
        {
            DateTime timeStart = DateTime.Now;
            VideoPanelItem[] vpis = new VideoPanelItem[data.Nodes.Length];
            for (int i = 0; i < data.Nodes.Length; i++)
            {
                var node = data.Nodes[i];
                VideoPanelItem panel = findAVideoPanel(node);
                vpis[i] = panel;
                await Task.Delay(10);
            }
            Console.WriteLine("Load Count:{0} UsedTime:{1}", gridMain.Children.Count, DateTime.Now - timeStart);
            return vpis;
        }

        private VideoPanelItem findAVideoPanel(SplitScreenNode node)
        {
            VideoPanelItem panel = null;
            if (_garbageItems.Count > 0)
            {
                panel = _garbageItems[_garbageItems.Count - 1];
                _garbageItems.RemoveAt(_garbageItems.Count - 1);
                Console.WriteLine("GetPanel from Garbage!");
            }
            else
                panel = new VideoPanelItem(ViewModel.CanItemClose);

            panel.ViewModel.CanClose = ViewModel.CanItemClose;
            if (!string.IsNullOrWhiteSpace(node.VideoId))
                panel.PlayVideo(node.VideoId);
            //绑定属性
            panel.SetBinding(Grid.RowProperty, CreateBinding(node, BindingMode.TwoWay, nameof(node.Row)));
            panel.SetBinding(Grid.ColumnProperty, CreateBinding(node, BindingMode.TwoWay, nameof(node.Column)));
            panel.SetBinding(Grid.RowSpanProperty, CreateBinding(node, BindingMode.TwoWay, nameof(node.RowSpan)));
            panel.SetBinding(Grid.ColumnSpanProperty, CreateBinding(node, BindingMode.TwoWay, nameof(node.ColumnSpan)));
            panel.ViewModel.SplitScreenNode = node;
            SetDragDropParam(panel);
            //监听鼠标点击事件，以隐藏VideoControl的控制面板。
            panel.PreviewMouseDown += Panel_PreviewMouseDown;
            return panel;
        }

        //设置视频窗口的可拖拽属性。
        private void SetDragDropParam(VideoPanelItem vpi)
        {
            vpi.SetValue(DragDropTool.AllowDropProperty, true);
            vpi.SetValue(DragDropTool.DropHandlerProperty, new VideoPanelDropHandler());
            vpi.SetValue(DragDropTool.DragHandlerProperty, new VideoPanelDragHandler());
            //允许拖动
            Binding binding = CreateBinding(vpi.DataContext, BindingMode.OneWay, "ControlViewModel.IsVisible");
            vpi.SetBinding(DragDropTool.AllowDragProperty, binding);

            //设置拖动数据
            binding = CreateBinding(vpi.DataContext, BindingMode.OneWay, "ControlViewModel.VideoId");
            vpi.SetBinding(DragDropData.DataProperty, binding);

            //设置可视化
            vpi.SetValue(DragDropVisual.ShadowVisibleProperty, true);
            vpi.SetValue(DragDropVisual.ShadowColorProperty, Color.FromRgb(0, 255, 0));
            vpi.SetValue(DragDropVisual.BorderThicknessProperty, new Thickness(2));
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

        public void PlayVideoOnBlank(string videoId)
        {
            var item = getFirstBlankItem();
            if (item != null)
            {
                item.PlayVideo(videoId);
            }
        }

        VideoPanelItem getFirstBlankItem()
        {
            List<ItemSequence> list = new List<ItemSequence>();

            for (int i = 0; i < gridMain.Children.Count; i++)
            {
                VideoPanelItem item = gridMain.Children[i] as VideoPanelItem;
                int split = ViewModel.SplitScreenData.Split;
                if (item != null && !item.IsPlaying)
                {
                    int row = (int)item.GetValue(Grid.RowProperty);
                    int col = (int)item.GetValue(Grid.ColumnProperty);
                    list.Add(new ItemSequence(item, row * split + col));
                }
            }
            list = list.OrderBy(e => e.SerialNumber).ToList();
            return list.FirstOrDefault()?.PanelItem;
        }

        public void StopAll()
        {
            for (int i = 0; i < gridMain.Children.Count; i++)
            {
                VideoPanelItem item = gridMain.Children[i] as VideoPanelItem;
                item?.PlayVideo(null);
            }
        }

        private void Panel_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel.OnSwitching && !ViewModel.SwitchPaused)
            {
                ViewModel.SwitchPaused = true;
                e.Handled = true;
                return;
            }
            VideoPanelItem item = sender as VideoPanelItem;
            foreach (VideoPanelItem vpi in gridMain.Children)
            {
                if (!item.Equals(vpi))
                {
                    vpi.ViewModel.ControlViewModel.IsControlPanelVisible = false;
                    vpi.ViewModel.SearchedResultModel.IsVisible = false;
                    vpi.ViewModel.PlusSignModel.IsVisible = true;
                }
            }
        }

        private void VideoControl_FullScreen(object sender, RoutedEventArgs e)
        {
            VideoPanelItem vpi = e.Source as VideoPanelItem;
            if (vpi != null)
            {
                VideoControl vc = e.OriginalSource as VideoControl;
                if (vc.ViewModel.IsFullScreen)
                {
                    topCover.InsertVideoPanel(vpi, true);
                }
            }
        }

        private void VideoControl_PanelVisible(object sender, RoutedEventArgs e)
        {
            VideoPanelItem vpi = e.Source as VideoPanelItem;
            if (vpi != null)
            {
                VideoControl vc = e.OriginalSource as VideoControl;
                if (vc.ViewModel.IsControlPanelVisible && !vc.ViewModel.IsFullScreen)
                    topCover.InsertVideoPanel(vpi, false);
                //else if (string.IsNullOrWhiteSpace(vc.ViewModel.VideoId) && !vc.ViewModel.IsFullScreen)
                //    topCover.ClearVideoPanel();
            }
        }

        private void SearchPanel_Visible(object sender, RoutedEventArgs e)
        {
            VideoPanelItem vpi = e.Source as VideoPanelItem;
            if (vpi != null)
            {
                if (vpi.ViewModel.SearchedResultModel.IsVisible)
                    topCover.InsertVideoPanel(vpi, false);
            }
        }

        private void ClearUI()
        {
            if (gridMain != null)
            {
                foreach (VideoPanelItem vpi in gridMain.Children)
                {
                    destoryVideoPanel(vpi);
                    _garbageItems.Add(vpi);
                }

                gridMain.Children.Clear();
                gridMain.ColumnDefinitions.Clear();
                gridMain.RowDefinitions.Clear();
            }
        }

        private void destoryVideoPanel(VideoPanelItem vpi)
        {
            vpi.PreviewMouseDown -= Panel_PreviewMouseDown;
            vpi.ViewModel.SplitScreenNode = null;
            vpi.PlayVideo(null);
        }

        private bool _isDisposed = false;

        public bool Disposed { get { return _isDisposed; } }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool dispose)
        {
            if (!_isDisposed)
            {
                try
                {
                    if (dispose)
                    {
                        ClearUI();
                        this.DataContext = null;
                        this.DataContextChanged -= SplitScreenPanel_DataContextChanged;
                    }
                    _isDisposed = true;
                }
                catch (InvalidOperationException e)
                {
                    //在关闭应用程序后的系统资源回收阶段，报出此异常，目前不清楚跨域访问的原因。
                    Console.WriteLine("未解析的异常CCTV2_0:" + e.Message);
                }
            }
        }

        ~SplitScreenPanel()
        {
            Dispose(false);
        }

        private class EditingStatusToDelayConverter : IValueConverter
        {
            public static readonly EditingStatusToDelayConverter Instance = new EditingStatusToDelayConverter();

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                bool editing = (bool)value;
                return editing ? 500 : 10;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }


        class ItemSequence
        {
            public VideoPanelItem PanelItem { get; set; }
            public int SerialNumber { get; set; }
            public ItemSequence(VideoPanelItem item, int number)
            {
                PanelItem = item;
                SerialNumber = number;
            }
        }
    }
}

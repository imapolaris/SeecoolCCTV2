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
using VideoNS.BaseXaml;
using VideoNS.Layout;
using VideoNS.VideoInfo.Search;

namespace VideoNS
{
    /// <summary>
    /// VideoPanelItem.xaml 的交互逻辑
    /// </summary>
    public partial class VideoPanelItem : UserControl
    {
        #region 【路由事件】
        public static readonly RoutedEvent SearchPanelVisibleChangedEvent = EventManager.RegisterRoutedEvent
            (nameof(SearchPanelVisibleChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SearcherControl));

        public event RoutedEventHandler SearchPanelVisibleChanged
        {
            add { this.AddHandler(SearchPanelVisibleChangedEvent, value); }
            remove { this.RemoveHandler(SearchPanelVisibleChangedEvent, value); }
        }

        protected virtual void OnSearchPanelVisibleChanged()
        {
            this.RaiseEvent(new RoutedEventArgs(SearchPanelVisibleChangedEvent, this));
        }
        #endregion 【路由事件】

        public VideoPanelItem(bool canClose= true)
        {
            InitializeComponent();
            ViewModel.CanClose = canClose;
            loadUI();
            loadObservable(ViewModel, true);
            DataContextChanged += onDataContextChanged;
        }

        public VideoItemViewModel ViewModel
        {
            get { return DataContext as VideoItemViewModel; }
        }

        private void onDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            VideoItemViewModel oldModel = e.OldValue as VideoItemViewModel;
            loadObservable(oldModel, false);
            VideoItemViewModel newModel = e.NewValue as VideoItemViewModel;
            loadObservable(newModel, true);
            loadUI();
        }

        void loadObservable(VideoItemViewModel model, bool isPlus)
        {
            if (model == null)
                return;
            if (isPlus)
            {
                model.PropertyChanged += onPropertyChanged;
                model.PlusSignModel.PropertyChanged += plusSignPropertyChanged;
                model.SearchedResultModel.PropertyChanged += searcherPropertyChanged;
                model.ControlViewModel.PropertyChanged += controlPropertyChanged;
            }
            else
            {
                model.PropertyChanged -= onPropertyChanged;
                model.PlusSignModel.PropertyChanged -= plusSignPropertyChanged;
                model.SearchedResultModel.PropertyChanged -= searcherPropertyChanged;
                model.ControlViewModel.PropertyChanged -= controlPropertyChanged;
            }
        }

        private void searcherPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ViewModel.SearchedResultModel.IsVisible):
                    loadUI();
                    OnSearchPanelVisibleChanged();
                    break;
            }
        }

        private void controlPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ViewModel.ControlViewModel.IsVisible):
                    loadUI();
                    break;
                case nameof(ViewModel.ControlViewModel.IsOnEditting):
                    updateItemControl();
                    break;
            }
        }

        private void onPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ViewModel.IsInEditStatus):
                    loadUI();
                    break;
            }
        }

        private void plusSignPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ViewModel.PlusSignModel.IsVisible):
                    loadUI();
                    break;
            }
        }

        #region load UI

        private void loadUI()
        {
            if (ViewModel.ControlViewModel.IsVisible)
                loadWhenVideoPlay();
            else
                loadWhenVideoUnplay();
        }

        void loadWhenVideoPlay()
        {
            disposePlus();
            disposeSearch();
            loadVideoControl();
            updateItemControl();
        }

        void loadWhenVideoUnplay()
        {
            disposeItemControl();
            hideVideoControl();
            if (!ViewModel.IsInEditStatus)
            {
                disposeSearch();
                disposePlus();
            }
            else if (ViewModel.SearchedResultModel.IsVisible)
            {
                disposePlus();
                loadSearch();
            }
            else if (ViewModel.PlusSignModel.IsVisible)
            {
                disposeSearch();
                loadPlus();
            }
        }

        void updateItemControl()
        {
            if (ViewModel.ControlViewModel.IsVisible && !ViewModel.ControlViewModel.IsOnEditting)
                loadItemControl();
            else
                disposeItemControl();
        }

        #endregion load UI

        #region Plus Button

        VideoPlusPanel _plusPanel;
        private void loadPlus()
        {
            if (_plusPanel == null && ViewModel.CanClose)
            {
                _plusPanel = new VideoPlusPanel();
                _plusPanel.DataContext = ViewModel.PlusSignModel;
                gridPanel.Children.Add(_plusPanel);
            }
        }

        private void disposePlus()
        {
            if (_plusPanel != null)
            {
                gridPanel.Children.Remove(_plusPanel);
                _plusPanel.DataContext = null;
                _plusPanel = null;
            }
        }

        #endregion Plus Button

        #region 搜索选项
        SearcherControl _searchControl;

        private void loadSearch()
        {
            if (_searchControl == null && ViewModel.CanClose)
            {
                _searchControl = new SearcherControl();
                _searchControl.DataContext = ViewModel.SearchedResultModel;
                _searchControl.Margin = new Thickness(10, 10, 10, 10);
                gridPanel.Children.Add(_searchControl);
                if (ViewModel.SearchedResultModel.ResultsSource?.Source == null)
                    ViewModel.SearchedResultModel.ResetSearchContext();
            }
        }

        private void disposeSearch()
        {
            if (_searchControl != null)
            {
                gridPanel.Children.Remove(_searchControl);
                _searchControl.DataContext = null;
                _searchControl = null;
            }
        }

        #endregion 搜索选项

        #region VideoControl

        public VideoControl VideoControl { get; private set; }

        void hideVideoControl()
        {
            if (VideoControl != null)
                VideoControl.Visibility = Visibility.Hidden;
        }

        void loadVideoControl()
        {
            if (VideoControl == null)
            {
                VideoControl = new VideoControl();
                VideoControl.DataContext = ViewModel.ControlViewModel;
                //绑定透明度
                Binding binding = new Binding();
                binding.Mode = BindingMode.OneWay;
                binding.Path = new PropertyPath(nameof(ViewModel.ControlViewModel.Opacity));
                VideoControl.SetBinding(VideoControl.OpacityProperty, binding);
                gridPanel.Children.Add(VideoControl);
            }
            else
                VideoControl.Visibility = Visibility.Visible;
        }

        #endregion VideoControl

        #region VideoPanelItem Control

        VideoPanelItemControl _itemControl;

        void loadItemControl()
        {
            if (_itemControl == null && ViewModel.CanClose)
            {
                _itemControl = new VideoPanelItemControl();
                _itemControl.DataContext = ViewModel.ControlViewModel;
                gridVideoPanelItemControl.Children.Add(_itemControl);
            }
        }

        void disposeItemControl()
        {
            if (_itemControl != null)
            {
                gridVideoPanelItemControl.Children.Remove(_itemControl);
                _itemControl.DataContext = new VideoControlModel();
                _itemControl = null;
            }
        }

        #endregion VideoPanelItem Control

        public void PlayVideo(string id)
        {
            ViewModel.ControlViewModel.VideoId = id;
            if (ViewModel.CanClose)
                ViewModel.ControlViewModel.CloseBtnVisibility = Visibility.Visible;
            else
                ViewModel.ControlViewModel.CloseBtnVisibility = Visibility.Collapsed;
        }

        public bool IsPlaying
        {
            get { return !string.IsNullOrWhiteSpace(ViewModel.ControlViewModel.VideoId); }
        }
    }
}

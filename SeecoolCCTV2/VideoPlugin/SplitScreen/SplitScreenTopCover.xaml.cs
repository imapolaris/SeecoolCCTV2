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
using VideoNS.Helper;
using System.Windows.Media.Animation;
using System.Globalization;
using System.Windows.Controls.Primitives;

namespace VideoNS.SplitScreen
{
    /// <summary>
    /// SplitScreenTopCover.xaml 的交互逻辑
    /// </summary>
    public partial class SplitScreenTopCover : UserControl
    {
        public SplitScreenTopCover()
        {
            InitializeComponent();
            this.MouseUp += Canvas_MouseUp;
            this.AddHandler(VideoControl.FullScreenChangedEvent, new RoutedEventHandler(VideoControl_FullScreen));
            this.AddHandler(VideoControl.ControlPanelVisibleChangedEvent, new RoutedEventHandler(VideoControl_PanelVisible));
        }

        private void VideoControl_FullScreen(object sender, RoutedEventArgs e)
        {
            if (_vpi != null)
            {
                VideoControl vc = e.OriginalSource as VideoControl;
                if (vc.ViewModel.IsFullScreen)
                {
                    Panel parent = _vpi.Parent as Panel;
                    parent.Children.Remove(_vpi);
                    gridFull.Children.Add(_vpi);
                }
                else
                {
                    if (_isInitFullScreen)
                    {
                        ClearVideoPanel();
                    }
                    else
                    {
                        Panel parent = _vpi.Parent as Panel;
                        parent.Children.Remove(_vpi);
                        gridMain.Children.Add(_vpi);
                    }
                }
                e.Handled = true;
            }
        }

        private void VideoControl_PanelVisible(object sender, RoutedEventArgs e)
        {
            VideoControl vc = e.OriginalSource as VideoControl;
            if (string.IsNullOrWhiteSpace(vc.ViewModel.VideoId) && !vc.ViewModel.IsFullScreen)
                ClearVideoPanel();
            e.Handled = true;
        }

        private bool _isInitFullScreen;
        private Panel _parent;
        private int _lastIndex = -1;
        private VideoPanelItem _vpi;
        private Size _initSize;
        private Point _initPos;

        public void InsertVideoPanel(VideoPanelItem vpi, bool isFullScreen)
        {
            bool inserted = isFullScreen ? InsertFullScreen(vpi) : InsertPopup(vpi);
            if (inserted)
            {
                _onScaling = false;
                DisplayCurrent();
                _isInitFullScreen = isFullScreen;
                BindElementProp(vpi);
            }
        }

        private bool InsertFullScreen(VideoPanelItem vpi)
        {
            RemoveVideoFromInitParent(vpi);
            gridFull.Children.Add(vpi);
            return true;
        }

        private void BindElementProp(VideoPanelItem vpi)
        {
            //按钮绑定。
            MultiBinding multi = new MultiBinding();
            multi.Mode = BindingMode.OneWay;
            multi.Converter = new MultiConverter();
            multi.Bindings.Add(CreateBinding(vpi, BindingMode.OneWay, "DataContext.ControlViewModel.IsVisible"));
            multi.Bindings.Add(CreateBinding(vpi, BindingMode.OneWay, "DataContext.ControlViewModel.IsFullScreen"));
            btnToNormal.SetBinding(VisibilityProperty, multi);

            Binding binding = CreateBinding(vpi, BindingMode.TwoWay, "DataContext.ControlViewModel.IsFullScreen");
            btnToNormal.SetBinding(ToggleButton.IsCheckedProperty, binding);

            IValueConverter converter = new InnerBoolToVisibility();
            //全屏窗绑定
            binding = CreateBinding(vpi, BindingMode.OneWay, "DataContext.ControlViewModel.IsFullScreen");
            binding.Converter = converter;
            binding.ConverterParameter = "AND";
            gridFull.SetBinding(VisibilityProperty, binding);
            //弹出窗绑定
            binding = CreateBinding(vpi, BindingMode.OneWay, "DataContext.ControlViewModel.IsFullScreen");
            binding.Converter = converter;
            binding.ConverterParameter = "XOR";
            canvasPopup.SetBinding(VisibilityProperty, binding);
            //源父窗体绑定
            _parent.SetBinding(VisibilityProperty, binding);
        }

        private void UnbindElementProp()
        {
            BindingOperations.ClearBinding(btnToNormal, VisibilityProperty);
            BindingOperations.ClearBinding(btnToNormal, ToggleButton.IsCheckedProperty);
            BindingOperations.ClearBinding(gridFull, VisibilityProperty);
            BindingOperations.ClearBinding(canvasPopup, VisibilityProperty);
            BindingOperations.ClearBinding(_parent, VisibilityProperty);
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

        private bool InsertPopup(VideoPanelItem vpi)
        {
            Size toSize = default(Size);
            if (!TryCalcToSize(vpi, ref toSize))
                return false;

            Point fromPos = vpi.TranslatePoint(new Point(), this);
            Point toPos = CalcToLocation(toSize);

            RemoveVideoFromInitParent(vpi);
            RecordVideoPreLocation(vpi, fromPos);
            gridMain.Children.Add(vpi);
            //动画
            Storyboard sb = CreateStoryboard(vpi.ActualWidth, toSize.Width, vpi.ActualHeight, toSize.Height, fromPos, toPos);
            sb.Begin(gridMain);

            return true;
        }

        private void RecordVideoPreLocation(VideoPanelItem vpi, Point fromPos)
        {
            //记录初始值。
            _initSize = new Size(vpi.ActualWidth, vpi.ActualHeight);
            _initPos = fromPos;
        }

        private void RemoveVideoFromInitParent(VideoPanelItem vpi)
        {
            _parent = vpi.Parent as Panel;
            _lastIndex = _parent.Children.IndexOf(vpi);
            _parent.Children.Remove(vpi);
            _vpi = vpi;
            _vpi.SetValue(DragDropTool.AllowDragProperty, false);
            _parent.Visibility = Visibility.Collapsed;
        }

        private void AddVideoToInitParent()
        {
            _vpi.ViewModel.ControlViewModel.IsControlPanelVisible = false;
            _vpi.ViewModel.SearchedResultModel.IsVisible = false;
            _vpi.ViewModel.PlusSignModel.IsVisible = true;

            Panel panel = _vpi.Parent as Panel;
            panel.Children.Remove(_vpi);
            _parent.Children.Insert(_lastIndex, _vpi);
            _lastIndex = -1;
            _vpi.SetValue(DragDropTool.AllowDragProperty, true);
            _vpi.ViewModel.ControlViewModel.IsFullScreen = false;
            _parent.Visibility = Visibility.Visible;
            _parent = null;
        }

        private void DisplayCurrent()
        {
            this.Opacity = 1;
            this.IsHitTestVisible = true;
            this.SizeChanged += SplitScreenTopCover_SizeChanged;
        }

        private void HideCurrent()
        {
            this.Opacity = 0;
            this.IsHitTestVisible = false;
            this.SizeChanged -= SplitScreenTopCover_SizeChanged;
        }

        private bool TryCalcToSize(VideoPanelItem vpi, ref Size toSize)
        {
            if (vpi.ActualWidth == 0 || vpi.ActualHeight == 0)
                return false;
            double toWidth = Math.Min(AppConstants.VideoControlMinWidth, this.ActualWidth);
            double toHeight = Math.Min(AppConstants.VideoControlMinHeight, this.ActualHeight);
            double ratioW = toWidth / vpi.ActualWidth;
            double ratioH = toHeight / vpi.ActualHeight;
            double ratio = Math.Max(ratioW, ratioH);
            if (ratio <= 1)
                return false;
            toWidth = vpi.ActualWidth * ratio;
            toHeight = vpi.ActualHeight * ratio;
            if (toWidth > this.ActualWidth - AppConstants.VideoControlMargin * 2)
                toWidth = this.ActualWidth - AppConstants.VideoControlMargin * 2;
            if (toHeight > this.ActualHeight - AppConstants.VideoControlMargin * 2)
                toHeight = this.ActualHeight - AppConstants.VideoControlMargin * 2;
            toSize = new Size(toWidth, toHeight);
            return true;
        }

        private Point CalcToLocation(Size toSize)
        {
            return new Point((this.ActualWidth - toSize.Width) / 2, (this.ActualHeight - toSize.Height) / 2);
        }

        Storyboard CreateStoryboard(double widthFrom, double widthTo, double heightFrom, double heightTo, Point pointFrom, Point pointTo)
        {
            Storyboard sb = new Storyboard();
            Duration dura = new Duration(TimeSpan.FromSeconds(0.3));
            ExponentialEase easing = new ExponentialEase() { Exponent = 4, EasingMode = EasingMode.EaseInOut };

            sb.Children.Add(CreateAnim(gridMain, widthFrom, widthTo, "Width", dura, easing));
            sb.Children.Add(CreateAnim(gridMain, heightFrom, heightTo, "Height", dura, easing));
            sb.Children.Add(CreateAnim(gridMain, pointFrom.X, pointTo.X, "(Canvas.Left)", dura, easing));
            sb.Children.Add(CreateAnim(gridMain, pointFrom.Y, pointTo.Y, "(Canvas.Top)", dura, easing));
            return sb;
        }

        private DoubleAnimation CreateAnim(DependencyObject target, double from, double to, string path, Duration dura, IEasingFunction easing)
        {
            DoubleAnimation daL = new DoubleAnimation(from, to, dura);
            daL.SetValue(Storyboard.TargetProperty, target);
            daL.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath(path));
            daL.EasingFunction = easing;
            return daL;
        }

        public void ClearVideoPanel()
        {
            if (_vpi != null)
            {
                UnbindElementProp();
                AddVideoToInitParent();
                _vpi = null;
                HideCurrent();
            }
        }

        private bool _onScaling = false;
        private void ScaleBack()
        {
            if (!_onScaling)
            {
                _onScaling = true;
                Point curPos = new Point((double)gridMain.GetValue(Canvas.LeftProperty), (double)gridMain.GetValue(Canvas.TopProperty));
                Storyboard sb = CreateStoryboard(gridMain.ActualWidth, _initSize.Width, gridMain.ActualHeight, _initSize.Height, curPos, _initPos);
                sb.Completed += (s, e) =>
                {
                    ClearVideoPanel();
                    _onScaling = false;
                };
                sb.Begin(gridMain);
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Canvas)
            {
                ScaleBack();
            }
        }

        private void SplitScreenTopCover_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_vpi != null && !_vpi.ViewModel.ControlViewModel.IsFullScreen)
            {
                ScaleBack();
            }
        }

        private class InnerBoolToVisibility : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                bool full = (bool)value;
                string logic = parameter.ToString().ToUpper();
                if (full)
                {
                    if("AND".Equals(logic))
                        return Visibility.Visible;
                    else
                        return Visibility.Collapsed;
                }
                else
                {
                    if ("AND".Equals(logic))
                        return Visibility.Collapsed;
                    else
                        return Visibility.Visible;
                }
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        private class MultiConverter : IMultiValueConverter
        {
            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                bool visi = (bool)values[0];
                bool full = (bool)values[1];
                if (full && !visi)
                    return Visibility.Visible;
                return Visibility.Collapsed;
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }
}

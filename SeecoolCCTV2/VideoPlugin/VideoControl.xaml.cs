using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using Common.Util;
using VideoTrackingCmd;
using VideoNS.VideoTrack;
using System.Windows.Media;
using System.Windows.Shapes;

namespace VideoNS
{
    /// <summary>
    /// VideoControl.xaml 的交互逻辑
    /// </summary>
    public partial class VideoControl : UserControl
    {
        public VideoControlModel ViewModel { get { return this.DataContext as VideoControlModel; } }

        #region 【路由事件定义】
        public static readonly RoutedEvent FullScreenChangedEvent = EventManager.RegisterRoutedEvent
            (nameof(FullScreenChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(VideoControl));

        /// <summary>
        /// 回看事件的CLR包装事件。
        /// </summary>
        public event RoutedEventHandler FullScreenChanged
        {
            add { this.AddHandler(FullScreenChangedEvent, value); }
            remove { this.RemoveHandler(FullScreenChangedEvent, value); }
        }

        protected virtual void OnFullScreenChanged()
        {
            this.RaiseEvent(new RoutedEventArgs(FullScreenChangedEvent, this));
        }

        //控制面板显隐事件。
        public static readonly RoutedEvent ControlPanelVisibleChangedEvent = EventManager.RegisterRoutedEvent
            (nameof(ControlPanelVisibleChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(VideoControl));

        public event RoutedEventHandler ControlPanelVisibleChanged
        {
            add { this.AddHandler(ControlPanelVisibleChangedEvent, value); }
            remove { this.RemoveHandler(ControlPanelVisibleChangedEvent, value); }
        }

        protected virtual void OnControlPanelVisibleChanged()
        {
            this.RaiseEvent(new RoutedEventArgs(ControlPanelVisibleChangedEvent, this));
        }

        //视频ID切换事件。
        public static readonly RoutedEvent VideoIdChangedEvent = EventManager.RegisterRoutedEvent
            (nameof(VideoIdChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(VideoControl));

        public event RoutedEventHandler VideoIdChanged
        {
            add { this.AddHandler(VideoIdChangedEvent, value); }
            remove { this.RemoveHandler(VideoIdChangedEvent, value); }
        }

        protected virtual void OnVideoIdChanged()
        {
            this.RaiseEvent(new RoutedEventArgs(VideoIdChangedEvent, this));
        }
        #endregion 【路由事件定义】

        #region 【构造函数】
        /// <summary>
        /// 默认构造函数。
        /// <para>默认显示视频关闭按钮。</para>
        /// </summary>
        public VideoControl()
        {
            InitializeComponent();
            gridMain.MouseUp += GridMain_MouseUp;
            gridMain.MouseDown += GridMain_MouseDown;
            gridMain.MouseMove += GridMain_MouseMove;
            gridMain.MouseWheel += GridMain_MouseWheel;
            gridMain.SizeChanged += GridMain_SizeChanged;
            this.Loaded += onLoaded;
        }

        private void onLoaded(object sender, RoutedEventArgs e)
        {
            //if (ViewModel != null)
            //{
            //    ViewModel.SelectedDisplayType = "实时";
            //    ViewModel.RealTimeControl.SelectedStreamType = "高清";
            //    ViewModel.ReplayControl.SelectedSpeedType = "正常";
            //}
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ViewModel.IsControlPanelVisible):
                    if (ViewModel.IsControlPanelVisible)
                        StartMonitor();
                    else
                        StopMonitor();
                    OnControlPanelVisibleChanged();
                    break;
                case nameof(ViewModel.IsFullScreen):
                    OnFullScreenChanged();
                    break;
                case nameof(ViewModel.VideoId):
                    OnVideoIdChanged();
                    break;
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property.Equals(UserControl.DataContextProperty))
            {
                updatePropertyChanged(e.OldValue as VideoControlModel, false);
                updatePropertyChanged(e.NewValue as VideoControlModel, true);
            }
        }

        void updatePropertyChanged(VideoControlModel model, bool isLoad)
        {
            if (model != null)
            {
                if (isLoad)
                {
                    addTrackEvent(model);
                    model.RealTimeControl.PropertyChanged += RealTimeControl_PropertyChanged;
                    model.PropertyChanged += ViewModel_PropertyChanged;
                    VideoIdChanged += onVideoId;
                }
                else
                {
                    model.RealTimeControl.TrackSource.PropertyChanged -= trackSourcePropertyChanged;
                    model.RealTimeControl.TrackSource.TrackRectEvent -= onTrackRect;
                    model.RealTimeControl.PropertyChanged -= RealTimeControl_PropertyChanged;
                    model.PropertyChanged -= ViewModel_PropertyChanged;
                    VideoIdChanged -= onVideoId;
                }
            }
        }

        private void RealTimeControl_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ViewModel.RealTimeControl.IsVisible):
                    updateTrackRect();
                    break;
                case nameof(ViewModel.RealTimeControl.TrackSource):
                    addTrackEvent(ViewModel);
                    break;
            }
        }

        void addTrackEvent(VideoControlModel model)
        {
            if (model.RealTimeControl.TrackSource.TrackRectEvent == null)
            {
                model.RealTimeControl.TrackSource.TrackRectEvent += onTrackRect;
                model.RealTimeControl.TrackSource.PropertyChanged += trackSourcePropertyChanged;
            }
        }

        private void trackSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ViewModel.RealTimeControl.IsVisible):
                    updateTrackRect();
                    break;
            }
        }
        #endregion 【构造函数】

        #region 鼠标控制云镜

        double _mouseX = 0;
        double _mouseY = 0;

        void mouseDown(MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(videoDisp);
            _mouseX = pt.X / videoDisp.ActualWidth;
            _mouseY = pt.Y / videoDisp.ActualHeight;
            if (ViewModel.RealTimeControl.TrackSource.IsSetPointStatus)
            {
                ViewModel.RealTimeControl.TrackSource.StartTrackFromPoint(_mouseX, _mouseY);
                ViewModel.RealTimeControl.TrackSource.IsSetPointStatus = false;
                _mouseDown = false;
            }
            else if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 1)
            {
                ViewModel.RealTimeControl.TrackSource.StartDrag(_mouseX, _mouseY);
            }
            e.Handled = true;
        }

        void mouseMove(MouseEventArgs e)
        {
            Point pt = e.GetPosition(videoDisp);
            _mouseX = pt.X / videoDisp.ActualWidth;
            _mouseY = pt.Y / videoDisp.ActualHeight;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ViewModel.RealTimeControl.TrackSource.Draging(_mouseX, _mouseY);
                Thread.Sleep(100);
            }
        }

        void mouseUp(MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(videoDisp);
            _mouseX = pt.X / videoDisp.ActualWidth;
            _mouseY = pt.Y / videoDisp.ActualHeight;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ViewModel.RealTimeControl.TrackSource.Draging(_mouseX, _mouseY);
                ViewModel.RealTimeControl.TrackSource.StopDrag();
            }
        }

        void mouseWheel(MouseWheelEventArgs e)
        {
            int delta = e.Delta / 120;
            double scale = 1;
            while (delta > 0)
            {
                scale *= 1.1f;
                delta--;
            }
            while (delta < 0)
            {
                scale /= 1.1f;
                delta++;
            }
            ViewModel.RealTimeControl.TrackSource.SetZoomScale(scale);
            Thread.Sleep(200);
        }

        private void updateBeTrackingPosition(MouseEventArgs e)
        {
            if (canDragPTZ() && ViewModel.RealTimeControl.TrackSource.IsSetPointStatus && e.LeftButton == MouseButtonState.Released)
                Cursor = CursorsExpand.FillGreenEllipseCursor;
            else if (canDragPTZ() && e.LeftButton == MouseButtonState.Pressed)
                Cursor = Cursors.SizeAll;
            else if (_moveVideo)
                Cursor = Cursors.Hand;
            else
                Cursor = Cursors.Arrow; 
        }

        #endregion 鼠标控制云镜

        #region 跟踪框

        Rectangle _rectangle;
        TrackRect _rect = new TrackRect();
        void loadTrackRect()
        {
            if (_rectangle == null)
            {
                _rectangle = new Rectangle();
                _rectangle.StrokeDashArray = new DoubleCollection(new List<double> { 1, 1 });
                _rectangle.StrokeThickness = 2;
                _rectangle.Stroke = System.Windows.Media.Brushes.Red;
                _rectangle.Opacity = 0.6;
                _rectangle.HorizontalAlignment = HorizontalAlignment.Left;
                _rectangle.VerticalAlignment = VerticalAlignment.Top;
                grid.Children.Add(_rectangle);
            }
        }

        void disposeTrackRect()
        {
            if (_rectangle != null)
            {
                grid.Children.Remove(_rectangle);
                _rectangle = null;
            }
        }

        void updateTrackRect()
        {
            if (_rect.valid && canShowTrack())
            {
                loadTrackRect();
                _rectangle.Margin = new Thickness(_rect.x * gridMain.ActualWidth,
                    _rect.y * gridMain.ActualHeight, 0, 0);
                _rectangle.Width = _rect.width * gridMain.ActualWidth;
                _rectangle.Height = _rect.height * gridMain.ActualHeight;
            }
            else
            {
                disposeTrackRect();
            }
        }

        void onTrackRect(TrackRect rect)
        {
            try
            {
                _rect = rect;
                WindowUtil.BeginInvoke(() =>
                {
                    updateTrackRect();
                });
            }
            catch
            { }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            updateTrackRect();
        }
        private void onVideoId(object sender, RoutedEventArgs e)
        {
            updateTrackRect();
        }

        #endregion 跟踪框

        #region 【UI逻辑】
        private Point _downPoint;
        private bool _mouseDown = false;
        private bool _mouseMoved = false;

        private bool _moveVideo = false;
        private Point _mvLeft = new Point(0, 0);
        private Point _mvRight = new Point(0, 0);

        private void GridMain_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender.Equals(e.Source))
            {
                _mouseDown = true;
                _mouseMoved = false;
                _downPoint = e.GetPosition(gridMain);
                if (canDragPTZ())
                {
                    mouseDown(e);
                }
                else if (ViewModel.IsOnEditting)
                {
                    _moveVideo = beginMoveVideo();
                    if (_moveVideo)
                        e.Handled = true;
                }
                updateBeTrackingPosition(e);
            }
        }

        //监听主面板鼠标按下事件，用于显示和隐藏控制面板。
        private void GridMain_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (canDragPTZ())
            {
                mouseUp(e);
            }
            if (sender.Equals(e.Source) && _mouseDown && !_mouseMoved && ViewModel.IsOnEditting)
            {
                ViewModel.IsControlPanelVisible = !ViewModel.IsControlPanelVisible;
            }
        }

        private void GridMain_MouseMove(object sender, MouseEventArgs e)
        {
            updateBeTrackingPosition(e);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point pos = e.GetPosition(gridMain);
                if (pos.X - _downPoint.X != 0 || pos.Y - _downPoint.Y != 0)
                    _mouseMoved = true;
                if (_moveVideo)
                    movingVideo(pos);
            }
            else
            {
                _mouseDown = false;
                _moveVideo = false;
            }
            if (canDragPTZ())
                mouseMove(e);
            ResetInterval();
        }

        private void GridMain_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (canDragPTZ())
                mouseWheel(e);
            else if (ViewModel.IsOnEditting)
                scaleVideo(e);
        }

        //重新计算缩放和平移。
        private void GridMain_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            TransformParam tp = ViewModel.DisplayModel.VideoTransform;
            Vector transVec = new Vector(tp.TranslateX, tp.TranslateY);
            transVec = calcTranslate(transVec, tp);

            tp.TranslateX = transVec.X;
            tp.TranslateY = transVec.Y;
        }

        private void scaleVideo(MouseWheelEventArgs e)
        {
            TransformParam tp = ViewModel.DisplayModel.VideoTransform;

            Point p = e.GetPosition(videoDisp);
            if (e.Delta > 0 && tp.ScaleX <= 4.9)
            {
                tp.ScaleY = tp.ScaleX = tp.ScaleX + 0.1;
            }
            else if (e.Delta < 0 && tp.ScaleX > 1)
            {
                tp.ScaleY = tp.ScaleX = tp.ScaleX - 0.1;
            }

            p = new Point(p.X * tp.ScaleX, p.Y * tp.ScaleY);
            Point parentPos = e.GetPosition(gridMain);
            Vector transVec = parentPos - p;
            transVec = calcTranslate(transVec, tp);

            tp.TranslateX = transVec.X;
            tp.TranslateY = transVec.Y;
        }

        private Vector calcTranslate(Vector vec, TransformParam tp)
        {
            if (vec.X > 0) vec.X = 0;
            if (vec.Y > 0) vec.Y = 0;

            double realW = videoDisp.ActualWidth * tp.ScaleX;
            double realH = videoDisp.ActualHeight * tp.ScaleY;
            if (vec.X + realW < gridMain.ActualWidth)
                vec.X = gridMain.ActualWidth - realW;
            if (vec.Y + realH < gridMain.ActualHeight)
                vec.Y = gridMain.ActualHeight - realH;
            return vec;
        }

        private bool beginMoveVideo()
        {
            TransformParam st = ViewModel.DisplayModel.VideoTransform;
            if (st.ScaleX <= 1 && st.ScaleY <= 1)
                return false;
            else
            {
                _mvLeft = gridMain.TranslatePoint(new Point(), videoDisp);
                _mvRight = gridMain.TranslatePoint(new Point(gridMain.ActualWidth, gridMain.ActualHeight), videoDisp);
                _mvRight = new Point(_mvRight.X - videoDisp.ActualWidth, _mvRight.Y - videoDisp.ActualHeight);

                _mvLeft = new Point(_mvLeft.X * st.ScaleX, _mvLeft.Y * st.ScaleY);
                _mvRight = new Point(_mvRight.X * st.ScaleX, _mvRight.Y * st.ScaleY);
                return true;
            }
        }

        private void movingVideo(Point pos)
        {
            TransformParam st = ViewModel.DisplayModel.VideoTransform;
            Vector vec = pos - _downPoint;
            if (vec.X > _mvLeft.X)
                vec.X = _mvLeft.X;
            if (vec.Y > _mvLeft.Y)
                vec.Y = _mvLeft.Y;

            if (vec.X < _mvRight.X)
                vec.X = _mvRight.X;
            if (vec.Y < _mvRight.Y)
                vec.Y = _mvRight.Y;
            Point p = _mvLeft - vec;
            st.TranslateX = -p.X;
            st.TranslateY = -p.Y;
        }

        bool canDragPTZ()
        {
            return canShowTrack() && ViewModel.RealTimeControl.TrackSource.IsVisible;
        }

        bool canShowTrack()
        {
            return ViewModel != null && ViewModel.RealTimeControl.IsVisible && ViewModel.RealTimeControl.TrackSource.IsVisible && ViewModel.RealTimeControl.TrackSource.CanBeTracked;
        }

        private const int DefaultVisiInterval = 15;
        private int m_visiInterval = DefaultVisiInterval; //控制面板可见持续时间:秒。
        private readonly object m_visiLock = new object(); //修改可见持续时间的资源锁。
        private Thread m_visiMonitor = null; //控制面板显示时间监控器。


        private void StartMonitor()
        {
            StopMonitor();
            m_visiMonitor = new Thread(DoMonitor);
            m_visiMonitor.IsBackground = true;
            m_visiMonitor.Start();
        }

        private void StopMonitor()
        {
            if (m_visiMonitor != null && m_visiMonitor.IsAlive)
                m_visiMonitor.Abort();
            ResetInterval();
        }

        private void ResetInterval()
        {
            lock (m_visiLock)
            {
                m_visiInterval = DefaultVisiInterval;
            }
        }

        /// <summary>
        /// 可见性监控线程。
        /// </summary>
        private void DoMonitor()
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(1000);
                    //只有在主网格区域无鼠标捕获或者仅在空白区域捕获鼠标时才执行计时。
                    bool isActive = this.Dispatcher.Invoke(new Func<bool>(IsMonitorActive), DispatcherPriority.Normal);
                    int interval = 0;
                    lock (m_visiLock)
                    {
                        if (isActive)
                            m_visiInterval--;
                        interval = m_visiInterval;
                    }

                    //Console.WriteLine(string.Format("hide control panel after {0}s",interval));
                    if (interval == 0)
                    {
                        this.Dispatcher.BeginInvoke(new Action(HideControlPanel), null);
                        break;
                    }
                }
            }
            catch (ThreadAbortException)
            {
                //无操作。
            }
            catch (TaskCanceledException)
            {

            }
        }

        private bool IsMonitorActive()
        {
            Point pnt = Mouse.GetPosition(gridMain);
            if (pnt.X <= 0 || pnt.Y <= 0)
                return true;
            if (gridMain.ActualWidth < pnt.X || gridMain.ActualHeight < pnt.Y)
                return true;

            if (gridMain.IsMouseDirectlyOver)
                return true;
            return false;
        }

        /// <summary>
        /// 隐藏控制面板。
        /// </summary>
        public void HideControlPanel()
        {
            ViewModel.IsControlPanelVisible = false;
        }
        #endregion 【UI逻辑】
    }

    public class MultiBoolOrToVisibleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Any(e => (bool)e))
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MultiBoolAndToVisibleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.All(e => (bool)e))
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class ScaleToVisible : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double scale = 1;
            if (double.TryParse(value.ToString(), out scale) && scale == 1)
            {
                if (scale == 1)
                    return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class ScaleToTooltip : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double scale = 1;
            double.TryParse(value.ToString(), out scale);
            return $"数字变倍:×{scale:F1}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

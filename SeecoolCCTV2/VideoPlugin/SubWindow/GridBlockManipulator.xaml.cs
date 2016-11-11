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

namespace VideoNS.SubWindow
{
    /// <summary>
    /// GridBlockManipulator.xaml 的交互逻辑
    /// </summary>
    public partial class GridBlockManipulator : UserControl
    {
        #region 【自定义路由事件】
        public static readonly RoutedEvent ResizeStartEvent = EventManager.RegisterRoutedEvent
            (nameof(ResizeStart), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GridBlockManipulator));

        public event RoutedEventHandler ResizeStart
        {
            add { AddHandler(ResizeStartEvent, value); }
            remove { RemoveHandler(ResizeStartEvent, value); }
        }

        private void OnResizeStart()
        {
            this.RaiseEvent(new RoutedEventArgs(ResizeStartEvent, this));
        }

        public static readonly RoutedEvent ResizingEvent = EventManager.RegisterRoutedEvent
            (nameof(Resizing), RoutingStrategy.Bubble, typeof(ShiftEventHandler), typeof(GridBlockManipulator));

        public event ShiftEventHandler Resizing
        {
            add { AddHandler(ResizingEvent, value); }
            remove { RemoveHandler(ResizingEvent, value); }
        }

        private void OnResizing(ShiftEventArgs e)
        {
            this.RaiseEvent(e);
        }

        public static readonly RoutedEvent ResizeEndEvent = EventManager.RegisterRoutedEvent
            (nameof(ResizeEnd), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GridBlockManipulator));

        public event RoutedEventHandler ResizeEnd
        {
            add { AddHandler(ResizeEndEvent, value); }
            remove { RemoveHandler(ResizeEndEvent, value); }
        }
        private void OnResizeEnd()
        {
            this.RaiseEvent(new RoutedEventArgs(ResizeEndEvent, this));
        }

        public static readonly RoutedEvent RepositionEvent = EventManager.RegisterRoutedEvent
            (nameof(Reposition), RoutingStrategy.Bubble, typeof(ShiftEventHandler), typeof(GridBlockManipulator));

        public event ShiftEventHandler Reposition
        {
            add { AddHandler(RepositionEvent, value); }
            remove { RemoveHandler(RepositionEvent, value); }
        }

        private void OnReposition(ShiftEventArgs e)
        {
            this.RaiseEvent(e);
        }
        #endregion 【自定义路由事件】

        internal GridBlockModel ViewModel { get { return DataContext as GridBlockModel; } }

        public GridBlockManipulator()
        {
            InitializeComponent();
            gridMain.PreviewMouseDown += GridMain_PreviewMouseDown;
            gridMain.PreviewMouseMove += GridMain_PreviewMouseMove;
            gridMain.PreviewMouseUp += GridMain_PreviewMouseUp;
        }

        private bool _mouseDown;
        private Point _startPos;
        private Size _startSize;
        private void GridMain_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is Button)
            {
                _mouseDown = true;
                _startPos = gridMain.PointToScreen(e.GetPosition(gridMain));
                _startSize = new Size(gridMain.ActualWidth, gridMain.ActualHeight);
                OnResizeStart();
            }

        }

        private void GridMain_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseDown)
            {
                Point pos = gridMain.PointToScreen(e.GetPosition(gridMain));
                Point trans = new Point(pos.X - _startPos.X, pos.Y - _startPos.Y);
                if (Math.Abs(trans.X) >= SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(trans.Y) >= SystemParameters.MinimumVerticalDragDistance)
                {
                    if (btnBack.Equals(e.Source))
                    {
                        OnReposition(new ShiftEventArgs(trans.X, trans.Y, 0, 0, RepositionEvent, this));
                    }
                    else
                    {
                        Direction dir = ArrowDirection.GetDirection(e.Source as DependencyObject);
                        Point deltaSize = new Point();
                        Point deltaPos = new Point();
                        switch (dir)
                        {
                            case Direction.Left:
                                {
                                    deltaPos.X = trans.X;
                                    deltaSize.X = -trans.X;
                                    if (trans.X > _startSize.Width)
                                    {
                                        deltaPos.X = _startSize.Width;
                                        deltaSize.X = (trans.X - _startSize.Width) - _startSize.Width;
                                    }
                                }
                                break;
                            case Direction.Up:
                                {
                                    deltaPos.Y = trans.Y;
                                    deltaSize.Y = -trans.Y;
                                    if (trans.Y > _startSize.Height)
                                    {
                                        deltaPos.Y = _startSize.Height;
                                        deltaSize.Y = (trans.Y - _startSize.Height) - _startSize.Height;
                                    }
                                }
                                trans.X = 0;
                                break;
                            case Direction.Right:
                                {
                                    deltaSize.X = trans.X;
                                    if (trans.X < -_startSize.Width)
                                    {
                                        deltaPos.X = _startSize.Width + trans.X;
                                        deltaSize.X = -(trans.X + _startSize.Width + _startSize.Width);
                                    }
                                }
                                break;
                            case Direction.Down:
                                {
                                    deltaSize.Y = trans.Y;
                                    if (trans.Y < -_startSize.Height)
                                    {
                                        deltaPos.Y = trans.Y + _startSize.Height;
                                        deltaSize.Y = -(trans.Y + _startSize.Height + _startSize.Height);
                                    }
                                }
                                break;
                            case Direction.LeftUp:
                                {
                                    deltaPos.X = trans.X;
                                    deltaPos.Y = trans.Y;
                                    deltaSize.X = -trans.X;
                                    deltaSize.Y = -trans.Y;
                                    if (trans.X > _startSize.Width)
                                    {
                                        deltaPos.X = _startSize.Width;
                                        deltaSize.X = trans.X - _startSize.Width - _startSize.Width;
                                    }
                                    if (trans.Y > _startSize.Height)
                                    {
                                        deltaPos.Y = _startSize.Height;
                                        deltaSize.Y = trans.Y - _startSize.Height - _startSize.Height;
                                    }
                                }
                                break;
                            case Direction.RightUp:
                                {
                                    deltaPos.Y = trans.Y;
                                    deltaSize.X = trans.X;
                                    deltaSize.Y = -trans.Y;
                                    if (trans.X < -_startSize.Width)
                                    {
                                        deltaPos.X = _startSize.Width + trans.X;
                                        deltaSize.X = -(trans.X + _startSize.Width + _startSize.Width);
                                    }
                                    if (trans.Y > _startSize.Height)
                                    {
                                        deltaPos.Y = _startSize.Height;
                                        deltaSize.Y = trans.Y - _startSize.Height - _startSize.Height;
                                    }
                                }
                                break;
                            case Direction.RightDown:
                                {
                                    deltaSize.X = trans.X;
                                    deltaSize.Y = trans.Y;
                                    if (trans.X < -_startSize.Width)
                                    {
                                        deltaPos.X = _startSize.Width + trans.X;
                                        deltaSize.X = -(trans.X + _startSize.Width + _startSize.Width);
                                    }
                                    if (trans.Y < -_startSize.Height)
                                    {
                                        deltaPos.Y = trans.Y + _startSize.Height;
                                        deltaSize.Y = -(trans.Y + _startSize.Height + _startSize.Height);
                                    }
                                }
                                break;
                            case Direction.LeftDown:
                                {
                                    deltaPos.X = trans.X;
                                    deltaSize.X = -trans.X;
                                    deltaSize.Y = trans.Y;
                                    if (trans.X > _startSize.Width)
                                    {
                                        deltaPos.X = _startSize.Width;
                                        deltaSize.X = (trans.X - _startSize.Width) - _startSize.Width;
                                    }
                                    if (trans.Y < -_startSize.Height)
                                    {
                                        deltaPos.Y = trans.Y + _startSize.Height;
                                        deltaSize.Y = -(trans.Y + _startSize.Height + _startSize.Height);
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                        OnResizing(new ShiftEventArgs(deltaPos.X, deltaPos.Y, deltaSize.X, deltaSize.Y, ResizingEvent, this));
                    }
                }
            }
        }

        private void GridMain_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_mouseDown)
            {
                _mouseDown = false;
                OnResizeEnd();
            }
        }
    }

    public delegate void ShiftEventHandler(object sender, ShiftEventArgs e);

    public class ShiftEventArgs : RoutedEventArgs
    {
        public double LeftShift { get; private set; }
        public double TopShift { get; private set; }
        public double WidthShift { get; private set; }
        public double HeightShift { get; private set; }

        public ShiftEventArgs(double left, double top, double width, double height, RoutedEvent e, object source)
            : base(e, source)
        {
            this.LeftShift = left;
            this.TopShift = top;
            this.WidthShift = width;
            this.HeightShift = height;
        }
    }
}

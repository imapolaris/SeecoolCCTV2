using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VideoNS.Helper;

namespace VideoNS
{
    /// <summary>
    /// PTZControlPanel.xaml 的交互逻辑
    /// </summary>
    public partial class PTZControlPanel : UserControl
    {
        private Point m_startPos = new Point();
        private List<Grid> m_grids;
        bool _moving = false;
        public PTZControlPanel()
        {
            InitializeComponent();
            this.SizeChanged += (s, e) =>
            {
                RecordRotCenter();
            };
            this.Loaded += (s, e) =>
            {
                RecordRotCenter();
            };
            m_grids = new List<Grid>() { gridAnimLeft, gridAnimUp, gridAnimRight, gridAnimDown };
            girdMain.MouseDown += (s, e) => e.Handled = true;
            new Thread(run) { IsBackground = true }.Start();
        }

        /// <summary>
        /// 记录旋转变换中心点。
        /// </summary>
        private void RecordRotCenter()
        {
            Point pnt = gridArcBall.TranslatePoint(new Point(gridArcBall.ActualWidth / 2, gridArcBall.ActualHeight / 2), gridAnimRight);
            this.Resources["rotCenterX"] = (double)pnt.X;
            this.Resources["rotCenterY"] = (double)pnt.Y;
        }
        static int _count = 0;
        private void SetPTZSpeed(double panSpeed, double tiltSpeed)
        {
            PTZControlModel ptzModel = this.DataContext as PTZControlModel;
            if (ptzModel != null)
            {
                ptzModel.PanTiltMove(panSpeed, tiltSpeed);
            }
            _count++;
            Console.WriteLine(DateTime.Now.TimeOfDay + " Count: " + _count + " " + panSpeed + "  " + tiltSpeed);
            //Console.WriteLine("X:" + panSpeed + "__Y:" + tiltSpeed);
        }

        private void resetCtrlArrow()
        {
            btnCtrlArrow.DataContext = new TransformParam();
            speedBar1.DataContext = new TransformParam();
        }

        private double getRotateAngle(Point pt)
        { //计算角度
            return Math.Atan2(pt.Y - gridArrows.ActualHeight / 2, pt.X - gridArrows.ActualWidth / 2) * 180 / Math.PI;
        }

        private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _moving = true;
            Button btn = e.Source as Button;
            if (btn == null)
                return;
            Console.WriteLine(btn);
            m_startPos = e.GetPosition(gridArcBall);
            btn.AddHandler(Button.MouseMoveEvent, new RoutedEventHandler(Button_MouseMove), true);
            //计算控制按钮的平移
            GeneralTransform gt = btn.TransformToVisual(gridArcBall);
            Point center = gt.Transform(new Point(btn.ActualWidth / 2, btn.ActualHeight / 2));
            //获取旋转角度。
            double angle = getRotateAngle(center);
            //计算相对中心点偏移量。
            Point refCenter = new Point(center.X - gridArcBall.ActualWidth / 2, center.Y - gridArcBall.ActualHeight / 2);
            double maxLen = (gridArcBall.ActualHeight - btn.ActualHeight) / 2;

            TransformParam tp = (TransformParam)btnCtrlArrow.DataContext;
            tp.TranslateX = center.X - btnCtrlArrow.ActualWidth / 2;
            tp.TranslateY = center.Y - btnCtrlArrow.ActualHeight / 2;
            tp.RotateAngle = angle;

            if (!btn.Equals(btnCenter))
            {
                SetPTZSpeed(refCenter.X / maxLen, refCenter.Y / maxLen);
            }
            Console.WriteLine("MouseDown");
        }

        private void Button_MouseMove(object sender, RoutedEventArgs e)
        {
            if (!_moving)
                return;
            Button btn = e.Source as Button;
            if (btn == null)
                return;

            Point pnt = ((MouseEventArgs)e).GetPosition(gridArcBall);
            Point temp = new Point(pnt.X - m_startPos.X, pnt.Y - m_startPos.Y);
            
            //计算控制按钮的平移
            GeneralTransform gt = btn.TransformToVisual(gridArcBall);
            Point center = gt.Transform(new Point(btn.ActualWidth / 2, btn.ActualHeight / 2));
            center = new Point(center.X + temp.X, center.Y + temp.Y);
            //计算相对中心点偏移量。
            Point refCenter = new Point(center.X - gridArcBall.ActualWidth / 2, center.Y - gridArcBall.ActualHeight / 2);
            double len = Math.Sqrt(refCenter.X * refCenter.X + refCenter.Y * refCenter.Y);
            double maxLen = (gridArcBall.ActualHeight - btn.ActualHeight) / 2;
            if (len >= maxLen)
            {
                double scale = maxLen / len;
                refCenter.X *= scale;
                refCenter.Y *= scale;
                len = maxLen;
            }
            //获取旋转角度。
            double angle = getRotateAngle(center);

            //计算速度提示条的平移量。
            double ratio = 1d - (btn.ActualWidth + speedBar1.ActualWidth) / 2 / len;
            Point speedCenter = new Point(refCenter.X * ratio, refCenter.Y * ratio);
            speedCenter = new Point(speedCenter.X + gridArcBall.ActualWidth / 2, speedCenter.Y + gridArcBall.ActualHeight / 2);
            //变换速度提示条。
            TransformParam scTP = (TransformParam)speedBar1.DataContext;
            scTP.TranslateX = speedCenter.X - speedBar1.ActualWidth / 2;
            scTP.TranslateY = speedCenter.Y - speedBar1.ActualHeight / 2;
            scTP.RotateAngle = angle;

            center = new Point(refCenter.X + gridArcBall.ActualWidth / 2, refCenter.Y + gridArcBall.ActualHeight / 2);
            TransformParam tp = (TransformParam)btnCtrlArrow.DataContext;
            tp.TranslateX = center.X - btnCtrlArrow.ActualWidth / 2;
            tp.TranslateY = center.Y - btnCtrlArrow.ActualHeight / 2;
            tp.RotateAngle = angle;
            //设置速度
            if (Math.Abs(temp.X) > 0 || Math.Abs(temp.Y) > 0)
            {
                lock(_obj)
                    _curPosition = new Point(refCenter.X / maxLen, refCenter.Y / maxLen);
                //SetPTZSpeed(refCenter.X / maxLen, refCenter.Y / maxLen);
            }
            speedBar1.Speed = len / maxLen;
        }

        private void Button_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            _moving = false;
            //所有状态归位。
            Button btn = e.Source as Button;
            if (btn == null)
                return;
            btn.RemoveHandler(Button.MouseMoveEvent, new RoutedEventHandler(Button_MouseMove));
            lock(_obj)
            {
                _curPosition = null;
                Common.Log.Logger.Default.Trace("Mouse Up To Stop!");
                SetPTZSpeed(0, 0);
            }
            resetCtrlArrow();
        }

        private void run()
        {
            while (true)
            {
                Thread.Sleep(400);
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    lock (_obj)
                    {
                        if (_curPosition != null && _moving)
                        {
                            Common.Log.Logger.Default.Trace("Mouse Control To:  {0},{1}", _curPosition.Value.X, _curPosition.Value.Y);
                            SetPTZSpeed(_curPosition.Value.X, _curPosition.Value.Y);
                            _curPosition = null;
                        }
                    }
                }));
            }
        }
        object _obj = new object();
        Point? _curPosition = null;
    }

    public class SpeedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double maxLen = System.Convert.ToDouble(parameter);
            double ratio = (double)value / maxLen;
            ratio = ratio > 1 ? 1 : ratio;
            ratio = ratio < -1 ? -1 : ratio;
            return ratio;
        }
    }

    public class OpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 1d - (double)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 1d - (double)value;
        }
    }

    public class FontSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double d = (double)value-13;
            return d > 0 ? d : 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

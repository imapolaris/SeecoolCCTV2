using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using DragDropHelper;
using VideoNS.Adorners;
using VideoNS.AutoSave;
using VideoNS.SplitScreen;
using VideoNS.Helper;

namespace VideoNS.DragDropHandler
{
    public class VideoPanelDropHandler : IDropHandler
    {
        readonly Duration ConstDuration = new Duration(TimeSpan.FromSeconds(0.4));
        VideoPanelDropAdorner _adorner;
        VideoControlModel _videoMode;
        Storyboard _lastSB;

        #region 【装饰器】
        VideoPanelDropAdorner DropAdorner
        {
            get { return _adorner; }
            set
            {
                if (_adorner != null)
                    _adorner.Detatch();
                _adorner = value;
            }
        }

        VideoPanelDropAdorner CreateAdorner(VideoPanelItem vpi)
        {
            VideoPanelDropAdorner adorner = new VideoPanelDropAdorner(vpi, vpi.VideoControl?.videoDisp);
            return adorner;
        }

        //重置装饰器位置。
        void ResetAdorner()
        {
            if (DropAdorner != null)
            {
                if (_lastSB != null)
                {
                    _lastSB.Stop(DropAdorner);
                    _lastSB = null;
                }
                DropAdorner.Visibility = Visibility.Hidden;
                DropAdorner.VisualCover.ViewModel.TransParam.Reset();
            }
        }

        Storyboard CreateStoryboard(Point toPnt, double scaleX, double scaleY, double opacity)
        {
            Storyboard sb = new Storyboard();
            ExponentialEase easing = new ExponentialEase() { Exponent = 4, EasingMode = EasingMode.EaseInOut };

            DoubleAnimation daX = new DoubleAnimation(toPnt.X, ConstDuration);
            daX.SetValue(Storyboard.TargetProperty, DropAdorner.VisualCover);
            daX.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("DataContext.TransParam.TranslateX"));
            daX.EasingFunction = easing;
            sb.Children.Add(daX);

            DoubleAnimation daY = new DoubleAnimation(toPnt.Y, ConstDuration);
            daY.SetValue(Storyboard.TargetProperty, DropAdorner.VisualCover);
            daY.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("DataContext.TransParam.TranslateY"));
            daY.EasingFunction = easing;
            sb.Children.Add(daY);

            DoubleAnimation daSX = new DoubleAnimation(scaleX, ConstDuration);
            daSX.SetValue(Storyboard.TargetProperty, DropAdorner.VisualCover);
            daSX.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("DataContext.TransParam.ScaleX"));
            daSX.EasingFunction = easing;
            sb.Children.Add(daSX);

            DoubleAnimation daSY = new DoubleAnimation(scaleY, ConstDuration);
            daSY.SetValue(Storyboard.TargetProperty, DropAdorner.VisualCover);
            daSY.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("DataContext.TransParam.ScaleY"));
            daSY.EasingFunction = easing;
            sb.Children.Add(daSY);

            DoubleAnimation daOpa = new DoubleAnimation(opacity, ConstDuration);
            daOpa.SetValue(Storyboard.TargetProperty, DropAdorner.VisualBlur);
            daOpa.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("Opacity"));
            sb.Children.Add(daOpa);

            _lastSB = sb;
            return sb;
        }
        #endregion 【装饰器】

        #region 【延时控制】
        private Thread m_monitor = null;
        bool _mouseOn = false;
        bool _accepted = false;

        private void Monitor()
        {
            try
            {
                Thread.Sleep(30);
                if (_accepted && !_mouseOn)
                {
                    if (DropAdorner != null)
                        DropAdorner.Dispatcher.BeginInvoke(new Action(MonitorActive), null);
                }
            }
            catch (ThreadAbortException)
            {
            }
        }

        private void MonitorActive()
        {
            if (DropAdorner != null)
            {
                Storyboard sb = CreateStoryboard(new Point(0, 0), 1, 1, 0);
                sb.Completed += (s, e) =>
                {
                    if (_videoMode != null)
                    {
                        _videoMode.Opacity = 1;
                        _videoMode = null;
                    }
                    DropAdorner.Visibility = Visibility.Hidden;
                    _accepted = false;
                };
                sb.Begin(DropAdorner, true);
            }
        }
        #endregion 【延时控制】

        public void DragEnter(DropInfo info)
        {
            _mouseOn = true;
            _startTick = Environment.TickCount;
        }

        public void DragLeave(DropInfo info)
        {
            _mouseOn = false;
            if (_accepted)
            {
                if (m_monitor != null && m_monitor.IsAlive)
                    m_monitor.Abort();
                m_monitor = new Thread(Monitor);
                m_monitor.IsBackground = true;
                m_monitor.Start();
            }
        }

        //延时拖动
        private int _startTick;
        private int _delayTime = 300; //延时时间。

        public void DragOver(DropInfo info)
        {
            _mouseOn = true;
            if (info.KeyStates.HasFlag(DragDropKeyStates.ControlKey))
                info.Effects = DragDropEffects.Copy;
            else
                info.Effects = DragDropEffects.Move;
            if (!_accepted)
            {
                VideoPanelItem vpi = info.Target as VideoPanelItem;
                VideoPanelItem srcVpi = info.Source as VideoPanelItem;

                //延时拖动接收。
                if (Environment.TickCount - _startTick < _delayTime)
                    return;

                string videoId = info.Data as string;
                if (videoId == null && info.DataType.ToUpper().EndsWith(".STRING"))
                {
                    videoId = info.GetDataFromJson<string>();
                }
                if (videoId == null)
                {
                    info.Effects = DragDropEffects.None;
                    return;
                }
                if (vpi != null && videoId != null && !vpi.Equals(srcVpi))
                {
                    _videoMode = vpi.ViewModel.ControlViewModel;
                    DropAdorner = CreateAdorner(vpi);
                    DropAdorner.VisualCover.Visibility = Visibility.Hidden;
                    //
                    Point toPnt = new Point(0, 0);
                    double scaleX = 1;
                    double scaleY = 1;
                    if (srcVpi != null && IsBrothers(srcVpi, vpi))
                    {
                        //隐藏源的视频。
                        if (vpi.VideoControl != null && vpi.ViewModel.ControlViewModel.IsVisible)
                        {
                            _videoMode.Opacity = 0;
                            DropAdorner.VisualCover.Visibility = Visibility.Visible;
                            toPnt = vpi.PointFromScreen(srcVpi.PointToScreen(new Point(0, 0)));
                            scaleX = srcVpi.ActualWidth / vpi.ActualWidth;
                            scaleY = srcVpi.ActualHeight / vpi.ActualHeight;
                        }
                    }
                    //启动动画。
                    Storyboard sb = CreateStoryboard(toPnt, scaleX, scaleY, 1);
                    sb.Begin(DropAdorner, true);
                    _accepted = true;
                }
            }
        }

        public void Drop(DropInfo info)
        {
            _mouseOn = false;
            _accepted = false;
            ResetAdorner();
            if (_videoMode != null)
            {
                _videoMode.Opacity = 1;
                _videoMode = null;
            }
            string videoId = info.Data as string;
            if (videoId == null)
                videoId = info.GetDataFromJson<string>();
            if (videoId != null)
            {
                VideoPanelItem vpi = info.Target as VideoPanelItem;
                if (vpi != null)
                {
                    VideoPanelItem srcVpi = info.Source as VideoPanelItem;
                    if (srcVpi != null && !srcVpi.Equals(vpi)
                        && !info.KeyStates.HasFlag(DragDropKeyStates.ControlKey))
                    {
                        //没有交换则直接设置ID。
                        if (!Exchange(srcVpi, vpi))
                        {
                            vpi.PlayVideo(videoId);
                            info.Effects = DragDropEffects.Copy;
                        }
                        else
                            info.Effects = DragDropEffects.Move;
                    }
                    else
                    {
                        vpi.PlayVideo(videoId);
                        info.Effects = DragDropEffects.Move;
                    }
                }
            }
        }

        //仅限于交换同级panel的位置。
        private bool Exchange(FrameworkElement srcFe, FrameworkElement tarFe)
        {
            if (IsBrothers(srcFe, tarFe))
            {
                int rowT = Grid.GetRow(tarFe);
                int colT = Grid.GetColumn(tarFe);
                int rowSpanT = Grid.GetRowSpan(tarFe);
                int colSpanT = Grid.GetColumnSpan(tarFe);

                int rowS = Grid.GetRow(srcFe);
                int colS = Grid.GetColumn(srcFe);
                int rowSpanS = Grid.GetRowSpan(srcFe);
                int colSpanS = Grid.GetColumnSpan(srcFe);

                Grid.SetRow(tarFe, rowS);
                Grid.SetColumn(tarFe, colS);
                Grid.SetRowSpan(tarFe, rowSpanS);
                Grid.SetColumnSpan(tarFe, colSpanS);

                Grid.SetRow(srcFe, rowT);
                Grid.SetColumn(srcFe, colT);
                Grid.SetRowSpan(srcFe, rowSpanT);
                Grid.SetColumnSpan(srcFe, colSpanT);

                //更改顺序。
                Panel parent = tarFe.Parent as Panel;
                int index1 = parent.Children.IndexOf(srcFe);
                int index2 = parent.Children.IndexOf(tarFe);
                SplitScreenPanel ssp = parent.FindVisualParent<SplitScreenPanel>();
                if (ssp.ViewModel!=null)
                {
                    //自动保存中数据项交换
                    var ssd = ssp.ViewModel.SplitScreenData;
                    var first = ssd.Nodes[index1];
                    ssd.Nodes[index1] = ssd.Nodes[index2];
                    ssd.Nodes[index2] = first;
                    ssp.ViewModel.SplitScreenData = ssd;
                }

                int min = Math.Min(index1, index2);
                int max = Math.Max(index1, index2);

                UIElement uiMin = parent.Children[min];
                UIElement uiMax = parent.Children[max];
                parent.Children.RemoveAt(max);
                parent.Children.RemoveAt(min);
                parent.Children.Insert(min, uiMax);
                parent.Children.Insert(max, uiMin);
                return true;
            }
            return false;
        }

        private bool IsBrothers(FrameworkElement srcFe, FrameworkElement tarFe)
        {
            Panel panelT = tarFe.Parent as Panel;
            Panel panelS = srcFe.Parent as Panel;
            return panelT != null && panelT.Equals(panelS);
        }
    }
}

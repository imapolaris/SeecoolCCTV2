using DragDropHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using VideoNS.Adorners;
using VideoNS.SplitScreen;

namespace VideoNS.DragDropHandler
{
    public class SplitScreenDropHandler : IDropHandler
    {
        readonly Duration ConstDuration = new Duration(TimeSpan.FromSeconds(0.4));
        SplitScreenAdorner _adorner;
        Storyboard _lastSB;

        #region 【装饰器】
        SplitScreenAdorner SplitAdorner
        {
            get { return _adorner; }
            set
            {
                if (_adorner != null)
                    _adorner.Detatch();
                _adorner = value;
            }
        }

        SplitScreenAdorner CreateAdorner(SplitScreenPanel ssp)
        {
            SplitScreenAdorner adorner = new SplitScreenAdorner(ssp);
            return adorner;
        }

        //重置装饰器位置。
        void ResetAdorner()
        {
            if (SplitAdorner != null)
            {
                if (_lastSB != null)
                {
                    _lastSB.Stop(SplitAdorner);
                    _lastSB = null;
                }
                SplitAdorner.Visibility = Visibility.Hidden;
            }
        }

        Storyboard CreateStoryboard(double opacity)
        {
            Storyboard sb = new Storyboard();

            DoubleAnimation daOpa = new DoubleAnimation(opacity, ConstDuration);
            daOpa.SetValue(Storyboard.TargetProperty, SplitAdorner.VisualBlur);
            daOpa.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("Opacity"));
            sb.Children.Add(daOpa);

            sb.AccelerationRatio = 0.5;
            sb.DecelerationRatio = 0.5;

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
                    if (SplitAdorner != null)
                        SplitAdorner.Dispatcher.BeginInvoke(new Action(MonitorActive), null);
                }
            }
            catch (ThreadAbortException)
            {
            }
        }

        private void MonitorActive()
        {
            if (SplitAdorner != null)
            {
                Storyboard sb = CreateStoryboard(0);
                sb.Completed += (s, e) =>
                {
                    SplitAdorner.Visibility = Visibility.Hidden;
                    _accepted = false;
                };
                sb.Begin(SplitAdorner, true);
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
        private int _delayTime = 100; //延时时间。

        public void DragOver(DropInfo info)
        {
            _mouseOn = true;
            if (!_accepted)
            {
                SplitScreenPanel ssp = info.Target as SplitScreenPanel;
                //延时拖动接收。
                if (Environment.TickCount - _startTick < _delayTime)
                    return;
                List<string> ids = info.Data as List<string>;
                if (ids == null)
                {
                    info.Effects = DragDropEffects.None;
                    return;
                }

                SplitAdorner = CreateAdorner(ssp);
                //启动动画。
                Storyboard sb = CreateStoryboard(1);
                sb.Begin(SplitAdorner, true);
                _accepted = true;
            }
        }

        public void Drop(DropInfo info)
        {
            _mouseOn = false;
            _accepted = false;
            ResetAdorner();
            List<string> ids = info.Data as List<string>;
            if (ids != null)
            {
                SplitScreenPanel ssp = info.Target as SplitScreenPanel;
                if (ssp != null)
                {
                    foreach (string id in ids)
                    {
                        ssp.PlayVideoOnBlank(id);
                    }
                }
            }
        }
    }
}

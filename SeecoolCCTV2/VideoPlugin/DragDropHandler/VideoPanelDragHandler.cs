using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using DragDropHelper;
using VideoNS.SubWindow;
using VideoNS.Helper;

namespace VideoNS.DragDropHandler
{
    public class VideoPanelDragHandler : IDragHandler
    {
        private Window _topWin;

        public void DragEnd(DragInfo info)
        {
            VideoPanelItem vpi = info.Source as VideoPanelItem;
            if (info.Effects == DragDropEffects.None)
            {
                //当鼠标移出窗体且没有接收源时，打开一个新视频窗口，同时关闭源视频。
                if (!IsCursorInWindow(info) && vpi.ViewModel != null && vpi.ViewModel.IsInEditStatus)
                {
                    if (ShowVideoWin(info, vpi))
                    {
                        if (vpi.ViewModel != null && vpi.ViewModel.ControlViewModel != null)
                            vpi.ViewModel.ControlViewModel.VideoId = null;
                    }
                }
            }
            _topWin = null;
            updateOpacity(vpi, 1);
        }

        public void Dragging(DragInfo info)
        {
            VideoPanelItem vpi = info.Source as VideoPanelItem;
            if (info.Effects == DragDropEffects.None)
            {
                if (!IsCursorInWindow(info) && vpi.ViewModel != null && vpi.ViewModel.IsInEditStatus)
                {
                    Mouse.SetCursor(Cursors.Hand);
                    info.Handled = true;
                }
            }
            else
            {
                Mouse.SetCursor(Cursors.SizeAll);
                info.Handled = true;
            }

            if (vpi != null)
            {
                if (info.Effects.HasFlag(DragDropEffects.Copy))
                {
                    updateOpacity(vpi, 1);
                }
                else
                    updateOpacity(vpi, 0);
            }

        }

        public void DragStart(DragInfo info)
        {
            VideoPanelItem vpi = info.Source as VideoPanelItem;
            if (vpi.VideoControl != null)
            {
                Binding binding = new Binding();
                binding.Mode = BindingMode.OneWay;
                binding.Source = vpi.VideoControl.videoDisp;
                vpi.SetBinding(DragDropVisual.VisualProperty, binding);
                _topWin = FindTopWindow(vpi);
                updateOpacity(vpi, 0);
            }
        }

        private void updateOpacity(VideoPanelItem vpi, double opacity)
        {
            if (vpi?.ViewModel?.ControlViewModel != null)
                vpi.ViewModel.ControlViewModel.Opacity = opacity;
        }

        private Window FindTopWindow(FrameworkElement fe)
        {
            Window topWin = null;
            DependencyObject parent = fe?.Parent;
            while (parent != null)
            {
                topWin = parent as Window;
                if (topWin != null)
                    break;
                fe = parent as FrameworkElement;
                if (fe != null)
                    parent = fe.Parent;
            }
            return topWin;
        }

        private bool ShowVideoWin(DragInfo info, VideoPanelItem vpi)
        {
            if (_topWin != null)
            {
                string videoID = (string)info.Data;
                PopupWinModel model = new PopupWinModel(videoID);
                PopupWin popup = new PopupWin();
                popup.IsTopmost = true;
                popup.DataContext = model;
                Point startPos = info.GetPosition(vpi);
                startPos = vpi.PointToScreen(startPos);
                startPos = new Point(startPos.X - vpi.ActualWidth / 2, startPos.Y - vpi.ActualHeight / 2);
                if (startPos.X < 0)
                    startPos.X = 0;
                if (startPos.Y < 0)
                    startPos.Y = 0;
                popup.Left = startPos.X;
                popup.Top = startPos.Y;
                if (vpi.ActualWidth < AppConstants.VideoControlMinWidth)
                {
                    popup.Width = AppConstants.VideoControlMinWidth;
                    if (vpi.ActualHeight == 0)
                    {
                        popup.Height = AppConstants.VideoControlMinWidth * 0.75;
                    }
                    else
                    {
                        popup.Height = vpi.ActualHeight * (AppConstants.VideoControlMinWidth / vpi.ActualWidth);
                    }
                }
                else
                {
                    popup.Width = vpi.ActualWidth;
                    popup.Height = vpi.ActualHeight;
                }
                popup.Show();
                return true;
            }
            return false;
        }

        private bool IsCursorInWindow(DragInfo info)
        {
            if (_topWin == null)
                return false;
            Point cursorPos = info.GetPosition(_topWin);
            return cursorPos.X > 0 && cursorPos.X < _topWin.ActualWidth && cursorPos.Y > 0 && cursorPos.Y < _topWin.ActualHeight;
        }
    }
}

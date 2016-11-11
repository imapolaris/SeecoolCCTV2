using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DragDropHelper;
using VideoNS.SubWindow;

namespace VideoNS.DragDropHandler
{
    public class PopupWinDragHandler : IDragHandler
    {
        private PopupWin _popupWin;
        private Point _startPos;
        public void DragEnd(DragInfo info)
        {
            if (!(info.Effects == DragDropEffects.Move || info.Effects == DragDropEffects.Copy))
            {
                Point pos = info.PointOnScreen;
                _popupWin.Left = _popupWin.Left + (pos.X - _startPos.X);
                _popupWin.Top = _popupWin.Top + (pos.Y - _startPos.Y);
                _popupWin.Visibility = Visibility.Visible;
            }
            else
            {
                _popupWin.Close();
            }
        }

        public void Dragging(DragInfo info)
        {
        }

        public void DragStart(DragInfo info)
        {
            _popupWin = FindTopWindow(info.Source as FrameworkElement);
            _startPos = info.PointOnScreen;
            _popupWin.ViewModel.ControlModel.IsControlPanelVisible = false;
            _popupWin.Visibility = Visibility.Collapsed;
        }

        private PopupWin FindTopWindow(FrameworkElement fe)
        {
            PopupWin topWin = null;
            DependencyObject parent = fe?.Parent;
            while (parent != null)
            {
                topWin = parent as PopupWin;
                if (topWin != null)
                    break;
                fe = parent as FrameworkElement;
                if (fe != null)
                    parent = fe.Parent;
            }
            return topWin;
        }
    }
}

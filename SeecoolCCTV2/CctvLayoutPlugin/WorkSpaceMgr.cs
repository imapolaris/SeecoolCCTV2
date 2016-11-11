using Common.EventArg;
using Common.Message;
using Common.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Docking;

namespace CctvLayoutPlugin
{
    internal class WorkSpaceMgr
    {
        ContentControl _contentControl;

        public WorkSpaceMgr(ContentControl contentControl)
        {
            _contentControl = contentControl;

            RemoteCalls.Global.Register<object>("CCTV2_LayoutPlugin_SetPane", setPane);
        }

        void setPane(object pane)
        {
            WindowUtil.Invoke(() => { _contentControl.Content = pane; });
        }
    }
}

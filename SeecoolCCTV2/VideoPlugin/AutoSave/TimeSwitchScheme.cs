using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VideoNS.AutoSave;
using VideoNS.Json;
using VideoNS.TimeSwitch;

namespace VideoNS.TimeSwitch
{
    public class TimeSwitchScheme
    {
        private static TimeSwitchScheme _instance;

        public static TimeSwitchScheme Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TimeSwitchScheme();
                return _instance;
            }
        }

        private TimeSwitchScheme()
        {

        }

        private TimeSwitchInfo[] _scheme;
        public TimeSwitchInfo[] Scheme
        {
            get
            {
                if (_scheme == null)
                {
                    LoadScheme();
                }
                return _scheme;
            }
            private set
            {
                UninstallSubEvent(_scheme);
                _scheme = value;
                InstallSubEvent(_scheme);
            }
        }

        private void LoadScheme()
        {
            TimeSwitchInfo[] schemes = TimeSwitchAutoSave.LoadData();
            Scheme = (schemes == null ? new TimeSwitchInfo[0] : schemes);
        }

        public void Reset()
        {
            Scheme = null;
        }

        private void InstallSubEvent(TimeSwitchInfo[] nodes)
        {
            if (nodes != null)
            {
                foreach (TimeSwitchInfo node in nodes)
                    if (node != null)
                        node.DataChanged += Node_DataChanged;
            }
        }

        private void UninstallSubEvent(TimeSwitchInfo[] nodes)
        {
            if (nodes != null)
            {
                foreach (TimeSwitchInfo node in nodes)
                    if (node != null)
                        node.DataChanged -= Node_DataChanged;
            }
        }
        private void Node_DataChanged(object sender, EventArgs e)
        {
            AutoSave();
        }

        #region 【执行AutoSave】
        private void AutoSave()
        {
            TimeSwitchAutoSave.LazySaveData(Scheme);
        }
        #endregion 【执行AutoSave】
    }
}

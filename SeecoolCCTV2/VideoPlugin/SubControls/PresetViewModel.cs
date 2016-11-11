using AopUtil.WpfBinding;
using Common.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace VideoNS.SubControls
{
    public class PresetViewModel : ObservableObject
    {
        public PresetViewModel()
        {
            SelectedIndex = -1;
            SaveCmd = new DelegateCommand(_ => DoSave());
            GotoCmd = new DelegateCommand(_ => DoGoto());
        }

        [AutoNotify]
        public bool IsVisible { get; set; }

        [AutoNotify]
        public int SelectedIndex { get; set; }

        [AutoNotify]
        public string VideoId { get; set; }

        public ICommand SaveCmd { get; private set; }

        public ICommand GotoCmd { get; private set; }

        private void DoSave()
        {
            if (SelectedIndex >= 1 && SelectedIndex <= 32 && VideoId != null)
                CameraControlRemoteCall.Instance.CameraControl(VideoId, "SetPreset", SelectedIndex);
        }

        private void DoGoto()
        {
            if (SelectedIndex >= 1 && SelectedIndex <= 32 && VideoId != null)
                CameraControlRemoteCall.Instance.CameraControl(VideoId, "GoPreset", SelectedIndex);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AopUtil.WpfBinding;
using VideoNS.AutoSave;

namespace VideoNS.SplitScreen
{
    public class SplitControlModel : ObservableObject
    {
        public SplitControlModel()
        {
            SettingModel = new SettingControlModel();
            CenterGridRowIndex = 1;
            CenterGridRowSpan = 2;

            SettingModel.PropertyChanged += SettingModel_PropertyChanged;
        }

        private void SettingModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SettingModel.WindowState):
                    {
                        if(SettingModel.WindowState== System.Windows.WindowState.Maximized)
                        {
                            CenterGridRowIndex = 0;
                            CenterGridRowSpan = 3;
                        }
                        else
                        {
                            CenterGridRowIndex = 1;
                            CenterGridRowSpan = 2;
                        }
                    }
                    break;
            }
        }
        [AutoNotify]
        public SettingControlModel SettingModel { get; private set; }

        [AutoNotify]
        public int CenterGridRowIndex { get; set; }
        [AutoNotify]
        public int CenterGridRowSpan { get; set; }
    }
}

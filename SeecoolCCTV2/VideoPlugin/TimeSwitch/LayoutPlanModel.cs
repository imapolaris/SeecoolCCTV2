using AopUtil.WpfBinding;
using Common.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VideoNS.Layout;
using VideoNS.Model;
using VideoNS.SplitScreen;

namespace VideoNS.TimeSwitch
{
    public class LayoutPlanModel : ObservableObject
    {
        public LayoutPlanModel()
        {
            PropertyChanged += onPropertyChanged;
            StaySeconds = 20;
            LayoutSource = new SplitScreenModel();
            LayoutSource.SplitScreenData = SplitScreenLayoutsModel.Get一大多小(3).SplitScreenInfom;
            DeleteCommand = new DelegateCommand(_ => ToDelete());
        }

        [AutoNotify]
        public double StaySeconds { get; set; }

        private SplitScreenModel _layoutSource;
        public SplitScreenModel LayoutSource
        {
            get { return _layoutSource; }
            set
            {
                UninstallSubEvent(_layoutSource);
                updateProperty(ref _layoutSource, value);
                InstallSubEvent(_layoutSource);
            }
        }

        public Action<LayoutPlanModel> ToBeDelete { get; set; }
        public ICommand DeleteCommand { get; set; }
        public void ToDelete()
        {
            if (ToBeDelete != null)
                ToBeDelete(this);
        }

        public Action ToBeClearLink { get; set; }

        private void onPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(StaySeconds):
                    if (StaySeconds <= 0)
                        StaySeconds = 1;
                    else
                        OnDataChanged(e);
                    break;
                case nameof(LayoutSource):
                    OnDataChanged(e);
                    break;
            }
        }

        private void InstallSubEvent(SplitScreenModel layout)
        {
            if (layout != null)
                layout.DataChanged += Layout_DataChanged;
        }

        private void UninstallSubEvent(SplitScreenModel layout)
        {
            if (layout != null)
                layout.DataChanged -= Layout_DataChanged;
        }

        private void Layout_DataChanged(object sender, EventArgs e)
        {
            OnDataChanged(e);
        }

        public event EventHandler DataChanged;

        protected virtual void OnDataChanged(EventArgs e)
        {
            if (DataChanged != null)
                DataChanged(this, e);
        }
    }
}

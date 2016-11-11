using AopUtil.WpfBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using VideoNS.VideoDisp;
using VideoNS.VideoInfo;
using System.Collections.ObjectModel;
using VideoNS.VideoControlViewModel;
using Common.Util;
using VideoNS.SubControls;
using CCTVModels;

namespace VideoNS
{
    public class RealTimeControlModel : ObservableObject
    {
        private const string DefaultStreamType = "未知";
        private bool _streamRefreshing = false;

        public RealTimeControlModel()
        {
            StreamType = new CollectionViewSource();
            SelectedStreamType = DefaultStreamType;
            VideoInfoMessage = new VideoInfoMessageViewModel();
            PTZControl = new PTZControlModel();
            PresetModel = new PresetViewModel();
            SwitchModel = new SwitchPanelViewModel();
            TrackSource = new VideoTrackViewModel(VideoId);
            PropertyChanged += onPropertyChanged;
            PTZControl.PropertyChanged += PTZControl_PropertyChanged;
            TrackSource.PropertyChanged += TrackSource_PropertyChanged;
        }

        private void TrackSource_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(TrackSource.IsVisible):
                    if (TrackSource.IsVisible)
                    {
                        PTZControl.IsVisible = false;
                        DisplayModel.VideoTransform.Reset();
                    }
                    break;
            }
        }

        private void PTZControl_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(PTZControl.IsVisible):
                    if (PTZControl.IsVisible)
                        TrackSource.IsVisible = false;
                    break;
            }
        }

        private void onPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SelectedStreamType):
                    IsShowStreamType = false;
                    if (DisplayModel.StreamInfoArray != null && !_streamRefreshing)
                    {
                        StreamInfo si = DisplayModel.StreamInfoArray.FirstOrDefault<StreamInfo>(s => { return s.Name.Equals(SelectedStreamType); });
                        if (si != null)
                            updateVideo(si.Index);
                    }
                    break;
                case nameof(VideoId):
                    IsShowStreamType = false;
                    PTZControl.VideoId = VideoId;
                    PresetModel.VideoId = VideoId;
                    SwitchModel.VideoId = VideoId;
                    updateVideo();
                    if (TrackSource != null)
                    {
                        TrackSource.PropertyChanged -= TrackSource_PropertyChanged;
                        TrackSource.Dispose();
                    }
                    TrackSource = new VideoTrackViewModel(VideoId);
                    TrackSource.PropertyChanged += TrackSource_PropertyChanged;
                    break;
            }
        }

        private void DisplayModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(DisplayModel.StreamInfoArray):
                    updateStreamTypes();
                    break;
                case nameof(DisplayModel.StreamName):
                    WindowUtil.BeginInvoke(() => { SelectedStreamType = DisplayModel.StreamName == null ? DefaultStreamType : DisplayModel.StreamName; });
                    break;
                case nameof(DisplayModel.VideoName):
                    if (PTZControl.IsShowControl)
                    {
                        PTZControl.UpdatePTZStatus();
                        TrackSource.LoadTrackSwap(VideoId);
                        SwitchModel.UpdateSwitchInfo();
                    }
                    break;
            }
        }

        void updateVideo(int steamIndex = -1)
        {
            if (string.IsNullOrEmpty(VideoId))
            {
                DisplayModel.StopVideo();
                DisplayModel.ClearVideoImage();
            }
            else
            {
                DisplayModel.PlayVideo(VideoId, steamIndex);
            }
        }

        private void updateStreamTypes()
        {
            _streamRefreshing = true;
            StreamInfo[] infos = DisplayModel.StreamInfoArray;
            if (infos != null && infos.Length > 0)
            {
                var names = from info in infos
                            select info.Name;
                StreamType.Source = new ObservableCollection<string>(names);
            }
            else
            {
                StreamType.Source = new ObservableCollection<string>(new string[] { DefaultStreamType });
            }
            _streamRefreshing = false;
        }

        private VideoDisplayViewModel _displayModel;
        public VideoDisplayViewModel DisplayModel
        {
            get { return _displayModel; }
            set
            {
                if (_displayModel != null)
                    _displayModel.PropertyChanged -= DisplayModel_PropertyChanged;
                updateProperty(ref _displayModel, value);
                if (_displayModel != null)
                    _displayModel.PropertyChanged += DisplayModel_PropertyChanged;
            }
        }

        [AutoNotify]
        public bool IsVisible { get; set; }

        [AutoNotify]
        public bool IsShowStreamType { get; set; }

        [AutoNotify]
        public CollectionViewSource StreamType { get; set; }

        [AutoNotify]
        public string VideoId { get; set; }

        [AutoNotify]
        public string SelectedStreamType { get; set; }

        //【信息提示】
        [AutoNotify]
        public VideoInfoMessageViewModel VideoInfoMessage { get; set; }

        //【云镜控制】
        [AutoNotify]
        public PTZControlModel PTZControl { get; set; }

        [AutoNotify]
        public PresetViewModel PresetModel { get; set; }

        [AutoNotify]
        public SwitchPanelViewModel SwitchModel { get; set; }

        [AutoNotify]
        public VideoTrackViewModel TrackSource { get; set; }
    }
}
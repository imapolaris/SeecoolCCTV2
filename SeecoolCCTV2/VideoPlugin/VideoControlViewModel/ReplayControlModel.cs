using AopUtil.WpfBinding;
using Common.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using VideoNS.VideoDisp;

namespace VideoNS
{
    public class ReplayControlModel : ObservableObject
    {
        public ReplayControlModel()
        {
            PreFrameCommand = new DelegateCommand(_ => preFrame());
            NextFrameCommand = new DelegateCommand(_ => nextFrame());
            RePlaySpeedCommand = new DelegateCommand(_ => replaySpeedCommand());
            SpeedType = new CollectionViewSource();
            PlaySlider = new PlaySliderViewModel();
            PropertyChanged += onPropertyChanged;
            SpeedType.Source = new ObservableCollection<string>(new List<string>() { "8X", "4X", "2X", "正常", "1/2", "1/4", "1/8" });
            SelectedSpeedType = "正常";
        }

        private void onPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IsVisible):
                    SelectedSpeedType = "正常";
                    PlaySlider.IsVisible = IsVisible;
                    DisplayModel.FlashbackState = IsVisible; //回放命令键可见时，进入回放状态。
                    IsPlay = IsVisible;
                    break;
                case nameof(SelectedSpeedType):
                    {
                        double speed = getReplaySpeed(SelectedSpeedType);
                        if (DisplayModel != null)
                            DisplayModel.FlashbackSpeed = speed;
                        IsShowReplayControl = false;
                    }
                    break;
                case nameof(DisplayModel):
                    PlaySlider.DisplayModel = DisplayModel;
                    break;
                case nameof(IsPlay):
                    DisplayModel.FlashbackPlaying = IsPlay;
                    break;
            }
        }

        double getReplaySpeed(string speedString)
        {
            double speed = 1;
            switch (speedString)
            {
                case "8X":
                    speed = 8;
                    break;
                case "4X":
                    speed = 4;
                    break;
                case "2X":
                    speed = 2;
                    break;
                case "正常":
                    speed = 1;
                    break;
                case "1/2":
                    speed = 0.5;
                    break;
                case "1/4":
                    speed = 0.25;
                    break;
                case "1/8":
                    speed = 0.125;
                    break;
            }
            return speed;
        }

        [AutoNotify]
        public VideoDisplayViewModel DisplayModel { get; set; }

        [AutoNotify]
        public bool IsVisible { get; set; }

        [AutoNotify]
        public bool IsPlay { get; set; }

        #region 播放速度
        public ICommand RePlaySpeedCommand { get; set; }

        private void replaySpeedCommand()
        {
            IsShowReplayControl = !IsShowReplayControl;
        }

        [AutoNotify]
        public bool IsShowReplayControl { get; set; }

        [AutoNotify]
        public CollectionViewSource SpeedType { get; set; }

        [AutoNotify]
        public string SelectedSpeedType { get; set; }

        #endregion 播放速度

        #region 前后帧
        public DelegateCommand PreFrameCommand { get; set; }
        public DelegateCommand NextFrameCommand { get; set; }

        void preFrame()
        {//TODO: 前一帧
            Console.WriteLine("Pre Frame!");
        }

        void nextFrame()
        {//TODO: 后一帧
            Console.WriteLine("Next Frame!");
        }
        #endregion 前后帧

        #region 播放时间
        [AutoNotify]
        public PlaySliderViewModel PlaySlider { get; set; }
        #endregion 播放时间
    }
}

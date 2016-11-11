using AopUtil.WpfBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Globalization;
using VideoNS.VideoDisp;

namespace VideoNS
{
    public class PlaySliderViewModel : ObservableObject
    {
        public double PlayMinutes = 5;
        private bool _innerUpdate = false;
        public PlaySliderViewModel()
        {
            PropertyChanged += onPropertyChanged;
        }

        private void onPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SliderTime):
                    Slider = getSliderFromTime();
                    break;
                case nameof(Slider):
                    if (!_innerUpdate)
                    {
                        SliderTime = getTimeFromSlider();
                        DisplayModel.SeekFlashback(SliderTime);
                    }
                    break;
            }
        }
        
        private void InitSlider()
        {
            if (BeginTime > EndTime)
                return;
            SliderMaximum = (int)(Math.Round((EndTime - BeginTime).TotalSeconds * 1000));
        }

        private void DisplayModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(DisplayModel.StartFlashbackTime):
                    BeginTime = DisplayModel.StartFlashbackTime;
                    InitSlider();
                    break;
                case nameof(DisplayModel.EndFlashbackTime):
                    EndTime = DisplayModel.EndFlashbackTime;
                    InitSlider();
                    break;
                case nameof(DisplayModel.CurrentFlashbackTime):
                    _innerUpdate = true;
                    SliderTime = DisplayModel.CurrentFlashbackTime;
                    _innerUpdate = false;
                    break;
            }
        }

        private VideoDisplayViewModel _displayModel;
        public VideoDisplayViewModel DisplayModel
        {
            get { return _displayModel; }
            set
            {
                if(_displayModel!=null)
                    _displayModel.PropertyChanged -= DisplayModel_PropertyChanged;
                updateProperty(ref _displayModel, value);
                if (_displayModel != null)
                    _displayModel.PropertyChanged += DisplayModel_PropertyChanged;
            }
        }

        

        [AutoNotify]
        public bool IsVisible { get; set; }
        
        [AutoNotify]
        public DateTime BeginTime { get; set; }

        [AutoNotify]
        public DateTime EndTime { get; set; }

        [AutoNotify]
        public DateTime SliderTime { get; set; }
        
        [AutoNotify]
        public double Slider { get; set; }
        
        [AutoNotify]
        public double SliderMaximum { get; set; }
        
        int getSliderFromTime()
        {
            return (int)Math.Round((SliderTime - BeginTime).TotalSeconds * 1000);
        }

        DateTime getTimeFromSlider()
        {
            return BeginTime.AddSeconds(Slider / 1000.0);
        }
    }
}

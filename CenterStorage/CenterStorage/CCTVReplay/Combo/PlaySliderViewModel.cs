using AopUtil.WpfBinding;
using CCTVReplay.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CCTVReplay.Combo
{
    public class PlaySliderViewModel: ObservableObject, IDisposable
    {
        [AutoNotify]
        public DateTime BeginTime { get; private set; }
        [AutoNotify]
        public DateTime EndTime { get; private set; }

        [AutoNotify]
        public int SliderMaximum { get; private set; } = 10;
        [AutoNotify]
        public int Slider { get; set; }

        [AutoNotify]
        public DateTime SliderTime { get; set; }

        public Action ProgressOffsetEvent;
        ITimeProcess _timeProcess;
        ManualResetEvent _disposeEvent = new ManualResetEvent(false);
        
        public PlaySliderViewModel(ITimeProcess timeProcess)
        {
            _timeProcess = timeProcess;
            ProgressBarMouseDownCmd = new CommandDelegate(_ => doProgBarMouseDownCmd());
            ProgressBarMouseUpCmd = new CommandDelegate(_ => doProgBarMouseUpCmd());
            PropertyChanged += onPropertyChanged;
            _disposeEvent.Reset();
            new Thread(run) { IsBackground=true }.Start();
        }

        private void onPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case nameof(Slider):
                    {
                        //Console.WriteLine(Slider);
                        SliderTime = (DateTime)BeginTime.AddSeconds(Slider);
                        onProgressOffset();
                        if (_sliderMouseDown)
                            onJump();
                    }
                    break;
                case nameof(BeginTime):
                    {
                        if (BeginTime != null)
                            Slider = 0;
                    }
                    break;
            }
        }

        private void run()
        {
            while (!_disposeEvent.WaitOne(100))
            {
                if (!_sliderMouseDown)
                {
                    DateTime time = _timeProcess.GetPlayingTime();
                    if (EndTime != null && time > EndTime)
                        time = EndTime;
                    if (BeginTime != null)
                        Slider = (int)(time - BeginTime).TotalSeconds;
                }
            }
        }

        private void onProgressOffset()
        {
            var handler = ProgressOffsetEvent;
            if (handler != null)
                handler();
        }

        public void UpdateTimes(DateTime begin, DateTime end)
        {
            BeginTime = begin;
            EndTime = end;
            SliderTime = begin;
            Slider = 0;
            SliderMaximum = (int)Math.Max(1, (end - begin).TotalSeconds);
        }

        public void JumpTo(DateTime time)
        {
            doProgBarMouseDownCmd();
            if (time <= BeginTime)
                Slider = 0;
            else if (time >= EndTime)
                Slider = SliderMaximum;
            else
                Slider = (int)(time - BeginTime).TotalSeconds;
            doProgBarMouseUpCmd();
        }

        [AutoNotify]
        public ICommand ProgressBarMouseDownCmd { get; private set; }
        [AutoNotify]
        public ICommand ProgressBarMouseUpCmd { get; private set; }

        private bool _sliderMouseDown = false;
        Guid _guid = Guid.NewGuid();
        private int _recordProg = 0;
        private void doProgBarMouseDownCmd()
        {
            _sliderMouseDown = true;
            _timeProcess.AddCache(_guid);
            _recordProg = Slider;
        }

        private void doProgBarMouseUpCmd()
        {
            if (_recordProg != Slider)
                onJump();
            _sliderMouseDown = false;
            _timeProcess.RemoveCache(_guid);
        }

        public Action JumpEvent { get; set; }
        private void onJump()
        {
            var handler = JumpEvent;
            if (handler != null)
                handler();
        }

        public void Dispose()
        {
            _disposeEvent.Set();
        }
    }
}
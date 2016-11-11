using AopUtil.WpfBinding;
using Common.Command;
using System;
using System.Windows.Input;

namespace VideoNS
{
    public class PTZControlModel : ObservableObject
    {
        public string VideoId { get; set; }
        [AutoNotify]
        public bool IsVisible { get; set; }
        [AutoNotify]
        public bool IsEnable { get; set; }
        [AutoNotify]
        public bool IsShowControl { get; set; }

        public ICommand ZoomOutCommand { get; set; }
        public ICommand ZoomInCommand { get; set; }
        public ICommand ZoomStopCommand { get; set; }
        public ICommand FocusNearCommand { get; set; }
        public ICommand FocusFarCommand { get; set; }
        public ICommand FocusStopCommand { get; set; }

        public PTZControlModel()
        {
            ZoomInCommand = newCommand("ZoomIn");
            ZoomOutCommand = newCommand("ZoomOut");
            ZoomStopCommand = newCommand("StopZoom");
            FocusNearCommand = newCommand("FocusNear");
            FocusFarCommand = newCommand("FocusFar");
            FocusStopCommand = newCommand("StopFocus");
            IsShowControl = true;
        }

        public void UpdatePTZStatus()
        {
            bool isEnable = false;
            if (!string.IsNullOrWhiteSpace(VideoId))
            {
                var control = CCTVInfoManager.Instance.GetControlConfig(VideoId);
                isEnable = (control != null && control.Type != CCTVModels.CCTVControlType.UnControl);
            }
            IsEnable = isEnable;
        }

        public void PanTiltMove(double panSpeed, double tiltSpeed)
        {
            double len = Math.Min(1, Math.Sqrt(panSpeed * panSpeed + tiltSpeed * tiltSpeed));
            byte speed = (byte)(len * len * len * 255);
            if (speed < 1)
                control("StopPT");
            else
            {
                string action = getActionFromPT(panSpeed, tiltSpeed);
                control(action, speed);
            }
        }

        private string getActionFromPT(double panSpeed, double tiltSpeed)
        {
            int distribute = (int)Math.Round(Math.Atan2(tiltSpeed, panSpeed) * 4 / Math.PI);//计算角度分布（以45°为单位，四舍五入），范围-4~4,tilt值向下为正
            switch (distribute)
            {
                case -3:
                    return "LeftUp";
                case -2:
                    return "Up";
                case -1:
                    return "RightUp";
                case -0:
                    return "Right";
                case 1:
                    return "RightDown";
                case 2:
                    return "Down";
                case 3:
                    return "LeftDown";
                default:
                    return "Left";
            }
        }

        private ICommand newCommand(string action)
        {
            return new DelegateCommand(_ => control(action));
        }

        private void control(string action, int actData = 0)
        {
            CameraControlRemoteCall.Instance.CameraControl(VideoId, action, actData);
        }
    }
}
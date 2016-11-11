using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using AopUtil.WpfBinding;
using Common.Command;
using Common.Util;
using Telerik.Windows.Controls;
using VideoNS.Helper;
using VideoNS.TimeSwitch;
using System.Diagnostics;
using VideoNS.AutoSave;

namespace VideoNS.SplitScreen
{
    public class SettingControlModel : ObservableObject
    {
        private const string SwitchTip = "{0:F0}'";
        public SettingControlModel()
        {
            SwitchButtonText = "定时切换";
            SwitchLabelText = "倒计时";
            Title = "视酷 CCTV2.0";
            this.RemainingTime = string.Format(SwitchTip, 0);

            this.ChangeControlStateCmd = new Common.Command.DelegateCommand((o) => { ChangeControlState(); });

            this.LayoutEditCmd = new Common.Command.DelegateCommand((o) => { doLayoutEdit(); });
            this.TimingSwitchCmd = new Common.Command.DelegateCommand((o) => { doTimingSwitch(); });
            this.PauseCmd = new Common.Command.DelegateCommand((o) => { doSwitchPause(); });
            this.MouseEnterCmd = new Common.Command.DelegateCommand((o) => { MouseEnter(); });
            this.MouseLeaveCmd = new Common.Command.DelegateCommand((o) => { MouseLeave(); });
            //this.MinWinCmd = new DelegateCommand((o) => { ToMinWin(o); });
            //this.MaxWinCmd = new DelegateCommand((o) => { ToNormalOrMaxWin(); });
            //this.CloseWinCmd = new DelegateCommand((o) => { TestCloseWin(); });

            this.MinWinCmd = WindowCommands.Minimize;
            this.MaxWinCmd = WindowCommands.Maximize;
            this.NormalWinCmd = WindowCommands.Restore;
            this.CloseWinCmd = WindowCommands.Close;

            this.ControlState = Visibility.Collapsed;
            this.ButtonOpacity = 0.5d;
            this.PropertyChanged += thisPropertyChanged;
            SplitModel = LayoutScheme.Instance.Scheme;
        }

        private void thisPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ControlState):
                    ButtonOpacity = ControlState == Visibility.Visible ? 1d : 0.5d;
                    break;
                case nameof(WindowState):
                    if (WindowState == WindowState.Maximized)
                    {
                        //模拟一次鼠标进出事件。
                        MouseEnter();
                        MouseLeave();
                    }
                    break;
            }
        }

        private void controlModelEvent(SplitScreenModel model, bool isInstall)
        {
            if (model != null)
            {
                if (isInstall)
                {
                    model.PropertyChanged += SplitModel_PropertyChanged;
                }
                else
                {
                    model.PropertyChanged -= SplitModel_PropertyChanged;
                }
            }
        }

        private void SplitModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SplitModel.SwitchProgress):
                    {
                        RemainingTime = string.Format(SwitchTip, Math.Round((SplitModel.SwitchInterval - SplitModel.SwitchProgress * SplitModel.SwitchInterval) / 1000));
                        ProgressAngle = SplitModel.SwitchProgress * Math.PI * 2;
                        if (SplitModel.SwitchProgress <= 0.5)
                            IsLargeArc = false;
                        else
                            IsLargeArc = true;
                        break;
                    }
                case nameof(SplitModel.SwitchPaused):
                    {
                        SwitchLabelText = SplitModel.SwitchPaused ? "继续" : "倒计时";
                        break;
                    }
                case nameof(SplitModel.OnSwitching):
                    {
                        SwitchButtonText = SplitModel.OnSwitching ? "结束切换" : "定时切换";
                        ClockVisibility = SplitModel.OnSwitching ? Visibility.Visible : Visibility.Collapsed;
                        break;
                    }
            }
        }

        private void doLayoutEdit()
        {
            SplitModel.StopTimingSwitch();
            if (LayoutEditAction != null)
                LayoutEditAction();
        }

        private void doTimingSwitch()
        {
            if (SplitModel.OnSwitching)
                SplitModel.StopTimingSwitch();
            else
                SplitModel.StartTimingSwitch();

        }

        private void doSwitchPause()
        {
            if (SplitModel.OnSwitching)
                SplitModel.SwitchPaused = !SplitModel.SwitchPaused;
        }

        #region 【UI显示隐藏】
        private bool _mouseLeaved = false;
        private bool _monitorFlag = false;
        private Task _monitor;
        private object _monitorLock = new object();

        private void MouseEnter()
        {
            lock (_monitorLock)
            {
                _monitorFlag = false;
                _mouseLeaved = false;
            }
            this.ControlState = Visibility.Visible;
            if (_monitor == null || _monitor.Status.HasFlag(TaskStatus.RanToCompletion))
            {
                _monitor = Task.Factory.StartNew(MoniterMouseLeave);
            }
        }

        private void MouseLeave()
        {
            SetMouseLeaved(true);
        }

        private void MoniterMouseLeave()
        {
            do
            {
                Thread.Sleep(1000);
                bool flag = GetIsMouseLeaved();
                if (flag)
                {
                    SetMonitorFlag(true);
                    Thread.Sleep(1000);
                    flag = GetIsMouseLeaved() && GetMonitorFlag();
                    if (flag)
                    {
                        try
                        {
                            WindowUtil.BeginInvoke(() =>
                            {
                                if (this.ControlState != Visibility.Hidden && WindowState == WindowState.Maximized)
                                    this.ControlState = Visibility.Collapsed;
                            });
                        }
                        catch (NullReferenceException)
                        {
                            //在应用程序(域)退出时，有可能引发此异常。
                        }
                        break;
                    }
                }
            }
            while (true);
        }

        private bool GetIsMouseLeaved()
        {
            lock (_monitorLock) { return _mouseLeaved; }
        }
        private void SetMouseLeaved(bool flag)
        {
            lock (_monitorLock) { _mouseLeaved = flag; }
        }
        private bool GetMonitorFlag()
        {
            lock (_monitorLock) { return _monitorFlag; }
        }
        private void SetMonitorFlag(bool flag)
        {
            lock (_monitorLock) { _monitorFlag = flag; }
        }
        #endregion 【UI显示隐藏】

        private SplitScreenModel _splitModel;
        public SplitScreenModel SplitModel
        {
            get { return _splitModel; }
            set
            {
                controlModelEvent(_splitModel, false);
                updateProperty(ref _splitModel, value);
                controlModelEvent(_splitModel, true);
            }
        }
        /// <summary>
        /// 面板状态：
        /// Visible：全部显示。
        /// Hidden：不存在此状态。
        /// Collapsed：折叠隐藏，只显示下拉按钮。
        /// </summary>
        [AutoNotify]
        public Visibility ControlState { get; set; }
        [AutoNotify]
        public WindowState WindowState { get; set; }
        [AutoNotify]
        public string Title { get; set; }
        [AutoNotify]
        public double ButtonOpacity { get; set; }
        [AutoNotify]
        public ICommand ChangeControlStateCmd { get; private set; }
        [AutoNotify]
        public ICommand LayoutEditCmd { get; set; }
        public Action LayoutEditAction;
        [AutoNotify]
        public ICommand TimingSwitchCmd { get; private set; }

        [AutoNotify]
        public string SwitchButtonText { get; private set; }
        [AutoNotify]
        public string SwitchLabelText { get; private set; }
        [AutoNotify]
        public string RemainingTime { get; private set; }
        [AutoNotify]
        public bool IsLargeArc { get; private set; } = false;
        [AutoNotify]
        public double ProgressAngle { get; set; } = 0;
        [AutoNotify]
        public Visibility ClockVisibility { get; set; } = Visibility.Collapsed;
        [AutoNotify]
        public bool EmerencyChecked { get; set; } = false;

        [AutoNotify]
        public ICommand MinWinCmd { get; set; }
        [AutoNotify]
        public ICommand MaxWinCmd { get; set; }

        [AutoNotify]
        public ICommand NormalWinCmd { get; set; }

        [AutoNotify]
        public ICommand CloseWinCmd { get; set; }

        [AutoNotify]
        public ICommand MouseEnterCmd { get; set; }

        [AutoNotify]
        public ICommand MouseLeaveCmd { get; set; }

        [AutoNotify]
        public ICommand PauseCmd { get; set; }

        private void ChangeControlState()
        {
            if (this.ControlState == Visibility.Hidden)
                return;
            if (this.ControlState == Visibility.Visible)
                this.ControlState = Visibility.Collapsed;
            else
                this.ControlState = Visibility.Visible;
        }
    }

    public class StateToAngleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return 0d;
            Visibility vis = (Visibility)value;
            switch (vis)
            {
                case Visibility.Visible:
                    return -90;
                case Visibility.Hidden:
                case Visibility.Collapsed:
                default:
                    return 90;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ProgressToPoint : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string inStr = parameter.ToString();
            string[] strs = inStr.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            double radius = System.Convert.ToDouble(strs[0]);
            double centerX = System.Convert.ToDouble(strs[1]);
            double centerY = System.Convert.ToDouble(strs[2]);
            double angle = (double)value;
            double x = centerX + Math.Sin(angle) * radius;
            double y = centerY - Math.Cos(angle) * radius;
            return new Point(x, y);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ProgressToTrans : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string inStr = parameter.ToString();
            string[] strs = inStr.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            double radius = System.Convert.ToDouble(strs[0]);
            string ct = strs[1].ToUpper();
            double angle = (double)value;
            double trans = 0;
            if (ct.Equals("X"))
            {
                trans = Math.Sin(angle) * radius;
            }
            else
            {
                trans = -Math.Cos(angle) * radius;
            }
            return trans;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

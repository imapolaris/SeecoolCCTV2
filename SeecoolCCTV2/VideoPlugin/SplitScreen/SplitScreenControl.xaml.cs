using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Common.Command;
using VideoNS.Json;
using VideoNS.Layout;
using VideoNS.Model;
using System.Globalization;
using Telerik.Windows.Controls;
using VideoNS.Helper;
using VideoNS.AutoSave;
using Common.Message;
using System.Dynamic;
using VideoNS.SubWindow;
using System.IO;

namespace VideoNS.SplitScreen
{
    /// <summary>
    /// SplitScreenControl.xaml 的交互逻辑
    /// </summary>
    public partial class SplitScreenControl : UserControl
    {
        #region 【覆屏背景_测试】
        private static SplitScreenControl _lastInstance;
        internal static void SetCoverVisible(bool visible)
        {
            if (_lastInstance != null)
            {
                _lastInstance.coverBorder.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        #endregion 【覆屏背景_测试】
        public SplitControlModel ViewModel { get { return DataContext as SplitControlModel; } }

        public SplitScreenControl()
        {
            InitializeComponent();
            ViewModel.SettingModel.LayoutEditAction += ToLayoutEditting;
            _lastInstance = this;
            this.Loaded += onLoaded;
            LayoutScheme.Instance.LayoutDataChanged += onLayoutDataChanged;
            ShortcutScheme.Instance.DataChanged += Shortcuts_DataChanged;
            Shortcuts_DataChanged(null, null); //模拟读取一次快捷键。
        }

        private void onLayoutDataChanged()
        {
            splitPanel.UpdateSelectedVideos();
        }

        private void Shortcuts_DataChanged(object sender, EventArgs e)
        {
            ICommand cmd = new Common.Command.DelegateCommand(x => doShortcut(x));
            List<InputBinding> bindings = new List<InputBinding>();
            foreach (ShortcutItem si in ShortcutScheme.Instance.Scheme)
            {
                try
                {
                    KeyGesture kg = new KeyGesture(si.Key, si.Modifiers, si.ShortcutString);
                    KeyBinding kb = new KeyBinding(cmd, kg)
                    {
                        CommandParameter = si
                    };
                    bindings.Add(kb);
                }
                catch (NotSupportedException se)
                {
                    Common.Log.Logger.Default.Error("---在加载快捷键时出错---", se);
                }
            }
            Application.Current.MainWindow.InputBindings.Clear();
            Application.Current.MainWindow.InputBindings.AddRange(bindings);
        }

        private void doShortcut(object obj)
        {
            if (splitPanel.ViewModel != null && !(splitPanel.ViewModel.OnSwitching || splitPanel.ViewModel.IsOnEditting))
            {
                ShortcutItem si = obj as ShortcutItem;
                if (si != null && si.FileName != null)
                {
                    if (!File.Exists(si.FileName))
                    {
                        DialogWin.Show("未找到布局文件:" + si.FileName, DialogWinImage.Warning);
                        return;
                    }
                    SplitScreenInfo data = JsonParser.DeserializeFromFile<SplitScreenInfo>(si.FileName);
                    if (data != null)
                        LayoutScheme.Instance.Scheme.SplitScreenData = data;
                }
            }
        }

        private void onLoaded(object sender, RoutedEventArgs e)
        {
            #region 【测试SplitScreenPanel的RemoteCall调用】
            //Window testWin = new Window();
            //testWin.Width = 800;
            //testWin.Height = 600;
            //UserControl ctrl = (UserControl)RemoteCalls.Global.Call("CCTV2_VideoPlugin_CreateSplitPanel");
            //dynamic obj = new ExpandoObject();
            //obj.VideoId = "CCTV1_50BAD15900010301";
            //obj.Row = 0;
            //obj.Column = 0;
            //obj.RowSpan = 2;
            //obj.ColumnSpan = 2;
            //dynamic obj1 = new ExpandoObject();
            //obj1.VideoId = "CCTV1_50BAD15900020301";
            //obj1.Row = 0;
            //obj1.Column = 2;
            //obj1.RowSpan = 1;
            //obj1.ColumnSpan = 1;
            //dynamic obj2 = new ExpandoObject();
            //obj2.VideoId = "CCTV1_50BAD15900020302";
            //obj2.Row = 1;
            //obj2.Column = 2;
            //obj2.RowSpan = 1;
            //obj2.ColumnSpan = 1;
            //dynamic obj3 = new ExpandoObject();
            //obj3.VideoId = null;
            //obj3.Row = 2;
            //obj3.Column = 2;
            //obj3.RowSpan = 1;
            //obj3.ColumnSpan = 1;

            //RemoteCalls.Global.Call("CCTV2_VideoPlugin_PlayMultiVideos", ctrl, 3, new List<dynamic>() { obj, obj1, obj2, obj3 });
            //testWin.Content = ctrl;
            //ctrl.Background = Brushes.Black;
            //testWin.Show();
            //testWin.Closed += (s, ie) =>
            //{
            //    RemoteCalls.Global.Call("CCTV2_VideoPlugin_StopMultiVideos", ctrl);
            //};
            #endregion 【测试SplitScreenPanel的RemoteCall调用】

            RadWindow win = this.FindVisualParent<RadWindow>();
            if (win != null)
            {
                WindowStatusInfo info = WindowStatusAutoSave.LoadData();
                if (info != null)
                {
                    info.Left = CheckLimited(info.Left, 0, SystemParameters.PrimaryScreenWidth - 100);
                    info.Top = CheckLimited(info.Top, 0, SystemParameters.PrimaryScreenHeight - 100);
                    info.Width = CheckLimited(info.Width, AppConstants.WindowMinWidth, SystemParameters.PrimaryScreenWidth);
                    info.Height = CheckLimited(info.Height, AppConstants.WindowMinHeight, SystemParameters.PrimaryScreenHeight);

                    win.BeginInit();
                    if (info.WindowState == WindowState.Normal)
                    {
                        win.Left = info.Left;
                        win.Top = info.Top;
                        win.Width = info.Width;
                        win.Height = info.Height;
                    }
                    win.WindowState = info.WindowState;
                    win.EndInit();
                }
                win.Closed += Win_Closed;
                ViewModel.SettingModel.WindowState = win.WindowState;
                win.SetBinding(RadWindow.WindowStateProperty, BindingHelper.CreateBinding(this, BindingMode.TwoWay, "DataContext.SettingModel.WindowState"));
            }
        }

        private void Win_Closed(object sender, WindowClosedEventArgs e)
        {
            RadWindow win = this.FindVisualParent<RadWindow>();
            if (win != null)
            {
                WindowStatusInfo info = new WindowStatusInfo()
                {
                    Width = win.ActualWidth,
                    Height = win.ActualHeight,
                    Left = win.Left,
                    Top = win.Top,
                    WindowState = win.WindowState
                };
                WindowStatusAutoSave.SaveData(info);
            }
        }

        private double CheckLimited(double value, double min, double max)
        {
            if (value > max)
                value = max;
            if (value < min)
                value = min;
            return value;
        }

        LayoutPanel _layout = null;
        private void ToLayoutEditting()
        {
            _layout = new LayoutPanel();
            _layout.ViewModel.ReturnAction += BackToRealTime;

            gridCenter.Children.Clear();
            splitPanel.ViewModel.IsOnEditting = true;
            _layout.SetSplitScreen(splitPanel);
            gridCenter.Children.Add(_layout);
        }

        private void BackToRealTime()
        {
            if (_layout != null)
            {
                _layout.SetSplitScreen(null);
                _layout.ViewModel.SearcherModel.ClearSearcher();
                _layout = null;
            }

            gridCenter.Children.Clear();
            splitPanel.ViewModel.IsOnEditting = false;
            gridCenter.Children.Add(splitPanel);
        }
    }
}

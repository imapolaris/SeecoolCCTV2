using Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Telerik.Windows.Controls;
using UIManipulator;
using UIManipulator.Localization;
using Common.Log;
using System.IO;

namespace SeecoolCCTV2
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
#if !DEBUG
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
#endif
            Logger.Default.Trace("程序启动。");

            base.OnStartup(e);

            LocalizationManager.Manager = new TelerikLocalizationManager();

            //判断是否设置过数据服务IP地址。
            //if (!File.Exists("cctvexe.dat"))
            //{
            //    AppDomain domain = AppDomain.CreateDomain("SettingDomain");
            //    domain.ExecuteAssembly("ConfSetting.exe");
            //    AppDomain.Unload(domain);
            //}

            UIService.PrepareApp();

            var window = new MainWindow();
            string appName = getAppName();
            if (appName != null)
                window.Header = appName;
            window.Loaded += (_s, _e) => UIService.InitWindow(window);
            window.PreviewClosed += (_s, _e) => _e.Cancel = UIService.ExistsOpposerForExit();
            window.Closed += (_s, _e) => UIService.OnClosed();
            window.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Logger.Default.Trace("程序退出。");
            base.OnExit(e);
        }

        private string getAppName()
        {
            try
            {
                return ConfigurationManager.AppSettings["AppName"];
            }
            catch
            {
                return null;
            }
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.Default.Error(e.Exception);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Default.Error(e.ExceptionObject.ToString());
        }
    }
}

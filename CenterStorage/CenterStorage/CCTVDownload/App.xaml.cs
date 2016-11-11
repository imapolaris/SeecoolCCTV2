using Common.Log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows;

namespace CCTVDownload
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                base.OnStartup(e);
#if !DEBUG
                var service = new ServiceController("Seecool.StorageDownloadService");
                if (service.Status != ServiceControllerStatus.Running)
                {
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 1, 0));
                    Common.Log.Logger.Default.Trace("启动服务进程: {0}", service.ServiceName);
                }
#endif
                this.DispatcherUnhandledException += App_DispatcherUnhandledException;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            }
            catch (Exception ex)
            {
                Logger.Default.Error(ex);
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Default.Error(e.ExceptionObject.ToString());
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.Default.Error(e.Exception);
        }
    }
}

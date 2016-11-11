using CenterStorageCmd.Url;
using Common.Log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CCTVReplay
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
                this.DispatcherUnhandledException += App_DispatcherUnhandledException;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                initSettings(e.Args);
            }
            catch (Exception ex)
            {
                Logger.Default.Error(ex.ToString());
                Util.DialogUtil.ShowError(ex.Message);
            }
        }

        private void initSettings(string[] args)
        {
            if (args.Length > 0)
            {
                string url = args[0];
                try
                {
                    IUrl ui = null;
                    try
                    {
                        ui = LocalUrl.Parse(url);
                    }
                    catch { }
                    if (ui == null)
                    {
                        try
                        {
                            ui = RemoteUrl.Parse(url);
                        }
                        catch { }
                    }
                    if (ui != null)
                        return;
                    if(!tryToRemote(url))
                        throw new ErrorMessageException("URL未能正确解析！");
                }
                catch (ErrorMessageException ae)
                {
                    Common.Log.Logger.Default.Error(ae);
                    Util.DialogUtil.ShowError(ae.Message);
                }
            }
        }

        private bool tryToRemote(string url)
        {
            int index = url.IndexOf(',');
            if (index > 0)
            {
                if (url.Substring(0, index).ToLower().Equals("remote"))
                {
                    Util.ConstSettings.Remote = url.Substring(index + 1);
                    return true;
                }
            }
            return false;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null &&  ex is ErrorMessageException)
            {
                Util.DialogUtil.ShowError(ex.Message);
            }
            else
            {
                Logger.Default.Error(e.ExceptionObject.ToString());
            }
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception is ErrorMessageException || 
                e.Exception is System.Net.Sockets.SocketException ||
                e.Exception is System.FormatException ||
                e.Exception is System.NotSupportedException)
            {
                Logger.Default.Error(e.Exception.Message);
            }
            else
            {
                Logger.Default.Error(e.Exception);
            }
                Util.DialogUtil.ShowError(e.Exception.Message);
                e.Handled = true;//使用这一行代码告诉运行时，该异常被处理了，不再作为UnhandledException抛出了。
        }
    }
}

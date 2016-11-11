using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace TestCCTVStreamCmd
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            try
            {
                this.DispatcherUnhandledException += App_DispatcherUnhandledException;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                MessageBox.Show(ex.Message);
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.ExceptionObject.ToString());
            MessageBox.Show((e.ExceptionObject as Exception)?.Message);
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.Exception.ToString());
            MessageBox.Show(e.Exception.Message);
        }
    }
}

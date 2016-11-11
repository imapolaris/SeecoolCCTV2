using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;

namespace CCTVInfoAdapterOld
{
    class Program
    {
        static Common.Logging.ILog _log { get { return Common.Logging.LogManager.GetLogger<Program>(); } }

        static void Main(string[] args)
        {
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;

#if !DEBUG
            AppDomain.CurrentDomain.UnhandledException += currentDomain_UnhandledException;
            try
            {
#endif

                string cmdline = Environment.CommandLine;
                _log.InfoFormat("{0} 启动。", cmdline);
                ManualResetEvent exitEvent = new ManualResetEvent(false);

                string cctv1Host = ConfigurationManager.AppSettings["CCTV1Host"];
                string infoServiceBaseUri = ConfigurationManager.AppSettings["InfoServiceBaseUri"];
                using (CCTV1Adapter adapter = new CCTV1Adapter(cctv1Host, infoServiceBaseUri))
                {
                    ManualResetEvent closeEvent = new ManualResetEvent(false);
                    ConsoleMonitor.Closed += () => { closeEvent.Set(); exitEvent.WaitOne(); };
                    closeEvent.WaitOne();
                }

                _log.InfoFormat("{0} 退出。", cmdline);
                exitEvent.Set();

#if !DEBUG
            }
            catch (Exception ex)
            {
                handleException(ex);
            }
#endif
        }

        static void currentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            handleException(e.ExceptionObject as Exception);
        }

        static void handleException(Exception ex)
        {
            if (ex != null)
                _log.Fatal(ex.ToString());
            else
                _log.Fatal("空异常");

            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
    }
}

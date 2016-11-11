using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace InfoService
{
    class Program
    {
        static Common.Logging.ILog _log { get { return Common.Logging.LogManager.GetLogger<Program>(); } }

        const string _logPath = "LOGPATH";

        static void Main(string[] args)
        {
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;

#if !DEBUG
            AppDomain.CurrentDomain.UnhandledException += currentDomain_UnhandledException;
            try
            {
#endif

            string[] appContext = parseArgs(args).ToArray();
            if (Environment.GetEnvironmentVariable(_logPath) == null)
                Environment.SetEnvironmentVariable(_logPath, "log");

            string cmdline = Environment.CommandLine;

            _log.InfoFormat("{0} 启动。", cmdline);

            ManualResetEvent exitEvent = new ManualResetEvent(false);

            using (WebApiHost webApi = new WebApiHost())
            {
                ManualResetEvent closeEvent = new ManualResetEvent(false);
                ConsoleMonitor.Closed += () => { closeEvent.Set(); exitEvent.WaitOne(); };
                closeEvent.WaitOne();
            }

            _log.InfoFormat("{0} 退出。", cmdline);
            exitEvent.Set();

#if ! DEBUG
            }
            catch (Exception ex)
            {
                handleException(ex);
            }
#endif
        }

        private static IEnumerable<string> parseArgs(string[] args)
        {
            foreach (string arg in args)
            {
                int index = arg.IndexOf(':');
                if (index < 0)
                    yield return arg;
                else
                    useArg(arg.Substring(0, index), arg.Substring(index + 1));
            }
        }

        private static void useArg(string key, string value)
        {
            key = key.ToUpper();
            if (key.Length > 0 && (key[0] == '-' || key[0] == '/'))
                key = key.Substring(1);

            switch (key)
            {
                case _logPath:
                    Environment.SetEnvironmentVariable(_logPath, value);
                    break;
            }
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

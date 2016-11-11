using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace CCTVReplay.Proxy
{
    public class DownloadServerProxy
    {
        public static bool IsDownloadServerAlive()
        {
#if DEBUG
            var service = new ServiceController("Seecool.CCTV.Download.Service");
            return service.Status == ServiceControllerStatus.Running;
#else
            return true;
#endif
        }

        public static void StartDownloadServer()
        {
            var service = new ServiceController("Seecool.CCTV.Download.Service");
            service.Start();
            service.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 1, 0));
            Common.Log.Logger.Default.Trace("启动服务进程: {0}", service.ServiceName);
        }
    }
}

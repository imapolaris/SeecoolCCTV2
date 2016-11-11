using CCTVDownloadService;
using Common.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace CCTVDownloadServiceHost
{
    public partial class CCTVDownloadService : ServiceBase
    {
        DownloadSocketsManager _sockets;
        OnlineDownloadsManager _downloads;
        public CCTVDownloadService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Logger.Default.Trace("-----------------启动CCTV2.0下载服务-----------------");
            try
            {
                _sockets = DownloadSocketsManager.Instance;
                _downloads = OnlineDownloadsManager.Instance;
            }
            catch (Exception ex)
            {
                Logger.Default.Error(ex);
                throw ex;
            }
        }

        protected override void OnStop()
        {
            Logger.Default.Trace("-----------------结束下载服务-----------------");
        }
    }
}

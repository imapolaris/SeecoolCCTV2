using AopUtil.WpfBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CCTVDownload.Download;
using CenterStorageCmd;
using CenterStorageCmd.Url;
using CCTVDownload.Util;

namespace CCTVDownload
{
    public class DownloadsCommandViewModel: ObservableObject
    {
        public ICommand NewDownloadsCommand { get; set; }
        public ICommand StartCommand { get; set; }
        public ICommand PauseCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        [AutoNotify]
        public bool MultiSelected { get; set; }
        public Action StartEvent;
        public Action PauseEvent;
        public Action DeleteEvent;
        public Action<DownloadInfoParam[]> NewDownloadsEvent;

        public DownloadsCommandViewModel()
        {
            StartCommand = new CommandDelegate(_ => onAction(StartEvent));
            PauseCommand = new CommandDelegate(_ => onAction(PauseEvent));
            DeleteCommand = new CommandDelegate(_ => onAction(DeleteEvent));
            NewDownloadsCommand = new CommandDelegate(_ => onNewDownloads());
        }

        private void onAction(Action action)
        {
            if (action != null)
                action();
        }

        private void onNewDownloads()
        {
            NewTaskWin ntWin = new NewTaskWin();
            ntWin.Owner = System.Windows.Application.Current.MainWindow;
            if ((bool)ntWin.ShowDialog())
            {
                IRemoteUrl ui = ntWin.ViewModel.GetParsedUrlInfo();
                string downPath = System.IO.Path.Combine(ntWin.ViewModel.DownloadDirectory, ntWin.ViewModel.DownloadName);

                var downArray = ui.VideoInfos.Select(vi => new DownloadInfoParam(ui.SourceIp, ui.SourcePort, ui.BeginTime, ui.EndTime, vi.VideoId, vi.StreamId, downPath, vi.VideoName)).ToArray();
                var handle = NewDownloadsEvent;
                if (handle != null && downArray != null && downArray.Length > 0)
                    handle(downArray);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CCTVDownload.Util
{
    public static class DownloadCommands
    {
        static DownloadCommands()
        {
            NewTask = new RoutedUICommand("New Task", "新建", typeof(DownloadCommands));
            Start = new RoutedUICommand("Start Download", "开始", typeof(DownloadCommands));
            Pause = new RoutedUICommand("Pause Download", "暂停", typeof(DownloadCommands));
            Delete= new RoutedUICommand("Delete Seleted", "删除", typeof(DownloadCommands));
            SelectAll = new RoutedUICommand("Selete All Items", "全选", typeof(DownloadCommands));
        }
        public static ICommand NewTask { get; private set; }
        public static ICommand Start { get; private set; }
        public static ICommand Pause { get; private set; }
        public static ICommand Delete { get; private set; }
        public static ICommand SelectAll { get; private set; }
    }
}

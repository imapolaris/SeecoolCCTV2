using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageCmd
{
    /// <summary>
    /// 可能的状态转换为:
    /// Ready--Downloading
    /// Ready--Error
    /// Waiting--Downloading
    /// Waiting--Deleted
    /// Waiting--Paused
    /// Downloading--Error
    /// Downloading--Paused
    /// Downloading--Deleted
    /// Downloading--Completed
    /// Paused--Waiting
    /// Paused--Deleted
    /// </summary>
    public enum DownloadStatus
    {
        Ready = 0,
        Waiting = 1,
        Downloading = 2,
        Paused = 3,
        Completed = 4,
        Deleted = 5,
        Error = 99,
    }

    public static class DownloadStatusManager
    {
        public static string ToHanZi(DownloadStatus status)
        {
            switch (status)
            {
                case DownloadStatus.Ready:
                    return "准备";
                case DownloadStatus.Waiting:
                    return "等待";
                case DownloadStatus.Downloading:
                    return "正在下载";
                case DownloadStatus.Paused:
                    return "暂停";
                case DownloadStatus.Completed:
                    return "已完成";
                case DownloadStatus.Deleted:
                    return "已删除";
                case DownloadStatus.Error:
                    return "下载错误";
            }
            return status.ToString();
        }

        public static bool IsWaitingOrPause(DownloadStatus status)
        {
            return status == DownloadStatus.Waiting || status == DownloadStatus.Paused;
        }

        public static bool IsDownloading(DownloadStatus status)
        {
            return status == DownloadStatus.Downloading || status == DownloadStatus.Ready;
        }

        public static bool IsDownloadingOrWaiting(DownloadStatus status)
        {
            return IsDownloading(status) || status == DownloadStatus.Waiting;
        }

        public static bool IsEndOfDownload(DownloadStatus status)
        {
            return status == DownloadStatus.Completed || status == DownloadStatus.Error || status == DownloadStatus.Deleted;
        }

        public static bool IsPauseOrEnd(DownloadStatus status)
        {
            return IsEndOfDownload(status) || status == DownloadStatus.Paused;
        }
    }
}
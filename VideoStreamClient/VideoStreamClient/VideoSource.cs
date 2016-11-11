using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoStreamClient.Entity;
using VideoStreamClient.Events;

namespace VideoStreamClient
{
    public interface VideoSource : IDisposable
    {
        string VideoId { get; }
        string Url { get; }
        HikM4Header HikM4Header { get; }
        FfmpegHeader FfmpegHeader { get; }
        bool IsDisposed { get; }

        /// <summary>
        /// 设置接收FFMPEG数据包通知的方法。
        /// <para>改操作不会影响码流接收的开关状态。</para>
        /// </summary>
        /// <param name="notify"></param>
        void SetNotifyDataAction(Action<FfmpegPackage> notify);

        /// <summary>
        /// 设置接收Hikm4数据包通知的方法。
        /// <para>改操作不会影响码流接收的开关状态。</para>
        /// </summary>
        /// <param name="notify"></param>
        void SetNotifyDataAction(Action<HikM4Package> notify);

        /// <summary>
        /// 设置接收Uniview数据包通知的方法。
        /// <para>改操作不会影响码流接收的开关状态。</para>
        /// </summary>
        /// <param name="notify"></param>
        void SetNotifyDataAction(Action<UniviewPackage> notify);

        #region 【事件定义】
        event EventHandler<VideoFrameEventArgs> VideoFrameReceived;
        event EventHandler<HikM4HeaderEventArgs> Hikm4HeaderReceived;
        event EventHandler<HikM4StreamEventArgs> Hikm4StreamReceived;
        event EventHandler<FfmpegHeaderEventArgs> FfmpegHeaderReceived;
        event EventHandler<FfmpegStreamEventArgs> FfmpegStreamReceived;
        event EventHandler<StreamTypeEventArgs> StreamTypeChanged;
        event EventHandler<UniviewStreamEventArgs> UniviewStreamReceived;
        #endregion 【事件定义】
    }
}

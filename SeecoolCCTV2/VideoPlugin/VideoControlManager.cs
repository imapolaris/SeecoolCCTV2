using Common.Message;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using VideoNS.VideoDisp;
using System.Windows;
using VideoNS.SplitScreen;
using VideoNS.Model;

namespace VideoNS
{
    internal class VideoControlManager
    {
        public static readonly VideoControlManager Instance = new VideoControlManager();

        public void Init()
        {
        }

        private VideoControlManager()
        {
            RemoteCalls.Global.RegisterFunc<VideoControl>("CCTV2_VideoPlugin_CreateVideoControl", createVideoControl);
            RemoteCalls.Global.Register<VideoControl, string>("CCTV2_VideoPlugin_PlayVideo", playVideo);
            RemoteCalls.Global.Register<VideoControl>("CCTV2_VideoPlugin_StopVideo", stopVideo);
            RemoteCalls.Global.Register<VideoControl>("CCTV2_VideoPlugin_ClearVideoImage", clearVideoImage);
            RemoteCalls.Global.Register<VideoControl, Stretch>("CCTV2_VideoPlugin_SetVideoStretch", setVideoStretch);
            RemoteCalls.Global.RegisterFunc<VideoControl, System.Drawing.Image>("CCTV2_VideoPlugin_GetSnapshot", getSnapshot);

            RemoteCalls.Global.RegisterFunc<SplitScreenPanel>("CCTV2_VideoPlugin_CreateSplitPanel", createSplitPanel);
            RemoteCalls.Global.Register<SplitScreenPanel, int, IEnumerable<dynamic>>("CCTV2_VideoPlugin_PlayMultiVideos", playMultiVideos);
            RemoteCalls.Global.Register<SplitScreenPanel>("CCTV2_VideoPlugin_StopMultiVideos", stopMultiVideos);
        }

        private void stopMultiVideos(SplitScreenPanel panel)
        {
            panel.StopAll();
        }

        private SplitScreenPanel createSplitPanel()
        {
            SplitScreenPanel ssp = new SplitScreenPanel();
            SplitScreenModel model = new SplitScreenModel(false)
            {
                CanItemClose = false
            };
            ssp.DataContext = model;
            return ssp;
        }

        private void playMultiVideos(SplitScreenPanel panel, int split, IEnumerable<dynamic> videos)
        {
            List<SplitScreenNode> nodes = new List<SplitScreenNode>();
            foreach (var one in videos)
            {
                SplitScreenNode node = new SplitScreenNode()
                {
                    VideoId = one.VideoId,
                    Row = one.Row,
                    Column = one.Column,
                    RowSpan = one.RowSpan,
                    ColumnSpan = one.ColumnSpan
                };
                nodes.Add(node);
            }

            if (nodes.Count > 0)
            {
                SplitScreenInfo ssi = new SplitScreenInfo()
                {
                    Split = split,
                    Nodes = nodes.ToArray()
                };
                panel.ViewModel.SplitScreenData = ssi;
            }
        }

        private void clearVideoImage(VideoControl control)
        {
            control.videoDisp.ViewModel.ClearVideoImage();
        }

        private System.Drawing.Image getSnapshot(VideoControl control)
        {
            return control.videoDisp.ViewModel.GetSnapshot();
        }

        private void setVideoStretch(VideoControl control, Stretch stretch)
        {
            control.videoDisp.ViewModel.StretchMode = stretch;
        }

        private void stopVideo(VideoControl control)
        {
            control.ViewModel.VideoId = null;
        }

        private void playVideo(VideoControl control, string videoId)
        {
            control.ViewModel.VideoId = videoId;
        }

        private VideoControl createVideoControl()
        {
            VideoControl ctrl = new VideoControl();
            ctrl.DataContext = new VideoControlModel(true) { CloseBtnVisibility = Visibility.Collapsed, FullScreenBtnVisibility = Visibility.Collapsed };
            return ctrl;
        }
    }
}

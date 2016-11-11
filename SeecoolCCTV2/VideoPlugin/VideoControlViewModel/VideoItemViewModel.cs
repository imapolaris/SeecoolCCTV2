using AopUtil.WpfBinding;
using Common.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Globalization;
using System.Windows;
using VideoNS.VideoInfo.Search;
using VideoNS.Model;

namespace VideoNS
{
    public class VideoItemViewModel: ObservableObject,IDisposable
    {
        public VideoItemViewModel()
        {
            PlusSignModel = new PlusSignViewModel();
            SearchedResultModel = new SearchedResultViewModel() { AllowDrag = false };
            ControlViewModel = new VideoControlModel();
            ControlViewModel.PropertyChanged += ControlViewModel_PropertyChanged;
            PlusSignModel.PlusCommand = new DelegateCommand(_ => gotoSearch());
            PropertyChanged += onPropertyChanged;
            IsInEditStatus = true;
        }

        private void onPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IsInEditStatus):
                    ControlViewModel.IsOnEditting = IsInEditStatus;
                    break;
            }
        }

        private void ControlViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ControlViewModel.VideoId):
                    updateShowStatusFromVideoId();
                    if (SplitScreenNode != null)
                        SplitScreenNode.VideoId = ControlViewModel.VideoId;
                    break;
            }
        }
        List<string> _videoIds = null;
        public void UpdateDisplayVideos(List<string> videoIds)
        {
            _videoIds = videoIds;
            updateDisplayVideos();
        }

        void updateDisplayVideos()
        {
            if (SearchedResultModel != null)
                SearchedResultModel.UpdateDisplayVideos(_videoIds);
        }


        void onSearchPlayVideo(string videoId)
        {
            if (!string.IsNullOrWhiteSpace(videoId))
            {
                disabledSearcherSource();
                ControlViewModel.IsVisible = true;
                ControlViewModel.VideoId = videoId;
            }
        }

        private bool isValidVideoNode(CctvNode node)
        {
            return node != null && node.Type == CctvNode.NodeType.Video && !string.IsNullOrEmpty(node.ID);
        }

        private void gotoSearch()
        {
            PlusSignModel.IsVisible = false;
            enableSearcherSource();
        }
        [AutoNotify]
        public bool CanClose { get; set; } = true;
        [AutoNotify]
        public bool IsInEditStatus { get; set; }

        [AutoNotify]
        public SplitScreenNode SplitScreenNode{ get; set; }

        [AutoNotify]
        public VideoControlModel ControlViewModel { get; set; }

        [AutoNotify]
        public SearchedResultViewModel SearchedResultModel { get; set; }

        public PlusSignViewModel PlusSignModel { get; set; }

        void updateShowStatusFromVideoId()
        {
            disabledSearcherSource();
            if (string.IsNullOrEmpty(ControlViewModel.VideoId))
            {
                SearchedResultModel.IsVisible = false;
                ControlViewModel.IsVisible = false;
                PlusSignModel.IsVisible = true;
            }
            else
            {
                PlusSignModel.IsVisible = false;
                ControlViewModel.IsVisible = true;
            }
        }

        void enableSearcherSource()
        {
            disabledSearcherSource();
            SearchedResultModel.IsVisible = true;
            SearchedResultModel.PlayVideoEvent += onSearchPlayVideo;
        }

        void disabledSearcherSource()
        {
            if (SearchedResultModel.IsVisible)
            {
                SearchedResultModel.IsVisible = false;
                SearchedResultModel.PlayVideoEvent -= onSearchPlayVideo;
            }
        }

        private bool _isDisposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool flag)
        {
            if (!_isDisposed)
            {
                SplitScreenNode = null;
                if (ControlViewModel != null)
                    ControlViewModel.Dispose();
            }
        }

        ~VideoItemViewModel()
        {
            Dispose(false);
        }
    }
}

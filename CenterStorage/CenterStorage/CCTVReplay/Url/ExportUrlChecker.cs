using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCTVReplay.Combo;
using CCTVReplay.Source;
using CenterStorageCmd;
using CenterStorageCmd.Url;

namespace CCTVReplay.Url
{
    public class ExportUrlChecker
    {
        private PlayControlViewModel _playVM;
        public ExportUrlChecker(PlayControlViewModel vm)
        {
            _playVM = vm;
        }

        public bool CanExport
        {
            get
            {
                _errorMsg = null;
                return CheckExportValid(ref _errorMsg);
            }
        }

        private string _errorMsg;
        public string ErrorMessage { get { return _errorMsg; } }

        private bool CheckExportValid(ref string message)
        {
            if (_playVM.Source == null)
            {
                message = "尚未设置数据源。";
                return false;
            }
            if (_playVM.Source.SrcType == SourceType.Local)
            {
                message = "暂不支持本地数据源导出。";
                return false;
            }
            if (_playVM.PlaySlider.BeginTime == null || _playVM.PlaySlider.EndTime == null)
            {
                message = "尚未设置查询时间段。";
                return false;
            }
            IEnumerable<IVideoInfo> playingVideos = _playVM.GetPlayingVideos();
            if (playingVideos.Count() == 0)
            {
                message = "未选择任何视频。";
                return false;
            }
            return true;
        }

        public IRemoteUrl GetExportUrl()
        {
            IEnumerable<IVideoInfo> viList= _playVM.GetPlayingVideos();
            VideoInfo[] vis = viList.Select(x => new VideoInfo(x.VideoId, x.StreamId, x.VideoName)).ToArray();
            return new RemoteUrl(_playVM.Source.Storage.Ip, _playVM.Source.Storage.Port, _playVM.PlaySlider.BeginTime, _playVM.PlaySlider.EndTime, vis, null);
        }
    }
}

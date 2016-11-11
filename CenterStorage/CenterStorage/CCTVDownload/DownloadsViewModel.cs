using AopUtil.WpfBinding;
using CenterStorageCmd;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CCTVDownload
{
    public class DownloadsViewModel : ObservableObject
    {
        ObservableCollection<DownloadViewModel> _downloads;
        public DownloadsCommandViewModel CommandModel { get; set; }
        public CollectionViewSource DownloadViewSource { get; set; }
        public CollectionViewSource DownloadSorts { get; private set; }
        [AutoNotify]
        public DownloadViewSort SelectedDownloadViewSort { get; set; }
        public Action<DownloadControlCode, byte[]> DownloadControlEvent;

        public DownloadsViewModel()
        {
            _downloads = new ObservableCollection<DownloadViewModel>();
            initDownloadType();
            initViewSource();
            initCommand();
            PropertyChanged += onPropertyChanged;
            RefreshSelected();
        }

        public void RefreshSelected()
        {
            var list = _downloads.ToList();
            bool multi = CommandModel.MultiSelected;
            if (!multi)
                list.ForEach(down => down.Selected = false);
            list.ForEach(down => down.SingleSelected = !multi);
        }

        private void onPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedDownloadViewSort))
                refreshViewSource();
        }

        public void Load(DownloadInfoExpandPacket[] packets)
        {
            _downloads.Clear();
            foreach (var packet in packets)
                Add(packet);
        }

        public void Add(DownloadInfoExpandPacket packet)
        {
            var down = new DownloadViewModel(packet);
            down.DownloadControlEvent += onDownloadControl;
            down.DeleteEvent += onDelete;
            down.RefreshEvent += refreshViewSource;
            down.SingleSelected = !CommandModel.MultiSelected;
            _downloads.Add(down);
        }

        private void onDelete(DownloadViewModel down)
        {
            down.DownloadControlEvent -= onDownloadControl;
            down.DeleteEvent -= onDelete;
            down.RefreshEvent -= refreshViewSource;
            _downloads.Remove(down);
        }

        public void Update(DownloadExpandPart part)
        {
            var down = _downloads.FirstOrDefault(_ => _.GuidCode == part.GuidCode);
            if (down != null)
            {
                if (part.Code == DownloadCode.GoTop)
                {
                    var index = _downloads.IndexOf(down);
                    if (index > 0)
                        _downloads.Move(index, 0);
                }
                else
                    down.Update(part.Code, part.Value);
            }
        }

        private void initDownloadType()
        {
            DownloadSorts = new CollectionViewSource();
            DownloadSorts.Source = new List<DownloadViewSort>()
            {
                DownloadViewSort.正在下载,
                DownloadViewSort.已完成,
            };
            SelectedDownloadViewSort = DownloadViewSort.正在下载;
        }

        #region 全局按键控制
        private void initCommand()
        {
            CommandModel = new DownloadsCommandViewModel();
            CommandModel.PropertyChanged += CommandModel_PropertyChanged;
            CommandModel.NewDownloadsEvent += onNewDownloads;
            CommandModel.StartEvent += startSelected;
            CommandModel.PauseEvent += pauseSelected;
            CommandModel.DeleteEvent += deleteSelected;
        }

        private void CommandModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(CommandModel.MultiSelected):
                    RefreshSelected();
                    break;
            }
        }

        private void onNewDownloads(DownloadInfoParam[] infos)
        {
            if (infos != null && infos.Length > 0)
                onDownloadControl(DownloadControlCode.Add, DownloadInfoParam.EncodeArray(infos));
        }

        private void startSelected()
        {
            for (int i = 0; i < _downloads.Count; i++)
            {
                var down = _downloads[i];
                if (down.Selected)
                    down.Start();
            }
        }

        private void pauseSelected()
        {
            for (int i = 0; i < _downloads.Count; i++)
            {
                var down = _downloads[i];
                if (down.Selected)
                    down.Pause();
            }
        }

        private void deleteSelected()
        {
            for (int i = _downloads.Count - 1; i >= 0; i--)
            {
                var down = _downloads[i];
                if (down.Selected)
                    down.Delete();
            }
        }

        #endregion 全局按键控制

        #region 下载分类（正在下载、已下载）

        private void initViewSource()
        {
            DownloadViewSource = new CollectionViewSource();
            DownloadViewSource.Source = _downloads;
            DownloadViewSource.View.MoveCurrentTo(null);
            DownloadViewSource.Filter += DownloadViewSource_Filter;
        }

        private void DownloadViewSource_Filter(object sender, FilterEventArgs e)
        {
            var model = e.Item as DownloadViewModel;
            e.Accepted = (model != null && model.IsLocalDownload && isViewSource(model.DownloadStatus));
        }

        private bool isViewSource(DownloadStatus status)
        {
            return status != DownloadStatus.Deleted
                && ((SelectedDownloadViewSort == DownloadViewSort.已完成 && status == DownloadStatus.Completed)
                || (SelectedDownloadViewSort == DownloadViewSort.正在下载 && status != DownloadStatus.Completed)
                || SelectedDownloadViewSort == DownloadViewSort.所有下载);
        }

        private void refreshViewSource()
        {
            DownloadViewSource.View.Refresh();
        }
        #endregion 下载分类（正在下载、已下载）

        private void onDownloadControl(DownloadControlCode code, Guid guid)
        {
            Console.WriteLine(code);
            onDownloadControl(code, guid.ToByteArray());
        }

        void onDownloadControl(DownloadControlCode code, byte[] buffer)
        {
            var handle = DownloadControlEvent;
            if (handle != null)
                handle(code, buffer);
        }
    }
}
using AopUtil.WpfBinding;
using CCTVReplay.Interface;
using CCTVReplay.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using CCTVReplay.StaticInfo;

namespace CCTVReplay.Source
{
    public class SourceManagerViewModel : ObservableObject
    {
        private ISourcePersistence _srcPersis;
        public SourceManagerViewModel()
        {
            _srcPersis = NinjectResolver.Instance.Get<ISourcePersistence>();
            SourceInfoModel = new SourceInfoViewModel(_srcPersis);
            SourceInfoModel.SourceCreated += SIM_SourceCreated;
            SourceInfoModel.SourceUpdated += SIM_SourceUpdated;


            CloseCmd = new CommandDelegate(_ => doCloseCmd());
            CreateNewCmd = new CommandDelegate(_ => doCreateNewCmd());
            DeleteCmd = new CommandDelegate(_ => doDeleteCmd());
            ConnectCmd = new CommandDelegate(_ => doConnectCmd());

            SourceItems = new CollectionViewSource();
            this.PropertyChanged += thisPropertyChanged;
            reloadSource();
        }

        public void SetCurrent(DataSource current)
        {
            if (current != null)
            {
                int index = 0;
                bool flag = true;
                foreach (DataSource ds in SourceItems.Source as IEnumerable<DataSource>)
                {
                    flag = ds.SourceType == current.SourceType;
                    flag &= ds.SourceName == current.SourceName;
                    if (flag)
                    {
                        switch (ds.SourceType)
                        {
                            case SourceType.Local:
                                flag &= ds.LocalSourcePath == current.LocalSourcePath;
                                break;
                            case SourceType.Remote:
                                flag &= ds.RemoteSourceIp == current.RemoteSourceIp;
                                break;
                        }
                    }
                    if (flag)
                        break;
                    index++;
                }
                if (flag)
                    SourceItems.View.MoveCurrentToPosition(index);
            }
        }

        private void SIM_SourceUpdated(object sender, SourceInfoEventArgs e)
        {
            reloadSource();
        }

        private void SIM_SourceCreated(object sender, SourceInfoEventArgs e)
        {
            reloadSource();
        }

        private bool _srcReloading = false;
        private void reloadSource()
        {
            _srcReloading = true;
            SourceItems.Source = new ObservableCollection<DataSource>(_srcPersis.SelectAll());
            SourceItems.View.MoveCurrentToPosition(-1);
            _srcReloading = false;
        }

        private void thisPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SelectedSource):
                    if (!_srcReloading)
                        SourceInfoModel.SetToUpdate(SelectedSource);
                    break;
            }
        }

        [AutoNotify]
        public SourceInfoViewModel SourceInfoModel { get; private set; }
        [AutoNotify]
        public CollectionViewSource SourceItems { get; private set; }
        [AutoNotify]
        public DataSource SelectedSource { get; set; }
        [AutoNotify]
        public ICommand CloseCmd { get; private set; }
        [AutoNotify]
        public ICommand CreateNewCmd { get; private set; }
        [AutoNotify]
        public ICommand DeleteCmd { get; private set; }
        [AutoNotify]
        public ICommand ConnectCmd { get; private set; }

        private void doCloseCmd()
        {

        }

        private void doCreateNewCmd()
        {
            SourceInfoModel.CreateNew();
            SourceItems.View.MoveCurrentTo(null);
        }

        private void doDeleteCmd()
        {
            if (SelectedSource != null)
            {
                _srcPersis.Delete(SelectedSource.SourceName);
                reloadSource();
            }
        }

        private void doConnectCmd()
        {
            DataSource si = SourceInfoModel.GetSourceInfo();
            VideoInfoManager.Instance.CheckServerValid(si);
            VideoInfoManager.Instance.UpdateSource(si);
            ConnectSource = SourceInfoModel.GetSourceInfo();
            OnConnected();
        }

        public DataSource ConnectSource { get; private set; }

        #region 【事件定义】
        public event EventHandler Connected;

        private void OnConnected()
        {
            EventHandler handler = Connected;
            if (handler != null)
                handler(this, new EventArgs());
        }
        #endregion 【事件定义】
    }
}

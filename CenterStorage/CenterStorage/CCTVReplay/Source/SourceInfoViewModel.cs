using AopUtil.WpfBinding;
using CCTVReplay.Interface;
using CCTVReplay.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Forms;

namespace CCTVReplay.Source
{
    public class SourceInfoViewModel : ObservableObject
    {
        private ISourcePersistence _srcPersis;
        private DataSource _oldSI;
        public SourceInfoViewModel(ISourcePersistence srcPersis)
        {
            _srcPersis = srcPersis;
            SourceTypes = new CollectionViewSource();
            SourceTypes.Source = new ObservableCollection<SourceTypeView>() { SourceTypeView.Remote, SourceTypeView.Local };
            SourceTypes.View.MoveCurrentToFirst();

            PathSelCmd = new CommandDelegate(_ => doPathSelCmd());
            UpdateCmd = new CommandDelegate(_ => doUpdateCmd());
            CreateNewCmd = new CommandDelegate(_ => doCreateNewCmd());
            this.PropertyChanged += thisPropertyChanged;
        }

        private void thisPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SelectedSourceType):
                    clearAll();
                    break;
            }
        }

        public CollectionViewSource SourceTypes { get; private set; }
        [AutoNotify]
        public SourceTypeView SelectedSourceType { get; set; }
        [AutoNotify]
        public string SourceName { get; set; }
        [AutoNotify]
        public string LocalSourcePath { get; set; }
        [AutoNotify]
        public string RemoteSourceIp { get; set; }
        [AutoNotify]
        public string Username { get; set; }
        [AutoNotify]
        public string Password { get; set; }
        [AutoNotify]
        public bool StoreUserInfo { get; set; }
        [AutoNotify]
        public bool IsCreateNew { get; set; }
        [AutoNotify]
        public ICommand PathSelCmd { get; set; }
        [AutoNotify]
        public ICommand UpdateCmd { get; private set; }
        [AutoNotify]
        public ICommand CreateNewCmd { get; private set; }

        private void doPathSelCmd()
        {
            FolderBrowserDialog ofd = new FolderBrowserDialog();
            ofd.SelectedPath = LocalSourcePath;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                LocalSourcePath = ofd.SelectedPath;
            }
        }

        private void doUpdateCmd()
        {
            if (_oldSI != null && !IsCreateNew && !string.IsNullOrWhiteSpace(SourceName))
            {
                DataSource si = buildSI();
                if (_srcPersis.Update(_oldSI.SourceName, si))
                    OnSourceUpdated(si);
            }
        }

        private void doCreateNewCmd()
        {
            if (IsCreateNew && !string.IsNullOrWhiteSpace(SourceName))
            {
                DataSource si = buildSI();
                if (_srcPersis.Insert(si))
                    OnSourceCreated(si);
            }
        }

        private DataSource buildSI()
        {
            DataSource si = new DataSource();
            si.SourceName = SourceName;
            si.SourceType = SelectedSourceType.SourceType;
            if (SelectedSourceType.SourceType == SourceType.Local)
            {
                si.LocalSourcePath = LocalSourcePath;
            }
            else
            {
                si.RemoteSourceIp = RemoteSourceIp;
                if (StoreUserInfo)
                {
                    si.Username = Username;
                    si.Password = Password;
                }
            }
            return si;
        }

        public DataSource GetSourceInfo()
        {
            return buildSI();
        }

        public void SetToUpdate(DataSource si)
        {
            if (si != null)
            {
                copyToCurrent(si);
                IsCreateNew = false;
                _oldSI = si;
            }
        }

        public void CreateNew()
        {
            copyToCurrent(DataSource.Empty);
            IsCreateNew = true;
        }

        private void copyToCurrent(DataSource si)
        {
            foreach (SourceTypeView stv in SourceTypes.Source as IEnumerable<SourceTypeView>)
            {
                if (stv.SourceType == si.SourceType)
                {
                    SourceTypes.View.MoveCurrentTo(stv);
                    break;
                }
            }

            SourceName = si.SourceName;
            LocalSourcePath = si.LocalSourcePath;
            RemoteSourceIp = si.RemoteSourceIp;
            Username = si.Username;
            Password = si.Password;
            if (!string.IsNullOrWhiteSpace(Username) || !string.IsNullOrWhiteSpace(Password))
                StoreUserInfo = true;
            //TODO；
        }

        private void clearAll()
        {
            SourceName = "";
            LocalSourcePath = "";
            RemoteSourceIp = "";
            Username = "";
            Password = "";
            IsCreateNew = true;
        }

        #region 【事件】
        public event EventHandler<SourceInfoEventArgs> SourceCreated;
        public event EventHandler<SourceInfoEventArgs> SourceUpdated;

        private void OnSourceCreated(DataSource si)
        {
            EventHandler<SourceInfoEventArgs> handler = SourceCreated;
            if (handler != null)
                handler(this, new SourceInfoEventArgs(si));
        }

        private void OnSourceUpdated(DataSource si)
        {
            EventHandler<SourceInfoEventArgs> handler = SourceUpdated;
            if (handler != null)
                handler(this, new SourceInfoEventArgs(si));
        }
        #endregion 【事件】
    }

    public class SourceInfoEventArgs : EventArgs
    {
        public SourceInfoEventArgs(DataSource si)
        {
            this.SourceInfo = si;
        }
        public DataSource SourceInfo { get; private set; }
    }
}

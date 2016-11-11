using AopUtil.WpfBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVDownload.AutoSave
{
    public class CustomSetting : ObservableObject
    {
        public CustomSetting()
        {
            PropertyChanged += CustomSetting_PropertyChanged;
        }

        private void CustomSetting_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnDataChanged();
        }

        [AutoNotify]
        public string DownloadPath { get; set; } = "D:\\视酷下载";

        #region 【事件】
        public event EventHandler DataChanged;
        private void OnDataChanged()
        {
            EventHandler handler = DataChanged;
            if (handler != null)
                handler(this, new EventArgs());
        }
        #endregion 【事件】
    }
}

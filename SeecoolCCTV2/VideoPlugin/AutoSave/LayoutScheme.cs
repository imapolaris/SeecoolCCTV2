using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoNS.SplitScreen;

namespace VideoNS.AutoSave
{
    public class LayoutScheme
    {
        private static LayoutScheme _instance;
        public static LayoutScheme Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LayoutScheme();
                return _instance;
            }
        }

        private LayoutScheme()
        {

        }

        private SplitScreenModel _splitModel;

        public SplitScreenModel Scheme
        {
            get
            {
                if (_splitModel == null)
                    LoadScheme();
                return _splitModel;
            }
            private set
            {
                UninstallSubEvent(_splitModel);
                _splitModel = value;
                InstallSubEvent(_splitModel);
            }
        }

        private void LoadScheme()
        {
            SplitScreenModel scheme = new SplitScreenModel(false);
            scheme.SplitScreenData = LayoutAutoSave.LoadData();
            if (scheme.SplitScreenData == null)
                scheme.SplitScreenData = new Model.SplitScreenInfo()
                {
                    Split = 1,
                    Nodes = new Model.SplitScreenNode[]
                    {
                        new Model.SplitScreenNode()
                    }
                };
            Scheme = scheme;
        }

        public void Reset()
        {
            Scheme = null;
        }

        private void InstallSubEvent(SplitScreenModel node)
        {
            if (node != null)
            {
                node.DataChanged += Node_DataChanged;
                node.PropertyChanged += Node_PropertyChanged;
            }
        }

        private void UninstallSubEvent(SplitScreenModel node)
        {
            if (node != null)
            {
                node.DataChanged -= Node_DataChanged;
                node.PropertyChanged -= Node_PropertyChanged;
            }
        }
        private void Node_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Scheme.IsOnEditting))
            {
                //当由编辑状态返回时，执行一次保存。
                if (!Scheme.IsOnEditting)
                    AutoSave();
            }
        }

        private void Node_DataChanged(object sender, EventArgs e)
        {
            if(!Scheme.OnSwitching)
            AutoSave();
        }

        #region 【执行AutoSave】
        private void AutoSave()
        {
            LayoutAutoSave.LazySaveData(Scheme.SplitScreenData);
            if (LayoutDataChanged != null)
                LayoutDataChanged();
        }
        #endregion 【执行AutoSave】

        public event Action LayoutDataChanged;
    }
}

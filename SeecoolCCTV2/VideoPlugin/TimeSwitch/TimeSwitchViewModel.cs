using AopUtil.WpfBinding;
using Common.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using VideoNS.Base;
using VideoNS.Json;
using VideoNS.Layout;
using VideoNS.Model;
using System.Threading;
using VideoNS.AutoSave;
using System.Collections.ObjectModel;
using VideoNS.Helper;
using VideoNS.SplitScreen;

namespace VideoNS.TimeSwitch
{
    public class TimeSwitchViewModel : ObservableObject
    {
        public TimeSwitchViewModel()
        {
            PlansSource = new CollectionViewSource();
            loadTimingSwitch(TimeSwitchAutoSave.LoadData());
            ImportCommand = new DelegateCommand(_ => importLayout());
            ExportCommand = new DelegateCommand(_ => exportLayout());
            ClearCommand = new DelegateCommand(_ => clearLayout());
            ReturnCommand = new DelegateCommand(_ => returnLayout());
            CurrentPlan = new LayoutViewModel() { IsVisibleReturn = false };
            updateCurrentPlan();
        }

        public void ShowSelected()
        {
            if (SelectedPlan != null)
            {
                if (SelectedPlan == AddPlanStatic)
                    addNewPlan(getDefaultPlanLayout());
                else
                    updateCurrentPlan();
            }
        }

        private void loadTimingSwitch(TimeSwitchInfo[] datas)
        {
            var infos = new ObservableCollection<LayoutPlanModel>(datas == null ? new LayoutPlanModel[0] : datas.Select(e => getLayoutPlanModel(e)));
            loadPlans(infos);
        }

        [AutoNotify]
        public CollectionViewSource PlansSource { get; set; }

        [AutoNotify]
        public LayoutPlanModel SelectedPlan { get; set; }
        LayoutPlanModel _lastSelectedPlan;

        [AutoNotify]
        public LayoutViewModel CurrentPlan { get; set; }

        [AutoNotify]
        public LayoutPlanModel DragPlan { get; set; }

        void updateCurrentPlan()
        {
            if (SelectedPlan != null && SelectedPlan.LayoutSource != null)
            {
                if (CurrentPlan.SplitScreenModel != SelectedPlan.LayoutSource)
                {
                    CurrentPlan.SplitScreenModel = SelectedPlan.LayoutSource;
                    CurrentPlan.SplitLayoutsModel.SplitScreenSource.View.Refresh();
                }
            }
            else
            {
                CurrentPlan.SplitScreenModel = new SplitScreenModel();
            }
            _lastSelectedPlan = SelectedPlan;
        }

        void clearSelectedAndUpdateCurrentPlan()
        {
            SelectedPlan = null;
            updateCurrentPlan();
        }

        #region Drag And Drop

        public void ToDragPlan()
        {
            if (SelectedPlan != null)
            {
                var source = getSource();
                DragPlan = SelectedPlan;
                SelectedPlan = null;
                source.Remove(DragPlan);
            }
        }

        public void ToDropPlan(int index)
        {
            if (DragPlan != null && index >= 0)
            {
                var source = getSource();
                index = Math.Min(index, source.Count - 1);
                source.Insert(index, DragPlan);
                SelectedPlan = DragPlan;
                updateCurrentPlan();
                DragPlan = null;
            }
        }

        #endregion Drag And Drop

        #region 导入
        public ICommand ImportCommand { get; set; }

        private void importLayout()
        {
            TimeSwitchInfo[] datas = JsonParserExpand.DeserializeFromFile<TimeSwitchInfo[]>(DirectoryHelper.TimeSwitchSchemeName, DirectoryHelper.TimeSwitchFileExt);
            if (datas != null)
            {
                loadTimingSwitch(datas);
                AutoSave();
            }
        }
        #endregion 导入

        #region 导出
        public ICommand ExportCommand { get; set; }

        private void exportLayout()
        {
            TimeSwitchInfo[] datas = getTimeSwitchInfos();
            JsonParserExpand.SerializeToFile<TimeSwitchInfo[]>(datas, DirectoryHelper.TimeSwitchSchemeName,DirectoryHelper.TimeSwitchFileExt);
        }
        #endregion 导出

        #region 清空

        public ICommand ClearCommand { get; set; }

        private void clearLayout()
        {//TODO:清空界面布局方案
            clearSelectedAndUpdateCurrentPlan();
            var source = getSource();
            source.ToList().ForEach(e =>
            {
                if (e != AddPlanStatic)
                {
                    e.ToDelete();
                    e.DataChanged -= LayoutPlan_DataChanged;
                }
            });
            while (source.Count() > 1)
            {
                source.RemoveAt(0);
            }
            _lastSelectedPlan = null;
            AutoSave();
        }
        #endregion 清空

        #region 返回
        public ICommand ReturnCommand { get; set; }
        public Action ReturnAction;

        private void returnLayout()
        {
            AutoSave();
            clearSelectedAndUpdateCurrentPlan();
            if (ReturnAction != null)
                ReturnAction();
        }
        #endregion 返回

        #region 资源

        void addNewPlan(LayoutPlanModel newPlan)
        {
            newPlan.ToBeDelete += toBeClose;
            newPlan.DataChanged += LayoutPlan_DataChanged;
            var source = getSource();
            source.Insert(source.Count - 1, newPlan);
            SelectedPlan = newPlan;
            updateCurrentPlan();
            AutoSave();
        }

        private void toBeClose(LayoutPlanModel plan)
        {
            RemovePlan(plan);
        }

        public void RemovePlan(LayoutPlanModel plan)
        {
            plan.ToBeDelete -= toBeClose;
            plan.DataChanged -= LayoutPlan_DataChanged;
            clearSelectedAndUpdateCurrentPlan();
            var source = getSource();
            source.Remove(plan);
            AutoSave();
            _lastSelectedPlan = null;
        }
        
        public void loadPlans(ObservableCollection<LayoutPlanModel> plans)
        {
            ObservableCollection<LayoutPlanModel> models = plans ?? new ObservableCollection<LayoutPlanModel>();
            if (models.FirstOrDefault(e => e == AddPlanStatic) == null)
                models.Add(AddPlanStatic);
            PlansSource.Source = models;
            plans?.ToList().ForEach(e =>
            {
                if (e != AddPlanStatic)
                {
                    e.ToBeDelete += toBeClose;
                    e.DataChanged += LayoutPlan_DataChanged;
                }
            });
        }

        List<LayoutPlanModel> getValidSource()
        {
            var source = getSource();
            return source.Where(e => e != AddPlanStatic).ToList();
        }

        ObservableCollection<LayoutPlanModel> getSource()
        {
            if (PlansSource.Source == null)
                PlansSource.Source = new ObservableCollection<LayoutPlanModel> { AddPlanStatic };
            var source = PlansSource.Source as ObservableCollection<LayoutPlanModel>;
            return source;
        }

        private void LayoutPlan_DataChanged(object sender, EventArgs e)
        {
            AutoSave();
        }
        #endregion 资源

        #region 数据转换

        private LayoutPlanModel getLayoutPlanModel(TimeSwitchInfo timeSwitchInfo)
        {
            return new LayoutPlanModel()
            {
                LayoutSource = new SplitScreen.SplitScreenModel() { SplitScreenData = timeSwitchInfo.Plan },
                StaySeconds = timeSwitchInfo.StayTime
            };
        }

        TimeSwitchInfo[] getTimeSwitchInfos()
        {
            var source = getValidSource();
            return source.Select(e => getLayoutPlanModel(e)).ToArray();
        }

        private TimeSwitchInfo getLayoutPlanModel(LayoutPlanModel model)
        {
            return new TimeSwitchInfo()
            {
                Plan = model.LayoutSource.SplitScreenData,
                StayTime = model.StaySeconds
            };
        }
        #endregion 数据转换

        #region 【AutoSave操作】
        public void AutoSave()
        {
            TimeSwitchAutoSave.LazySaveData(getTimeSwitchInfos());
            TimeSwitchScheme.Instance.Reset();
        }
        #endregion 【AutoSave操作】
        
        public static LayoutPlanModel AddPlanStatic { get; set; } = new LayoutPlanModel() { LayoutSource = null };
        LayoutPlanModel getDefaultPlanLayout()
        {
            LayoutPlanModel model = new LayoutPlanModel();
            LayoutPlanModel plan = null;
            if (_lastSelectedPlan != null && _lastSelectedPlan != AddPlanStatic)
                plan = _lastSelectedPlan;
            if (plan == null)
            {
                var source = getValidSource();
                plan = source.Count == 0 ? null : source.Last();
            }
            if (plan != null)
            {
                var splitData = plan.LayoutSource.SplitScreenData.Clone();
                splitData.Nodes.ToList().ForEach(e => e.VideoId = string.Empty);
                model.LayoutSource.SplitScreenData = splitData;
                model.StaySeconds = plan.StaySeconds;
            }
            return model;
        }
    }

    public class TimeSwitchInfo : ObservableObject
    {
        public TimeSwitchInfo()
        {
            this.PropertyChanged += TimeSwitchInfo_PropertyChanged;
        }

        private void TimeSwitchInfo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnDataChanged(new EventArgs());
        }

        [AutoNotify]
        public double StayTime { get; set; }

        private SplitScreenInfo _plan;
        public SplitScreenInfo Plan
        {
            get { return _plan; }
            set
            {
                UninstallSubEvent(_plan);
                updateProperty(ref _plan, value);
                InstallSubEvent(_plan);
            }
        }

        private void InstallSubEvent(SplitScreenInfo info)
        {
            if (info != null)
                info.DataChanged += Info_DataChanged;
        }

        private void UninstallSubEvent(SplitScreenInfo info)
        {
            if (info != null)
                info.DataChanged -= Info_DataChanged;
        }

        private void Info_DataChanged(object sender, EventArgs e)
        {
            OnDataChanged(e);
        }

        public event EventHandler DataChanged;
        protected virtual void OnDataChanged(EventArgs e)
        {
            if (DataChanged != null)
                DataChanged(this, e);
        }
    }
}

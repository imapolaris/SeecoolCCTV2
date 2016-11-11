using AopUtil.WpfBinding;
using Common.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VideoNS.Model;
using VideoNS.VideoInfo.Search;
using VideoNS.SplitScreen;
using VideoNS.Base;
using VideoNS.Json;
using System.Windows;
using VideoNS.Helper;
using VideoNS.SubWindow;

namespace VideoNS.Layout
{
    public class LayoutViewModel : ObservableObject
    {
        public LayoutViewModel()
        {
            SplitLayoutsModel = new SplitScreenLayoutsModel();
            SearcherModel = new SearchedResultViewModel();
            SplitScreenModel = new SplitScreenModel();
            SplitLayoutsModel.PropertyChanged += SplitLayoutsModel_PropertyChanged;

            ImportCommand = new DelegateCommand(_ => importLayout());
            ExportCommand = new DelegateCommand(_ => exportLayout());
            ClearCommand = new DelegateCommand(_ => clearLayout());
            ReturnCommand = new DelegateCommand(_ => returnLayout());
            ShortcutCmd = new DelegateCommand(x => doShortcut());

            AutoSave.LayoutScheme.Instance.LayoutDataChanged += onLayoutDataChanged;
            PropertyChanged += onPropertyChanged;
            SearcherModel.IsVisible = true;
        }

        private void onPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SplitScreenModel):
                    updateDisplayVideos();
                    break;
            }
        }

        private void SplitLayoutsModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SplitLayoutsModel.CurrentSplitLayout):
                    updateCurrentSplitLayout();
                    break;
            }
        }

        private void onLayoutDataChanged()
        {
            updateDisplayVideos();
        }

        [AutoNotify]
        public SplitScreenLayoutsModel SplitLayoutsModel { get; set; }

        [AutoNotify]
        public SearchedResultViewModel SearcherModel { get; set; }

        [AutoNotify]
        public SplitScreenModel SplitScreenModel { get; set; }

        #region 导入

        public ICommand ImportCommand { get; set; }

        private void importLayout()
        {
            SplitScreenInfo data = JsonParserExpand.DeserializeFromFile<SplitScreenInfo>(DirectoryHelper.LayoutSchemeName, DirectoryHelper.LayoutFileExt);
            if (data != null)
                SplitScreenModel.SplitScreenData = data;
        }

        #endregion 导入

        #region 导出
        public ICommand ExportCommand { get; set; }

        private void exportLayout()
        {
            JsonParserExpand.SerializeToFile<SplitScreenInfo>(SplitScreenModel.SplitScreenData, DirectoryHelper.LayoutSchemeName, DirectoryHelper.LayoutFileExt);
        }
        #endregion 导出

        #region 清空

        public ICommand ClearCommand { get; set; }
        public Action ClearAllDisplayEvent;

        private void clearLayout()
        {//TODO:清空界面布局方案
            SplitScreenModel?.SplitScreenData?.Nodes.ToList().ForEach(_ => _.VideoId = null);
            if (ClearAllDisplayEvent != null)
                ClearAllDisplayEvent();
        }
        #endregion 清空

        #region 【快捷键设置】
        public ICommand ShortcutCmd{ get; private set; }

        private void doShortcut()
        {
            ShortcutWin sWin = new ShortcutWin();
            sWin.Owner = Application.Current.MainWindow;
            sWin.ShowDialog();
        }
        #endregion 【快捷键设置】

        #region 返回
        [AutoNotify]
        public bool IsVisibleReturn { get; set; } = true;
        public ICommand ReturnCommand { get; set; }
        public Action ReturnAction;

        private void returnLayout()
        {
            if (ReturnAction != null)
                ReturnAction();
        }
        #endregion 返回

        #region 更新选中的布局模板

        void updateCurrentSplitLayout()
        {
            if (SplitLayoutsModel.CurrentSplitLayout != null)
            {
                var infos = SplitLayoutsModel.CurrentSplitLayout;
                var videoIds = getVideoIds();
                if (videoIds != null && videoIds.Count() > 0)
                {
                    int count = Math.Min(videoIds.Count(), infos.Nodes.Length);
                    for (int i = 0; i < count; i++)
                        infos.Nodes[i].VideoId = videoIds.ElementAt(i);
                }
                SplitScreenModel.SplitScreenData = infos;
            }
        }

        private void updateDisplayVideos()
        {
            List<string> videoIds = SplitScreenModel?.SplitScreenData?.Nodes?.Where(e => !string.IsNullOrWhiteSpace(e.VideoId)).Select(v => v.VideoId).ToList();
            SearcherModel.UpdateDisplayVideos(videoIds);
        }

        IEnumerable<string> getVideoIds()
        {
            if (SplitScreenModel?.SplitScreenData?.Nodes == null)
                return null;
            int split = SplitScreenModel.SplitScreenData.Split;
            return SplitScreenModel.SplitScreenData.Nodes.Where(_ => !string.IsNullOrWhiteSpace(_.VideoId)).Select(e => e.VideoId);
        }

        #endregion 更新选中的布局模板
    }
}

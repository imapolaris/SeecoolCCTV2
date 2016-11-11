using AopUtil.WpfBinding;
using Common.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using VideoNS.AutoSave;
using VideoNS.SubWindow;

namespace VideoNS.Model
{
    public class SplitScreenLayoutsModel : ObservableObject
    {
        ObservableCollection<SplitScreenLayoutModel> _listLayout;
        public SplitScreenLayoutsModel()
        {
            SplitTypeSource = new CollectionViewSource();
            SplitTypeSource.Source = new List<SplitScreenType>() {
                SplitScreenType.等分屏,
                SplitScreenType.一大多小,
                SplitScreenType.两大多小,
                SplitScreenType.自定义
            };
            updateListLayoutSource();
            SplitScreenSource = new CollectionViewSource();
            SplitScreenSource.Source = _listLayout;
            SplitScreenSource.View.MoveCurrentTo(null);
            SplitScreenSource.Filter += SplitScreenSource_Filter;
            PropertyChanged += SplitScreenLayoutsModel_PropertyChanged;
        }

        private void SplitScreenSource_Filter(object sender, FilterEventArgs e)
        {
            e.Accepted = this.reviewFilteredFilter(e.Item);
        }

        private bool reviewFilteredFilter(object item)
        {
            var model = item as SplitScreenLayoutModel;
            return (model != null && model.SplitType == SelectedSplitType);
        }

        private void SplitScreenLayoutsModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SelectedSplitType):
                    SplitScreenSource.View.Refresh();
                    break;
            }
        }

        public CollectionViewSource SplitScreenSource { get; set; }
        public CollectionViewSource SplitTypeSource { get; set; }

        [AutoNotify]
        public SplitScreenType SelectedSplitType { get; set; }

        [AutoNotify]
        public SplitScreenLayoutModel SelectedSplitLayout { get; set; }

        [AutoNotify]
        public SplitScreenInfo CurrentSplitLayout { get; set; }

        public void AddSplitScreenLayoutModel(SplitScreenLayoutModel model)
        {
            if (model != null)
            {
                if (model != HandSetIcon && model.SplitType == SplitScreenType.自定义)
                {
                    model.DeleteCommand = new DelegateCommand(_ => removeLayoutModel(model));
                }
                _listLayout.Insert(_listLayout.Count - 1, model);
            }
        }

        void removeLayoutModel(SplitScreenLayoutModel model)
        {
            _listLayout.Remove(model);
            CustomLayoutScheme.Instance.Remove(model.SplitScreenInfom as CustomLayout);
        }

        public static SplitScreenLayoutModel HandSetIcon = new SplitScreenLayoutModel()
        {
            Header = "",
            SplitType = SplitScreenType.自定义,
            SplitScreenInfom = new SplitScreenInfo(),
            IsValibleCloseButton =false
        };

        private void updateListLayoutSource()
        {
            _listLayout = new ObservableCollection<SplitScreenLayoutModel>();
            _listLayout.Add(HandSetIcon);
            AddSplitScreenLayoutModel(Get等分屏(1));
            AddSplitScreenLayoutModel(Get等分屏(2));
            AddSplitScreenLayoutModel(Get等分屏(3));
            AddSplitScreenLayoutModel(Get等分屏(4));
            AddSplitScreenLayoutModel(Get等分屏(5));
            AddSplitScreenLayoutModel(Get等分屏(6));

            AddSplitScreenLayoutModel(Get一大多小(3));
            AddSplitScreenLayoutModel(Get一大多小(4));
            AddSplitScreenLayoutModel(Get一大多小(5));
            AddSplitScreenLayoutModel(Get一大多小(6));

            AddSplitScreenLayoutModel(Get两大多小(4));
            AddSplitScreenLayoutModel(Get两大多小左侧(4));
            AddSplitScreenLayoutModel(Get两大多小(6));
            AddSplitScreenLayoutModel(Get两大多小左侧(6));

            foreach (SplitScreenLayoutModel model in getCustomLayout())
                AddSplitScreenLayoutModel(model);
        }

        public static List<SplitScreenLayoutModel> getCustomLayout()
        {
            CustomLayout[] layouts = CustomLayoutScheme.Instance.Layouts;
            List<SplitScreenLayoutModel> models = new List<SplitScreenLayoutModel>();
            foreach (CustomLayout info in layouts)
            {
                if (info != null)
                    models.Add(new SplitScreenLayoutModel()
                    {
                        Header = info.LayoutName,
                        SplitScreenInfom = info,
                        SplitType = SplitScreenType.自定义,
                        IsValibleCloseButton = true
                    });
            }
            return models;
        }

        public void UpdateSelectedLayout()
        {
            if (SelectedSplitLayout != null)
            {
                if (SelectedSplitLayout != HandSetIcon)
                    CurrentSplitLayout = SelectedSplitLayout.SplitScreenInfom.Clone();
                else
                    addUserDefinedLayoutDesign();
                SelectedSplitLayout = null;
            }
        }

        private void addUserDefinedLayoutDesign()
        {//DialogWin.Show("未定义自动添加界面");
            if (SelectedSplitLayout == HandSetIcon)
            {
                LayoutDesignWin win = new LayoutDesignWin();
                if ((bool)win.ShowDialog())
                {
                    AddSplitScreenLayoutModel(win.SplitScreenLayout);
                }
            }
        }

        #region 默认布局模板

        public static SplitScreenLayoutModel Get等分屏(int split)
        {
            SplitScreenInfo info = new SplitScreenInfo() { Split = split };
            SplitScreenNode[] nodes = new SplitScreenNode[split * split];
            int index = 0;
            for (int i = 0; i < split; i++)
                for (int j = 0; j < split; j++)
                {
                    nodes[index++] = new SplitScreenNode() { Row = i, Column = j, RowSpan = 1, ColumnSpan = 1 };
                }
            info.Nodes = nodes;
            SplitScreenLayoutModel model = new SplitScreenLayoutModel() { Header = string.Format("{0}×{0}", split), SplitScreenInfom = info, SplitType = SplitScreenType.等分屏 };
            return model;
        }

        public static SplitScreenLayoutModel Get一大多小(int split)
        {
            SplitScreenInfo info = new SplitScreenInfo() { Split = split };
            SplitScreenNode[] nodes = new SplitScreenNode[split * 2];
            int index = 0;
            nodes[index++] = new SplitScreenNode() { Row = 0, Column = 0, RowSpan = split - 1, ColumnSpan = split - 1 };
            for (int i = 0; i < split - 1; i++)
                nodes[index++] = new SplitScreenNode() { Row = i, Column = split - 1, RowSpan = 1, ColumnSpan = 1 };
            for (int j = 0; j < split; j++)
                nodes[index++] = new SplitScreenNode() { Row = split - 1, Column = j, RowSpan = 1, ColumnSpan = 1 };
            info.Nodes = nodes;
            SplitScreenLayoutModel model = new SplitScreenLayoutModel() { Header = string.Format("1+{0}", split * 2 - 1), SplitScreenInfom = info, SplitType = SplitScreenType.一大多小 };
            return model;
        }

        public static SplitScreenLayoutModel Get两大多小(int split)
        {
            SplitScreenInfo info = new SplitScreenInfo() { Split = split };
            SplitScreenNode[] nodes = new SplitScreenNode[2 + split * split / 2];
            int index = 0;
            nodes[index++] = new SplitScreenNode() { Row = 0, Column = 0, RowSpan = split / 2, ColumnSpan = split / 2 };
            nodes[index++] = new SplitScreenNode() { Row = 0, Column = split / 2, RowSpan = split / 2, ColumnSpan = split / 2 };
            for (int i = split / 2; i < split; i++)
                for (int j = 0; j < split; j++)
                {
                    nodes[index++] = new SplitScreenNode() { Row = i, Column = j, RowSpan = 1, ColumnSpan = 1 };
                }

            info.Nodes = nodes;
            SplitScreenLayoutModel model = new SplitScreenLayoutModel() { Header = string.Format("2+{0}", split * split / 2), SplitScreenInfom = info, SplitType = SplitScreenType.两大多小 };
            return model;
        }

        public static SplitScreenLayoutModel Get两大多小左侧(int split)
        {
            SplitScreenInfo info = new SplitScreenInfo() { Split = split };
            SplitScreenNode[] nodes = new SplitScreenNode[split * split / 2 + 2];
            int index = 0;
            nodes[index++] = new SplitScreenNode() { Row = 0, Column = 0, RowSpan = split / 2, ColumnSpan = split / 2 };
            nodes[index++] = new SplitScreenNode() { Row = split / 2, Column = 0, RowSpan = split / 2, ColumnSpan = split / 2 };
            for (int i = 0; i < split; i++)
                for (int j = split / 2; j < split; j++)
                {
                    nodes[index++] = new SplitScreenNode() { Row = i, Column = j, RowSpan = 1, ColumnSpan = 1 };
                }

            info.Nodes = nodes;
            SplitScreenLayoutModel model = new SplitScreenLayoutModel() { Header = string.Format("2+{0}", split * split / 2), SplitScreenInfom = info, SplitType = SplitScreenType.两大多小 };
            return model;
        }

        public static SplitScreenLayoutModel Get三大多小(int split)
        {
            int smallCount = (split / 2) * (split / 2);
            SplitScreenInfo info = new SplitScreenInfo() { Split = split };
            SplitScreenNode[] nodes = new SplitScreenNode[3 + smallCount];
            int index = 0;
            nodes[index++] = new SplitScreenNode() { Row = 0, Column = 0, RowSpan = split / 2, ColumnSpan = split / 2 };
            nodes[index++] = new SplitScreenNode() { Row = 0, Column = split / 2, RowSpan = split / 2, ColumnSpan = split / 2 };
            nodes[index++] = new SplitScreenNode() { Row = split / 2, Column = 0, RowSpan = split / 2, ColumnSpan = split / 2 };
            for (int i = split / 2; i < split; i++)
                for (int j = split / 2; j < split; j++)
                {
                    nodes[index++] = new SplitScreenNode() { Row = i, Column = j, RowSpan = 1, ColumnSpan = 1 };
                }

            info.Nodes = nodes;
            SplitScreenLayoutModel model = new SplitScreenLayoutModel() { Header = string.Format("3+{0}", smallCount), SplitScreenInfom = info, SplitType = SplitScreenType.自定义, IsValibleCloseButton=false };
            return model;
        }

        #endregion 默认布局模板
    }
}

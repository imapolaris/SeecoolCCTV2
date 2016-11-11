using AopUtil.WpfBinding;
using Common.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VideoNS.AutoSave;
using VideoNS.Model;

namespace VideoNS.SubWindow
{
    internal class LayoutDesignModel : ObservableObject
    {
        private List<GridBlockModel> _customModel;
        public LayoutDesignModel()
        {
            this.ResetCmd = new DelegateCommand(_ => DoReset());
            this.SaveCmd = new DelegateCommand(_ => DoSave());
            this.UndoCmd = new DelegateCommand(_ => DoUndo());
            _customModel = new List<GridBlockModel>();
            LayoutName = CustomLayoutScheme.Instance.FindValidName();
            Split = 10;
        }

        private int _split = 0;
        public int Split
        {
            get { return _split; }
            set
            {
                if (value > 0 && value <= 16)
                    updateProperty(ref _split, value);
            }
        }

        [AutoNotify]
        public string LayoutName { get; set; }

        [AutoNotify]
        public ICommand ResetCmd { get; private set; }

        [AutoNotify]
        public ICommand UndoCmd { get; private set; }

        [AutoNotify]
        public ICommand SaveCmd { get; private set; }
        public Action SaveAction;

        public SplitScreenLayoutModel SplitScreenLayoutModel
        {
            get
            {
                return new SplitScreenLayoutModel() {
                    SplitType = SplitScreenType.自定义,
                    Header = LayoutName,
                    SplitScreenInfom = _info,
                    IsValibleCloseButton = true
                };
            }
        }

        public event EventHandler<BlockCreateEventArgs> CustomBlockAdded;
        public event EventHandler<BlockCreateEventArgs> CustomBlockRemoved;
        public event EventHandler CustomBlockCleared;
        private void OnCustomBlockAdded(BlockCreateEventArgs e)
        {
            if (CustomBlockAdded != null)
                CustomBlockAdded(this, e);
        }

        private void OnCustomBlockRemoved(BlockCreateEventArgs e)
        {
            if (CustomBlockRemoved != null)
                CustomBlockRemoved(this, e);
        }

        private void OnCustomBlockCleared()
        {
            if (CustomBlockCleared != null)
                CustomBlockCleared(this, new EventArgs());
        }

        private void DoReset()
        {
            _customModel.Clear();
            OnCustomBlockCleared();
        }
        CustomLayout _info;
        private void DoSave()
        {
            if (_customModel.Count > 0)
            {
                _info = new CustomLayout() { Split = this.Split, Nodes = _customModel.ToArray(), LayoutName = this.LayoutName };
                CustomLayoutScheme.Instance.Add(_info);
                if (SaveAction != null)
                    SaveAction();
            }
        }

        private void DoUndo() {
            if (_customModel.Count > 0)
            {
                GridBlockModel block = _customModel[_customModel.Count - 1];
                _customModel.RemoveAt(_customModel.Count - 1);
                OnCustomBlockRemoved(new BlockCreateEventArgs(block));
            }
        }


        private RowColumn _starRC;
        private GridBlockModel _currentBlock;
        public void StartDesignBlock(RowColumn startRC)
        {
            _starRC = startRC;
        }

        public void UpdateDesignBlock(RowColumn curRC)
        {
            if (_currentBlock == null)
            {
                _currentBlock = new GridBlockModel(false);
                //fe.Children.Add(_currentBlock);
                AddCustomBlock(_currentBlock);
                OnCustomBlockAdded(new BlockCreateEventArgs(_currentBlock));
            }

            int rSpan = Math.Abs(curRC.Row - _starRC.Row) + 1;
            int cSpan = Math.Abs(curRC.Column - _starRC.Column) + 1;

            _currentBlock.Row = Math.Min(curRC.Row, _starRC.Row);
            _currentBlock.Column = Math.Min(curRC.Column, _starRC.Column);
            _currentBlock.RowSpan = rSpan;
            _currentBlock.ColumnSpan = cSpan;
        }

        public void CompleteDesignBlock()
        {
            if (_currentBlock != null)
            {
                _currentBlock = null;
            }
        }

        public void RemoveCustomBlock(GridBlockModel block)
        {
            if (block != null)
            {
                if (_customModel.Remove(block))
                    OnCustomBlockRemoved(new BlockCreateEventArgs(block));
            }
        }

        private void AddCustomBlock(GridBlockModel block)
        {
            if (block != null)
                _customModel.Add(block);
        }

        internal struct RowColumn
        {
            public int Row;
            public int Column;
            public RowColumn(int row, int col)
            {
                this.Row = row;
                this.Column = col;
            }

            public override string ToString()
            {
                return string.Format($"RowColumn({Row},{Column})");
            }
        }

        internal class BlockCreateEventArgs : EventArgs
        {
            private GridBlockModel _block;

            public GridBlockModel Block
            {
                get { return _block; }
            }

            public BlockCreateEventArgs(GridBlockModel block)
            {
                _block = block;
            }
        }
    }
}

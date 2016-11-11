using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VideoNS.Model;

namespace VideoNS.SubWindow
{
    /// <summary>
    /// LayoutDesignWin.xaml 的交互逻辑
    /// </summary>
    public partial class LayoutDesignWin : Window
    {
        private LayoutDesignModel ViewModel { get { return DataContext as LayoutDesignModel; } }

        public SplitScreenLayoutModel SplitScreenLayout
        {
            get
            {
                return ViewModel?.SplitScreenLayoutModel;
            }
        }

        private GridBlockManipulator _blockManip;
        private GridBlockManipulator BlockManipulator
        {
            get
            {
                if (_blockManip == null)
                    _blockManip = new GridBlockManipulator();
                return _blockManip;
            }
        }

        public LayoutDesignWin()
        {
            InitializeComponent();
            this.Loaded += LayoutDesignWin_Loaded;
            panelTop.MouseMove += PanelTop_MouseMove;
        }

        private void PanelTop_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void LayoutDesignWin_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            ViewModel.SaveAction += Model_Saved;
            ViewModel.CustomBlockAdded += ViewModel_CustomBlockAdded;
            ViewModel.CustomBlockRemoved += ViewModel_CustomBlockRemoved;
            ViewModel.CustomBlockCleared += ViewModel_CustomBlockCleared;
            gridBottom.PreviewMouseDown += GridCenter_MouseDown;
            gridBottom.PreviewMouseMove += GridCenter_MouseMove;
            gridBottom.PreviewMouseUp += GridCenter_MouseUp;
            gridBottom.AddHandler(GridBlock.DeleteEvent, new RoutedEventHandler(Block_Delete));

            gridCover.AddHandler(GridBlockManipulator.ResizeStartEvent, new RoutedEventHandler(Manip_ResizeStart));
            gridCover.AddHandler(GridBlockManipulator.ResizingEvent, new ShiftEventHandler(Manip_Resizing));
            gridCover.AddHandler(GridBlockManipulator.RepositionEvent, new ShiftEventHandler(Manip_Reposition));
            ReloadUI();
        }

        private Point _initPos;
        private Size _initSize;
        private void Manip_ResizeStart(object sender, RoutedEventArgs e)
        {
            GridBlockManipulator gbm = e.Source as GridBlockManipulator;
            _initPos = gbm.TranslatePoint(new Point(), gridCover);
            _initSize = new Size(gbm.ActualWidth, gbm.ActualHeight);
        }

        private void Manip_Resizing(object sender, ShiftEventArgs e)
        {
            //Console.WriteLine("{0},{1},{2},{3}", e.LeftShift, e.TopShift, e.WidthShift, e.HeightShift);
            Point pos = new Point(_initPos.X + e.LeftShift, _initPos.Y + e.TopShift);
            Point size = new Point(_initSize.Width + e.WidthShift, _initSize.Height + e.HeightShift);
            Point endPos = new Point(pos.X + size.X - 5, pos.Y + size.Y - 5); //5个像素偏移，以避开边界。
            LayoutDesignModel.RowColumn start = CalcRowColumn(pos);
            LayoutDesignModel.RowColumn end = CalcRowColumn(endPos);
            GridBlockManipulator gbm = e.Source as GridBlockManipulator;
            gbm.ViewModel.Row = start.Row;
            gbm.ViewModel.Column = start.Column;
            gbm.ViewModel.RowSpan = end.Row - start.Row + 1;
            gbm.ViewModel.ColumnSpan = end.Column - start.Column + 1;
        }

        private void Manip_Reposition(object sender, ShiftEventArgs e)
        {
            Point pos = new Point(_initPos.X + e.LeftShift, _initPos.Y + e.TopShift);
            LayoutDesignModel.RowColumn start = CalcRowColumn(pos);
            GridBlockManipulator gbm = e.Source as GridBlockManipulator;
            if (start.Row + gbm.ViewModel.RowSpan <= ViewModel.Split)
                gbm.ViewModel.Row = start.Row;
            if (start.Column + gbm.ViewModel.ColumnSpan <= ViewModel.Split)
                gbm.ViewModel.Column = start.Column;
        }

        private void ViewModel_CustomBlockCleared(object sender, EventArgs e)
        {
            gridBlocks.Children.Clear();
        }

        private void ViewModel_CustomBlockRemoved(object sender, LayoutDesignModel.BlockCreateEventArgs e)
        {
            FrameworkElement fe = null;
            foreach (FrameworkElement ff in gridBlocks.Children)
            {
                if (e.Block.Equals(ff.DataContext))
                {
                    fe = ff;
                    break;
                }
            }
            if (fe != null)
                gridBlocks.Children.Remove(fe);
        }

        private void ViewModel_CustomBlockAdded(object sender, LayoutDesignModel.BlockCreateEventArgs e)
        {
            gridBlocks.Children.Add(new GridBlock() { DataContext = e.Block });
        }

        private void Model_Saved()
        {
            this.DialogResult = true;
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ViewModel.Split):
                    ReloadUI();
                    break;
            }
        }

        private void ReloadUI()
        {
            ClearUI();
            for (int i = 0; i < ViewModel.Split; i++)
            {
                gridCover.RowDefinitions.Add(new RowDefinition());
                gridCover.ColumnDefinitions.Add(new ColumnDefinition());
                gridCover.Children.Add(CreateLine(i, ViewModel.Split, false));
                gridCover.Children.Add(CreateLine(i, ViewModel.Split, true));
                gridBlocks.RowDefinitions.Add(new RowDefinition());
                gridBlocks.ColumnDefinitions.Add(new ColumnDefinition());
            }
            //最后两条线
            Line line1 = CreateLine(0, ViewModel.Split, false);
            line1.VerticalAlignment = VerticalAlignment.Top;
            Line line2 = CreateLine(0, ViewModel.Split, true);
            line2.HorizontalAlignment = HorizontalAlignment.Left;
            gridCover.Children.Add(line1);
            gridCover.Children.Add(line2);

            BlockManipulator.Visibility = Visibility.Collapsed;
            gridCover.Children.Add(BlockManipulator);
        }

        private Line CreateLine(int rcIndex, int span, bool isVertical)
        {
            Line line = new Line();
            line.X1 = 0;
            line.Y1 = 0;
            if (isVertical)
            {
                line.SetValue(Grid.ColumnProperty, rcIndex);
                line.SetValue(Grid.RowSpanProperty, span);
                line.HorizontalAlignment = HorizontalAlignment.Right;
                line.X2 = 0;
                line.SetBinding(Line.Y2Property, CreateBinding(gridCover, BindingMode.OneWay, "ActualHeight"));
            }
            else
            {
                line.SetValue(Grid.RowProperty, rcIndex);
                line.SetValue(Grid.ColumnSpanProperty, span);
                line.VerticalAlignment = VerticalAlignment.Bottom;
                line.Y2 = 0;
                line.SetBinding(Line.X2Property, CreateBinding(gridCover, BindingMode.OneWay, "ActualWidth"));
            }
            line.Stroke = Brushes.White;
            line.Opacity = 0.5;
            line.StrokeThickness = 1;
            line.StrokeDashArray = new DoubleCollection(new double[] { 4, 4, 4, 4 });
            return line;
        }
        private Binding CreateBinding(object source, BindingMode mode, string path)
        {
            Binding binding = new Binding();
            binding.Mode = mode;
            binding.Source = source;
            if (path != null)
                binding.Path = new PropertyPath(path);
            return binding;
        }

        private void ClearUI()
        {
            gridCover.Children.Clear();
            gridCover.RowDefinitions.Clear();
            gridCover.ColumnDefinitions.Clear();

            gridBlocks.Children.Clear();
            gridBlocks.RowDefinitions.Clear();
            gridBlocks.ColumnDefinitions.Clear();
        }

        private bool _isMouseDown = false;
        private bool _mouseMoved = false;
        private Point _startPos;
        private void GridCenter_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _isMouseDown = true;
            _mouseMoved = false;
            _startPos = e.GetPosition(gridBottom);
            ViewModel.StartDesignBlock(CalcRowColumn(e.GetPosition(gridBottom)));
            BlockManipulator.Visibility = Visibility.Collapsed;
        }

        private void GridCenter_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isMouseDown)
            {
                Point pos = e.GetPosition(gridBottom);
                if (Math.Abs(pos.X - _startPos.X) >= SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(pos.Y - _startPos.Y) >= SystemParameters.MinimumVerticalDragDistance)
                {
                    _mouseMoved = true;
                    ViewModel.UpdateDesignBlock(CalcRowColumn(e.GetPosition(gridBottom)));
                }
            }
        }

        private void GridCenter_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isMouseDown)
            {
                _isMouseDown = false;
                ViewModel.CompleteDesignBlock();
            }
            if (!_mouseMoved && e.Source is GridBlock)
            {
                BlockManipulator.DataContext = (e.Source as GridBlock).ViewModel;
                BlockManipulator.Visibility = Visibility.Visible;
            }
        }

        private void Block_Delete(object sender, RoutedEventArgs e)
        {
            GridBlock gb = e.Source as GridBlock;
            if (gb != null)
            {
                ViewModel.RemoveCustomBlock(gb.ViewModel);
                BlockManipulator.Visibility = Visibility.Collapsed;
            }
        }

        private LayoutDesignModel.RowColumn CalcRowColumn(Point mousePos)
        {
            int row = (int)(mousePos.Y / gridCover.ActualHeight * ViewModel.Split);
            row = ReRightIndex(row);
            int col = (int)(mousePos.X / gridCover.ActualWidth * ViewModel.Split);
            col = ReRightIndex(col);
            return new LayoutDesignModel.RowColumn(row, col);
        }

        private int ReRightIndex(int index)
        {
            if (index < 0)
                index = 0;
            if (index >= ViewModel.Split)
                index = ViewModel.Split - 1;
            return index;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }



        private void tbSplit_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!isNumberic(e.Text))
                e.Handled = true;
        }

        private void tbSplit_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void tbSplit_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (text.Any(_ => _ == '.') && (sender as TextBox).Text.Any(_ => _ == '.'))
                { e.CancelCommand(); }
                else if (!isNumberic(text))
                { e.CancelCommand(); }
            }
            else { e.CancelCommand(); }
        }

        //isDigit是否是数字
        static bool isNumberic(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;
            foreach (char c in str)
            {
                if (!char.IsDigit(c))
                    return false;
            }
            return true;
        }
    }
}

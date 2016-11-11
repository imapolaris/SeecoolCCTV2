using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VideoNS.Model;
using VideoNS.VideoInfo;

namespace VideoNS.TimeSwitch
{
    /// <summary>
    /// LayoutIcon.xaml 的交互逻辑
    /// </summary>
    public partial class LayoutIcon : UserControl, IDisposable
    {
        VideoThumbnailsViewModel _model;
        public LayoutIcon()
        {
            InitializeComponent();
            _model = new VideoThumbnailsViewModel();
            img.DataContext = _model;
            DataContextChanged += onDataContextChanged;
        }

        private void onDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            SplitScreenNode oldModel = e.OldValue as SplitScreenNode;
            if (oldModel != null)
            {
                //_model.ID = oldModel.VideoId;
                oldModel.PropertyChanged -= onPropertyChanged;
            }
            SplitScreenNode newModel = e.NewValue as SplitScreenNode;
            if (newModel != null)
            {
                newModel.PropertyChanged += onPropertyChanged;
                _model.ID = newModel.VideoId;
            }   
        }

        public SplitScreenNode ViewModel {
            get { return DataContext as SplitScreenNode; }
        }

        private void onPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ViewModel.VideoId):
                    _model.ID = ViewModel.VideoId;
                    break;
            }
        }

        public void Dispose()
        {
            _model.ID = string.Empty;
            DataContext = null;
            DataContextChanged -= onDataContextChanged;
            GC.SuppressFinalize(this);
        }
    }
}

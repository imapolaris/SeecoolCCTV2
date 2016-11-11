using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using AopUtil.WpfBinding;

namespace VideoNS.Adorners
{
    public class CoverViewModel : ObservableObject
    {
        public CoverViewModel()
        {
            BrushVisual = new Border();
            TransParam = new TransformParam();
        }

        [AutoNotify]
        public Visual BrushVisual { get; set; }

        [AutoNotify]
        public TransformParam TransParam { get; set; }
    }
}

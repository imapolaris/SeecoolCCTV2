using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace VideoNS.Adorners
{
    internal class SplitScreenAdorner : Adorner
    {

        private VideoPanelDropBlur _blur;

        internal VideoPanelDropBlur VisualBlur
        {
            get { return _blur; }
        }

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        private AdornerLayer _layer;

        public SplitScreenAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            _blur = new VideoPanelDropBlur();
            _blur.Opacity = 0;
            this.AddVisualChild(_blur);

            _layer = AdornerLayer.GetAdornerLayer(adornedElement);
            _layer.Add(this);
            IsHitTestVisible = false;
        }


        protected override Visual GetVisualChild(int index)
        {
            return _blur;
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            _blur.Arrange(new Rect(arrangeBounds));
            return arrangeBounds;
        }

        /// <summary>
        /// 分离装饰器。
        /// </summary>
        public void Detatch()
        {
            _layer.Remove(this);
        }
    }
}

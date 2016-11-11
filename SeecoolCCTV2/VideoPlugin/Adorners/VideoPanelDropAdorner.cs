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
    internal class VideoPanelDropAdorner : Adorner
    {
        private VideoPanelDropCover _cover;

        internal VideoPanelDropCover VisualCover
        {
            get { return _cover; }
        }

        private VideoPanelDropBlur _blur;

        internal VideoPanelDropBlur VisualBlur
        {
            get { return _blur; }
        }

        protected override int VisualChildrenCount
        {
            get { return 2; }
        }

        private AdornerLayer _layer;

        public VideoPanelDropAdorner(UIElement adornedElement, Visual fillVisual)
            : base(adornedElement)
        {
            _cover = new VideoPanelDropCover();
            _cover.ViewModel.BrushVisual = fillVisual;
            this.AddVisualChild(_cover);

            _blur = new VideoPanelDropBlur();
            _blur.Opacity = 0;
            this.AddVisualChild(_blur);

            _layer = AdornerLayer.GetAdornerLayer(adornedElement);
            _layer.Add(this);
            IsHitTestVisible = false;
        }


        protected override Visual GetVisualChild(int index)
        {
            if (index == 0)
                return _blur;
            else
                return _cover;
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            _cover.Arrange(new Rect(arrangeBounds));
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoNS.VideoTrack
{
    public class BoundSizeControl
    {
        double _startZoom;
        BoundSize _startSize;
        BoundSize _sizeInf;
        BoundSize _sizeSup;

        double _zoom;
        public BoundSizeControl(double zoom, double width, double height, double widthInf = 0.02, double widthSup = 0.3, double heightInf = 0.02, double heightSup = 0.3)
        {
            _startZoom = zoom;
            _sizeInf = new BoundSize() { Width = widthInf, Height = heightInf };
            _sizeSup = new BoundSize() { Width = widthSup, Height = heightSup };
            _startSize = new BoundSize() { Width = width, Height = height };
            _zoom = _startZoom;
            setSizeByLimits(ref _startSize);
        }

        public double Zoom
        {
            get { return _zoom; }
            set
            {
                _zoom = value;
            }
        }

        public BoundSize Size
        {
            get
            {
                BoundSize size = _startSize;
                double zoomScale = Math.Sqrt(Zoom / _startZoom);
                size.Width *= zoomScale;
                size.Height *= zoomScale;
                setSizeByLimits(ref size);
                return size;
            }
        }

        public void ResetScale(double scaleX, double scaleY)
        {
            _startSize.Width *= scaleX;
            _startSize.Height *= scaleY;
            resetBoundSize();
        }

        public void ResetWidthScale(double scale)
        {
            _startSize.Width *= scale;
            resetBoundSize();
        }

        public void ResetHeightScale(double scale)
        {
            _startSize.Height *= scale;
            resetBoundSize();
        }

        private void resetBoundSize()
        {
            _startSize = Size;
            _startZoom = _zoom;
        }

        private void setSizeByLimits(ref BoundSize size)
        {
            setWidthByLimits(ref size.Width);
            setHeightByLimits(ref size.Height);
        }

        private void setWidthByLimits(ref double width)
        {
            width = Math.Max(_sizeInf.Width, Math.Min(_sizeSup.Width, width));
        }

        private void setHeightByLimits(ref double height)
        {
            height = Math.Max(_sizeInf.Height, Math.Min(_sizeSup.Height, height));
        }

        public struct BoundSize
        {
            public double Width;
            public double Height;
        }
    }
}

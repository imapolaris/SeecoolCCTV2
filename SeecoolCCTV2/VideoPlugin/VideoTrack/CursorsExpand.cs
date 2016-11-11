using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace VideoNS.VideoTrack
{
    public class CursorsExpand
    {
        public static Cursor FillGreenEllipseCursor
        {
            get {
                const int w = 25;
                const int h = 25;
                const int f = 5;

                var bmp = new Bitmap(w, h);

                Graphics g = Graphics.FromImage(bmp);
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                g.FillEllipse(System.Drawing.Brushes.Green, new System.Drawing.Rectangle(f, f, w - 2 * f, w - 2 * f));
                g.Flush();
                g.Dispose();

                return BitmapCursor.CreateBmpCursor(bmp);
            }
        }
    }
}

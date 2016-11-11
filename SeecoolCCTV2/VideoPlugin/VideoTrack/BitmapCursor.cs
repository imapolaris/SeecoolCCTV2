using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Interop;

namespace VideoNS.VideoTrack
{
    internal class BitmapCursor : SafeHandle
    {

        public override bool IsInvalid
        {
            get
            {
                return handle == (IntPtr)(-1);
            }
        }


        public static Cursor CreateBmpCursor(Bitmap cursorBitmap)
        {

            var c = new BitmapCursor(cursorBitmap);

            return CursorInteropHelper.Create(c);
        }



        protected BitmapCursor(Bitmap cursorBitmap)
            : base((IntPtr)(-1), true)
        {
            handle = cursorBitmap.GetHicon();
        }


        protected override bool ReleaseHandle()
        {
            bool result = DestroyIcon(handle);

            handle = (IntPtr)(-1);

            return result;
        }

        [DllImport("user32")]
        private static extern bool DestroyIcon(IntPtr hIcon);
    }
}

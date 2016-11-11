using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVModels
{
    public class ThumbnailInfo
    {
        public string VideoId;
        public DateTime Time;
        public byte[] ImageBytes;
        public bool IsDefault;

        public Image Thumbnail
        {
            get
            {
                using (MemoryStream ms = new MemoryStream(ImageBytes))
                    return new Bitmap(Image.FromStream(ms));
            }
        }
    }
}

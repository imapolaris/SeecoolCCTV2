using AopUtil.WpfBinding;
using Common.Command;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VideoNS.SubWindow
{
    internal class PreviewWinModel : ObservableObject
    {
        private Image _img;

        public PreviewWinModel()
        {
            SaveCommand = new DelegateCommand(_ => DoSave());
            ImageSize = "未知";
        }

        [AutoNotify]
        public ImageSource ImgSource { get; private set; }

        public void LoadImage(Image img)
        {
            if (img != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    img.Save(ms, ImageFormat.Png);
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.StreamSource = new MemoryStream(ms.ToArray());
                    bi.EndInit();
                    ImgSource = bi;
                    ImageSize = string.Format("{0}*{1}", img.Width, img.Height);
                }
            }
            _img = img;
        }

        [AutoNotify]
        public string ImageSize { get; private set; }
        [AutoNotify]
        public string VideoName { get; set; }   

        public ICommand SaveCommand{ get; set; }
        public event Action SaveAction;

        private void DoSave()
        {
            if (_img != null)
            {
                ImageSaver.SavedByHandle(_img, VideoName);
            }
            if (SaveAction != null)
                SaveAction();
        }
    }
}

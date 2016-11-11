using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using VideoNS.SubWindow;

namespace VideoNS
{
    public class ImageSaver
    {
        public static void SavedByHandle(Image img, string videoName)
        {
            if (img != null)
                saveImage(img, videoName);
            else
                DialogWin.Show("获取视频截图失败！");
        }

        private static void saveImage(System.Drawing.Image image, string videoName)
        {
            videoName = Regex.Replace(videoName, @"[~!@#$%\^&*()+=|{}':;',\\]", "_");
            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.Filter = @"jpeg|*.jpg";
            sfd.Title = "导出查询结果";
            sfd.FileName = videoName + "_" + DateTime.Now.ToString("yyyyMMddhhmmss");
            sfd.FilterIndex = 2;
            sfd.RestoreDirectory = true;
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                saveImageFile(image, sfd.FileName);
        }

        private static void saveImageFile(System.Drawing.Image image, string fileName)
        {
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                image.Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                image.Dispose();
                image = null;
            }
        }
    }
}

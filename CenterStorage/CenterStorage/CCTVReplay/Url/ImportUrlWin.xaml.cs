using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using CCTVReplay.Util;
using CenterStorageCmd;
using CenterStorageCmd.Url;

namespace CCTVReplay.Url
{
    /// <summary>
    /// ImportUrlWin.xaml 的交互逻辑
    /// </summary>
    public partial class ImportUrlWin : Window
    {
        public ImportUrlWin()
        {
            InitializeComponent();
            //txtUrl.Text = genTestUrl();//默认值
            txtUrl.Focus();
        }

        private string genTestUrl()
        {
            DateTime begin = new DateTime(2016, 5, 17, 14, 01, 0);
            DateTime end = new DateTime(2016, 5, 17, 15, 0, 0);
            List<VideoInfo> viList = new List<VideoInfo>();
            string videoId = "CCTV1_50BAD15900010302";
            int streamId = 0;
            viList.Add(new VideoInfo(videoId, streamId, "1101安迅士"));
            videoId = "CCTV1_50BAD15900010301";
            streamId = 0;
            viList.Add(new VideoInfo(videoId, streamId, "155球机"));
            //return new UrlInfo("192.168.13.22", 10000, begin, end, viList.ToArray()).ToString();
            return new RemoteUrl("192.168.13.22", 10000, begin, end, viList.ToArray(), @"D:\视酷下载\Time_201605171401_201605171500").ToString();
            //return new UrlInfo(@"D:\视酷下载\Time_201605171401_201605171500", begin, end, viList.ToArray()).ToString();
        }

        public IRemoteUrl ImportUrl { get; private set; }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IRemoteUrl ui = RemoteUrl.Parse(txtUrl.Text.Trim()) as IRemoteUrl;
                if (ui == null)
                {
                    DialogUtil.ShowWarning("不支持导入数据源格式。");
                    return;
                }
                ImportUrl = ui;
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                Common.Log.Logger.Default.Error(ex);
                DialogUtil.ShowError(ex.Message);
            }
        }

        private void headerBtnDownHandler(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}

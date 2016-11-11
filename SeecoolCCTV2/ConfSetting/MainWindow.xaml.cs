using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using Seecool.ShareMemory;

namespace ConfSetting
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string WebApiBaseUri = "WebApiBaseUri";
        private const string FullThumbnail = "FullThumbnail";
        private const string VideoBuffering = "VideoBuffering";
        private const string ShowFluent = "ShowFluent";
        private const string UserInfoApiUri = "UserInfoApiUri";

        private const string ConfFile = "Conf/base.conf";
        private const string UriInfo = "http://{0}:27010";

        private const string template = "<?xml version=\"1.0\"?>\n<Root>\n{0}\n</Root>";
        private Settings _baseConf;
        public MainWindow()
        {
            InitializeComponent();
            panelTop.MouseMove += PanelTop_MouseMove;
            _baseConf = loadSetttings();
            updateUI(_baseConf);
        }

        private void updateUI(Settings setting)
        {
            SettingItem si = setting.VideoInfoCollection.GetByKey(WebApiBaseUri);
            if (si == null)
                txtIp.Text = "127.0.0.1";
            else {
                Uri uri = new Uri(si.Value);
                txtIp.Text = uri.Host;
            }

            si = setting.VideoInfoCollection.GetByKey(FullThumbnail);
            if (si == null)
                ckbThumb.IsChecked = false;
            else
            {
                bool ischecked = false;
                bool.TryParse(si.Value, out ischecked);
                ckbThumb.IsChecked = ischecked;
            }

            si = setting.VideoInfoCollection.GetByKey(ShowFluent);
            if (si == null)
                ckbFluent.IsChecked = false;
            else
            {
                bool ischecked = false;
                bool.TryParse(si.Value, out ischecked);
                ckbFluent.IsChecked = ischecked;
            }
        }

        private Settings loadSetttings()
        {
            if (File.Exists(ConfFile))
            {
                string str = null;
                using (FileStream fs = new FileStream(ConfFile, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        str = sr.ReadToEnd();
                    }
                }
                if (str != null)
                {
                    string bConf = string.Format(template, str);
                    using (StringReader sr = new StringReader(bConf))
                    {
                        XmlSerializer cpSer = new XmlSerializer(typeof(Settings));
                        Settings temp = cpSer.Deserialize(sr) as Settings;
                        return temp;
                    }
                }
            }
            return new Settings();
        }

        private void saveSetting(Settings setting)
        {
            string ip = txtIp.Text.Trim();
            if (string.IsNullOrWhiteSpace(ip))
                ip = "127.0.0.1";
            string baseUri = string.Format(UriInfo, ip);
            string thumb = ckbThumb.IsChecked.ToString();
            string fluent = ckbFluent.IsChecked.ToString();
            ensurePath();
            using (FileStream fs = new FileStream(ConfFile, FileMode.OpenOrCreate, FileAccess.Write))
            {
                fs.SetLength(0);
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    SettingItem si = setting.VideoInfoCollection.GetByKey(WebApiBaseUri);
                    if (si != null)
                        si.Value = baseUri;
                    else
                        setting.VideoInfoCollection.Add(new SettingItem(WebApiBaseUri, baseUri));

                    si = setting.VideoInfoCollection.GetByKey(FullThumbnail);
                    if (si != null)
                        si.Value = thumb;
                    else
                        setting.VideoInfoCollection.Add(new SettingItem(FullThumbnail, thumb));

                    si = setting.VideoInfoCollection.GetByKey(ShowFluent);
                    if (si != null)
                        si.Value = fluent;
                    else
                        setting.VideoInfoCollection.Add(new SettingItem(ShowFluent, fluent));

                    si = setting.UserCollection.GetByKey(UserInfoApiUri);
                    if (si != null)
                        si.Value = baseUri;
                    else
                        setting.UserCollection.Add(new SettingItem(UserInfoApiUri, baseUri));

                    sw.Write(setting.ToString());
                }
            }
            recordSetting();
        }

        private void ensurePath()
        {
            FileInfo fi = new FileInfo(ConfFile);
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }
        }

        private void recordSetting()
        {
            string rFile = "cctvexe.dat";
            using (FileStream fs = new FileStream(rFile, FileMode.OpenOrCreate, FileAccess.Write))
            {
                fs.SetLength(0);
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine("SeecoolCCTV2.0 Setting Initialized！");
                }
            }
        }

        private void PanelTop_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            saveSetting(_baseConf);
            //判断当前已经启动的CCTV2.0客户端实例的个数。
            SharedDataBulk sdb = new SharedDataBulk("SeecoolCCTV_2_0_SharedMemory", 4);
            if (sdb.AllInstanceIds.Length > 1)
            {
                this.Hide();
                new TipWin().ShowDialog();
                sdb.Dispose();
                sdb = null;
            }
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

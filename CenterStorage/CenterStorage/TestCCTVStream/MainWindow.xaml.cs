using CenterStorageCmd;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestCCTVStream
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btPath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = tbPath.Text;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                tbPath.Text = dialog.SelectedPath;
            }
        }

        private void btOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string data = null;
                tbData.Text = null;
                string path = tbPath.Text;
                if (!new DirectoryInfo(path).Exists)
                    throw new InvalidDataException("无效的视频路径");
                var packets = FolderManager.GetIndexesPackets(path);
                foreach (var packet in packets)
                    writeLine(string.Format(packet.BeginTime.TimeOfDay + " - " + packet.EndTime.TimeOfDay + " " + packet.StartIndex), ref data);
                tbData.Text = data;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void writeLine(string str, ref string data)
        {
            Console.WriteLine(str);
            data += str + "\r\n";
            //tbData.Text += str + "\r\n";
        }

        private void btReadAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string data = null;
                tbData.Text = null;
                string path = tbPath.Text;
                if (!new DirectoryInfo(path).Exists)
                    throw new InvalidDataException("无效的视频路径");
                var packets = FolderManager.GetIndexesPackets(path);
                foreach (var packet in packets)
                {
                    writeLine(string.Format(packet.BeginTime.TimeOfDay + " - " + packet.EndTime.TimeOfDay + " " + packet.StartIndex), ref data);
                    var streams = FolderManager.GetVideoStreamsPacket(path, packet.BeginTime);
                    
                    foreach (var stream in streams.VideoStreams)
                    {
                        writeLine($"{stream.Time.TimeOfDay}\t{stream.Buffer.Length}\t{BitConverter.ToString(stream.Buffer.Take(20).ToArray())}", ref data);
                    }
                }
                tbData.Text = data;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
    }
}

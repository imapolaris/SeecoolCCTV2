using Seecool.VideoStreamBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCCTVStreamCmd
{
    class TestHikPlayer: IDisposable
    {
        CCTVStreamCmd.Hikvision.HikStream _hik;
        public TestHikPlayer(IntPtr hwnd)
        {
            _hik = new CCTVStreamCmd.Hikvision.HikStream("192.168.9.155", 8000, "admin", "admin12345", 1, false,hwnd);
            //_hik = new CCTVStreamCmd.Hikvision.HikStream("192.168.9.98", 8000, "admin", "12345", 2, false, hwnd);
            //_hik = new CCTVStreamCmd.Hikvision.HikStream("192.168.9.97", 8000, "admin", "12345", 3, false, hwnd);
            _hik.HeaderEvent += onHeader;
            _hik.StreamEvent += onStream;
        }

        private void onStream(IStreamPacket packet)
        {
            savetoTxt(packet.Buffer);
            //string rtspstr = $"hik packet {packet.FrameType} {packet.Buffer.Length}:";
            //for (int i = 0; i < Math.Min(100, packet.Buffer.Length); i++)
            //    rtspstr += string.Format("{0:X2}, ", packet.Buffer[i]);
            //Console.WriteLine(rtspstr);

            //if (packet.FrameType == CCTVFrameType.StreamFrame)
            //    Console.WriteLine($"{packet.FrameType}  ---- {packet.Buffer.Length}---{packet.Time.TimeOfDay}");
            //else
            //    Console.WriteLine($"{packet.FrameType}  ---- {packet.Buffer.Length}---{packet.Time.TimeOfDay}-------------------------------");
        }

        private void onHeader(IHeaderPacket packet)
        {
            HikHeaderPacket header = packet as HikHeaderPacket;
            if (header != null)
            {
                savetoTxt("Header: ");
                savetoTxt(header.Buffer);

                //Console.WriteLine();
                //string rtspstr = $"hik Header {header.Buffer.Length}:";
                //for (int i = 0; i < header.Buffer.Length; i++)
                //    rtspstr += string.Format("{0:X2}, ", header.Buffer[i]);
                //Console.WriteLine(rtspstr);
            }
        }

        FileStream _file;
        StreamWriter _sw;
        private void savetoTxt(byte[] buffer)
        {
            savetoTxt(buffer.Length.ToString());
            savetoTxt(BitConverter.ToString(buffer));
        }

        private void savetoTxt(string str)
        {
            if (_file == null)
            {
                _file = new FileStream(@"d:\hik.txt", FileMode.Create);
                _sw = new StreamWriter(_file);
            }
            _sw?.WriteLine(str);
            _sw?.Flush();
        }

        public void Dispose()
        {
            _sw?.Close();
            _sw = null;
            _file?.Close();
            //_file = null;
        }
    }
}

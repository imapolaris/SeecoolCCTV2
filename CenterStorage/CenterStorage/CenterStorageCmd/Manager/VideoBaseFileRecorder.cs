using CenterStorageCmd;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageCmd
{
    public class VideoBaseFileRecorder
    {
        object _lockObj = new object();
        string _fileName;
        public IDownloadInfo DownloadInfo { get; private set; }
        public VideoTimePeriodsPacket TimePeriods { get; private set; }
        public VideoBasePacket VideoBase { get; private set; }
        public VideoBaseFileRecorder(string path)
        {
            _fileName = Path.Combine(path, "base.base");
            Console.WriteLine(_fileName);
            read();
        }

        public void UpdateDownloadInfo(IDownloadInfo info)
        {
            DownloadInfo = info;
        }

        public void UpdateTimePeriods(VideoTimePeriodsPacket packet)
        {
            TimePeriods = packet;
        }

        public void UpdateVideoBase(VideoBasePacket packet)
        {
            VideoBase = packet;
            write();
        }

        private void write()
        {
            try
            {
                if (!new FileInfo(_fileName).Directory.Exists)
                    new FileInfo(_fileName).Directory.Create();
                using (FileStream fs = new FileStream(_fileName, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    write(fs, ParamCode.DownloadBase, DownloadInfoParam.Encode(DownloadInfo));
                    write(fs, ParamCode.VideoBaseInfo, VideoBasePacket.Encode(VideoBase));
                    write(fs, ParamCode.TimePeriods, VideoTimePeriodsPacket.Encode(TimePeriods));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        void write(FileStream fs, ParamCode code, byte[] buffer)
        {
            PacketBase.WriteBytes(fs, buffer.Length + 4);
            PacketBase.WriteBytes(fs, (int)code);
            PacketBase.WriteBytes(fs, buffer);
        }

        void read()
        {
            if (new FileInfo(_fileName).Exists)
            {
                using (FileStream fs = new FileStream(_fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    while (fs.Position != fs.Length)
                        readPacket(fs);
                }
            }
        }

        private void readPacket(Stream ms)
        {
            int length = PacketBase.ReadInt(ms);
            int code = PacketBase.ReadInt(ms);
            byte[] buffer = PacketBase.ReadByteArray(ms, length - 4);
            switch ((ParamCode)code)
            {
                case ParamCode.VideoBaseInfo:
                    if(buffer.Length > 20 && (VideoBase == null || VideoBase.Length == 0))
                        VideoBase = VideoBasePacket.Decode(buffer);
                    break;
                case ParamCode.TimePeriods:
                    TimePeriods = VideoTimePeriodsPacket.Decode(buffer);
                    break;
                case ParamCode.DownloadBase:
                    DownloadInfo = DownloadInfoParam.Decode(buffer);
                    break;
            }
        }
    }
}
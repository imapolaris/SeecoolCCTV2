using CCTVClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoNS.VideoDistribute;
using VideoStreamClient.Entity;

namespace VideoNS.VideoFlashback
{
    internal class FlashbackChannel : IDisposable
    {
        static TimeSpan _saveTime = TimeSpan.FromMinutes(10);
        string _dir;
        public FlashbackChannel(string videoId, string baseDir)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(videoId);
            string localPath = Convert.ToBase64String(bytes);
            localPath = localPath.Replace('/', '-');
            _dir = Path.Combine(baseDir, localPath);
            //Directory.CreateDirectory(_dir);
        }

        enum RecType { Ffmpeg, Hik, Uniview };
        RecType _recType = RecType.Ffmpeg;

        ConcurrentQueue<Record<HikM4Package>> _hikRecQueue = new ConcurrentQueue<Record<HikM4Package>>();
        ConcurrentQueue<Record<FfmpegPackage>> _ffmpegRecQueue = new ConcurrentQueue<Record<FfmpegPackage>>();
        ConcurrentQueue<Record<UniviewPackage>> _univiewRecQueue = new ConcurrentQueue<Record<UniviewPackage>>();

        byte[] _lastHeader;
        public void InputHikM4Package(HikM4Package package)
        {
            _recType = RecType.Hik;
            if (HikM4Decoder.HeaderType == package.Type)
                _lastHeader = package.Data;
            else
            {
                if (_lastHeader != null)
                {
                    Record<HikM4Package> record = new Record<HikM4Package>()
                    {
                        Time = DateTime.Now,
                        IsKey = package.Type == 1,
                        Package = package,
                        Header = _lastHeader,
                    };
                    _hikRecQueue.Enqueue(record);
                    clearTimeout();
                }
            }
        }

        public IFlashbackPlayer GetPlayer()
        {
            switch (_recType)
            {
                case RecType.Ffmpeg:
                    return new FfmpegFlashbackPlayer(_ffmpegRecQueue.ToArray());
                case RecType.Hik:
                    return new HikM4FlashbackPlayer(_hikRecQueue.ToArray());
                case RecType.Uniview:
                    return new UniviewFlashbackPlayer(_univiewRecQueue.ToArray());
                default:
                    return null;
            }
        }

        private void clearTimeout()
        {
            DateTime timeout = DateTime.Now - _saveTime;
            {
                Record<HikM4Package> record;
                while (_hikRecQueue.TryPeek(out record))
                {
                    if (record.Time < timeout)
                        _hikRecQueue.TryDequeue(out record);
                    else
                        break;
                }
            }
            {
                Record<FfmpegPackage> record;
                while (_ffmpegRecQueue.TryPeek(out record))
                {
                    if (record.Time < timeout)
                        _ffmpegRecQueue.TryDequeue(out record);
                    else
                        break;
                }
            }
            {
                Record<UniviewPackage> record;
                while (_univiewRecQueue.TryPeek(out record))
                {
                    if (record.Time < timeout)
                        _univiewRecQueue.TryDequeue(out record);
                    else
                        break;
                }
            }
        }

        public void InputFfmpegPackage(FfmpegPackage package)
        {
            _recType = RecType.Ffmpeg;
            Record<FfmpegPackage> record = new Record<FfmpegPackage>()
            {
                Time = DateTime.Now,
                IsKey = package.Type == 1 || package.Type == 2,
                Package = package,
            };
            _ffmpegRecQueue.Enqueue(record);
            clearTimeout();
        }

        public void InputUniviewPackage(UniviewPackage package)
        {
            _recType = RecType.Uniview;
            Record<UniviewPackage> record = new Record<UniviewPackage>()
            {
                Time = DateTime.Now,
                IsKey = false,
                Package = package,
            };
            _univiewRecQueue.Enqueue(record);
            clearTimeout();
        }

        public void Dispose()
        {
        }
    }
}

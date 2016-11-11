using CenterStorageCmd;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoStreamClient;
using VideoStreamClient.Entity;

namespace CenterStorageService
{
    public class VideoSourceCmd: IDisposable
    {
        public Action<DateTime, DataType, byte[]> VideoDisplayEvent;
        public Action StreamIdChanged;
        public int StreamId { get; private set; } = -1;
        public string StreamName { get; private set; }
        public string StreamUrl { get; private set; }
        HikHeaderPacket _hikPacket;
        FfmpegHeaderPacket _ffmpegPacket;
        VideoSource _source;
        StreamKeyDetector _keyDetector = new StreamKeyDetector();
        public VideoSourceCmd(VideoSource source)
        {
            _source = source;
            source.StreamTypeChanged += onStreamTypeChanged;
            source.FfmpegHeaderReceived += onFfmpegHeaderReceived;
            source.FfmpegStreamReceived += onFfmpegStreamReceived;
            source.Hikm4HeaderReceived += onHikm4HeaderReceived;
            source.Hikm4StreamReceived += onHikm4StreamReceived;
        }
        
        HikStreamCombine _hikStream = new HikStreamCombine();
        DateTime _lastKeyTime = DateTime.MinValue;
        private void onHikm4StreamReceived(object sender, VideoStreamClient.Events.HikM4StreamEventArgs e)
        {
            if (_hikPacket == null || _hikStream == null)
                return;
            var infos = _hikStream.Update(DateTime.Now, e.Package.Data);

            foreach(var packets in infos)
            {
                if (packets != null && packets.Length > 0)
                {
                    int length = packets.Sum(_ => _.Buffer.Length);
                    bool isKey = _keyDetector.Update(packets.First().Time, length);
                    if (isKey)
                        Console.WriteLine("Hikm4StreamReceived {0}  {1}  {2} - {3}", packets.First().Time.TimeOfDay, e.Package.Type, length, _source.VideoId);
                    DataType type = DataType.StreamData;
                    if (_keyDetector.Update(packets.First().Time, length))
                        type = DataType.StreamDataKey;

                    fireVideoDisplay(packets.First().Time, type, packets.First().Buffer);//合并视频流的第一个packet
                    for (int i = 1; i < packets.Length; i++)//合并视频流的其他packet
                        fireVideoDisplay(packets[i].Time, DataType.StreamData, packets[i].Buffer);
                }
            }
        }

        private void onHikm4HeaderReceived(object sender, VideoStreamClient.Events.HikM4HeaderEventArgs e)
        {
            updateHikHeader(e.Header.Type, e.Header.Data);
        }

        byte[] keyBuffer = null;
        private void onFfmpegStreamReceived(object sender, VideoStreamClient.Events.FfmpegStreamEventArgs e)
        {
            if (_ffmpegPacket == null)
                return;
            if (e.Package.Pts == 0)
                keyBuffer = e.Package.Data;
            DateTime timeNow = TimeCmd.GTMToTime(e.Package.Pts);
            bool isKey = _keyDetector.Update(timeNow, e.Package.Data.Length);
            if(isKey)
                Console.WriteLine("ffmpegStreamReceived key {0} {1} {2} Size({3},{4})", timeNow, e.Package.Type, e.Package.Data.Length, _ffmpegPacket.Width, _ffmpegPacket.Height);
            if (isKey && (timeNow - _lastKeyTime) > TimeSpan.FromSeconds(1))
            {
                if (keyBuffer != null)
                {
                    fireVideoDisplay(timeNow, DataType.StreamDataKey, keyBuffer);
                    fireVideoDisplay(timeNow, DataType.StreamData, e.Package.Data);
                }
                else
                    fireVideoDisplay(timeNow, DataType.StreamDataKey, e.Package.Data);
            }
            else
                fireVideoDisplay(timeNow, DataType.StreamData, e.Package.Data);
        }

        private void onFfmpegHeaderReceived(object sender, VideoStreamClient.Events.FfmpegHeaderEventArgs e)
        {
            updateFfmpegHeader(e.CodecId, e.Width, e.Height);
        }

        private void onStreamTypeChanged(object sender, VideoStreamClient.Events.StreamTypeEventArgs e)
        {
            if (StreamId == e.StreamIndex && StreamName == e.StreamName)
                return;
            Console.WriteLine("streamTypeChanged " + _source.VideoId);
            StreamId = e.StreamIndex;
            StreamName = e.StreamName;
            StreamUrl = "";
            if (_ffmpegPacket != null)
                updateFfmpegHeader(_ffmpegPacket.CodecID, _ffmpegPacket.Width, _ffmpegPacket.Height);
            if (_hikPacket != null)
                updateHikHeader(_hikPacket.Type, _hikPacket.Header);
            onStreamIdChanged();
        }

        private void updateHikHeader(int type, byte[] header)
        {
            if(StreamId != -1)
            {
                if (_hikPacket != null && _hikPacket.Type == type && bytesEquals(_hikPacket.Header, header))
                    return;
                //Console.WriteLine("onHikm4HeaderReceived " + _source.VideoId);
                _hikPacket = new HikHeaderPacket(StreamId, StreamName, StreamUrl, type, header);
                fireVideoDisplay(DateTime.Now, DataType.SysHead, HikHeaderPacket.Encode(_hikPacket));
            }
        }

        void updateFfmpegHeader(int codecID, int width, int height)
        {
            if (StreamId != -1)
            {
                if (_ffmpegPacket != null && _ffmpegPacket.CodecID == codecID && _ffmpegPacket.Width == width && _ffmpegPacket.Height == height)
                    return;
                Console.WriteLine("ffmpegHeaderReceived " + _source.VideoId);
                _ffmpegPacket = new FfmpegHeaderPacket(StreamId, StreamName, StreamUrl, codecID, width, height);
                fireVideoDisplay(DateTime.Now, DataType.SysHead, FfmpegHeaderPacket.Encode(_ffmpegPacket));
            }
        }

        private void fireVideoDisplay(DateTime time, DataType type, byte[] buffer)
        {
            if (type == DataType.SysHead)
            {
                _keyDetector = new StreamKeyDetector();
            }
            else if (type == DataType.StreamDataKey)
            {
                _lastKeyTime = DateTime.Now;
            }

            var handle = VideoDisplayEvent;
            if (handle != null)
                handle(time, type, buffer);
        }

        private void onStreamIdChanged()
        {
            var handle = StreamIdChanged;
            if (handle != null)
                handle();
        }
        private static bool bytesEquals(byte[] b1, byte[] b2)
        {
            if (b1 == null || b2 == null) return false;
            if (b1.Length != b2.Length) return false;
            for (int i = 0; i < b1.Length; i++)
                if (b1[i] != b2[i])
                    return false;
            return true;
        }

        public void Dispose()
        {
            _source.StreamTypeChanged -= onStreamTypeChanged;
            _source.FfmpegHeaderReceived -= onFfmpegHeaderReceived;
            _source.FfmpegStreamReceived -= onFfmpegStreamReceived;
            _source.Hikm4HeaderReceived -= onHikm4HeaderReceived;
            _source.Hikm4StreamReceived -= onHikm4StreamReceived;
            _source.Dispose();
        }
    }
}

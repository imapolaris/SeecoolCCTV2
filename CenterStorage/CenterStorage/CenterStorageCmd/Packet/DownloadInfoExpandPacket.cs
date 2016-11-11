using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageCmd
{
    public class DownloadInfoExpandPacket: IDownloadInfoExpand
    {
        public IDownloadInfo DownloadInfo {get; private set;}
        public Guid GuidCode { get; private set; }
        public string Name { get; private set; }
        public string Quality { get; private set; }
        public long Size { get; private set; }
        public bool IsLocalDownload { get; private set; }
        public DownloadStatus DownloadStatus { get; private set; }
        public TimePeriodPacket[] TimePeriodsAll { get; private set; }
        public TimePeriodPacket[] TimePeriodsCompleted { get; private set; }
        public string ErrorInfo { get; private set; }
        public DateTime UpdatedLastestTime { get; private set; }
        public long Speed { get; private set; }
        public DownloadInfoExpandPacket(Guid guid, IDownloadInfo info, string name, string quality, long size, bool isLocalDownload, TimePeriodPacket[] timePeriods, TimePeriodPacket[] timePeriodsCompleted, DownloadStatus status, string errorInfo, DateTime updatedLastestTime, long speed)
        {
            GuidCode = guid;
            DownloadInfo = info;
            Name = name;
            Quality = quality;
            Size = size;
            IsLocalDownload = isLocalDownload;
            TimePeriodsAll = timePeriods;
            TimePeriodsCompleted = timePeriodsCompleted;
            DownloadStatus = status;
            ErrorInfo = errorInfo;
            UpdatedLastestTime = updatedLastestTime;
            Speed = speed;
        }

        public static byte[] Encode(IDownloadInfoExpand packet)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                PacketBase.WriteBytes(ms, packet.GuidCode);
                PacketBase.WriteBytes(ms, DownloadInfoParam.Encode(packet.DownloadInfo));
                PacketBase.WriteBytes(ms, packet.Name);
                PacketBase.WriteBytes(ms, packet.Quality);
                PacketBase.WriteBytes(ms, packet.Size);
                PacketBase.WriteBytes(ms, packet.IsLocalDownload);
                PacketBase.WriteBytes(ms, TimePeriodPacket.EncodeArray(packet.TimePeriodsAll));
                PacketBase.WriteBytes(ms, TimePeriodPacket.EncodeArray(packet.TimePeriodsCompleted));
                PacketBase.WriteBytes(ms, (int)packet.DownloadStatus);
                PacketBase.WriteBytes(ms, packet.ErrorInfo);
                PacketBase.WriteBytes(ms, packet.UpdatedLastestTime);
                PacketBase.WriteBytes(ms, packet.Speed);
                return ms.ToArray();
            }
        }

        public static DownloadInfoExpandPacket Decode(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                return Decode(ms);
            }
        }

        public static DownloadInfoExpandPacket Decode(MemoryStream ms)
        {
            Guid guid = PacketBase.ReadGuid(ms);
            IDownloadInfo di = DownloadInfoParam.Decode(ms);
            string name = PacketBase.ReadString(ms);
            string quality = PacketBase.ReadString(ms);
            long size = PacketBase.ReadLong(ms);
            bool isLocalDownload = PacketBase.ReadBool(ms);
            TimePeriodPacket[] tps = TimePeriodPacket.DecodeArray(ms);
            TimePeriodPacket[] tpsc = TimePeriodPacket.DecodeArray(ms);
            DownloadStatus status = (DownloadStatus)PacketBase.ReadInt(ms);
            string errorInfo = PacketBase.ReadString(ms);
            DateTime time = PacketBase.ReadTime(ms);
            long speed = PacketBase.ReadLong(ms);
            return new DownloadInfoExpandPacket(guid, di, name, quality, size, isLocalDownload, tps, tpsc, status, errorInfo, time,speed);
        }

        public static byte[] EncodeArray(IDownloadInfoExpand[] packets)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                PacketBase.WriteBytes(ms, packets.Length);
                for (int i = 0; i < packets.Length; i++)
                {
                    PacketBase.WriteBytes(ms, DownloadInfoExpandPacket.Encode(packets[i]));
                }
                return ms.ToArray();
            }
        }

        public static DownloadInfoExpandPacket[] DecodeArray(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                return DecodeArray(ms);
            }
        }

        public static DownloadInfoExpandPacket[] DecodeArray(MemoryStream ms)
        {
            int length = PacketBase.ReadInt(ms);
            DownloadInfoExpandPacket[] packets = new DownloadInfoExpandPacket[length];
            for (int i = 0; i < packets.Length; i++)
            {
                packets[i] = DownloadInfoExpandPacket.Decode(ms);
            }
            return packets;
        }
    }
}
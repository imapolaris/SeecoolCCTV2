using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageCmd
{
    public static class DownloadInfoPartConverter
    {
        public static byte[] Encode(IDownloadInfoExpand expand, string obj)
        {
            byte[] buffer = null;
            switch (obj)
            {
                case nameof(expand.DownloadInfo):
                    buffer = getBytes(expand.GuidCode, DownloadCode.DownloadInfo, expand.DownloadInfo);
                    break;
                case nameof(expand.Name):
                    buffer = getBytes(expand.GuidCode, DownloadCode.Name, expand.Name);
                    break;
                case nameof(expand.Quality):
                    buffer = getBytes(expand.GuidCode, DownloadCode.Quality, expand.Quality);
                    break;
                case nameof(expand.Size):
                    buffer = getBytes(expand.GuidCode, DownloadCode.Size, expand.Size);
                    break;
                case nameof(expand.IsLocalDownload):
                    buffer = getBytes(expand.GuidCode, DownloadCode.IsLocalDownload, expand.IsLocalDownload);
                    break;
                case nameof(expand.DownloadStatus):
                    buffer = getBytes(expand.GuidCode, DownloadCode.Status, expand.DownloadStatus);
                    break;
                case nameof(expand.TimePeriodsAll):
                    buffer = getBytes(expand.GuidCode, DownloadCode.TimePeriodsAll, expand.TimePeriodsAll);
                    break;
                case nameof(expand.TimePeriodsCompleted):
                    buffer = getBytes(expand.GuidCode, DownloadCode.TimePeriodsCompleted, expand.TimePeriodsCompleted);
                    break;
                case nameof(expand.ErrorInfo):
                    buffer = getBytes(expand.GuidCode, DownloadCode.ErrorInfo, expand.ErrorInfo);
                    break;
                case nameof(expand.UpdatedLastestTime):
                    buffer = getBytes(expand.GuidCode, DownloadCode.UpdatedLastestTime, expand.UpdatedLastestTime);
                    break;
                case nameof(expand.Speed):
                    buffer = getBytes(expand.GuidCode, DownloadCode.Speed, expand.Speed);
                    break;
                case nameof(DownloadCode.GoTop):
                    buffer = getBytes(expand.GuidCode, DownloadCode.GoTop, null);
                    break;
                default:
                    buffer = getBytes(expand.GuidCode, 0, null);
                    break;
            }
            return buffer;
        }

        public static DownloadExpandPart Decode(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                return Decode(ms);
            }
        }

        public static DownloadExpandPart Decode(MemoryStream ms)
        {
            Guid guid = PacketBase.ReadGuid(ms);
            DownloadCode code = (DownloadCode)PacketBase.ReadInt(ms);
            Object obj = null;
            switch (code)
            {
                case DownloadCode.DownloadInfo:
                    obj = DownloadInfoParam.Decode(ms);
                    break;
                case DownloadCode.Name:
                case DownloadCode.Quality:
                case DownloadCode.ErrorInfo:
                    obj = PacketBase.ReadString(ms);
                    break;
                case DownloadCode.Size:
                case DownloadCode.Speed:
                    obj = PacketBase.ReadLong(ms);
                    break;
                case DownloadCode.IsLocalDownload:
                    obj = PacketBase.ReadBool(ms);
                    break;
                case DownloadCode.TimePeriodsAll:
                case DownloadCode.TimePeriodsCompleted:
                    obj = TimePeriodPacket.DecodeArray(ms);
                    break;
                case DownloadCode.Status:
                    obj = (DownloadStatus)PacketBase.ReadInt(ms);
                    break;
                case DownloadCode.UpdatedLastestTime:
                    obj = PacketBase.ReadTime(ms);
                    break;
                case DownloadCode.GoTop:
                    obj = null;
                    break;
            }
            return new DownloadExpandPart(guid, code, obj);
        }
        
        static byte[] getBytes(Guid guid, DownloadCode code, object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                PacketBase.WriteBytes(ms, guid);
                PacketBase.WriteBytes(ms, (int)code);
                switch (code)
                {
                    case DownloadCode.DownloadInfo:
                        PacketBase.WriteBytes(ms, DownloadInfoParam.Encode((DownloadInfoParam)obj));
                        break;
                    case DownloadCode.Name:
                    case DownloadCode.Quality:
                    case DownloadCode.ErrorInfo:
                        PacketBase.WriteBytes(ms, (string)obj);
                        break;
                    case DownloadCode.Size:
                    case DownloadCode.Speed:
                        PacketBase.WriteBytes(ms, (long)obj);
                        break;
                    case DownloadCode.IsLocalDownload:
                        PacketBase.WriteBytes(ms, (bool)obj);
                        break;
                    case DownloadCode.TimePeriodsAll:
                    case DownloadCode.TimePeriodsCompleted:
                        PacketBase.WriteBytes(ms, TimePeriodPacket.EncodeArray((TimePeriodPacket[])obj));
                        break;
                    case DownloadCode.Status:
                        PacketBase.WriteBytes(ms, (int)(DownloadStatus)obj);
                        break;
                    case DownloadCode.UpdatedLastestTime:
                        PacketBase.WriteBytes(ms, (DateTime)obj);
                        break;
                    case DownloadCode.GoTop:
                        break;
                }
                return ms.ToArray();
            }
        }
    }
}

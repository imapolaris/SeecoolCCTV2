using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageCmd
{
    public class IndexesPacket: TimePeriodPacket, ITimePeriod
    {
        public long StartIndex { get; private set; }
        public IndexesPacket(DateTime beginTime, DateTime endTime, long startIndex):base(beginTime, endTime)
        {
            StartIndex = startIndex;
        }

        public static byte[] Encode(IndexesPacket data)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                PacketBase.WriteBytes(ms, data.BeginTime);
                PacketBase.WriteBytes(ms, data.EndTime);
                PacketBase.WriteBytes(ms, data.StartIndex);
                return ms.ToArray();
            }
        }

        public static new IndexesPacket Decode(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                DateTime beginTime = PacketBase.ReadTime(ms);
                DateTime endTime = PacketBase.ReadTime(ms);
                long startIndex = PacketBase.ReadLong(ms);
                return new IndexesPacket(beginTime, endTime, startIndex);
            }
        }
    }
}
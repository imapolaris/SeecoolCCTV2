using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CenterStorageCmd
{
    public class TimePeriodPacket : IComparable<TimePeriodPacket>, ITimePeriod
    {
        public DateTime BeginTime { get; private set; }
        public DateTime EndTime { get; private set; }
        public TimePeriodPacket(DateTime start, DateTime end)
        {
            BeginTime = start;
            EndTime = end;
        }

        public bool IsInRange(DateTime time)
        {
            return BeginTime <= time && EndTime > time;
        }

        public int CompareTo(TimePeriodPacket other)
        {
            if (other == null)
                return 1;
            return BeginTime.CompareTo(other.BeginTime);
        }

        public static byte[] Encode(ITimePeriod data)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                PacketBase.WriteBytes(ms, data.BeginTime);
                PacketBase.WriteBytes(ms, data.EndTime);
                return ms.ToArray();
            }
        }

        public static TimePeriodPacket Decode(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                return Decode(ms);
            }
        }

        public static TimePeriodPacket Decode(Stream ms)
        {
            DateTime beginTime = PacketBase.ReadTime(ms);
            DateTime endTime = PacketBase.ReadTime(ms);
            return new TimePeriodPacket(beginTime, endTime);
        }

        public static byte[] EncodeArray(TimePeriodPacket[] packets)
        {
            if (packets == null)
                packets = new TimePeriodPacket[0];
            using (MemoryStream ms = new MemoryStream())
            {
                PacketBase.WriteBytes(ms, packets.Length);
                for (int i = 0; i < packets.Length; i++)
                {
                    PacketBase.WriteBytes(ms, TimePeriodPacket.Encode(packets[i]));
                }
                return ms.ToArray();
            }
        }

        public static TimePeriodPacket[] DecodeArray(Stream ms)
        {
            int tiLen = PacketBase.ReadInt(ms);
            TimePeriodPacket[] tis = new TimePeriodPacket[tiLen];
            for (int i = 0; i < tiLen; i++)
                tis[i] = TimePeriodPacket.Decode(ms);
            return tis;
        }
    }
}
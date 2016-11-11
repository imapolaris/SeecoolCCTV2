using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageCmd
{
    public class TimePeriodManager
    {
        /// <summary>
        /// 获取所有有效的时间段列表
        /// </summary>
        /// <param name="tis">所有项</param>
        /// <returns>有效的时间段列表</returns>
        public static TimePeriodPacket[] GetValidArray(TimePeriodPacket[] tis)
        {
            List<TimePeriodPacket> tiList = new List<TimePeriodPacket>();
            for (int i = 0; i < tis.Length; i++)
            {
                if (isValid(tis[i]))
                    tiList.Add(tis[i]);
            }
            return tiList.ToArray();
        }

        /// <summary>
        /// 排序并合并连续时间段
        /// </summary>
        /// <param name="tis">时间段列表</param>
        /// <returns>合并结果</returns>
        public static TimePeriodPacket[] Combine(TimePeriodPacket[] tis)
        {
            List<TimePeriodPacket> tiList = new List<TimePeriodPacket>();
            if (tis != null && tis.Length > 0)
            {
                tis = GetValidArray(tis);
                tis = tis.OrderBy(_ => _.BeginTime).ToArray();
                TimePeriodPacket tiCombin = new TimePeriodPacket(tis[0].BeginTime, tis[0].EndTime);
                for (int i = 1; i < tis.Length; i++)
                {
                    if (tiCombin.EndTime < tis[i].BeginTime)
                    {
                        tryAddTimePeriod(tiList, tiCombin);
                        tiCombin = tis[i];
                    }
                    else
                        tiCombin = new TimePeriodPacket(tiCombin.BeginTime, tis[i].EndTime);
                }
                tryAddTimePeriod(tiList, tiCombin);
            }
            return tiList.ToArray();
        }
        
        /// <summary>时间段序列中去除某时间段 </summary>
        public static TimePeriodPacket[] Subtracts(TimePeriodPacket[] tisAll, TimePeriodPacket ti)
        {
            List<TimePeriodPacket> tiList = new List<TimePeriodPacket>();
            if (tisAll != null && tisAll.Length > 0)
            {
                for (int i = 0; i < tisAll.Length; i++)
                    tiList.AddRange(Subtract(tisAll[i], ti));
            }
            return tiList.ToArray();
        }

        /// <summary>
        /// 获取所有某时间段的有效时间段列表
        /// </summary>
        /// <param name="tis">所有时间段列表</param>
        /// <param name="validTi">有效时间列表</param>
        /// <returns></returns>
        public static TimePeriodPacket[] GetIntersections(TimePeriodPacket[] tis, TimePeriodPacket validTi)
        {
            List<TimePeriodPacket> tiList = new List<TimePeriodPacket>();
            if (tis != null)
            {
                for (int i = 0; i < tis.Length; i++)
                {
                    var ti = Intersection(tis[i], validTi);
                    if (ti != null)
                        tiList.Add(ti);
                }
            }
            return tiList.ToArray();
        }
        /// <summary>
        /// 获取所有有效时间段内有效时间段列表
        /// </summary>
        /// <param name="tis">所有时间段列表</param>
        /// <param name="validTis">所有有效时间段列表</param>
        /// <returns></returns>
        public static TimePeriodPacket[] GetIntersections(TimePeriodPacket[] tis, TimePeriodPacket[] validTis)
        {
            List<TimePeriodPacket> validPackets = new List<TimePeriodPacket>();
            if (tis != null && validTis != null)
            {
                foreach (var valid in validTis)
                {
                    var pts = GetIntersections(tis, valid);
                    if (pts.Length > 0)
                        validPackets.AddRange(pts);
                }
            }
            return Combine(validPackets.ToArray());
        }

        public static TimePeriodPacket Intersection(TimePeriodPacket ti1, TimePeriodPacket ti2)
        {
            DateTime b = ti1.BeginTime > ti2.BeginTime ? ti1.BeginTime : ti2.BeginTime;
            DateTime e = ti1.EndTime < ti2.EndTime ? ti1.EndTime : ti2.EndTime;
            var ti = new TimePeriodPacket(b, e);
            if (isValid(ti))
                return ti;
            return null;
        }

        public static TimePeriodPacket[] Subtract(TimePeriodPacket ti1, TimePeriodPacket ti2)
        {
            List<TimePeriodPacket> tis = new List<TimePeriodPacket>();
            if (ti1.BeginTime >= ti2.EndTime || ti1.EndTime <= ti2.BeginTime)
            {
                tis.Add(ti1);
            }
            else if (ti1.BeginTime < ti2.BeginTime || ti1.EndTime > ti2.EndTime)
            {
                if (ti1.BeginTime < ti2.BeginTime)
                {
                    tis.Add(new TimePeriodPacket(ti1.BeginTime, ti2.BeginTime));
                }
                if (ti1.EndTime > ti2.EndTime)
                {
                    tis.Add(new TimePeriodPacket(ti2.EndTime, ti1.EndTime));
                }
            }
            return tis.ToArray();
        }

        public static bool IsValid(TimePeriodPacket ti, DateTime time)
        {
            return ti.BeginTime <= time && ti.EndTime > time;
        }

        private static void tryAddTimePeriod(List<TimePeriodPacket> tiList, TimePeriodPacket tiCombin)
        {
            //if(isValid(tiCombin))
            tiList.Add(tiCombin);
        }

        private static bool isValid(TimePeriodPacket ti)
        {
            return ti != null && ti.EndTime > ti.BeginTime;
        }
    }
}

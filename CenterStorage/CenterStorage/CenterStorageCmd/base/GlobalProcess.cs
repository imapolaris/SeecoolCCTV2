using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageCmd
{
    public static class GlobalProcess
    {
        public static string FolderPath(DateTime time)
        {
            return string.Format("{0:0000}\\{1:00}\\{2:00}", time.Year, time.Month, time.Day);
        }

        public static string FileNameFromTime(DateTime time)
        {
            return time.ToString("yyyyMMddHHmmssfff");
        }

        public static string TimeFormatOfCn(DateTime time)
        {
            return time.ToString("yyyy年MM月dd日HH时mm分ss秒");
        }

        public static string GetFolderName(ITimePeriod timePeriod)
        {
            return string.Format($"录像_{timePeriod.BeginTime.ToString("yyyyMMddHHmm")}_{timePeriod.EndTime.ToString("yyyyMMddHHmm")}");
        }

        public static string FileNameFromDate(DateTime time)
        {
            return time.ToString("yyyyMMdd");
        }

        public static string SimpleIndexesFormat()
        {
            return ".SimpleIndexes";
        }

        public static string IndexesFormat()
        {
            return ".Indexes";
        }

        public static string RecFormat()
        {
            return ".rec";
        }

        public static string GetIndexesFileName(string recFileName)
        {
            return recFileName.Substring(0, recFileName.Length - RecFormat().Length) + IndexesFormat();
        }

        public static string GetRecFileName(string indexesFileName)
        {
            return indexesFileName.Substring(0, indexesFileName.Length - IndexesFormat().Length) + RecFormat();
        }

        public static bool IsYear(string str)
        {
            if (str.Length == 4)
            {
                return getInt(str) >= 1;
            }
            return false;
        }

        public static bool IsMonth(string str)
        {
            if (str.Length == 2)
            {
                int data = getInt(str);
                return data >= 1 && data <= 12;
            }
            return false;
        }

        public static bool IsDay(string str)
        {
            if (str.Length == 2)
            {
                int data = getInt(str);
                return data >= 1 && data <= 31;
            }
            return false;
        }
        
        static int getInt(string str)
        {
            int data = -1;
            int.TryParse(str, out data);
            return data;
        }
    }
}

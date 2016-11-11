using AopUtil.WpfBinding;
using CenterStorageCmd;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace CCTVReplay.Video
{
    public class VideoDataInfoViewModel : ObservableObject
    {
        public VideoDataInfoViewModel()
        {
            //ValidRange = new Range2D[] { new Range2D(0, 0.2), new Range2D(0.3, 0.5), new Range2D(0.7, 1.0) };
            //LoadedRange = new Range2D[] { new Range2D(0, 0.1), new Range2D(0.3, 0.45), new Range2D(0.8, 1.0) };
        }

        public VideoDataInfoViewModel(DateTime begin, DateTime end)
        {
            UpdateTimePacket(begin, end);
        }

        [AutoNotify]
        public DateTime Begin { get; private set; }
        [AutoNotify]
        public DateTime End { get; private set; }
        [AutoNotify]
        public Range2D[] ValidRange { get; set; }
        [AutoNotify]
        public Range2D[] LoadedRange { get; set; }

        public void UpdateTimePacket(DateTime begin, DateTime end)
        {
            this.Begin = begin;
            this.End = end;
        }

        public void UpdateValidRange(TimePeriodPacket[] timePairs)
        {
            ValidRange = TimePairToRange(timePairs);
        }

        public void UpdateLoadedRange(TimePeriodPacket[] timePairs)
        {
            LoadedRange = TimePairToRange(timePairs);
        }

        private Range2D[] TimePairToRange(TimePeriodPacket[] pairs)
        {
            Array.Sort(pairs);
            TimeSpan span = End - Begin;
            double mss = span.TotalMilliseconds;
            Range2D[] ranges = new Range2D[pairs.Length];
            for (int i = 0; i < ranges.Length; i++)
            {
                double from = (pairs[i].BeginTime - Begin).TotalMilliseconds / mss;
                double to = (pairs[i].EndTime - Begin).TotalMilliseconds / mss;
                ranges[i] = new Range2D(from > 1 ? 1 : from, to > 1 ? 1 : to);
            }
            return ranges;
        }
    }

    internal class RangeArrayToGeometry : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Range2D[] ranges = value as Range2D[];
            if (ranges != null && ranges.Length > 0)
            {
                bool isVertical = parameter.ToString().ToUpper().Equals("V");
                GeometryGroup gg = new GeometryGroup();
                foreach (Range2D range in ranges)
                {
                    Point start = isVertical ? new Point(0, range.From) : new Point(range.From, 0);
                    Point end = isVertical ? new Point(0, range.To) : new Point(range.To, 0);
                    gg.Children.Add(new LineGeometry(start, end));
                }
                //加入头尾
                gg.Children.Insert(0, new LineGeometry(new Point(0, 0), new Point(0, 0)));
                if (isVertical)
                {
                    gg.Children.Add(new LineGeometry(new Point(0, 1), new Point(0, 1)));
                }
                else
                {
                    gg.Children.Add(new LineGeometry(new Point(1, 0), new Point(1, 0)));
                }
                return gg;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public struct Range2D
    {
        public double From { get; set; }
        public double To { get; set; }

        public Range2D(double from, double to)
        {
            this.From = from;
            this.To = to;
        }
    }
}

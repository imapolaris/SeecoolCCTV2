using CenterStorageCmd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVDownloadService
{
    public class DownloadProgressManager
    {
        public long TicksLength { get; private set; }
        public long TicksMissingLength { get; private set; } = 0;
        private TimePeriodPacket[] _missingTi = new TimePeriodPacket[0];
        public bool IsDownloaded { get { return _missingTi.Length == 0; } }
        public DateTime ProbeTime { get; private set; }
        public double Percent { get { return TicksLength - TicksMissingLength == 0 ? 0 : (TicksLength - TicksMissingLength) * 100.0 / TicksLength; } }

        List<TimePeriodPacket> _downloadedTimePeriods = new List<TimePeriodPacket>();
        public TimePeriodPacket[] DownloadedTimePeriods { get { return _downloadedTimePeriods.ToArray(); } }

        public DownloadProgressManager(TimePeriodPacket[] tisAll = null, TimePeriodPacket[] tisCompleted = null)
        {
            ProbeTime = DateTime.MaxValue;
            TicksLength = tisAll == null ? 0 : tisAll.Sum(_ => (_.EndTime - _.BeginTime).Ticks);
            if(tisCompleted != null)
                _downloadedTimePeriods.AddRange(tisCompleted);
            Console.WriteLine("TicksLength: {0}", TicksLength);
            _missingTi = getMissingTIs(tisAll, tisCompleted);
            updateProbeTime(DateTime.MinValue);
            updateTicksMissing();
        }

        public void Download(TimePeriodPacket tpp)
        {
            updateDownloaded(tpp);
            _missingTi = TimePeriodManager.Subtracts(_missingTi.ToArray(), tpp);
            updateProbeTime(ProbeTime);
            updateTicksMissing();
        }

        private void updateDownloaded(TimePeriodPacket tpp)
        {
            _downloadedTimePeriods.Add(tpp);
            _downloadedTimePeriods = TimePeriodManager.Combine(_downloadedTimePeriods.ToArray()).ToList();
        }

        public bool IsValidTime(TimePeriodPacket timePeriod)
        {
            return (_missingTi.Any(_ => TimePeriodManager.Intersection(_, timePeriod) != null));
        }

        public void UpdateProbeTime(DateTime probeTime)
        {
            updateProbeTime(probeTime);
        }

        private void updateTicksMissing()
        {
            if (TicksLength > 0)
                TicksMissingLength = _missingTi.Sum(_ => (_.EndTime - _.BeginTime).Ticks);
        }

        private void updateProbeTime(DateTime probeTime)
        {
            ProbeTime = TimeProbeManager.GetProbeTime(_missingTi, probeTime);
            if (!IsDownloaded && ProbeTime == DateTime.MaxValue)
            {
                ProbeTime = TimeProbeManager.GetProbeTime(_missingTi, DateTime.MinValue);
            }
        }

        private static TimePeriodPacket[] getMissingTIs(TimePeriodPacket[] tisAll, TimePeriodPacket[] tisCompleted)
        {
            if (tisAll == null || tisAll.Length == 0)
                return new TimePeriodPacket[0];
            TimePeriodPacket[] subs = TimePeriodManager.Combine(tisAll);
            if (tisCompleted != null && tisCompleted.Length > 0)
            {
                tisCompleted = TimePeriodManager.Combine(tisCompleted);
                for (int i = 0; i < tisCompleted.Length; i++)
                {
                    subs = TimePeriodManager.Subtracts(subs, tisCompleted[i]);
                }
            }
            return subs;
        }
    }
}

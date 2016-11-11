using AopUtil.WpfBinding;
using CCTVReplay.Util;
using Common.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CCTVReplay.Combo
{
    public class TimeSelectorViewModel : ObservableObject
    {
        public TimeSelectorViewModel()
        {
            //BeginTime = DateTime.Now - TimeSpan.FromHours(1);
            //EndTime = DateTime.Now;
            //BeginTime = ConstSettings.BeginTime;
            //EndTime = ConstSettings.EndTime;
        }

        [AutoNotify]
        public DateTime BeginTime { get; set; }
        [AutoNotify]
        public DateTime EndTime { get; set; }
    }
}

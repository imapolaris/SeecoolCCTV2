using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVReplay.Combo
{
    public static class VideoPlaySpeedManager
    {
        public static ObservableCollection<string> SpeedSources()
        {
            return new ObservableCollection<string>(new List<string>() { "8X", "4X", "2X", "正常", "1/2", "1/4", "1/8" });
        }

        public static double GetSpeed(string speedStr)
        {
            double speed = 1;
            switch (speedStr)
            {
                case "8X":
                    speed = 8;
                    break;
                case "4X":
                    speed = 4;
                    break;
                case "2X":
                    speed = 2;
                    break;
                case "正常":
                    speed = 1;
                    break;
                case "1/2":
                    speed = 0.5;
                    break;
                case "1/4":
                    speed = 0.25;
                    break;
                case "1/8":
                    speed = 0.125;
                    break;
            }
            return speed;
        }
    }
}

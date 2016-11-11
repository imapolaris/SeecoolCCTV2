using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCTVModels;

namespace Persistence
{
    public class TargetTrackPersistence : BasePersistence<CCTVTargetTrack>
    {
        public static TargetTrackPersistence Instance { get; private set; }
        static TargetTrackPersistence()
        {
            Instance = new TargetTrackPersistence();
        }

        private TargetTrackPersistence() : base("CCTVTargetTrack")
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCTVModels;

namespace Persistence
{
    public class VideoTrackPersistence : BasePersistence<CCTVVideoTrack>
    {
        public static VideoTrackPersistence Instance { get; private set; }
        static VideoTrackPersistence()
        {
            Instance = new VideoTrackPersistence();
        }

        private VideoTrackPersistence() : base("CCTVVideoTrack")
        {
        }
    }
}

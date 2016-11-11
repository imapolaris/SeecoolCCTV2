using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraControlPlugin
{
    public interface ICameraControl
    {
        void Control(CameraAction action, int actData);

        void TrackTarget(string targetUid, double longitude, double latitude, double altitude, double width);
        void StopTrack();
    }
}

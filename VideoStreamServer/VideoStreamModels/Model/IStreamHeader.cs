using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoStreamModels.Model
{
    public interface IStreamHeader : IByteSerializer
    {
        VideoDeviceType DeviceType { get; }
    }
}

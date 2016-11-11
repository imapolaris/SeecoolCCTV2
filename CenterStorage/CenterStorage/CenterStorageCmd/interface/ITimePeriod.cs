using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageCmd
{
    public interface ITimePeriod
    {
        DateTime BeginTime { get; }
        DateTime EndTime { get; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageCmd
{
    public interface IVideoInfo
    {
        string VideoId { get; }
        int StreamId { get; }
        string VideoName { get; }
    }
}

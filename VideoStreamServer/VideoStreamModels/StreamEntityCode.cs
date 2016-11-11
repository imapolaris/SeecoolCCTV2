using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoStreamModels
{
    public class StreamEntityCode
    {
        public const int VideoInfo = 10001;
        public const int StreamAddress = 10002;

        public const int HikvHeader = 11001;
        public const int FfmpegHeader = 11002;
        public const int StreamData = 11003;

        public const int RemoteError = 12001;
    }
}

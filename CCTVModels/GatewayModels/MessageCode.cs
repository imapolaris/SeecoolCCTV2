using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayModels
{
    public static class MessageCode
    {
        public const int EnsureConnect = 20000;
        public const int StartServer = 20001;
        public const int StopServer = 20002;
        public const int IsServerStarted = 20003;
        public const int StartRegister = 20004;
        public const int StopRegister = 20005;
        public const int IsSuperiorOnline = 20006;
        public const int IsLowerOnline = 20007;
        public const int ShareDevice = 20008;
        public const int QueryDevice = 20009;
        public const int RemoveLowerPlatform = 20010;

        public const int PlayInfo = 30001;
        public const int PlaySyncInfo = 30002;
        public const int PlayStop = 30003;
    }
}

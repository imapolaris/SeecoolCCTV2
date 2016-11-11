using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVInfoAdapter
{
    internal static class ConsoleMonitor
    {
        public static event Action Closed;

        delegate bool ConsoleEventDelegate(int eventType);
        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);

        static ConsoleEventDelegate handler;
        static ConsoleMonitor()
        {
            handler = new ConsoleEventDelegate(ConsoleEventCallback);
            SetConsoleCtrlHandler(handler, true);
        }

        static bool ConsoleEventCallback(int eventType)
        {
            //0: ctrl+c     2: close button
            if (eventType == 0 || eventType == 2)
            {
                if (Closed != null)
                    Closed();
            }
            return false;
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCenterStorageService
{
    public static class ExceptionManager
    {
        public static void CheckInvalidOperationException(Action act)
        {
            try
            {
                action(act);
            }
            catch (InvalidOperationException)
            { }
        }

        public static void CheckIOException(Action act)
        {
            try
            {
                action(act);
            }
            catch (IOException)
            { }
        }

        public static void CheckException(Action act)
        {
            try
            {
                action(act);
            }
            catch (Exception)
            { }
        }

        static void action(Action act)
        {
            act();
            Assert.Fail();
        }
    }
}

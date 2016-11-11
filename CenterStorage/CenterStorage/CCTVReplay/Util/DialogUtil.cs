using Common.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CCTVReplay.Util
{
    public static class DialogUtil
    {
        public static void ShowMessage(string msg)
        {
            DialogWin.Show(msg);
        }

        public static void ShowError(string err)
        {
            DialogWin.Show(err, DialogWinImage.Error);
        }

        public static void ShowWarning(string warning)
        {
            DialogWin.Show(warning, DialogWinImage.Warning);
        }
    }
}

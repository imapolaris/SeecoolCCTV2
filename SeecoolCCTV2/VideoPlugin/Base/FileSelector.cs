using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace VideoNS.Base
{
    public static  class FileSelector
    {
        public static string SelectOpenFile(string filter)
        {
            string[] names = SelOpenFiles(filter, false);
            if (names != null && names.Length > 0)
                return names[0];
            return null;
        }

        public static string[] SelectOpenFiles(string filter)
        {
            return SelOpenFiles(filter, true);
        }

        private static string[] SelOpenFiles(string filter,bool multi)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = filter;
            ofd.Multiselect = multi;
            ofd.RestoreDirectory = true;
            ofd.CheckFileExists = true;
            if (ofd.ShowDialog() == true)
            {
                return ofd.FileNames;
            }
            return null;
        }

        public static string SelectSaveFile(string filter)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = filter;
            sfd.RestoreDirectory = true;
            if (sfd.ShowDialog() == true)
            {
                return sfd.FileName;
            }
            return null;
        }
    }
}

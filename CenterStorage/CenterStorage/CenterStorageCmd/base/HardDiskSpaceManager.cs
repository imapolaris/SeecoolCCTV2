using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterStorageCmd
{
    public class HardDiskSpaceManager
    {
        ///   
        /// 获取指定驱动器的空间总大小(单位为B) 
        ///   
        ///  只需输入代表驱动器的字母即可 （大写） 
        ///    
        public static long GetHardDiskSpace(string str_HardDiskName)
        {
            long totalSize = new long();
            System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();
            foreach (System.IO.DriveInfo drive in drives)
            {
                if (drive.Name == str_HardDiskName)
                {
                    totalSize = drive.TotalSize;
                }
            }
            return totalSize;
        }
        ///   
        /// 获取指定驱动器的剩余空间总大小(单位为B) 
        ///   
        ///  只需输入代表驱动器的字母即可  
        ///    
        public static long GetHardDiskFreeSpace(string str_HardDiskName)
        {
            long freeSpace = new long();
            System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();
            foreach (System.IO.DriveInfo drive in drives)
            {
                if (drive.Name == str_HardDiskName)
                {
                    freeSpace = drive.TotalFreeSpace;
                }
            }
            return freeSpace;
        }
    }
}

using Common.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CenterStorageCmd
{
    public static class ConfigFile<T> where T : class
    {
        static public T FromFile(string fileName)
        {
            try
            {
                if (!System.IO.File.Exists(fileName))
                {
                    Logger.Default.Trace("未能找到文件：" + fileName);
                    return null;
                }
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(T));
                    return xs.Deserialize(fs) as T;
                }
            }
            catch (Exception ex)
            {
                Logger.Default.Error("读取异常：" + fileName + Environment.NewLine + ex.Message);
            }
            return null;
        }

        public static bool SaveToFile(string fileName, T config)
        {
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(T));
                    xs.Serialize(fs, config);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Default.Error("保存异常：" + fileName + Environment.NewLine + ex.Message);
            }

            return false;
        }
    }
}

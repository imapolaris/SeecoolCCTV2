using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.IO;
using Common.Util.Serialization;

namespace VideoNS.Json
{
    /// <summary>
    /// Json数据序列化/反序列化帮助类。
    /// </summary>
    public static class JsonParser
    {
        /// <summary>
        /// 将数据对象序列化为Json字符串，并保存到指定路径文件中。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="data">数据对象实例。</param>
        /// <param name="filePath">路径文件名。</param>
        public static void SerializeToFile<T>(T data, string filePath)
        {
            SerializeToFile<T>(data, filePath, false);
        }

        /// <summary>
        /// 将数据对象序列化为Json字符串，并保存到指定路径文件中。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="data">数据对象实例。</param>
        /// <param name="filePath">路径文件名。</param>
        /// <param name="autoOverride">是否自动覆盖重名文件。</param>
        public static void SerializeToFile<T>(T data, string filePath, bool autoOverride)
        {
            FileInfo fi = new FileInfo(filePath);
            if (fi.Exists && !autoOverride)
            {
                throw new IOException(string.Format("文件:{0}已存在，请指定其他文件名或设置自动覆盖标识为true。", filePath));
            }

            if (!fi.Directory.Exists)
                fi.Directory.Create();

            using (FileStream fs = new FileStream(fi.FullName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                fs.SetLength(0);
                new JsonSerializer().Serialize(fs, data);
            }
        }

        /// <summary>
        /// 从指定路径文件中读取Json字符串并反序列化为数据对象。
        /// </summary>
        /// <typeparam name="T">返回的结果对象类型。</typeparam>
        /// <param name="filePath">路径文件名。</param>
        /// <returns></returns>
        public static T DeserializeFromFile<T>(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);
            if (!fi.Exists)
            {
                throw new FileNotFoundException(string.Format("未找到指定文件:{0}", filePath));
            }

            using (FileStream fs = new FileStream(fi.FullName, FileMode.Open, FileAccess.Read))
            {
                return new JsonSerializer().Deserialize<T>(fs);
            }
        }
    }
}

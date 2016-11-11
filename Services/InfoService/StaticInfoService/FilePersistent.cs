using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StaticInfo.Util
{
    internal class FilePersistent
    {
        string _filePath;

        Encoding _encoding = Encoding.UTF8;

        public FilePersistent(string baseDir, string name)
        {
            string dir = Path.Combine(baseDir, Path.GetDirectoryName(name));
            string fileName = Path.GetFileName(name) + ".json";
            Directory.CreateDirectory(dir);
            _filePath = Path.Combine(dir, fileName);
        }

        public void Save<T>(T obj)
        {
            string json = JsonConvert.SerializeObject(obj, typeof(T), Formatting.Indented, null);
            File.WriteAllText(_filePath, json, _encoding);
        }

        public T Load<T>()
        {
            string json = null;
            try
            {
                json = File.ReadAllText(_filePath, _encoding);
            }
            catch
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}

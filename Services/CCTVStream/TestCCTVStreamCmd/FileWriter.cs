using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCCTVStreamCmd
{
    public class FileWriter : IDisposable
    {
        FileStream _file;
        StreamWriter _sw;
        public FileWriter(string fileName)
        {
            _file = new FileStream(fileName, FileMode.Create);
            _sw = new StreamWriter(_file);
        }

        public void SavetoTxt(byte[] buffer)
        {
            _sw?.Write(buffer.Length + "\t:");
            _sw?.WriteLine(BitConverter.ToString(buffer));
            _sw?.Flush();
        }

        public void Dispose()
        {
            _sw?.Close();
            _sw = null;
            _file?.Close();
            _file?.Dispose();
            _file = null;
        }
    }
}

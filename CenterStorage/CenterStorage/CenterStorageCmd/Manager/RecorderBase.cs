using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CenterStorageCmd
{
    public abstract class RecorderBase : IDisposable
    {
        protected string _filePath;
        private FileStream _fsStream = null;
        private FileStream _fsIndexes = null;
        private object _lockObj = new object();
        StreamPacket _curStream;
        protected string _curFolder;
        List<TimePeriodPacket> _dayIndexes = new List<TimePeriodPacket>();
        protected DateTime _fileStartTime = DateTime.MaxValue;
        byte[] _header = null;
        DateTime _keyStartTime = DateTime.MaxValue;
        long _keyStartPosition = -1;
        public RecorderBase(string path)
        {
            _filePath = path;
            _curStream = new StreamPacket(DateTime.MinValue, DataType.StopSign, new byte[0]);
        }

        public void Set(StreamPacket packet)
        {
            Set(packet.Time, packet.Type, packet.Buffer);
        }

        public void Set(DateTime time, DataType type, byte[] buffer)
        {
            lock (_lockObj)
            {
                _curStream = new StreamPacket(time, type, buffer);
                switch (type)
                {
                    case DataType.SysHead:
                        _header = buffer;
                        closeFile();
                        break;
                    case DataType.StopSign:
                        closeFile();
                        break;
                    case DataType.StreamDataKey:
                    case DataType.StreamData:
                        writeToFile();
                        break;
                }
            }
        }

        public void FinishPackage(DateTime time)
        {
            writeToIndexes(time);
        }

        protected abstract void updateFolderPath();
        protected abstract bool isCanCloseFilesFromTime(DateTime curTime);
        protected abstract void updateShortIndexes(TimePeriodPacket newTi);

        private void startFile()
        {
            _fileStartTime = _curStream.Time;
            string streamName = $"{GlobalProcess.FileNameFromTime(_fileStartTime)}{GlobalProcess.RecFormat()}";
            string indexesName = $"{GlobalProcess.FileNameFromTime(_fileStartTime)}{GlobalProcess.IndexesFormat()}";
            updateFolderPath();
            Directory.CreateDirectory(_curFolder);
            _fsStream = newFileStream(streamName);
            _fsIndexes = newFileStream(indexesName);
            writeToVideoStream(_fileStartTime, DataType.SysHead, _header);
        }

        private FileStream newFileStream(string fileName)
        {
            return new FileStream(Path.Combine(_curFolder, fileName), FileMode.Create, FileAccess.Write, FileShare.Read);
        }
        
        private void writeToFile()
        {
            if (isWriteFinished())
                closeFile();
            if (needCreateFile())
                startFile();
            if (_fsStream != null)
            {
                if (_curStream.Type == DataType.StreamDataKey)
                {
                    writeToIndexes(_curStream.Time);
                    _keyStartPosition = _fsStream.Length;
                    _keyStartTime = _curStream.Time;
                }
                writeToVideoStream(_curStream.Time, _curStream.Type, _curStream.Buffer);
            }
        }

        private bool isWriteFinished()
        {
            if (_fsStream == null || _keyStartPosition < 0 || _curStream.Time < _keyStartTime)
                return false;
            if (_curStream.Type == DataType.StreamDataKey)
                return isCanCloseFilesFromTime(_curStream.Time);
            return false;
        }

        private bool needCreateFile()
        {
            return _fsStream == null && _header != null && _curStream.Type == DataType.StreamDataKey;
        }

        private void closeFile()
        {
            writeToIndexes(_curStream.Time);
            if (_fsStream != null)
            {
                _fsStream.Close();
                _fsStream.Dispose();
                _fsStream = null;
            }
            if (_fsIndexes != null)
            {
                _fsIndexes.Close();
                _fsIndexes.Dispose();
                _fsIndexes = null;
                updateShortIndexes(new TimePeriodPacket(_fileStartTime, _curStream.Time));
            }
            _keyStartTime = DateTime.MaxValue;
            _keyStartPosition = -1;
        }
        
        private void writeToVideoStream(DateTime time, DataType type, byte[] buffer)
        {
            if (type != DataType.StopSign)
            {
                StreamPacket data = new StreamPacket(time, type, buffer);
                byte[] block = StreamPacket.Encode(data);
                writeBuffer(_fsStream, block);
            }
        }

        private void writeToIndexes(DateTime curTime)
        {
            if (_fsIndexes != null && _keyStartPosition >= 0 && curTime >= _keyStartTime)
            {
                IndexesPacket indexesData = new IndexesPacket(_keyStartTime, curTime, _keyStartPosition);
                byte[] bytes = IndexesPacket.Encode(indexesData);
                writeBuffer(_fsIndexes, bytes);
                _keyStartPosition = -1;
                _keyStartTime = DateTime.MaxValue;
                _fsStream.Flush();
                _fsIndexes.Flush();
            }
        }

        protected static void writeBuffer(FileStream fs, byte[] buffer)
        {
            UInt32 blockLen = (UInt32)buffer.Length + 4;
            fs.Write(BitConverter.GetBytes(blockLen), 0, sizeof(UInt32));
            fs.Write(buffer, 0, buffer.Length);
        }

        public void Dispose()
        {
            closeFile();
            _header = null;
        }
    }
}
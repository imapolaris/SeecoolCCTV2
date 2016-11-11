using Seecool.ShareMemory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VideoNS.Helper
{
    public class ProcessHelper
    {
        private static ProcessHelper _instance;
        public static ProcessHelper Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ProcessHelper();
                return _instance;
            }
        }

        SharedDataBulk _bulk;
        const int _maxInstaces = 100;
        const int _unitLength = 3;
        private readonly int _processIndex;
        public int ProcessIndex
        {
            get { return _processIndex; }
        }

        private ProcessHelper()
        {
            _bulk = new SharedDataBulk(AppConstants.SharedMemoryName, sizeof(int) * _unitLength * _maxInstaces);
            _processIndex = FindValidIndex();
            if (ProcessIndex <= 0)
            {
                throw new MaxInstanceException();
            }
            AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
        }

        private void CurrentDomain_DomainUnload(object sender, EventArgs e)
        {
            ReleaseDomain();
        }

        private int CurrentBulkId { get { return _bulk.CurrentId; } }

        public string FormatKey(string key)
        {
            if (ProcessIndex <= 0)
            {
                throw new MaxInstanceException();
            }
            return string.Format("{0}_{1}_{2}", key, Process.GetCurrentProcess().ProcessName, ProcessIndex);
        }

        private int FindValidIndex()
        {
            Dictionary<int, InstanceIndex> procIndex = GetRegisteredDomains();
            FilterValidProcesses(procIndex);
            //所有已使用的索引位。
            List<int> usedIndices = procIndex.Values.Select(ii => ii.Index).ToList();
            int rtnIndex = -1;
            for (int i = 1; i <= _maxInstaces; i++)
            {
                if (!usedIndices.Contains(i))
                {
                    rtnIndex = i;
                    break;
                }
            }

            if (rtnIndex > 0)
            {
                procIndex[CurrentBulkId] = new InstanceIndex(rtnIndex, Process.GetCurrentProcess().Id);
                //更新共享内存区。
                UpdateSharedMemory(procIndex);
            }
            return rtnIndex;
        }

        //释放当前AppDomain占用的索引位。
        private void ReleaseDomain()
        {
            Dictionary<int, InstanceIndex> procIndex = GetRegisteredDomains();
            FilterValidProcesses(procIndex);
            if (procIndex.ContainsKey(CurrentBulkId))
            {
                procIndex.Remove(CurrentBulkId);
            }
            UpdateSharedMemory(procIndex);
        }

        private int getIntFromBytes(byte[] data, ref int startIndex)
        {
            int value = BitConverter.ToInt32(data, startIndex);
            startIndex += 4;
            return value;
        }

        private void fillBytes(byte[] data, int value, ref int startIndex)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            for (int i = 0; i < bytes.Length; i++)
            {
                data[startIndex++] = bytes[i];
            }
        }

        private Dictionary<int, InstanceIndex> GetRegisteredDomains()
        {
            Dictionary<int, InstanceIndex> procIndex = new Dictionary<int, InstanceIndex>();
            byte[] data = _bulk.ReadSharedData();
            int pos = 0;
            for (int i = 0; i < _maxInstaces; i++)
            {
                int bulkId = getIntFromBytes(data, ref pos);
                if (bulkId <= 0)
                    break;
                int index = getIntFromBytes(data, ref pos);
                int processId = getIntFromBytes(data, ref pos);
                procIndex[bulkId] = new InstanceIndex(index, processId);
            }
            return procIndex;
        }

        private void UpdateSharedMemory(Dictionary<int, InstanceIndex> procIndex)
        {
            byte[] data = new byte[_bulk.Capacity];
            int startIndex = 0;
            foreach (int bulkId in procIndex.Keys)
            {

                fillBytes(data, bulkId, ref startIndex);
                fillBytes(data, procIndex[bulkId].Index, ref startIndex);
                fillBytes(data, procIndex[bulkId].ProcessID, ref startIndex);
            }
            _bulk.WriteSharedData(data);
        }

        private void FilterValidProcesses(Dictionary<int, InstanceIndex> procIndex)
        {
            List<int> diedIDs = new List<int>();
            Process[] pros = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);
            List<int> pIds = pros.Select(p => p.Id).ToList();
            foreach (int bulkId in procIndex.Keys)
            {
                if (!SharedDataBulk.IsInstanceAlive(AppConstants.SharedMemoryName, bulkId))
                    diedIDs.Add(bulkId);
                else if (!pIds.Contains(procIndex[bulkId].ProcessID))
                    diedIDs.Add(bulkId);
            }
            //清除已结束进程占用的索引位。
            foreach (int bulkId in diedIDs)
            {
                procIndex.Remove(bulkId);
            }
        }

        struct InstanceIndex
        {
            public int Index { get; set; }
            public int ProcessID { get; set; }

            public InstanceIndex(int idx, int pid)
            {
                Index = idx;
                ProcessID = pid;
            }
        }
    }

    public class MaxInstanceException : Exception
    {
        public MaxInstanceException() : base(
            "当前应用的启动实例数已达到上限，无法再启动新实例。")
        {

        }

    }
}

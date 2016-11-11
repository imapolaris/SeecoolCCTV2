using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StaticInfoClient;

namespace CCTVInfoHub.Entity
{
    /// <summary>
    /// 同步刷新回掉委托。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values"></param>
    /// <param name="updatedKeys"></param>
    public delegate void SyncUpdateHandler(IEnumerable<string> updatedKeys);

    public class SyncParams<T>
    {
        public SyncParams(string section, TimeSpan interval) : this(section, interval, null, null)
        {
        }

        public SyncParams(string section, TimeSpan interval, SyncUpdateHandler updateHandler) : this(section, interval, updateHandler, null)
        {
        }

        public SyncParams(string section, TimeSpan interval, SyncUpdateHandler updateHandler, string savePath)
        {
            _guid = null;
            this.Section = section;
            this.Interval = interval;
            this.SavePath = savePath;
            this.UpdateHandler = updateHandler;
        }

        private string _guid;
        internal string GUID
        {
            get
            {
                if (_guid == null)
                    _guid = Guid.NewGuid().ToString();
                return _guid;
            }
        }

        public string Section { get; set; }
        /// <summary>
        /// 自动刷新的时间间隔。
        /// 当时间间隔为<see cref="Timeout.InfiniteTimeSpan"/>时，禁止自动刷新功能。
        /// </summary>
        public TimeSpan Interval { get; set; }
        /// <summary>
        /// 缓存文件的保存路径。
        /// </summary>
        public string SavePath { get; set; }
        /// <summary>
        /// 刷新回掉方法。
        /// <para>如果不需要刷新回掉，将该参数设置为空值<c>null</c></para>
        /// </summary>
        public SyncUpdateHandler UpdateHandler { get; set; }

        private bool _hasUpdated = false;
        internal bool HasUpdated() { return _hasUpdated; }

        internal void OnUpdate(StaticInfoSynchronizer<T> synchronizer, IEnumerable<string> keysUpdated)
        {
            _hasUpdated = true;
            if (UpdateHandler != null)
            {
                UpdateHandler(keysUpdated);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCTVInfoHub.Entity;
using StaticInfoClient;

namespace CCTVInfoHub
{
    public class CCTVInfoSync : IDisposable
    {
        protected interface IUpdate
        {
            void Update();
            bool HasUpdated();
            SyncUpdateHandler UpdateHandler { get; set; }
        }

        protected class ParamHolder<T> : IDisposable, IUpdate
        {
            public StaticInfoSynchronizer<T> Sync { get; set; }
            public SyncParams<T> Param { get; set; }

            public SyncUpdateHandler UpdateHandler
            {
                get
                {
                    return Param?.UpdateHandler;
                }
                set
                {
                    if (Param != null)
                        Param.UpdateHandler = value;
                }
            }
            public void Dispose()
            {
                Sync.Dispose();
            }

            public void Update()
            {
                Sync.GetUpdate();
            }

            public bool HasUpdated()
            {
                return Param.HasUpdated();
            }
        }

        //*******"http://192.168.9.222:27010";
        protected readonly string _baseAddress;
        private Dictionary<Type, object> _dictSyncs;

        public string BaseAddress { get { return _baseAddress; } }

        /// <summary>
        /// 生成数据信息服务同步集成对象。
        /// </summary>
        /// <param name="baseAddress">基础服务地址如:"http://192.168.1.1:8000"</param>
        public CCTVInfoSync(string baseAddress)
        {
            _baseAddress = baseAddress;
            _dictSyncs = new Dictionary<Type, object>();
        }

        #region 【附加刷新回掉】
        public void AddUpdateHandler<T>(SyncUpdateHandler updateHandler)
        {
            Type t = typeof(T);
            if (_dictSyncs.ContainsKey(t) && updateHandler != null)
            {
                IUpdate iu = _dictSyncs[t] as IUpdate;
                iu.UpdateHandler += updateHandler;
            }
        }

        public void RemoveUpdateHandler<T>(SyncUpdateHandler updateHandler)
        {
            Type t = typeof(T);
            if (_dictSyncs.ContainsKey(t) && updateHandler != null)
            {
                IUpdate iu = _dictSyncs[t] as IUpdate;
                iu.UpdateHandler -= updateHandler;
            }
        }
        #endregion 【附加刷新回掉】

        #region 【注册刷新服务】
        public bool HasRegistered<T>()
        {
            return _dictSyncs.ContainsKey(typeof(T));
        }

        public void RegisterSynchronizer<T>(SyncParams<T> param)
        {
            Type t = typeof(T);
            if (_dictSyncs.ContainsKey(t))
            {
                ParamHolder<T> holder = _dictSyncs[t] as ParamHolder<T>;
                holder.Sync.Dispose();
            }
            StaticInfoSynchronizer<T> newSync = new StaticInfoSynchronizer<T>(_baseAddress, param.Section, param.Interval, param.SavePath, param.OnUpdate);
            _dictSyncs[t] = new ParamHolder<T>()
            {
                Sync = newSync,
                Param = param
            };
        }

        public void UnregisterSynchronizer<T>()
        {
            Type t = typeof(T);
            if (_dictSyncs.ContainsKey(t))
            {
                ParamHolder<T> holder = _dictSyncs[t] as ParamHolder<T>;
                holder.Sync.Dispose();
                _dictSyncs.Remove(t);
            }
        }
        #endregion 【注册刷新服务】

        #region 【主动刷新】
        /// <summary>
        /// 手动刷新用户自定义注册的信息。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void UpdateRegistered<T>()
        {
            Type t = typeof(T);
            if (_dictSyncs.ContainsKey(t))
            {
                ParamHolder<T> holder = _dictSyncs[t] as ParamHolder<T>;
                holder.Sync.GetUpdate();
            }
        }
        #endregion 【主动刷新】

        #region 【获取全部信息】
        public T[] GetAllRegisteredInfos<T>()
        {
            Type t = typeof(T);
            if (_dictSyncs.ContainsKey(t))
            {
                ParamHolder<T> holder = _dictSyncs[t] as ParamHolder<T>;
                if (!holder.HasUpdated())
                    holder.Update();
                return holder.Sync.Values.ToArray();
            }
            else
                throw new InvalidOperationException("尚未注册此类型的刷新服务:" + t);
        }
        #endregion 【获取全部信息】

        #region 【获取单视频信息】
        public T GetRegisteredInfo<T>(string key)
        {
            Type t = typeof(T);
            if (_dictSyncs.ContainsKey(t))
            {
                ParamHolder<T> holder = _dictSyncs[t] as ParamHolder<T>;
                if (!holder.HasUpdated())
                    holder.Update();
                T value = default(T);
                holder.Sync.TryGetValue(key, out value);
                return value;
            }
            else
                throw new InvalidOperationException("尚未注册此类型的刷新服务:" + t);
        }
        #endregion 【获取单视频信息】

        #region 【更新信息】
        public void PutRegisteredInfo<T>(string key, T info, bool isDeleted)
        {
            Type t = typeof(T);
            if (_dictSyncs.ContainsKey(t))
            {
                ParamHolder<T> holder = _dictSyncs[t] as ParamHolder<T>;
                holder.Sync.PutUpdate(new List<ObjectItem<T>>()
                {
                    new ObjectItem<T>() {
                        Key=key,
                        IsDeleted=isDeleted,
                        Item=info
                    }
                });
            }
            else
                throw new InvalidOperationException("尚未注册此类型的刷新服务:" + t);
        }
        #endregion 【更新信息】

        #region 【实现Dispose接口】
        protected bool Disposed
        {
            get;
            private set;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    foreach (Type key in _dictSyncs.Keys)
                    {
                        IDisposable dis = _dictSyncs[key] as IDisposable;
                        if (dis != null)
                            dis.Dispose();
                    }
                }
                _dictSyncs.Clear();
                _dictSyncs = null;
                Disposed = true;
            }
        }

        ~CCTVInfoSync()
        {
            Dispose(false);
        }
        #endregion 【实现Dispose接口】
    }
}

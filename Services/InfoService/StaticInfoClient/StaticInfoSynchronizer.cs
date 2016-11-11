using Newtonsoft.Json;
using StaticInfo.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace StaticInfoClient
{
    public class StaticInfoSynchronizer<T> : IDisposable
    {
        StaticInfoClient _client;
        string _section;
        Timer _updateTimer;
        TimeSpan _updateInterval;
        long _version = 0;
        FilePersistent _persistent;

        ConcurrentDictionary<string, T> _objectDict = new ConcurrentDictionary<string, T>();
        ConcurrentDictionary<string, string> _itemDict = new ConcurrentDictionary<string, string>();

        public IEnumerable<string> Keys { get { return _objectDict.Keys.ToArray(); } }
        public IEnumerable<T> Values { get { return _objectDict.Values.ToArray(); } }
        public IEnumerable<KeyValuePair<string, T>> KeyValuePairs { get { return _objectDict.ToArray(); } }

        public bool TryGetValue(string key, out T value)
        {
            return _objectDict.TryGetValue(key, out value);
        }

        public delegate void OnUpdate(StaticInfoSynchronizer<T> synchronizer, IEnumerable<string> updatedKeys);
        public event OnUpdate UpdateEvent;
        private void fireUpdateEvent(IEnumerable<string> keysUpdated)
        {
            var callback = UpdateEvent;
            if (callback != null)
                callback(this, keysUpdated);
        }

        public StaticInfoSynchronizer(string baseAddress, string section, TimeSpan updateInterval, string savePath = null, OnUpdate updateHandler = null)
        {
            if (updateHandler != null)
                UpdateEvent += updateHandler;
            _updateInterval = updateInterval;
            _section = section;
            _client = new StaticInfoClient(baseAddress, section);
            if (!string.IsNullOrWhiteSpace(savePath))
            {
                _persistent = new FilePersistent(savePath, section);
                IEnumerable<string> updatedKeys = load();
                if (updatedKeys != null && updatedKeys.Count() > 0)
                    fireUpdateEvent(updatedKeys);
            }

            if (_updateInterval != InfiniteSpan && _updateInterval != TimeSpan.Zero)
                _updateTimer = new Timer(onUpdateTimerTick, null, 10, Timeout.Infinite);
        }

        class SaveObject
        {
            public string Section; 
            public long Version;
            public KeyValuePair<string, string>[] Items;
        }

        IEnumerable<string> load()
        {
            if (_persistent != null)
            {
                SaveObject saveObject = _persistent.Load<SaveObject>();
                if (saveObject != null && saveObject.Section == _section)
                {
                    _version = saveObject.Version;
                    foreach (var item in saveObject.Items)
                    {
                        _itemDict[item.Key] = item.Value;
                        _objectDict[item.Key] = DeserializeItem(item.Value);
                    }
                    return _objectDict.Keys;
                }
            }
            return null;
        }

        void save()
        {
            if (_persistent != null)
            {
                SaveObject saveObject = new SaveObject()
                {
                    Section = _section,
                    Version = _version,
                    Items = _itemDict.ToArray(),
                };
                _persistent.Save(saveObject);
            }
        }

        bool _disposed = false;

        public void Dispose()
        {
            _disposed = true;
            if (_updateTimer != null)
                _updateTimer.Dispose();
            _client.Dispose();
        }

        public static TimeSpan InfiniteSpan = TimeSpan.FromMilliseconds(-1);

        private void onUpdateTimerTick(object state)
        {
            GetUpdate();
            if (!_disposed)
            {
                try
                {
                    _updateTimer.Change(_updateInterval, InfiniteSpan);
                }
                catch
                {
                }
            }
        }

        public static string SerializeItem(T item)
        {
            return JsonConvert.SerializeObject(item, typeof(T), null);
        }

        public static T DeserializeItem(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public void GetUpdate()
        {
            IEnumerable<string> updatedKeys = null;
            lock (_objectDict)
            {
                UpdateInfo update = getUpdate(_version);
                if (update != null)
                {
                    updatedKeys = updateData(update);
                    _version = update.Version;
                    save();
                }
            }

            if (updatedKeys != null && updatedKeys.Count() > 0)
                fireUpdateEvent(updatedKeys);
        }

        IEnumerable<string> updateData(UpdateInfo update)
        {
            IEnumerable<string> updatedKeys = update.Items.Select(x => x.Key);
            if (update.IsWhole)
            {
                updatedKeys = updatedKeys.Union(_objectDict.Keys);
                _objectDict.Clear();
                _itemDict.Clear();
            }

            foreach (var item in update.Items)
            {
                T dummy;
                string dummyString;
                if (item.IsDeleted)
                {
                    _itemDict.TryRemove(item.Key, out dummyString);
                    _objectDict.TryRemove(item.Key, out dummy);
                }
                else
                {
                    _itemDict[item.Key] = item.Info;
                    _objectDict[item.Key] = DeserializeItem(item.Info);
                }
            }
            return updatedKeys;
        }

        UpdateInfo getUpdate(long version)
        {
            try
            {
                return _client.GetUpdate(version);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public void PutUpdate(IEnumerable<ObjectItem<T>> items)
        {
            var infoItems = items.Select(x => new InfoItem()
            {
                Key = x.Key,
                IsDeleted = x.IsDeleted,
                Info = SerializeItem(x.Item),
            });
            _client.PutUpdate(infoItems);
            GetUpdate();
        }

        static string _defaultInfo = SerializeItem(default(T));

        public void ReplaceAll(IEnumerable<KeyValuePair<string, T>> newPairs)
        {
            if (newPairs == null)
                newPairs = Enumerable.Empty<KeyValuePair<string, T>>();

            GetUpdate();
            Dictionary<string, T> oldDict = KeyValuePairs.ToDictionary(x => x.Key, x => x.Value);

            IEnumerable<string> deleteKeys = oldDict.Keys.Except(newPairs.Select(x => x.Key));
            IEnumerable<InfoItem> deleteItems = deleteKeys.Select(x => new InfoItem()
            {
                Key = x,
                IsDeleted = true,
                Info = _defaultInfo,
            });

            IEnumerable<InfoItem> newItems = newPairs.Select(x => new InfoItem()
            {
                Key = x.Key,
                IsDeleted = false,
                Info = SerializeItem(x.Value),
            });
            IEnumerable<InfoItem> updateItems = newItems.Where(x => !oldDict.ContainsKey(x.Key) || x.Info != SerializeItem(oldDict[x.Key]));

            _client.PutUpdate(updateItems.Union(deleteItems));
            GetUpdate();
        }
    }
}

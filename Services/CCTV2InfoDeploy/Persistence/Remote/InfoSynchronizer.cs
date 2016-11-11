using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCTVModels;
using Newtonsoft.Json;

namespace Persistence.Remote
{
    public class InfoSynchronizer<T> : IDisposable
    {
        InfoServiceApiClient _client;
        string _section;
        long _version = 0;

        ConcurrentDictionary<string, T> _objectDict = new ConcurrentDictionary<string, T>();
        ConcurrentDictionary<string, string> _itemDict = new ConcurrentDictionary<string, string>();

        public IEnumerable<string> Keys { get { return _objectDict.Keys.ToArray(); } }
        public IEnumerable<T> Values { get { return _objectDict.Values.ToArray(); } }
        public IEnumerable<KeyValuePair<string, T>> KeyValuePairs { get { return _objectDict.ToArray(); } }

        public bool TryGetValue(string key, out T value)
        {
            return _objectDict.TryGetValue(key, out value);
        }

        public InfoSynchronizer(string baseAddress, string section)
        {
            _section = section;
            _client = new InfoServiceApiClient(baseAddress, section);
        }

        bool _disposed = false;
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _client.Dispose();
            }
        }

        public static TimeSpan InfiniteSpan = TimeSpan.FromMilliseconds(-1);

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
                }
            }
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

        //public void ReplaceAll(IEnumerable<KeyValuePair<string, T>> newPairs)
        //{
        //    if (newPairs == null)
        //        newPairs = Enumerable.Empty<KeyValuePair<string, T>>();

        //    GetUpdate();
        //    Dictionary<string, T> oldDict = KeyValuePairs.ToDictionary(x => x.Key, x => x.Value);

        //    IEnumerable<string> deleteKeys = oldDict.Keys.Except(newPairs.Select(x => x.Key));
        //    IEnumerable<InfoItem> deleteItems = deleteKeys.Select(x => new InfoItem()
        //    {
        //        Key = x,
        //        IsDeleted = true,
        //        Info = _defaultInfo,
        //    });

        //    IEnumerable<InfoItem> newItems = newPairs.Select(x => new InfoItem()
        //    {
        //        Key = x.Key,
        //        IsDeleted = false,
        //        Info = SerializeItem(x.Value),
        //    });
        //    IEnumerable<InfoItem> updateItems = newItems.Where(x => !oldDict.ContainsKey(x.Key) || x.Info != SerializeItem(oldDict[x.Key]));

        //    _client.PutUpdate(updateItems.Union(deleteItems));
        //    GetUpdate();
        //}
    }
}

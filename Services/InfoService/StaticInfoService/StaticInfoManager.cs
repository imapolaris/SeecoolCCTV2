using StaticInfo.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace StaticInfoService
{
    internal class StaticInfoManager
    {
        const string _baseDir = @"Save";

        public string Section;
        FilePersistent _persistent;

        public StaticInfoManager(string section)
        {
            Section = section;
            _persistent = new FilePersistent(_baseDir, Section);
            load();
        }

        void load()
        {
            IEnumerable<InfoItemWithVersion> items = null;
            lock (_persistent)
                items = _persistent.Load<IEnumerable<InfoItemWithVersion>>();

            if (items != null)
            {
                foreach (InfoItemWithVersion item in items)
                    _itemDict[item.Item.Key] = item;
                clearDeletedDataLongTimeAgo();
            }
        }

        void save()
        {
            lock (_persistent)
                _persistent.Save(_itemDict.Values);
        }

        struct InfoItemWithVersion
        {
            public long Version;
            public InfoItem Item;
        }

        ConcurrentDictionary<string, InfoItemWithVersion> _itemDict = new ConcurrentDictionary<string, InfoItemWithVersion>();
        long _minVersion = 1;

        public UpdateInfo GetUpdate(long version)
        {
            bool isWhole = version < _minVersion;
            IEnumerable<InfoItemWithVersion> itemsWithVersion = null;

            if (isWhole)
                itemsWithVersion = _itemDict.Values;
            else
                itemsWithVersion = from item in _itemDict.Values where item.Version > version select item;

            UpdateInfo ret = new UpdateInfo();
            ret.Section = this.Section;
            ret.IsWhole = isWhole;
            ret.Version = itemsWithVersion.Count() > 0 ? itemsWithVersion.Select(x => x.Version).Max() : version;
            ret.Items = itemsWithVersion.Select(x => x.Item);
            return ret;
        }

        public void PutUpdate(IEnumerable<InfoItem> items)
        {
            long version = timeToVersion(DateTime.Now);
            foreach (InfoItem item in items)
            {
                InfoItemWithVersion itemWithVersion = new InfoItemWithVersion() { Version = version, Item = item };
                lock (_itemDict)
                    _itemDict[item.Key] = itemWithVersion;
            }

            clearDeletedDataLongTimeAgo();
            save();
        }

        private static long timeToVersion(DateTime time)
        {
            return time.ToUniversalTime().Ticks;
        }

        private void clearDeletedDataLongTimeAgo()
        {
            long oldVersion = timeToVersion(DateTime.Now - TimeSpan.FromDays(2));
            if (_minVersion < oldVersion)
                clearDeletedDataBefore(timeToVersion(DateTime.Now - TimeSpan.FromDays(1)));
        }

        private void clearDeletedDataBefore(long version)
        {
            _minVersion = version;
            lock (_itemDict)
            {
                InfoItemWithVersion dummy;
                var pairs = _itemDict.ToArray();
                foreach (var pair in pairs)
                    if (pair.Value.Item.IsDeleted && pair.Value.Version < _minVersion)
                        _itemDict.TryRemove(pair.Key, out dummy);
            }
        }
    }
}

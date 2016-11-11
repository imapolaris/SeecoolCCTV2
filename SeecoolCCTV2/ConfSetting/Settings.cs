using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConfSetting
{
    [XmlRoot("Root")]
    public class Settings
    {
        public Settings()
        {
            VideoInfoCollection = new SettingItemCollection();
            UserCollection = new SettingItemCollection();
        }
        [XmlIgnore]
        public SettingItemCollection VideoInfoCollection { get; private set; }
        [XmlIgnore]
        public SettingItemCollection UserCollection { get; private set; }

        public string VideoInfoPlugin
        {
            get { return VideoInfoCollection.ToString(); }
            set
            {
                VideoInfoCollection.Clear();
                if (value != null)
                    parseValue(value, VideoInfoCollection);
            }
        }

        public string UserPlugin
        {
            get { return UserCollection.ToString(); }
            set
            {
                UserCollection.Clear();
                if (value != null)
                    parseValue(value, UserCollection);
            }
        }

        private void parseValue(string value, SettingItemCollection settings)
        {
            string[] lines = value.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines != null)
            {
                foreach (string line in lines)
                {
                    string temp = line;
                    bool isAnno = false;
                    if (temp.StartsWith("#"))
                    {
                        isAnno = true;
                        temp = temp.TrimStart('#');
                    }
                    string[] segs = temp.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (segs.Length == 2)
                    {
                        settings.Add(new SettingItem(segs[0], segs[1], isAnno));
                    }
                }
            }
        }

        public override string ToString()
        {
            return $"<VideoInfoPlugin>{VideoInfoPlugin}</VideoInfoPlugin>\n<UserPlugin>{UserPlugin}</UserPlugin>";
        }
    }

    public class SettingItemCollection
    {
        private Dictionary<string, SettingItem> _items = new Dictionary<string, SettingItem>();
        public SettingItemCollection()
        {

        }

        [XmlIgnore]
        public SettingItem[] Items
        {
            get { return _items.Values.ToArray(); }
            set
            {
                Clear();
                if (value != null)
                    foreach (SettingItem si in value)
                        Add(si);
            }
        }



        public void Add(SettingItem item)
        {
            if (item != null)
                _items[item.Id] = item;
        }

        public void Remove(SettingItem item)
        {
            if (item != null)
                _items.Remove(item.Id);
        }

        public SettingItem GetByKey(string key)
        {
            foreach(string id in _items.Keys)
            {
                if (_items[id].Key == key && !_items[id].IsAnnotation)
                    return _items[id];
            }
            return null;
        }

        public bool Contains(string id)
        {
            return _items.ContainsKey(id);
        }

        public void Remove(string id)
        {
            if (id != null)
                _items.Remove(id);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string key in _items.Keys)
            {
                sb.AppendLine(_items[key].ToString());
            }
            if (sb.Length > 0)
            {
                sb.Insert(0, "\n");
            }
            return sb.ToString();
        }
    }

    public class SettingItem
    {
        public SettingItem()
        {
            Id = Guid.NewGuid().ToString();
        }
        public SettingItem(string key, string value) : this()
        {
            this.Key = key;
            this.Value = value;
        }

        public SettingItem(string key, string value, bool isAnno) : this(key, value)
        {
            IsAnnotation = isAnno;
        }

        public string Id { get; private set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public bool IsAnnotation { get; set; } = false;
        public override string ToString()
        {
            string str = $"\t{Key}\t{Value}";
            if (IsAnnotation)
                str = "#" + str;
            return str;
        }
    }
}

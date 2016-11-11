using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using GBTModels.Global;
using GBTModels.Query;

namespace GBTModels.Response
{
    [XmlRoot("Response")]
    public class DeviceCatalogResp : AbstractDeviceObject
    {
        public override string CmdType
        {
            get
            {
                return GBTCommandTypes.CataLog;
            }
            set
            {
            }
        }

        public int SumNum
        {
            get
            {
                return Items == null ? 0 : Items.Count;
            }
            set { }
        }

        private DeviceItemsCollection _items;
        [XmlElement("DeviceList")]
        public DeviceItemsCollection Items
        {
            get
            {
                if (_items == null)
                    _items = new DeviceItemsCollection();
                return _items;
            }
            set
            {
                _items = value;
            }
        }
    }

    public class DeviceItemsCollection
    {
        public DeviceItemsCollection()
        {
            Items = new List<ItemType>();
        }

        [XmlAttribute("Num")]
        public int Count
        {
            get { return Items.Count; }
            set { }
        }

        [XmlElement("Item")]
        public List<ItemType> Items { get; set; }

        public void Add(ItemType item)
        {
            Items.Add(item);
        }

        public void Remove(ItemType item)
        {
            Items.Remove(item);
        }
    }
}

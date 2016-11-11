using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using GBTModels.Global;
using GBTModels.Response;

namespace GBTModels.Notify
{
    [XmlRoot("Notify")]
    public class DeviceCatalogNotify : AbstractDeviceObject
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
}

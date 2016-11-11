using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoNS.SubWindow;

namespace VideoNS.AutoSave
{
    public class ShortcutScheme
    {
        public static ShortcutScheme Instance { get; private set; }
        static ShortcutScheme()
        {
            Instance = new ShortcutScheme();
        }

        private ShortcutScheme()
        {

        }

        private ShortcutItem[] _sItems = null;
        public ShortcutItem[] Scheme
        {
            get
            {
                if (_sItems == null)
                    loadData();
                return _sItems;
            }
            private set
            {
                _sItems = value;
            }
        }

        private void loadData()
        {
            var temp = ShortcutAutoSave.LoadData();
            if (temp == null)
                temp = new ShortcutItem[0];
            Scheme = temp;
        }

        public void UpdateShortcuts(IEnumerable<ShortcutItem> sItems)
        {
            if (sItems == null)
                sItems = new ShortcutItem[0];
            ShortcutAutoSave.SaveData(sItems.ToList());
            Scheme = null;
            OnDataChanged();
        }

        #region 【事件定义】
        public event EventHandler DataChanged;

        private void OnDataChanged()
        {
            var handler = DataChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
        #endregion 【事件定义】
    }
}

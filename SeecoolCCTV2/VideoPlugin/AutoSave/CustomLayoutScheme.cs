using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoNS.Model;

namespace VideoNS.AutoSave
{
    internal class CustomLayoutScheme
    {
        private static CustomLayoutScheme _instance;

        public static CustomLayoutScheme Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CustomLayoutScheme();
                return _instance;
            }
        }

        private List<CustomLayout> _layouts;
        private const string KeyPrefix = "自定义{0}";
        private CustomLayoutScheme()
        {
        }

        private List<CustomLayout> LayoutsList
        {
            get
            {
                if (_layouts == null)
                {
                    _layouts = CustomLayoutAutoSave.LoadData();
                    if (_layouts == null)
                        _layouts = new List<CustomLayout>();
                }
                return _layouts;
            }
        }

        public CustomLayout[] Layouts
        {
            get
            {
                return LayoutsList.ToArray();
            }
        }

        public void Add(CustomLayout info)
        {
            if (info != null)
            {
                LayoutsList.Add(info);
                Save();
            }
        }

        public void Remove(CustomLayout info)
        {
            if (info != null)
            {
                LayoutsList.Remove(info);
                Save();
            }
        }

        public string FindValidName()
        {
            int index = 1;
            while (true)
            {
                string keyName = string.Format(KeyPrefix, index++);
                bool flag = false;
                for(int i = 0; i < LayoutsList.Count; i++)
                {
                    if(keyName.Equals(LayoutsList[i].LayoutName))
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                    return keyName;
            }
        }

        public bool IsValidName(string name)
        {
            foreach(CustomLayout info in LayoutsList)
            {
                if (info.LayoutName.Equals(name))
                    return false;
            }
            return true;
        }

        private void Save()
        {
            CustomLayoutAutoSave.SaveData(LayoutsList);
        }
    }
}

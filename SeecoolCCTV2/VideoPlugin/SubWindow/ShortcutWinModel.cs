using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AopUtil.WpfBinding;
using Common.Command;
using Newtonsoft.Json;
using VideoNS.AutoSave;
using VideoNS.Base;
using VideoNS.Helper;

namespace VideoNS.SubWindow
{
    public class ShortcutWinModel : ObservableObject
    {
        private Dictionary<string, ShortcutItem> _dictShortcuts;
        public ShortcutWinModel()
        {
            _dictShortcuts = new Dictionary<string, ShortcutItem>();
            loadShortcuts();
            CurrentEdit = new ShortcutItem();

            SelectFileCmd = new DelegateCommand(x => doSelectFile());
            SaveCmd = new DelegateCommand(x => doSave());
            DeleteCmd = new DelegateCommand(x => doDelete());
            ClearCmd = new DelegateCommand(x => doClear());
            CloseCmd = new DelegateCommand(x => doClose());

            this.PropertyChanged += this_PropertyChanged;
        }

        private void this_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(CurrentSelected):
                    if (CurrentSelected != null)
                        CurrentEdit = new ShortcutItem(CurrentSelected);
                    break;
            }
        }

        private void loadShortcuts()
        {
            _dictShortcuts.Clear();
            ShortcutItem[] sis = ShortcutScheme.Instance.Scheme;
            foreach (ShortcutItem si in sis)
                _dictShortcuts[si.ShortcutString] = si;
            Shortcuts = new ObservableCollection<ShortcutItem>(_dictShortcuts.Values);
        }

        [AutoNotify]
        public ShortcutItem CurrentSelected { get; set; }
        [AutoNotify]
        public ShortcutItem CurrentEdit { get; private set; }
        [AutoNotify]
        public ObservableCollection<ShortcutItem> Shortcuts { get; private set; }
        [AutoNotify]
        public ICommand SelectFileCmd { get; private set; }
        [AutoNotify]
        public ICommand SaveCmd { get; private set; }
        [AutoNotify]
        public ICommand DeleteCmd { get; private set; }
        [AutoNotify]
        public ICommand ClearCmd { get; private set; }
        [AutoNotify]
        public ICommand CloseCmd { get; private set; }
        public event Action Closed;


        private void doSelectFile()
        {
            string ext = DirectoryHelper.LayoutFileExt;
            string file = FileSelector.SelectOpenFile($"布局文件(*.{ext})|*.{ext}");
            if (!string.IsNullOrWhiteSpace(file))
            {
                CurrentEdit.FileName = file;
            }
        }
        private void doSave()
        {
            if (string.IsNullOrWhiteSpace(CurrentEdit.FileName))
            {
                DialogWin.Show("没有关联布局文件。", DialogWinImage.Warning);
                return;
            }
            if (!CurrentEdit.IsShortcutValid)
            {
                DialogWin.Show("无效的快捷键。", DialogWinImage.Warning);
                return;
            }
            if (_dictShortcuts.ContainsKey(CurrentEdit.ShortcutString))
            {
                if (!(bool)DialogWin.Show("已存在相同的快捷键设置，是否要覆盖旧的快捷键?", "覆盖确认", true, DialogWinImage.None))
                    return;
            }

            //判断快捷键是否可用。
            try
            {
                KeyGesture kg = new KeyGesture(CurrentEdit.Key, CurrentEdit.Modifiers);
            }
            catch (NotSupportedException)
            {
                DialogWin.Show($"不支持当前快捷键组合:{CurrentEdit.ShortcutString}", DialogWinImage.Error);
                return;
            }

            _dictShortcuts[CurrentEdit.ShortcutString] = CurrentEdit;
            Shortcuts = new ObservableCollection<ShortcutItem>(_dictShortcuts.Values);
            CurrentEdit = new ShortcutItem();
        }
        private void doDelete()
        {
            if (CurrentSelected != null)
            {
                _dictShortcuts.Remove(CurrentSelected.ShortcutString);
                Shortcuts = new ObservableCollection<ShortcutItem>(_dictShortcuts.Values);
            }
        }
        private void doClear()
        {
            if ((bool)DialogWin.Show("确定清除所有快捷键？", DialogWinImage.Information))
            {
                _dictShortcuts.Clear();
                Shortcuts = new ObservableCollection<ShortcutItem>(_dictShortcuts.Values);
            }
        }

        private void doClose()
        {
            ShortcutScheme.Instance.UpdateShortcuts(_dictShortcuts.Values);
            //激发事件。
            if (Closed != null)
                Closed();
        }
    }

    public class ShortcutItem : ObservableObject
    {
        public ShortcutItem()
        {
            this.PropertyChanged += ShortcutItem_PropertyChanged;
        }

        public ShortcutItem(ShortcutItem si) : this()
        {
            if (si != null)
            {
                Name = si.Name;
                FileName = si.FileName;
                Key = si.Key;
                Modifiers = si.Modifiers;
            }
        }

        private void ShortcutItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Name):
                    buildDisStr();
                    break;
                case nameof(Key):
                case nameof(Modifiers):
                    buildShortcutStr();
                    buildDisStr();
                    break;
            }
        }



        private void buildDisStr()
        {
            string name = Name;
            if (string.IsNullOrWhiteSpace(name))
            {
                name = "未命名";
            }
            string scStr = ShortcutString;
            if (string.IsNullOrWhiteSpace(scStr))
            {
                scStr = "无效的快捷方式";
            }
            DisplayString = $"{name}({scStr})";
        }

        private void buildShortcutStr()
        {
            if (Key != Key.None)
            {
                if ((Key < Key.F1 || Key > Key.F12) && Modifiers == ModifierKeys.None)
                {
                    ShortcutString = "";
                }
                else {
                    ModifierKeys mk = ModifierKeys.None;
                    StringBuilder sb = new StringBuilder();
                    mk |= findModifier(Modifiers, ModifierKeys.Control, sb, "Ctrl");
                    mk |= findModifier(Modifiers, ModifierKeys.Shift, sb, "Shift");
                    mk |= findModifier(Modifiers, ModifierKeys.Alt, sb, "Alt");
                    sb.Append(Key);
                    ShortcutString = sb.ToString();
                }
            }
            else
                ShortcutString = "";
        }

        private ModifierKeys findModifier(ModifierKeys modifiers, ModifierKeys mk, StringBuilder disStr, string keyStr)
        {
            if ((modifiers & mk) == mk)
            {
                disStr.Append(keyStr).Append("+");
                return mk;
            }
            return ModifierKeys.None;
        }

        [AutoNotify]
        public string Name { get; set; }
        [AutoNotify]
        public string FileName { get; set; }
        [AutoNotify]
        public Key Key { get; set; } = Key.None;
        [AutoNotify]
        public ModifierKeys Modifiers { get; set; } = ModifierKeys.None;
        [AutoNotify]
        [JsonIgnore]
        public string ShortcutString { get; private set; }
        [AutoNotify]
        [JsonIgnore]
        public string DisplayString { get; private set; }
        [JsonIgnore]
        public bool IsShortcutValid
        {
            get
            {
                return !(Key == Key.None || ((Key < Key.F1 || Key > Key.F12) && Modifiers == ModifierKeys.None));
            }
        }
    }
}

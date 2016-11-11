using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VideoNS.SubWindow
{
    /// <summary>
    /// ShortcutWin.xaml 的交互逻辑
    /// </summary>
    public partial class ShortcutWin : Window
    {
        internal ShortcutWinModel ViewModel { get { return DataContext as ShortcutWinModel; } }

        public ShortcutWin()
        {
            InitializeComponent();
            panelTop.MouseMove += PanelTop_MouseMove;
            ViewModel.Closed += ViewModel_Closed;
            txtShortcut.PreviewKeyDown += txtShortcut_PreviewKeyDown;
        }

        private void txtShortcut_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!e.IsRepeat)
            {
                if (!isModifier(e.Key, e.SystemKey))
                {
                    ModifierKeys mk = ModifierKeys.None;
                    mk |= findModifier(ModifierKeys.Control);
                    mk |= findModifier(ModifierKeys.Shift);
                    mk |= findModifier(ModifierKeys.Alt);
                    ViewModel.CurrentEdit.Key = e.Key == Key.System ? e.SystemKey : e.Key;
                    ViewModel.CurrentEdit.Modifiers = mk;
                }
            }
            e.Handled = true;
        }

        private void ViewModel_Closed()
        {
            this.Close();
        }

        private void PanelTop_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        private ModifierKeys findModifier(ModifierKeys mk)
        {
            if ((Keyboard.Modifiers & mk) == mk)
            {
                return mk;
            }
            return ModifierKeys.None;
        }

        private bool isModifier(Key key, Key sysKey)
        {
            return key == Key.LeftCtrl || key == Key.RightCtrl
                || key == Key.LeftShift || key == Key.RightShift
                || key == Key.LeftAlt || key == Key.RightAlt
                || (key == Key.System && (sysKey == Key.LeftAlt || sysKey == Key.RightAlt));
        }

    }
}

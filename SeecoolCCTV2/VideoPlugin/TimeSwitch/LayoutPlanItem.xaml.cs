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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VideoNS.TimeSwitch
{
    /// <summary>
    /// LayoutPlanItem.xaml 的交互逻辑
    /// </summary>
    public partial class LayoutPlanItem : UserControl
    {
        public LayoutPlanItem()
        {
            InitializeComponent();
            this.Loaded += onLoaded;
        }

        public LayoutPlanModel ViewModel
        {
            get { return DataContext as LayoutPlanModel; }
        }

        private void onLoaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null)
            {
                ViewModel.ToBeDelete += toDelete;
            }
        }

        private void toDelete(LayoutPlanModel obj)
        {
            if (ViewModel != null)
                ViewModel.ToBeDelete -= toDelete;
            layoutPlan.DataContext = null;
        }

        private void tbStayingSeconds_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            bool havePoint = false;
            foreach (char c in e.Text)
            {
                if (c == '.')
                    havePoint = true;
                else if ((c < '0' || c > '9') && c != '.')
                {
                    e.Handled = true;
                    return;
                }
            }
            //小数点只能输入一次 
            if(havePoint && (sender as TextBox).Text.Count(_=>_ == '.') >= 1)
            {
                e.Handled = true;
            }
        }

        private void tbStayingSeconds_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void tbStayingSeconds_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if(text.Any(_ => _ == '.') && (sender as TextBox).Text.Any(_ => _ == '.'))
                { e.CancelCommand(); }
                else if (!isNumberic(text))
                { e.CancelCommand(); }
            }
            else { e.CancelCommand(); }
        }

        //isDigit是否是数字
        static bool isNumberic(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;
            foreach (char c in str)
            {
                if (!char.IsDigit(c))
                    return false;
            }
            return true;
        }
    }
}

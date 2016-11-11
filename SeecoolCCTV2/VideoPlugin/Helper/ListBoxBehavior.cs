using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace VideoNS.Helper
{
    internal class ListBoxBehavior
    {
        /// <summary>
        /// 自动隐藏选中的列表项。
        /// </summary>
        public static readonly DependencyProperty HideSelectedItemProperty =
            DependencyProperty.RegisterAttached("HideSelectedItem", typeof(bool), typeof(ListBoxBehavior),
                new UIPropertyMetadata(false, model_HSIChanged));

        public static bool GetHideSelectedItem(DependencyObject d)
        {
            return (bool)d.GetValue(HideSelectedItemProperty);
        }

        public static void SetHideSelectedItem(DependencyObject d, bool flag)
        {
            d.SetValue(HideSelectedItemProperty, flag);
        }

        private static void model_HSIChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ListBox list = d as ListBox;
            if (list != null)
            {
                if ((bool)e.NewValue)
                {
                    list.SelectionChanged += List_SelectionChanged;
                    list.Loaded += List_Loaded;
                    hideSelItems(list);
                }
                else
                {
                    list.SelectionChanged -= List_SelectionChanged;
                    list.Loaded -= List_Loaded;
                }
            }
        }


        private static void List_Loaded(object sender, RoutedEventArgs e)
        {
            hideSelItems(sender as ListBox);
        }

        private static void List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            hideSelItems(sender as ListBox);
        }

        private static void hideSelItems(ListBox list)
        {
            IEnumerable<ListBoxItem> items = list.FindVisualChildren<ListBoxItem>();
            if (items != null)
            {
                for (int i = 0; i < items.Count(); i++)
                    hideItem(items.ElementAt(i));
            }
        }

        private static void hideItem(ListBoxItem item)
        {
            if (item.IsSelected)
                item.Visibility = Visibility.Collapsed;
            else
                item.Visibility = Visibility.Visible;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace CCTVReplay.Util
{
    public static class ElementExtensions
    {
        public static T FindVisualChild<T>(this DependencyObject obj) where T : DependencyObject
        {
            if (obj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                    if (child != null && child is T)
                    {
                        return (T)child;
                    }
                    T childItem = FindVisualChild<T>(child);
                    if (childItem != null)
                        return childItem;
                }
            }
            return null;
        }

        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject obj) where T : DependencyObject
        {
            if (obj != null)
            {
                List<T> rst = new List<T>();
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                    if (child is T)
                        rst.Add(child as T);

                }
                if (rst.Count > 0)
                {
                    return rst;
                }
                else
                {
                    //只获取平级子节点集合。
                    for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
                    {
                        DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                        rst.AddRange(child.FindVisualChildren<T>());
                    }
                    return rst;
                }
            }
            return new List<T>();
        }

        public static T FindVisualParent<T>(this DependencyObject obj) where T : DependencyObject
        {
            while (obj != null)
            {
                DependencyObject parent = VisualTreeHelper.GetParent(obj);
                if (parent is T)
                    return parent as T;
                else
                    obj = parent;
            }
            return null;
        }
    }
}

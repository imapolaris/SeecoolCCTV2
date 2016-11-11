using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace VideoNS.Helper
{
    /// <summary>
    /// 自定义附加画刷属性包装类。
    /// </summary>
    public static class CustomBrush
    {
        /// <summary>
        /// 按钮普通状态画刷附加属性。
        /// </summary>
        public static readonly DependencyProperty NormalBrushProperty =
            DependencyProperty.RegisterAttached("NormalBrush", typeof(Brush), typeof(CustomBrush),
                new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0, 0, 0))));

        /// <summary>
        /// 设置 按钮普通状态画刷附加属性。
        /// </summary>
        /// <param name="obj">被添加附加属性的目标对象。</param>
        /// <param name="value">属性值。</param>
        public static void SetNormalBrush(DependencyObject obj, Brush value)
        {
            obj.SetValue(NormalBrushProperty, value);
        }

        /// <summary>
        /// 获取 按钮普通状态画刷附加属性。
        /// </summary>
        /// <param name="obj">被添加附加属性的目标对象。</param>
        public static Brush GetNormalBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(NormalBrushProperty);
        }

        /// <summary>
        /// 按钮鼠标滑过时的画刷附加属性。
        /// </summary>
        public static readonly DependencyProperty MouseOverBrushProperty =
            DependencyProperty.RegisterAttached("MouseOverBrush", typeof(Brush), typeof(CustomBrush),
                new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0, 0, 0))));

        /// <summary>
        /// 设置 按钮鼠标滑过时的画刷附加属性。
        /// </summary>
        /// <param name="obj">被添加附加属性的目标对象。</param>
        /// <param name="value">属性值。</param>
        public static void SetMouseOverBrush(DependencyObject obj, Brush value)
        {
            obj.SetValue(MouseOverBrushProperty, value);
        }

        /// <summary>
        /// 获取 按钮鼠标滑过时的画刷附加属性。
        /// </summary>
        /// <param name="obj">被添加附加属性的目标对象。</param>
        public static Brush GetMouseOverBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(MouseOverBrushProperty);
        }

        /// <summary>
        /// 按钮点击时的画刷附加属性。
        /// </summary>
        public static readonly DependencyProperty ClickBrushProperty =
            DependencyProperty.RegisterAttached("ClickBrush", typeof(Brush), typeof(CustomBrush),
                new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0, 0, 0))));

        /// <summary>
        /// 设置 按钮点击时的画刷附加属性。
        /// </summary>
        /// <param name="obj">被添加附加属性的目标对象。</param>
        /// <param name="value">属性值。</param>
        public static void SetClickBrush(DependencyObject obj, Brush value)
        {
            obj.SetValue(ClickBrushProperty, value);
        }

        /// <summary>
        /// 获取 按钮点击时的画刷附加属性。
        /// </summary>
        /// <param name="obj">被添加附加属性的目标对象。</param>
        public static Brush GetClickBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(ClickBrushProperty);
        }
    }
}

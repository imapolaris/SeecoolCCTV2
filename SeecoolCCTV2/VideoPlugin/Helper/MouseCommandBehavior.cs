using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace VideoNS.Helper
{
    /// <summary>
    /// 控件鼠标事件附加属性。
    /// </summary>
    public static class MouseCommandBehavior
    {
        #region TheCommandToRun

        private static Dictionary<int, CommandGroup> _cmdDicts;

        static MouseCommandBehavior()
        {
            _cmdDicts = new Dictionary<int, CommandGroup>();
        }
        /// <summary>
        /// MouseDown事件附加Command
        /// </summary>
        public static readonly DependencyProperty MouseDownCommandProperty =
            DependencyProperty.RegisterAttached("MouseDownCommand",
                typeof(ICommand),
                typeof(MouseCommandBehavior),
                new FrameworkPropertyMetadata(null, (obj, e) => OnMouseCommandChanged(obj, e, "MouseDown")));

        /// <summary>
        /// 获取 MouseDownCommand附加属性。
        /// </summary>
        public static ICommand GetMouseDownCommand(DependencyObject d)
        {
            return (ICommand)d.GetValue(MouseDownCommandProperty);
        }

        /// <summary>
        /// 设置 MouseDownCommand附加属性。
        /// </summary>
        public static void SetMouseDownCommand(DependencyObject d, ICommand value)
        {
            d.SetValue(MouseDownCommandProperty, value);
        }

        /// <summary>
        /// MouseUp事件附加Command
        /// </summary>
        public static readonly DependencyProperty MouseUpCommandProperty =
            DependencyProperty.RegisterAttached("MouseUpCommand",
                typeof(ICommand),
                typeof(MouseCommandBehavior),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback((obj, e) => OnMouseCommandChanged(obj, e, "MouseUp"))));

        /// <summary>
        /// 获取 MouseUpCommand附加属性。
        /// </summary>
        public static ICommand GetMouseUpCommand(DependencyObject d)
        {
            return (ICommand)d.GetValue(MouseUpCommandProperty);
        }

        /// <summary>
        /// 设置 MouseUpCommand附加属性。
        /// </summary>
        public static void SetMouseUpCommand(DependencyObject d, ICommand value)
        {
            d.SetValue(MouseUpCommandProperty, value);
        }

        public static readonly DependencyProperty MouseEnterCommandProperty =
            DependencyProperty.RegisterAttached("MouseEnterCommand",
                typeof(ICommand),
                typeof(MouseCommandBehavior),
                new FrameworkPropertyMetadata(null, (obj, e) => OnMouseCommandChanged(obj, e, "MouseEnter")));

        /// <summary>
        /// 获取 MouseEnterCommand 附加属性。
        /// </summary>
        public static ICommand GetMouseEnterCommand(DependencyObject d)
        {
            return (ICommand)d.GetValue(MouseEnterCommandProperty);
        }

        /// <summary>
        /// 设置 MouseEnterCommand 附加属性。
        /// </summary>
        public static void SetMouseEnterCommand(DependencyObject d, ICommand value)
        {
            d.SetValue(MouseEnterCommandProperty, value);
        }

        public static readonly DependencyProperty MouseLeaveCommandProperty =
            DependencyProperty.RegisterAttached("MouseLeaveCommand",
                typeof(ICommand),
                typeof(MouseCommandBehavior),
                new FrameworkPropertyMetadata(null, (obj, e) => OnMouseCommandChanged(obj, e, "MouseLeave")));

        /// <summary>
        /// 获取 MouseLeaveCommand 附加属性。
        /// </summary>
        public static ICommand GetMouseLeaveCommand(DependencyObject d)
        {
            return (ICommand)d.GetValue(MouseLeaveCommandProperty);
        }

        /// <summary>
        /// 设置 MouseLeaveCommand 附加属性。
        /// </summary>
        public static void SetMouseLeaveCommand(DependencyObject d, ICommand value)
        {
            d.SetValue(MouseLeaveCommandProperty, value);
        }

        public static readonly DependencyProperty MouseMoveCommandProperty =
            DependencyProperty.RegisterAttached("MouseMoveCommand",
                typeof(ICommand),
                typeof(MouseCommandBehavior),
                new FrameworkPropertyMetadata(null, (obj, e) => OnMouseCommandChanged(obj, e, "MouseMove")));

        /// <summary>
        /// 获取 MouseMoveCommand 附加属性。
        /// </summary>
        public static ICommand GetMouseMoveCommand(DependencyObject d)
        {
            return (ICommand)d.GetValue(MouseMoveCommandProperty);
        }

        /// <summary>
        /// 设置 MouseMoveCommand 附加属性。
        /// </summary>
        public static void SetMouseMoveCommand(DependencyObject d, ICommand value)
        {
            d.SetValue(MouseMoveCommandProperty, value);
        }
        #endregion

        /// <summary>
        /// 将命令与事件绑定。
        /// </summary>
        private static void OnMouseCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs args, string routedEventName)
        {
            var element = (FrameworkElement)d;
            int hashCode = element.GetHashCode();
            ICommand cmd = args.NewValue as ICommand;
            CommandGroup group = null;
            _cmdDicts.TryGetValue(hashCode, out group);
            if (group == null)
            {
                group = new CommandGroup();
                _cmdDicts[hashCode] = group;
            }

            switch (routedEventName)
            {
                case "MouseDown":
                    group.MouseDownCmd = cmd;
                    element.RemoveHandler(FrameworkElement.MouseDownEvent, new MouseButtonEventHandler(UI_MouseDown));
                    if (group.MouseDownCmd != null)
                        element.AddHandler(FrameworkElement.MouseDownEvent, new MouseButtonEventHandler(UI_MouseDown), true);
                    break;
                case "MouseUp":
                    group.MouseUpCmd = cmd;
                    element.RemoveHandler(FrameworkElement.MouseUpEvent, new MouseButtonEventHandler(UI_MouseUp));
                    if (group.MouseUpCmd != null)
                        element.AddHandler(FrameworkElement.MouseUpEvent, new MouseButtonEventHandler(UI_MouseUp), true);
                    break;
                case "MouseEnter":
                    group.MouseEnterCmd = cmd;
                    element.RemoveHandler(FrameworkElement.MouseEnterEvent, new MouseEventHandler(UI_MouseEnter));
                    if (group.MouseEnterCmd != null)
                        element.AddHandler(FrameworkElement.MouseEnterEvent, new MouseEventHandler(UI_MouseEnter), true);
                    break;
                case "MouseLeave":
                    group.MouseLeaveCmd = cmd;
                    element.RemoveHandler(FrameworkElement.MouseLeaveEvent, new MouseEventHandler(UI_MouseLeave));
                    if (group.MouseLeaveCmd != null)
                        element.AddHandler(FrameworkElement.MouseLeaveEvent, new MouseEventHandler(UI_MouseLeave), true);
                    break;
                case "MouseMove":
                    group.MouseMoveCmd = cmd;
                    element.RemoveHandler(FrameworkElement.MouseMoveEvent, new MouseEventHandler(UI_MouseMove));
                    if (group.MouseMoveCmd != null)
                        element.AddHandler(FrameworkElement.MouseMoveEvent, new MouseEventHandler(UI_MouseMove), true);
                    break;
            }
            if (group.IsObsolete)
            {
                _cmdDicts.Remove(hashCode);
            }
        }

        private static void UI_MouseDown(object sender, MouseButtonEventArgs e)
        {
            int hashCode = sender.GetHashCode();
            if (_cmdDicts.ContainsKey(hashCode))
            {
                if (_cmdDicts[hashCode].MouseDownCmd != null)
                    _cmdDicts[hashCode].MouseDownCmd.Execute(e);
            }
        }

        private static void UI_MouseUp(object sender, MouseButtonEventArgs e)
        {
            int hashCode = sender.GetHashCode();
            if (_cmdDicts.ContainsKey(hashCode))
            {
                if (_cmdDicts[hashCode].MouseUpCmd != null)
                    _cmdDicts[hashCode].MouseUpCmd.Execute(e);
            }
        }

        private static void UI_MouseEnter(object sender, MouseEventArgs e)
        {
            int hashCode = sender.GetHashCode();
            if (_cmdDicts.ContainsKey(hashCode))
            {
                if (_cmdDicts[hashCode].MouseEnterCmd != null)
                    _cmdDicts[hashCode].MouseEnterCmd.Execute(e);
            }
        }

        private static void UI_MouseLeave(object sender, MouseEventArgs e)
        {
            int hashCode = sender.GetHashCode();
            if (_cmdDicts.ContainsKey(hashCode))
            {
                if (_cmdDicts[hashCode].MouseLeaveCmd != null)
                    _cmdDicts[hashCode].MouseLeaveCmd.Execute(e);
            }
        }

        private static void UI_MouseMove(object sender, MouseEventArgs e)
        {
            int hashCode = sender.GetHashCode();
            if (_cmdDicts.ContainsKey(hashCode))
            {
                if (_cmdDicts[hashCode].MouseMoveCmd != null)
                    _cmdDicts[hashCode].MouseMoveCmd.Execute(e);
            }
        }

        private class CommandGroup : DependencyObject
        {
            public ICommand MouseDownCmd;
            public ICommand MouseUpCmd;
            public ICommand MouseEnterCmd;
            public ICommand MouseLeaveCmd;
            public ICommand MouseMoveCmd;

            public bool IsObsolete
            {
                get
                {
                    return MouseDownCmd == null && MouseUpCmd == null
                        && MouseEnterCmd == null && MouseLeaveCmd == null
                        && MouseMoveCmd == null;
                }
            }
        }
    }
}

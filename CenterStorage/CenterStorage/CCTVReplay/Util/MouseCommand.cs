using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CCTVReplay.Util
{
    public class MouseCommand
    {
        public static ICommand GetMouseDownCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(MouseDownCommandProperty);
        }

        public static void SetMouseDownCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(MouseDownCommandProperty, value);
        }

        // Using a DependencyProperty as the backing store for MouseDownCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MouseDownCommandProperty =
            DependencyProperty.RegisterAttached("MouseDownCommand", typeof(ICommand), typeof(MouseCommand),
                new PropertyMetadata(mouseDownCmdChanged));



        public static ICommand GetMouseUpCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(MouseUpCommandProperty);
        }

        public static void SetMouseUpCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(MouseUpCommandProperty, value);
        }

        // Using a DependencyProperty as the backing store for MouseUpCommad.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MouseUpCommandProperty =
            DependencyProperty.RegisterAttached("MouseUpCommand", typeof(ICommand), typeof(MouseCommand),
                new PropertyMetadata(mouseUpCmdChanged));



        private static void mouseDownCmdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement fe = d as FrameworkElement;
            if (fe != null)
            {
                ICommand cmd = e.NewValue as ICommand;
                fe.PreviewMouseDown -= Fe_PreviewMouseDown;
                if (cmd != null)
                    fe.PreviewMouseDown += Fe_PreviewMouseDown;
            }
        }

        private static void mouseUpCmdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement fe = d as FrameworkElement;
            if (fe != null)
            {
                ICommand cmd = e.NewValue as ICommand;
                fe.PreviewMouseUp -= Fe_PreviewMouseUp;
                if (cmd != null)
                    fe.PreviewMouseUp += Fe_PreviewMouseUp;
            }
        }

        private static void Fe_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            DependencyObject d = sender as DependencyObject;
            if (d != null)
            {
                ICommand cmd = GetMouseDownCommand(d);
                if (cmd != null)
                    cmd.Execute(null);
            }
        }

        private static void Fe_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            DependencyObject d = sender as DependencyObject;
            if (d != null)
            {
                ICommand cmd = GetMouseUpCommand(d);
                if (cmd != null)
                    cmd.Execute(null);
            }
        }
    }
}

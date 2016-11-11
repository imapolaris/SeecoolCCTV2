using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VideoNS.Helper
{
    internal static class ArrowDirection
    {
        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.RegisterAttached("Direction", typeof(Direction), typeof(ArrowDirection));

        public static Direction GetDirection(DependencyObject obj)
        {
            return (Direction)obj.GetValue(DirectionProperty);
        }

        public static void SetDirection(DependencyObject obj, Direction value)
        {
            obj.SetValue(DirectionProperty, value);
        }
    }

    internal enum Direction
    {
        Left = 0,
        Up = 1,
        Right = 2,
        Down = 3,
        LeftUp = 4,
        RightUp = 5,
        RightDown = 6,
        LeftDown = 7
    }
}
 
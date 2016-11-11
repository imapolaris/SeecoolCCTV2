using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AopUtil.WpfBinding;

namespace VideoNS
{

    /// <summary>
    /// 控件渲染偏移坐标类型。
    /// </summary>
    public class TransformParam : DependencyObject
    {
        public static readonly DependencyProperty TranslateXProperty = DependencyProperty.Register(nameof(TranslateX), typeof(double), typeof(TransformParam));
        public static readonly DependencyProperty TranslateYProperty = DependencyProperty.Register(nameof(TranslateY), typeof(double), typeof(TransformParam));
        public static readonly DependencyProperty ScaleXProperty = DependencyProperty.Register(nameof(ScaleX), typeof(double), typeof(TransformParam),new PropertyMetadata(1d));
        public static readonly DependencyProperty ScaleYProperty = DependencyProperty.Register(nameof(ScaleY), typeof(double), typeof(TransformParam), new PropertyMetadata(1d));
        public static readonly DependencyProperty RotateAngleProperty = DependencyProperty.Register(nameof(RotateAngle), typeof(double), typeof(TransformParam));
        public static readonly DependencyProperty HasTransformedProperty = DependencyProperty.Register(nameof(HasTransformed), typeof(bool), typeof(TransformParam));
        /// <summary>
        /// 默认构造方法。
        /// </summary>
        public TransformParam() : this(0, 0, 0)
        {
        }

        /// <summary>
        /// 生成一个渲染偏移坐标实例。
        /// </summary>
        /// <param name="transX">X方向偏移量。</param>
        /// <param name="transY">Y方向偏移量。</param>
        public TransformParam(double transX, double transY) : this(transX, transY, 0)
        {
        }

        /// <summary>
        /// 生成一个渲染偏移坐标实例。
        /// </summary>
        /// <param name="transX">X方向偏移量。</param>
        /// <param name="transY">Y方向偏移量。</param>
        /// <param name="rotAngle">旋转角度 -360--360</param>
        public TransformParam(double transX, double transY, double rotAngle)
        {
            TranslateX = transX;
            TranslateY = transY;
            RotateAngle = rotAngle;
            ScaleX = 1;
            ScaleY = 1;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            bool transformed = TranslateX != 0 && TranslateY != 0 && RotateAngle != 0
               && ScaleX != 0 && ScaleY != 0;

            if (transformed != HasTransformed)
                HasTransformed = transformed;
            base.OnPropertyChanged(e);
        }

        /// <summary>
        /// 生成一个渲染偏移坐标实例。
        /// </summary>
        /// <param name="pnt">偏移坐标。</param>
        public TransformParam(Point pnt) : this(pnt.X, pnt.Y)
        {
        }

        /// <summary>
        /// 生成一个渲染偏移坐标实例。
        /// </summary>
        /// <param name="pnt">偏移坐标。</param>
        /// <param name="rotAngle">旋转角度 0--360</param>
        public TransformParam(Point pnt, double rotAngle) : this(pnt.X, pnt.Y, rotAngle)
        {
        }


        /// <summary>
        /// X方向偏移量。
        /// </summary>
        public double TranslateX
        {
            get { return (double)GetValue(TranslateXProperty); }
            set { SetValue(TranslateXProperty, value); }
        }

        /// <summary>
        /// Y方向偏移量。
        /// </summary>
        public double TranslateY
        {
            get { return (double)GetValue(TranslateYProperty); }
            set { SetValue(TranslateYProperty, value); }
        }

        public double ScaleX
        {
            get { return (double)GetValue(ScaleXProperty); }
            set { SetValue(ScaleXProperty, value); }
        }

        public double ScaleY {
            get { return (double)GetValue(ScaleYProperty); }
            set { SetValue(ScaleYProperty, value); }
        }

        /// <summary>
        /// 旋转角度 -360--360
        /// </summary>
        public double RotateAngle
        {
            get { return (double)GetValue(RotateAngleProperty); }
            set { SetValue(RotateAngleProperty, value); }
        }

        /// <summary>
        /// 是否已发生了变换。
        /// </summary>
        public bool HasTransformed
        {
            get { return (bool)GetValue(HasTransformedProperty); }
            set { SetValue(HasTransformedProperty, value); }
        }

        public void Reset()
        {
            TranslateX = 0;
            TranslateY = 0;
            RotateAngle = 0;
            ScaleY = 1;
            ScaleX = 1;
        }
    }
}

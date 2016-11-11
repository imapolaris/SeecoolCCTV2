using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace CCTVReplay.VideoTree.Search
{
    [TemplatePart(Name = "PART_Cancel", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Textbox", Type = typeof(TextBox))]
    public class SearchBox : Control
    {
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var cancelButton = GetTemplateChild("PART_Cancel") as Button;
            cancelButton.Click += (s1, e1) => Text = string.Empty;
            var textEvent = GetTemplateChild("PART_Textbox") as TextBox;
            textEvent.TextChanged += TextEvent_TextChanged;
            textEvent.Focus();
        }

        private void TextEvent_TextChanged(object sender, TextChangedEventArgs e)
        {
            Text = (sender as TextBox).Text;
        }

        static SearchBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchBox), new FrameworkPropertyMetadata(typeof(SearchBox)));
        }

        public SearchBox()
        {
            this.PreviewGotKeyboardFocus += (s1, e1) => ShowFocusAdorner();// AdornerLayer.GetAdornerLayer(this).Add(focusAdorner);
            this.PreviewLostKeyboardFocus += (s1, e1) => RemoveFocusAdorner();// AdornerLayer.GetAdornerLayer(this).Remove(focusAdorner);
        }

        private void ShowFocusAdorner()
        {
            var layer = AdornerLayer.GetAdornerLayer(this);
            if (layer != null)
            {
                if (layer.GetAdorners(this) == null)
                {
                    layer.Add(new SearchBoxFocusAdorner(this));
                }
                else if (layer.GetAdorners(this).ToList().OfType<SearchBoxFocusAdorner>().Count() < 1)
                {
                    layer.Add(new SearchBoxFocusAdorner(this));
                }
            }
        }

        private void RemoveFocusAdorner()
        {
            var layer = AdornerLayer.GetAdornerLayer(this);
            if (layer != null)
            {
                if (layer.GetAdorners(this) != null)
                {
                    layer.GetAdorners(this).OfType<SearchBoxFocusAdorner>().ToList().ForEach(p => layer.Remove(p));
                }
            }
        }

        public string Watermark
        {
            get { return (string)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register("Watermark", typeof(string), typeof(SearchBox), new UIPropertyMetadata(""));

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(SearchBox), new UIPropertyMetadata("搜索视频列表", TextProperyChangedCallback));


        public static readonly RoutedEvent TextChangedEvent =
            EventManager.RegisterRoutedEvent("TextChanged",
             RoutingStrategy.Bubble, typeof(TextChangedEventHandler), typeof(SearchBox));

        private static void TextProperyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs arg)
        {
            if (sender != null && sender is SearchBox)
            {
                SearchBox sb = sender as SearchBox;
                sb.OnTextChanged((string)arg.OldValue, (string)arg.NewValue);
            }
        }
        protected virtual void OnTextChanged(string oldValue, string newValue)
        {
            TextChangedEventArgs arg =
                new TextChangedEventArgs(TextChangedEvent, UndoAction.None);

            this.RaiseEvent(arg);
        }
        [Description("Text变更后发生")]
        public event TextChangedEventHandler TextChanged
        {
            add
            {
                this.AddHandler(TextChangedEvent, value);
            }
            remove
            {
                this.RemoveHandler(TextChangedEvent, value);
            }
        }
    }

    public class SearchBoxFocusAdorner : Adorner
    {
        public SearchBoxFocusAdorner(UIElement arg)
            : base(arg)
        {
            this.IsHitTestVisible = false;
        }

        protected override void OnRender(DrawingContext dc)
        {
            //if (pen == null)
            //{
            //    pen = new Pen(new SolidColorBrush(Color.FromRgb(0x66, 0x99, 0xcc)), 2);
            //    pen.Freeze();
            //}
            //dc.DrawRoundedRectangle(null, pen, new Rect(this.RenderSize), 0, 0);
        }

        //private Pen pen;
    }
}


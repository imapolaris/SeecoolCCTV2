using CCTVReplay.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace CCTVReplay.VideoTree.Search
{
    /// <summary>
    /// HighlightTextBlock.xaml 的交互逻辑
    /// </summary>
    public partial class HighlightTextBlock : UserControl
    {
        public HighlightTextBlock()
        {
            InitializeComponent();
        }

        #region 依赖属性

        public static readonly DependencyProperty CompContentProperty =
            DependencyProperty.Register("CompContent", typeof(string), typeof(HighlightTextBlock),
                                        new FrameworkPropertyMetadata(null, OnHtContentChanged));

        [Description("获取或设置显示的内容")]
        public string CompContent
        {
            get { return (string)GetValue(CompContentProperty); }
            set { SetValue(CompContentProperty, value); }
        }

        public static readonly DependencyProperty HighlightColorProperty =
            DependencyProperty.Register("HlColor", typeof(Brush), typeof(HighlightTextBlock),
                new FrameworkPropertyMetadata(Brushes.Gold, new PropertyChangedCallback(OnHlColorChanged)));

        [Description("获取或设置高亮显示的颜色")]
        public Brush HlColor
        {
            get { return (Brush)GetValue(HighlightColorProperty); }
            set { SetValue(HighlightColorProperty, value); }
        }

        public static readonly DependencyProperty HighlightContentProperty =
            DependencyProperty.Register("HlContent", typeof(string), typeof(HighlightTextBlock),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnHlContentChanged)));

        [Description("获取或设置高亮显示的文本")]
        public string HlContent
        {
            get { return (string)GetValue(HighlightContentProperty); }
            set { SetValue(HighlightContentProperty, value); }
        }
        #endregion

        private static void OnHtContentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs arg)
        {
            if (sender is HighlightTextBlock)
                updateHightlightContent(sender as HighlightTextBlock);
        }

        private static void OnHlColorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs arg)
        {
            if (sender is HighlightTextBlock)
                updateHightlightContent(sender as HighlightTextBlock);
        }

        private static void OnHlContentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs arg)
        {
            if (sender is HighlightTextBlock)
                updateHightlightContent(sender as HighlightTextBlock);
        }

        private class KeyItem
        {
            public string Key { get; set; }
            public bool Finished { get; set; }
        }
        #region 文本高亮标记
        private static void updateHightlightContent(HighlightTextBlock ctrl)
        {
            if (ctrl == null)
                return;
            ctrl.innerTextBlock.Inlines.Clear();
            ctrl.innerTextBlock.Inlines.AddRange(makeRunsFromContent(ctrl.CompContent, ctrl.HlColor, ctrl.HlContent));
        }

        static char[] separator = new char[] { ' ' };
        private static IEnumerable<Run> makeRunsFromContent(string content, Brush hlColor, string hlContent)
        {
            List<Run> listRtn = new List<Run>();
            if (!string.IsNullOrWhiteSpace(content))
            {
                string hlInfo = hlContent;//HighlightContent.ToHighlight; //hlContent;
                if (string.IsNullOrWhiteSpace(hlInfo))
                    listRtn.Add(new Run(content));
                else
                {
                    List<string> searchInfos = hlInfo.ToLower().Split(separator, StringSplitOptions.RemoveEmptyEntries).ToList();
                    if (searchInfos.Count == 0)
                        listRtn.Add(new Run(content));
                    else
                    {
                        bool[] hlKeys = new bool[content.Length];
                        var szms = PinYinConverter.ToShouZiMuArray(content);
                        foreach (var info in searchInfos)
                        {
                            int index = szms.IndexOf(info);
                            if (index >= 0)
                            {
                                for (int i = 0; i < info.Length; i++)
                                    hlKeys[index + i] = true;
                            }
                        }
                        for (int i = 0; i < content.Length;)
                        {
                            int length = getHighlightLength(hlKeys, i);
                            if (hlKeys[i])
                                listRtn.Add(new Run(content.Substring(i, length)) { Foreground = hlColor });
                            else
                                listRtn.Add(new Run(content.Substring(i, length)));
                            i += length;
                        }
                    }
                }
            }
            return listRtn;
        }

        static int getHighlightLength(bool[] hlKeys, int index)
        {
            int length = 1;
            bool currentStatus = hlKeys[index];
            for (int i = index + 1; i < hlKeys.Length; i++)
            {
                if (hlKeys[i] == currentStatus)
                    length++;
                else
                    break;
            }
            return length;
        }
        #endregion 文本高亮标记
    }
}

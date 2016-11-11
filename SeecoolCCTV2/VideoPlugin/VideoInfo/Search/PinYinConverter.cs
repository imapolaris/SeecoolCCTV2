using Net.Sourceforge.Pinyin4j;
using Net.Sourceforge.Pinyin4j.Format;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoNS.VideoInfo.Search
{
    public class PinYinConverter
    {
        static HanyuPinyinOutputFormat outputFormat = new HanyuPinyinOutputFormat()
        {
            ToneType = HanyuPinyinToneType.WITHOUT_TONE
        };

        public static string ToShouZiMuString(string str)
        {
            char[] szms = new char[str.Length];
            for (int i = 0; i < szms.Length; i++)
            {
                char c = getShouZiMuChar(str[i]);
                szms[i] = c;
            }
            return new string(szms);
        }

        public static ShouZiMuArray ToShouZiMuArray(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return new ShouZiMuArray(new List<char[]>(), "");
            str = str.ToLower();
            List<char[]> szms = new List<char[]>();
            for (int i = 0; i < str.Length; i++)
            {
                szms.Add(getShouZiMuArray(str[i]));
            }
            return new ShouZiMuArray(szms, str);
        }

        private static char[] getShouZiMuArray(char ch)
        {
            string[] pinyinArray = PinyinHelper.ToHanyuPinyinStringArray(ch, outputFormat);
            if (pinyinArray == null)
                return new char[] { ch };
            else
                return pinyinArray.Select(e => e.First()).Distinct().ToArray();
        }

        private static char getShouZiMuChar(char ch)
        {
            string[] pinyinArray = PinyinHelper.ToHanyuPinyinStringArray(ch, outputFormat);
            if (pinyinArray == null)
                return ch;
            else
                return pinyinArray.First().First();
        }
    }

    public class ShouZiMuArray
    {
        List<char[]> _listSZM;
        string _baseInfo;
        int Length;
        public ShouZiMuArray(List<char[]> listSZM, string data)
        {
            _listSZM = listSZM ?? new List<char[]>();
            _baseInfo = data;
            updateLength();
        }

        public int IndexOf(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return -1;
            int index = _baseInfo.IndexOf(value);
            if (index >= 0)
                return index;
            int length = value.Length;
            for (int i = 0; i < Length - length + 1; i++)
            {
                if (isFound(value, i, length))
                    return i;
            }
            return -1;
        }

        private bool isFound(string value, int index, int length)
        {
            for (int i = 0; i < length; i++)
            {
                if (!_listSZM[index + i].Any(e => e == value[i]))
                    return false;
            }
            return true;
        }

        public void Remove(int index, int length)
        {
            _baseInfo = _baseInfo.Remove(index, length);
            _listSZM.RemoveRange(index, length);
            updateLength();
        }

        private void updateLength()
        {
            Length = Math.Min(_listSZM.Count, _baseInfo.Length);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VideoNS.Base;
using VideoNS.SubWindow;

namespace VideoNS.Json
{
    public class JsonParserExpand
    {
        public static bool SerializeToFile<T>(T data, string typeName,string extension)where T: class
        {
            if (data == null)
                return dealWhenDataIsNull(typeName);
            string filter = string.Format("{0}文件(*.{1}) | *.{1}", typeName,extension);
            string fileName = FileSelector.SelectSaveFile(filter);
            try
            {
                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    JsonParser.SerializeToFile<T>(data, fileName, true);
                    string message = string.Format("成功导出{0}文件！", typeName);
                    DialogWin.Show(message, "导出成功",DialogWinImage.Information);
                }
            }
            catch
            {
                string message = string.Format("{0}\"{1}\"导出失败!", typeName, fileName);
                DialogWin.Show(message, "导出错误",DialogWinImage.Error);
            }
            return false;
        }

        public static bool SerializeToFile<T>(List<T> datas, string typeName,string extension) where T : class
        {
            if (datas != null && datas.Count > 0)
                return SerializeToFile<List<T>>(datas, typeName,extension);
            return dealWhenDataIsNull(typeName);
        }

        public static T DeserializeFromFile<T>(string typeName,string extension)where T: class
        {
            string filter = string.Format("{0}(*.{1}) | *.{1}", typeName,extension);
            string fileName = FileSelector.SelectOpenFile(filter);
            try
            {
                if (!string.IsNullOrWhiteSpace(fileName))
                    return JsonParser.DeserializeFromFile<T>(fileName);
            }
            catch
            {
                string message = string.Format("{0}\"{1}\"导入失败,请检查文件中数据是否匹配!", typeName, fileName);
                DialogWin.Show(message, "导入失败",DialogWinImage.Error);
            }
            return null;
        }

        static bool dealWhenDataIsNull(string typeName)
        {
            string message = string.Format("\"{0}\"数据为空,无法执行导出操作！", typeName);
            DialogWin.Show(message, "提示",DialogWinImage.Information);
            return false;
        }
    }
}

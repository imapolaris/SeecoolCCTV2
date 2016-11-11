using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace VideoNS.Helper
{
    internal class BindingHelper
    {
        public static Binding CreateBinding(object source, BindingMode mode, string path)
        {
            Binding binding = new Binding();
            binding.Mode = mode;
            binding.Source = source;
            if (path != null)
                binding.Path = new PropertyPath(path);
            return binding;
        }

        /// <summary>
        /// 指定绑定到DataContext
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Binding CreateBinding(BindingMode mode, string path)
        {
            Binding binding = new Binding();
            binding.Mode = mode;
            if (path != null)
                binding.Path = new PropertyPath(path);
            return binding;
        }
    }
}

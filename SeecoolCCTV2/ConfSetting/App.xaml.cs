using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace ConfSetting
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var strTempAssmbPath = "";
            if ("Seecool.ShareMemory" == args.Name.Substring(0, args.Name.IndexOf(",")))
            {
                strTempAssmbPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Libs/Seecool.ShareMemory.dll");
            }
            return string.IsNullOrWhiteSpace(strTempAssmbPath) ? null : Assembly.LoadFrom(strTempAssmbPath);
        }
    }
}

using Common.Attribute;
using Common.Message;
using PluginBaseLib.Interface;
using PluginBaseLib.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Telerik.Windows.Controls;

namespace CctvLayoutPlugin
{
    [ManagedControlName(ContainerName)]
    public class LayoutPlugin : ILayoutPlugin, IMainLayoutProvider
    {
        public const string ContainerName = "CctvLayoutPlugin_Container";

        public FrameworkViewModel ViewModel
        {
            get
            {
                return null;
            }
        }

        public void Exit()
        {
        }

        public void Init()
        {
        }

        WorkSpaceMgr _mgr;

        public void InitRegisteredUI(IEnumerable<System.Windows.FrameworkElement> uiElements)
        {
            var container = uiElements.FirstOrDefault(n => n.Name == ContainerName) as ContentControl;
            if (container == null)
                throw new Exception($"未找到名为{ContainerName}的插件");
            _mgr = new WorkSpaceMgr(container);

            RemoteEvents.Global.Send("CCTV2_LayoutPlugin_RegisteredUIInited");
        }

        public string ProvideMainLayout()
        {
            return File.ReadAllText(Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location), "layout.xml"));
        }

        public string ProvideMainStyle()
        {
            return File.ReadAllText(Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location), "style.xml"));
        }
    }
}
